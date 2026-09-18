using UnityEngine;
using UnityEngine.InputSystem; // Sử dụng hệ thống Input System mới của Unity

public class CameraController : MonoBehaviour
{
    [Header("--- Cấu hình Camera TPS ---")]
    [Tooltip("Thành phần Transform của thân nhân vật để camera đi theo")]
    public Transform thanNhanVat;

    [Tooltip("Khoảng cách vị trí Camera so với nhân vật (Ví dụ: X=0, Y=2.5, Z=-4)")]
    public Vector3 khoangCachMuoctieu = new Vector3(0f, 2.5f, -4f);

    [Tooltip("Tốc độ camera đuổi theo nhân vật")]
    public float tocDoDuoiTheo = 10f;

    [Header("--- Cấu hình Nhích Theo Chuột (Parallax Shift) ---")]
    [Tooltip("Độ nhích tối đa của Camera theo con trỏ chuột")]
    public float doNhichToiDa = 1.5f;

    [Tooltip("Tốc độ nhích mượt của camera khi di chuyển chuột")]
    public float tocDoNhichMuot = 5f;

    // Biến lưu vị trí chuột hiện tại dạng Normalized (-1 đến 1)
    private Vector2 viTriChuotChuanHoa = Vector2.zero;

    // Biến lưu độ lệch vị trí camera tính toán được
    private Vector3 doLechNhichChuot = Vector3.zero;

    private void Start()
    {
        // Trong chế độ góc nhìn thứ 3 này, hiển thị con trỏ chuột tự do để nhích camera
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LateUpdate()
    {
        // Thực hiện cập nhật vị trí camera trong LateUpdate để tránh giật lag khung hình khi nhân vật di chuyển
        XuLyCameraDiTheoVaNhichChuot();
    }

    // Hàm nhận dữ liệu vị trí chuột từ Input System
    public void OnLook(InputAction.CallbackContext context)
    {
        // Đọc vị trí con trỏ chuột trên màn hình (đơn vị Pixel)
        Vector2 viTriChuotManHinh = context.ReadValue<Vector2>();

        // Chuyển đổi vị trí chuột sang tọa độ chuẩn hóa từ -1.0 đến 1.0 (Gốc 0,0 ở giữa màn hình)
        viTriChuotChuanHoa.x = (viTriChuotManHinh.x / Screen.width) * 2f - 1f;
        viTriChuotChuanHoa.y = (viTriChuotManHinh.y / Screen.height) * 2f - 1f;

        // Khóa giá trị chuẩn hóa trong khoảng -1 đến 1 để tránh chuột ra ngoài màn hình gây lỗi
        viTriChuotChuanHoa.x = Mathf.Clamp(viTriChuotChuanHoa.x, -1f, 1f);
        viTriChuotChuanHoa.y = Mathf.Clamp(viTriChuotChuanHoa.y, -1f, 1f);
    }

    // Logic chính xử lý camera đuổi theo nhân vật và nhích nhẹ theo chuột
    private void XuLyCameraDiTheoVaNhichChuot()
    {
        if (thanNhanVat == null) return;

        // 1. Tính toán vị trí gốc mà camera cần đến đằng sau nhân vật
        Vector3 viTriGocMucTieu = thanNhanVat.position + khoangCachMuoctieu;

        // 2. Tính toán độ lệch (Offset) dựa trên tọa độ chuột trên màn hình
        Vector3 doLechMucTieu = new Vector3(viTriChuotChuanHoa.x * doNhichToiDa, viTriChuotChuanHoa.y * (doNhichToiDa * 0.5f), 0f);

        // 3. Làm mượt độ lệch nhích chuột
        doLechNhichChuot = Vector3.Lerp(doLechNhichChuot, doLechMucTieu, Time.deltaTime * tocDoNhichMuot);

        // 4. Kết hợp vị trí gốc và độ lệch nhích chuột
        Vector3 viTriCuoiCung = viTriGocMucTieu + doLechNhichChuot;

        // 5. Di chuyển mượt mà camera tới vị trí cuối cùng bằng Lerp
        transform.position = Vector3.Lerp(transform.position, viTriCuoiCung, Time.deltaTime * tocDoDuoiTheo);

        // 6. Camera luôn luôn hướng mắt nhìn về phía nhân vật
        transform.LookAt(thanNhanVat.position + Vector3.up * 1.2f);
    }
}