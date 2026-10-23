using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizedVisibilityCuller : MonoBehaviour
{
    [Header("--- Tham Chiếu Cấu Hình ---")]
    [Tooltip("Camera chính dùng để kiểm tra tầm nhìn của Player")]
    public Camera mainCamera;

    [Tooltip("Transform của Player (Dùng để tính khoảng cách bán kính an toàn. Nếu trống sẽ lấy vị trí MainCamera)")]
    public Transform playerTransform;

    [Tooltip("Danh sách các MeshRenderer của các vật thể cần tối ưu")]
    public List<MeshRenderer> objectRenderers = new List<MeshRenderer>();

    [Header("--- Cài Đặt Tối Ưu & Bán Kính An Toàn ---")]
    [Tooltip("Bán kính quanh Player/Camera. Bất kỳ MeshRenderer nào nằm trong khoảng cách này sẽ KHÔNG BAO GIỜ bị ẩn.")]
    public float banKinhAnToan = 10.0f;

    [Tooltip("Thời gian giãn cách giữa các lần kiểm tra (giây). 0.1s tức là 10 lần/giây.")]
    public float thoiGianCapNhat = 0.1f;

    [Tooltip("Lề đệm mở rộng tầm quét (mét). Giúp hiện Mesh trước khi camera kịp lia tới để tránh bị giật/pop-in.")]
    public float doRongDem = 3.0f;

    private Plane[] cameraPlanes;
    private Coroutine processCoroutine;

    private void Awake()
    {
        // Tự động quét toàn bộ MeshRenderer ngay khi vừa vào game
        TuDongTimRenderer();
    }

    private void Start()
    {
        // Tự động tìm Main Camera nếu chưa gán từ Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Tự động tìm Player nếu chưa gán thủ công
        if (playerTransform == null && mainCamera != null)
        {
            playerTransform = mainCamera.transform;
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
                // Xác định tâm để tính bán kính an toàn (Ưu tiên Player, nếu không có lấy Camera)
                Vector3 tamKiemTra = playerTransform != null ? playerTransform.position : mainCamera.transform.position;

                // Bước 1: Trích xuất 6 mặt phẳng nón nhìn (Frustum Planes) gốc của Camera
                cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

                // Bước 2: Nới rộng các mặt phẳng nón nhìn ra thêm 1 khoảng doRongDem (Padding)
                if (doRongDem > 0f)
                {
                    for (int j = 0; j < cameraPlanes.Length; j++)
                    {
                        cameraPlanes[j].distance += doRongDem;
                    }
                }

                // Bước 3: Duyệt qua tất cả vật thể để ẩn/hiện dựa trên bán kính và khung nhìn
                for (int i = 0; i < objectRenderers.Count; i++)
                {
                    MeshRenderer rendererTarget = objectRenderers[i];

                    if (rendererTarget != null)
                    {
                        bool hienThi = false;

                        // Kiểm tra 1: Nếu vật thể nằm trong bán kính an toàn -> Luôn hiển thị
                        float khoangCachBinhPhuong = Vector3.SqrMagnitude(rendererTarget.transform.position - tamKiemTra);
                        if (khoangCachBinhPhuong <= (banKinhAnToan * banKinhAnToan))
                        {
                            hienThi = true;
                        }
                        else
                        {
                            // Kiểm tra 2: Nếu nằm ngoài bán kính -> Kiểm tra xem có đang nằm trong tầm nhìn Camera không
                            hienThi = GeometryUtility.TestPlanesAABB(cameraPlanes, rendererTarget.bounds);
                        }

                        // Chỉ đổi trạng thái enablement khi cần thiết để tiết kiệm CPU/GPU
                        if (rendererTarget.enabled != hienThi)
                        {
                            rendererTarget.enabled = hienThi;
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