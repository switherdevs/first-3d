using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("--- Tham chiếu Nhân vật ---")]
    [Tooltip("Thành phần Transform của Player để Camera đi theo")]
    public Transform thanNhanVat;

    [Header("--- Cấu hình Khoảng cách & Điểm nhìn ---")]
    [Tooltip("Khoảng cách mặc định từ Camera tới nhân vật")]
    public float khoangCach = 4f;

    [Tooltip("Khoảng cách tối thiểu khi Camera bị đẩy sát vào người")]
    public float khoangCachToiThieu = 0.8f;

    [Tooltip("Chiều cao điểm nhìn trên thân nhân vật (Ngang ngực/vai)")]
    public float chieuCaoDiemNhin = 1.5f;

    [Tooltip("Độ lệch ngang qua vai (Để = 0 nếu muốn cam ở chính giữa sau lưng)")]
    public float doLechGocVai = 0.5f;

    [Header("--- Cấu hình Va chạm Tường (Camera Collision) ---")]
    [Tooltip("Tag của tường/vật cản cần né")]
    public string tagTuong = "Wall";

    [Tooltip("Các Layer vật cản mà Camera không được xuyên qua")]
    public LayerMask lopVatCan;

    [Tooltip("Bán kính hình cầu kiểm tra va chạm của Camera")]
    public float banKinhKiemTraCam = 0.2f;

    [Tooltip("Thời gian làm mượt khi camera thu/phóng do va chạm tường (càng nhỏ càng nhanh)")]
    public float thoiGianThuPhongNeTuong = 0.05f;

    [Header("--- Cấu hình Độ nhạy Chuột ---")]
    [Tooltip("Độ nhạy xoay ngang (Chuột X)")]
    public float doNhayNgang = 3f;

    [Tooltip("Độ nhạy xoay dọc (Chuột Y)")]
    public float doNhayDoc = 3f;

    [Tooltip("Góc nhìn xuống thấp nhất (Độ)")]
    public float gocNhinThapNhat = -20f;

    [Tooltip("Góc nhìn lên cao nhất (Độ)")]
    public float gocNhinCaoNhat = 60f;

    // Biến lưu trữ góc xoay & khoảng cách thực tế
    private float gocXoayNgang = 0f;
    private float gocXoayDoc = 10f;
    private float khoangCachHienTai;
    private float vanTocThuPhongBoDem; // Biến phụ trợ cho SmoothDamp

    private void Start()
    {
        // Khóa con trỏ chuột vào giữa màn hình để xoay camera mượt mà
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Lấy góc xoay ban đầu của Camera
        Vector3 gocHienTai = transform.eulerAngles;
        gocXoayNgang = gocHienTai.y;
        gocXoayDoc = gocHienTai.x;

        khoangCachHienTai = khoangCach;
    }

    private void LateUpdate()
    {
        XuLyXoayCameraTheoChuot();
    }

    // Logic xoay camera phản hồi tức thì, không độ trễ
    private void XuLyXoayCameraTheoChuot()
    {
        if (thanNhanVat == null) return;

        // 1. Đọc tín hiệu di chuyển của chuột
        float chuotX = Input.GetAxis("Mouse X") * doNhayNgang;
        float chuotY = Input.GetAxis("Mouse Y") * doNhayDoc;

        // 2. Cập nhật góc xoay
        gocXoayNgang += chuotX;
        gocXoayDoc -= chuotY; // Trừ Y để không bị ngược hướng nhìn lên/xuống

        // 3. Giới hạn góc nhìn lên/xuống tránh lật ngửa camera
        gocXoayDoc = Mathf.Clamp(gocXoayDoc, gocNhinThapNhat, gocNhinCaoNhat);

        // 4. Tính toán góc xoay Quaternion
        Quaternion huongXoay = Quaternion.Euler(gocXoayDoc, gocXoayNgang, 0f);

        // 5. Xác định điểm mục tiêu nhìn trên thân Player (kèm lệch vai)
        Vector3 diemNhin = thanNhanVat.position + Vector3.up * chieuCaoDiemNhin + (huongXoay * Vector3.right * doLechGocVai);

        // 6. XỬ LÝ NÉ TƯỜNG (CAMERA COLLISION)
        float khoangCachMucTieu = khoangCach;
        Vector3 huongLuiCam = -(huongXoay * Vector3.forward);

        // Bắn 1 hình cầu từ điểm nhìn lùi về phía vị trí mong muốn của Camera
        if (Physics.SphereCast(diemNhin, banKinhKiemTraCam, huongLuiCam, out RaycastHit hitInfo, khoangCach, lopVatCan))
        {
            // Kiểm tra xem vật cản có phải Tag Wall hoặc nằm trong Layer cản không
            if (string.IsNullOrEmpty(tagTuong) || hitInfo.collider.CompareTag(tagTuong))
            {
                // Thu ngắn khoảng cách lại đúng bằng điểm va chạm
                khoangCachMucTieu = Mathf.Clamp(hitInfo.distance, khoangCachToiThieu, khoangCach);
            }
        }

        // Xử lý thu/phóng khoảng cách mượt mà và không giật lắc bằng SmoothDamp
        khoangCachHienTai = Mathf.SmoothDamp(khoangCachHienTai, khoangCachMucTieu, ref vanTocThuPhongBoDem, thoiGianThuPhongNeTuong);

        // 7. Tính vị trí Camera chính xác tức thì
        Vector3 viTriTarget = diemNhin - (huongXoay * Vector3.forward * khoangCachHienTai);

        // 8. Cập nhật trực tiếp vị trí và góc xoay (Loại bỏ Vector3.Lerp vị trí để xóa bỏ độ trễ)
        transform.position = viTriTarget;
        transform.rotation = huongXoay;
    }
}