using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizedVisibilityCuller : MonoBehaviour
{
    [Header("--- Tham Chiếu Cấu Hình ---")]
    [Tooltip("Camera chính dùng để kiểm tra tầm nhìn của Player")]
    public Camera mainCamera;

    [Tooltip("Danh sách các MeshRenderer của các vật thể cần tối ưu")]
    public List<MeshRenderer> objectRenderers = new List<MeshRenderer>();

    [Header("--- Cài Đặt Tối Ưu Performance ---")]
    [Tooltip("Thời gian giãn cách giữa các lần kiểm tra (giây). 0.1s tức là 10 lần/giây.")]
    public float thoiGianCapNhat = 0.1f;

    [Tooltip("Lề đệm mở rộng tầm quét (mét). Giúp hiện Mesh trước khi camera kịp lia tới để tránh bị giật/pop-in.")]
    public float doRongDem = 3.0f;

    private Plane[] cameraPlanes;
    private Coroutine processCoroutine;

    private void Start()
    {
        // Tự động tìm Main Camera nếu chưa gán từ Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Bắt đầu vòng lặp kiểm tra tối ưu
        processCoroutine = StartCoroutine(KiemTraTamNhinVongLap());
    }

    /// <summary>
    /// Hàm vòng lặp Coroutine giúp giãn cách việc kiểm tra, tránh làm nặng CPU ở mỗi frame (Update)
    /// </summary>
    private IEnumerator KiemTraTamNhinVongLap()
    {
        while (true)
        {
            if (mainCamera != null && objectRenderers.Count > 0)
            {
                // Bước 1: Trích xuất 6 mặt phẳng nón nhìn (Frustum Planes) gốc của Camera
                cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

                // Bước 2: Nới rộng các mặt phẳng nón nhìn ra thêm 1 khoảng doRongDem (Padding)
                if (doRongDem > 0f)
                {
                    for (int j = 0; j < cameraPlanes.Length; j++)
                    {
                        // Dịch chuyển khoảng cách mặt phẳng theo hướng pháp tuyến (Normal)
                        cameraPlanes[j].distance += doRongDem;
                    }
                }

                // Bước 3: Duyệt qua tất cả vật thể để ẩn/hiện dựa trên khung nhìn đã nới rộng
                for (int i = 0; i < objectRenderers.Count; i++)
                {
                    MeshRenderer rendererTarget = objectRenderers[i];

                    if (rendererTarget != null)
                    {
                        // Kiểm tra Bounding Box (AABB) vật thể với nón nhìn đã được nới rộng
                        bool dangTrongTamNhin = GeometryUtility.TestPlanesAABB(cameraPlanes, rendererTarget.bounds);

                        // Chỉ đổi trạng thái enablement khi cần thiết để tiết kiệm CPU/GPU
                        if (rendererTarget.enabled != dangTrongTamNhin)
                        {
                            rendererTarget.enabled = dangTrongTamNhin;
                        }
                    }
                }
            }

            // Chờ một khoảng thời gian trước khi kiểm tra lại
            yield return new WaitForSeconds(thoiGianCapNhat);
        }
    }

    /// <summary>
    /// Hàm hỗ trợ tự động tìm tất cả MeshRenderer trong Scene tương thích mọi phiên bản Unity
    /// </summary>
    [ContextMenu("Tự Động Tìm Tất Cả Renderer Của Map")]
    public void TuDongTimRenderer()
    {
        objectRenderers.Clear();

#if UNITY_2023_1_OR_NEWER
        // ĐÃ SỬA: Sửa đúng tên Enum có chữ 's' (FindObjectsInactive & FindObjectsSortMode)
        MeshRenderer[] allRenderers = FindObjectsByType<MeshRenderer>(UnityEngine.FindObjectsInactive.Exclude, UnityEngine.FindObjectsSortMode.None);
#else
#pragma warning disable CS0618
        MeshRenderer[] allRenderers = FindObjectsOfType<MeshRenderer>();
#pragma warning restore CS0618
#endif

        objectRenderers.AddRange(allRenderers);
        Debug.Log("Đã tự động thêm " + objectRenderers.Count + " vật thể vào danh sách tối ưu!");
    }

    private void OnDisable()
    {
        if (processCoroutine != null)
        {
            StopCoroutine(processCoroutine);
        }
    }
}