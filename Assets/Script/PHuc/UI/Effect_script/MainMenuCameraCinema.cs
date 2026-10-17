using UnityEngine;
using Unity.Cinemachine; // Dùng Cinemachine v3 mới nhất

/// <summary>
/// Script điều khiển Cinemachine Camera xoay và zoom theo con trỏ chuột trong Main Menu.
/// Đã được sửa lỗi camera bị chúc góc xuống đất.
/// </summary>
public class MainMenuCameraCinematic : MonoBehaviour
{
    [Header("--- Tham chiếu Cinemachine ---")]
    [Tooltip("Kéo CinemachineCamera trên GameObject vào đây")]
    public CinemachineCamera virtualCamera;

    [Header("--- Cấu hình Xoay theo Chuột ---")]
    [Tooltip("Góc ngẩng/cúi tối đa của Camera (Xoay quanh trục X)")]
    public float gocXoayToiDaX = 5f;

    [Tooltip("Góc xoay trái/phải tối đa của Camera (Xoay quanh trục Y)")]
    public float gocXoayToiDaY = 5f;

    [Tooltip("Tích vào đây nếu muốn đảo ngược chiều ngẩng/cúi khi di chuyển chuột lên xuống")]
    public bool daoNguocTrucY = false;

    [Header("--- Cấu hình Lens Zoom (FOV) ---")]
    [Tooltip("Số độ FOV giảm đi khi di chuyển chuột ra xa tâm màn hình")]
    public float giamFovToiDa = 5f;

    [Tooltip("Tốc độ chuyển động mượt của Camera và Zoom Lens")]
    public float tocDoMuot = 5f;

    [Header("--- Cấu hình Đung đưa Tự nhiên (Idle) ---")]
    [Tooltip("Tốc độ đung đưa theo thời gian")]
    public float tocDoDungDua = 1f;

    [Tooltip("Biên độ đung đưa run rẩy nhẹ")]
    public float bienDoDungDua = 0.05f;

    // --- Biến nội bộ lưu gốc ban đầu ---
    private Vector3 triGocBanDau;
    private Quaternion gocXoayGocBanDau;
    private float fovBanDau;

    private void Start()
    {
        // 1. Lưu lại Vị trí và Góc xoay ban đầu của Camera khi vừa bấm Play
        triGocBanDau = transform.position;
        gocXoayGocBanDau = transform.rotation;

        // 2. Lấy góc Lens FOV ban đầu từ Cinemachine Camera
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineCamera>();
        }

        if (virtualCamera != null)
        {
            fovBanDau = virtualCamera.Lens.FieldOfView;
        }
        else
        {
            Debug.LogWarning("[MainMenuCameraCinematic] Chưa gán CinemachineCamera vào Inspector!");
        }
    }

    private void Update()
    {
        // BƯỚC 1: Lấy vị trí chuột hiện tại trên màn hình
        Vector3 toadoChuotManHinh = Input.mousePosition;

        // BƯỚC 2: Chuẩn hóa tọa độ chuột về khoảng [-1, 1] với gốc (0,0) nằm ở CHÍNH GIỮA màn hình
        Vector2 toadoChuotChuanHoa = new Vector2(
            (toadoChuotManHinh.x / Screen.width) * 2f - 1f,
            (toadoChuotManHinh.y / Screen.height) * 2f - 1f
        );

        // BƯỚC 3: Tính góc Pitch (ngẩng/cúi) và Yaw (trái/phải)
        // Nếu daoNguocTrucY = true thì đảo dấu để tùy chỉnh góc nhìn thuận tay
        float heSoTrucY = daoNguocTrucY ? -1f : 1f;

        // Chuột Y điều khiển góc xoay quanh trục X (Pitch - Cúi/Ngẩng)
        float gocXoayPitch = toadoChuotChuanHoa.y * gocXoayToiDaX * heSoTrucY;

        // Chuột X điều khiển góc xoay quanh trục Y (Yaw - Trái/Phải)
        float gocXoayYaw = toadoChuotChuanHoa.x * gocXoayToiDaY;

        // BƯỚC 4: Tạo Quaternion góc xoay mục tiêu (kết hợp với góc xoay gốc ban đầu)
        Quaternion gocXoayMucTieu = gocXoayGocBanDau * Quaternion.Euler(gocXoayPitch, gocXoayYaw, 0f);

        // BƯỚC 5: Tính toán Zoom Lens dựa trên độ lệch chuột so với tâm màn hình
        float khoangCachDenTam = toadoChuotChuanHoa.magnitude;
        // Đảm bảo khoảng cách không vượt quá 1
        khoangCachDenTam = Mathf.Clamp01(khoangCachDenTam);
        float fovMucTieu = fovBanDau - (khoangCachDenTam * giamFovToiDa);

        // BƯỚC 6: Tạo hiệu ứng đung đưa run rẩy nhẹ (Sin/Cos)
        float dungDuaY = Mathf.Sin(Time.time * tocDoDungDua) * bienDoDungDua;
        float dungDuaX = Mathf.Cos(Time.time * tocDoDungDua) * bienDoDungDua;
        Vector3 lechDungDua = new Vector3(dungDuaX, dungDuaY, 0f);

        // BƯỚC 7: Cập nhật biến vào Camera qua các hàm nội suy Lerp/Slerp
        // 7.1 Giữ vị trí gốc + độ lắc đung đưa
        transform.position = Vector3.Lerp(transform.position, triGocBanDau + lechDungDua, Time.deltaTime * tocDoMuot);

        // 7.2 Xoay Camera theo con trỏ chuột mượt mà
        transform.rotation = Quaternion.Slerp(transform.rotation, gocXoayMucTieu, Time.deltaTime * tocDoMuot);

        // 7.3 Cập nhật Lens Field of View (Zoom)
        if (virtualCamera != null)
        {
            virtualCamera.Lens.FieldOfView = Mathf.Lerp(virtualCamera.Lens.FieldOfView, fovMucTieu, Time.deltaTime * tocDoMuot);
        }
    }
}