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

    [Header("--- BỔ SUNG: Cấu hình Đung Đưa Góc Xoay Camera (Rotation Bobbing) ---")]
    [Tooltip("Bật/Tắt tính năng đung đưa góc xoay Camera")]
    public bool kichHoatDungDua = true;

    [Tooltip("Tốc độ đung đưa (Càng cao nhịp lắc càng nhanh)")]
    public float tocDoDungDua = 4f;

    [Tooltip("Biên độ đung đưa theo Trục X - Pitch (Gật lên/xuống theo độ °)")]
    public float bienDoGocX = 0.8f;

    [Tooltip("Biên độ đung đưa theo Trục Y - Yaw (Lắc trái/phải theo độ °)")]
    public float bienDoGocY = 0.5f;

    // Biến lưu trữ góc xoay & khoảng cách thực tế
    private float gocXoayNgang = 0f;
    private float gocXoayDoc = 10f;
    private float khoangCachHienTai;
    private float vanTocThuPhongBoDem; // Biến phụ trợ cho SmoothDamp
    private float thoiGianDungDua = 0f; // Biến đếm thời gian sóng Sin/Cos

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

    // Logic xoay camera phản hồi tức thì, giữ cố định khoảng cách khi di chuyển
    private void XuLyXoayCameraTheoChuot()
    {
        if (thanNhanVat == null) return;

        // 1. Đọc tín hiệu di chuyển của chuột
        float chuotX = Input.GetAxis("Mouse X") * doNhayNgang;
        float chuotY = Input.GetAxis("Mouse Y") * doNhayDoc;

        // 2. Cập nhật góc xoay gốc từ chuột
        gocXoayNgang += chuotX;
        gocXoayDoc -= chuotY; // Trừ Y để không bị ngược hướng nhìn lên/xuống

        // 3. Giới hạn góc nhìn lên/xuống tránh lật ngửa camera
        gocXoayDoc = Mathf.Clamp(gocXoayDoc, gocNhinThapNhat, gocNhinCaoNhat);

        // Biến tạm lưu góc xoay dùng để tính toán Quaternion
        float gocDocThucTe = gocXoayDoc;
        float gocNgangThucTe = gocXoayNgang;

        // 4. BỔ SUNG: XỬ LÝ ĐUNG ĐƯA ROTATION (GÓC XOAY)
        if (kichHoatDungDua)
        {
            thoiGianDungDua += Time.deltaTime * tocDoDungDua;

            // Đung đưa xoay gật Lên / Xuống (Trục X)
            float doLechGocX = Mathf.Cos(thoiGianDungDua * 2f) * bienDoGocX;

            // Đung đưa xoay lắc Trái / Phải (Trục Y)
            float doLechGocY = Mathf.Sin(thoiGianDungDua) * bienDoGocY;

            // Cộng độ lệch độ (° ) vào góc xoay thực tế
            gocDocThucTe += doLechGocX;
            gocNgangThucTe += doLechGocY;
        }

        // 5. Tính toán góc xoay Quaternion đã kèm đung đưa
        Quaternion huongXoay = Quaternion.Euler(gocDocThucTe, gocNgangThucTe, 0f);

        // 6. Xác định điểm mục tiêu nhìn trên thân Player (kèm lệch vai)
        Vector3 diemNhin = thanNhanVat.position + Vector3.up * chieuCaoDiemNhin + (huongXoay * Vector3.right * doLechGocVai);

        // 7. XỬ LÝ NÉ TƯỜNG (CAMERA COLLISION)
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

        // Xử lý thu/phóng khoảng cách mượt mà khi chạm tường bằng SmoothDamp
        khoangCachHienTai = Mathf.SmoothDamp(khoangCachHienTai, khoangCachMucTieu, ref vanTocThuPhongBoDem, thoiGianThuPhongNeTuong);

        // 8. Tính vị trí Camera chính xác
        Vector3 viTriTarget = diemNhin - (huongXoay * Vector3.forward * khoangCachHienTai);

        // 9. Cập nhật vị trí và góc xoay cuối cùng cho Camera
        transform.position = viTriTarget;
        transform.rotation = huongXoay;
    }
}