using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("--- Tham chiếu Nhân vật ---")]
    [Tooltip("Thành phần Transform của Player để Camera đi theo")]
    public Transform thanNhanVat;

    [Header("--- Cấu hình Khoảng cách & Điểm nhìn ---")]
    [Tooltip("Khoảng cách từ Camera tới nhân vật")]
    public float khoangCach = 4f;

    [Tooltip("Chiều cao điểm nhìn trên thân nhân vật")]
    public float chieuCaoDiemNhin = 1.5f;

    [Tooltip("Độ lệch ngang qua vai (Space Marine 2 thường là 0.4 - 0.6)")]
    public float doLechGocVai = 0.5f;

    [Header("--- Cấu hình Tốc độ & Độ nhạy ---")]
    [Tooltip("Độ nhạy xoay ngang (Chuột X) - Tăng nếu muốn xoay nhanh hơn")]
    public float doNhayNgang = 0.3f;

    [Tooltip("Độ nhạy xoay dọc (Chuột Y) - Tăng nếu muốn xoay nhanh hơn")]
    public float doNhayDoc = 0.3f;

    [Tooltip("Góc nhìn xuống thấp nhất (Độ)")]
    public float gocNhinThapNhat = -20f;

    [Tooltip("Góc nhìn lên cao nhất (Độ)")]
    public float gocNhinCaoNhat = 60f;

    [Tooltip("Bật tùy chọn này để Camera dính chặt 100% không độ trễ")]
    public bool dinhChatKhongDoTre = true;

    [Tooltip("Tốc độ bám mượt (Chỉ có tác dụng khi 'dinhChatKhongDoTre' tắt)")]
    public float tocDoBamMuot = 50f;

    // Biến lưu trữ góc xoay
    private float gocXoayNgang = 0f;
    private float gocXoayDoc = 10f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 gocHienTai = transform.eulerAngles;
        gocXoayNgang = gocHienTai.y;
        gocXoayDoc = gocHienTai.x;
    }

    private void LateUpdate()
    {
        XuLyXoayCameraTheoChuot();
    }

    private void XuLyXoayCameraTheoChuot()
    {
        if (thanNhanVat == null) return;

        // 1. Đọc di chuyển chuột từ New Input System
        Vector2 giaTriChuot = Vector2.zero;
        if (Mouse.current != null)
        {
            giaTriChuot = Mouse.current.delta.ReadValue();
        }

        // Tốc độ di chuyển Camera được tùy chỉnh trực tiếp qua doNhayNgang và doNhayDoc
        float chuotX = giaTriChuot.x * doNhayNgang;
        float chuotY = giaTriChuot.y * doNhayDoc;

        // 2. Cập nhật góc xoay
        gocXoayNgang += chuotX;
        gocXoayDoc -= chuotY;

        // 3. Khóa giới hạn góc nhìn lên/xuống
        gocXoayDoc = Mathf.Clamp(gocXoayDoc, gocNhinThapNhat, gocNhinCaoNhat);

        // 4. Tính toán hướng xoay Quaternion
        Quaternion huongXoay = Quaternion.Euler(gocXoayDoc, gocXoayNgang, 0f);

        // 5. Xác định điểm mục tiêu nhìn trên thân Player
        Vector3 diemNhin = thanNhanVat.position + Vector3.up * chieuCaoDiemNhin + (huongXoay * Vector3.right * doLechGocVai);

        // 6. Tính vị trí mục tiêu cho Camera
        Vector3 viTriTarget = diemNhin - (huongXoay * Vector3.forward * khoangCach);

        // 7. Xử lý di chuyển Camera (Dính chặt hoặc Làm mượt nhẹ)
        if (dinhChatKhongDoTre)
        {
            // Gán trực tiếp vị trí -> Triệt tiêu 100% độ trễ (Zero Delay)
            transform.position = viTriTarget;
        }
        else
        {
            // Tốc độ bám mượt cao (tocDoBamMuot = 50f ~ 100f)
            transform.position = Vector3.Lerp(transform.position, viTriTarget, Time.deltaTime * tocDoBamMuot);
        }

        transform.rotation = huongXoay;
    }
}