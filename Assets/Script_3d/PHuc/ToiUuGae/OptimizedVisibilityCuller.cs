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
                // Bước 1: Trích xuất 6 mặt phẳng nón nhìn (Frustum Planes) của Camera
                cameraPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

                // Bước 2: Duyệt qua tất cả vật thể để ẩn/hiện
                for (int i = 0; i < objectRenderers.Count; i++)
                {
                    MeshRenderer rendererTarget = objectRenderers[i];

                    if (rendererTarget != null)
                    {
                        // Kiểm tra xem Bounding Box (AABB) của vật thể có nằm trong/giao với 6 mặt phẳng Camera không
                        bool dangTrongTamNhin = GeometryUtility.TestPlanesAABB(cameraPlanes, rendererTarget.bounds);

                        // Chỉ đổi trạng thái enablement khi cần thiết để tránh tốn tài nguyên GPU/CPU
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
    /// Hàm hỗ trợ tự động tìm tất cả MeshRenderer trong Scene hoặc vật thể cha
    /// </summary>

    [ContextMenu("Tự Động Tìm Tất Cả Renderer Của Map")]
    public void TuDongTimRenderer()
    {
        objectRenderers.Clear();
        MeshRenderer[] allRenderers = FindObjectsOfType<MeshRenderer>();
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