using UnityEngine;
using UnityEngine.UI; // Dùng để liên kết với UI Slider thanh thể lực
using UnityEngine.InputSystem; // Hệ thống Input System mới của Unity

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("--- Thành phần tham chiếu ---")]
    [Tooltip("Thành phần CharacterController để xử lý va chạm và di chuyển")]
    public CharacterController boDieuKhienNhanVat;

    [Tooltip("Thanh Slider hiển thị thể lực trên giao diện UI")]
    public Slider thanhTheLucUI;

    [Header("--- Tốc độ di chuyển ---")]
    [Tooltip("Tốc độ di chuyển bình thường (Mặc định)")]
    public float tocDoDiBo = 4f;

    [Tooltip("Tốc độ khi giữ Shift (Chạy nhanh)")]
    public float tocDoChay = 7f;

    [Tooltip("Tốc độ khi giữ Alt (Đi chậm/Rón rén)")]
    public float tocDoDiCham = 2f;

    [Tooltip("Tốc độ khi giữ Ctrl (Ngồi)")]
    public float tocDoNgoi = 1.5f;

    [Tooltip("Tốc độ xoay thân nhân vật theo hướng di chuyển TPS")]
    public float tocDoXoayNhanVat = 12f;

    [Header("--- Cấu hình Nhảy & Trọng lực ---")]
    [Tooltip("Lực nhảy của nhân vật")]
    public float lucNhay = 1.5f;

    [Tooltip("Gia tốc trọng lực kéo nhân vật xuống")]
    public float trongLuc = -9.81f;

    [Header("--- Cấu hình Tư thế Ngồi (Crouch) ---")]
    [Tooltip("Chiều cao nhân vật khi đứng")]
    public float chieuCaoDung = 2.0f;

    [Tooltip("Chiều cao nhân vật khi ngồi")]
    public float chieuCaoNgoi = 1.0f;

    [Tooltip("Tốc độ chuyển đổi giữa đứng và ngồi")]
    public float tocDoChuyenTuThe = 8f;

    [Header("--- Cấu hình Thể lực (Stamina) ---")]
    [Tooltip("Thể lực tối đa")]
    public float theLucToiDa = 100f;

    [Tooltip("Lượng thể lực tiêu hao mỗi giây khi chạy")]
    public float theLucTieuHaoMoiGiay = 20f;

    [Tooltip("Lượng thể lực hồi phục mỗi giây khi không chạy")]
    public float theLucHoiPhucMoiGiay = 15f;

    // --- Biến nội bộ quản lý trạng thái (Giữ nguyên không đổi tên) ---
    private float theLucHienTai;
    private float tocDoHienTai;
    private Vector3 vanTocTrongLuc; // Vận tốc rơi tự do
    private Vector2 giaTriDiChuyenDauVao; // Dữ liệu phím WASD

    private bool dangNhanChay = false;
    private bool dangNhanNgoi = false;
    private bool dangNhanDiCham = false;
    private bool dangDaThietLapNhay = false;
    private bool kiemTraDanGiapDat = false;

    private void Start()
    {
        // Lấy component CharacterController nếu chưa kéo vào Inspector
        if (boDieuKhienNhanVat == null)
        {
            boDieuKhienNhanVat = GetComponent<CharacterController>();
        }

        // Khởi tạo thể lực ban đầu
        theLucHienTai = theLucToiDa;

        // Khởi tạo UI Slider
        if (thanhTheLucUI != null)
        {
            thanhTheLucUI.maxValue = theLucToiDa;
            thanhTheLucUI.value = theLucHienTai;
        }
    }

    private void Update()
    {
        // 1. Kiểm tra trạng thái chạm đất
        kiemTraDanGiapDat = boDieuKhienNhanVat.isGrounded;
        if (kiemTraDanGiapDat && vanTocTrongLuc.y < 0)
        {
            // Đặt vận tốc rơi nhỏ để đảm bảo nhân vật bám sát mặt đất
            vanTocTrongLuc.y = -2f;
        }

        // 2. Xử lý thể lực và xác định tốc độ di chuyển
        XuLyTheLucVaTocDo();

        // 3. Xử lý di chuyển & xoay hướng nhân vật chuẩn TPS
        XuLyDiChuyen();

        // 4. Xử lý tư thế Ngồi (Crouch)
        XuLyTudTheNgoi();

        // 5. Xử lý Nhảy
        XuLyNhay();

        // 6. Áp dụng trọng lực rơi
        vanTocTrongLuc.y += trongLuc * Time.deltaTime;
        boDieuKhienNhanVat.Move(vanTocTrongLuc * Time.deltaTime);

        // 7. Cập nhật giao diện UI
        CapNhatGiaoDienUI();
    }

    #region Các Hàm Nhận Input Từ Input System Mới
    // Nhận sự kiện phím WASD
    public void OnMove(InputAction.CallbackContext context)
    {
        giaTriDiChuyenDauVao = context.ReadValue<Vector2>();
    }

    // Nhận sự kiện phím Shift (Chạy)
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
            dangNhanChay = true;
        else if (context.canceled)
            dangNhanChay = false;
    }

    // Nhận sự kiện phím Ctrl (Ngồi)
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
            dangNhanNgoi = true;
        else if (context.canceled)
            dangNhanNgoi = false;
    }

    // Nhận sự kiện phím Alt (Đi chậm)
    public void OnWalkSlow(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
            dangNhanDiCham = true;
        else if (context.canceled)
            dangNhanDiCham = false;
    }

    // Nhận sự kiện phím Space (Nhảy)
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            dangDaThietLapNhay = true;
        }
    }
    #endregion

    // Logic tính toán tốc độ di chuyển và trừ/hồi thể lực
    private void XuLyTheLucVaTocDo()
    {
        bool dangDiChuyen = giaTriDiChuyenDauVao.sqrMagnitude > 0.01f;

        // Nếu người dùng giữ Shift, có di chuyển và còn thể lực -> Cho phép chạy
        if (dangNhanChay && dangDiChuyen && theLucHienTai > 0 && !dangNhanNgoi)
        {
            tocDoHienTai = tocDoChay;
            // Trừ thể lực theo thời gian
            theLucHienTai -= theLucTieuHaoMoiGiay * Time.deltaTime;
            if (theLucHienTai < 0) theLucHienTai = 0;
        }
        else
        {
            // Tự động hồi phục thể lực nếu không chạy
            if (theLucHienTai < theLucToiDa)
            {
                theLucHienTai += theLucHoiPhucMoiGiay * Time.deltaTime;
                if (theLucHienTai > theLucToiDa) theLucHienTai = theLucToiDa;
            }

            // Phân bổ tốc độ dựa trên tư thế
            if (dangNhanNgoi)
            {
                tocDoHienTai = tocDoNgoi;
            }
            else if (dangNhanDiCham)
            {
                tocDoHienTai = tocDoDiCham;
            }
            else
            {
                tocDoHienTai = tocDoDiBo;
            }
        }
    }

    // Logic di chuyển thực tế bằng CharacterController và xoay nhân vật theo hướng di chuyển (TPS Style)
    private void XuLyDiChuyen()
    {
        // Tạo hướng di chuyển 3D trên mặt phẳng XZ từ phím bấm WASD
        Vector3 huongDiChuyen = new Vector3(giaTriDiChuyenDauVao.x, 0f, giaTriDiChuyenDauVao.y).normalized;

        // Kiểm tra xem người chơi có đang bấm phím di chuyển không
        if (huongDiChuyen.magnitude >= 0.1f)
        {
            // Tính góc xoay target theo hướng bấm phím
            float gocXoayMucTieu = Mathf.Atan2(huongDiChuyen.x, huongDiChuyen.z) * Mathf.Rad2Deg;

            // Xoay thân nhân vật mượt mà về hướng di chuyển bằng Quaternion.Slerp
            Quaternion gocXoayMoi = Quaternion.Euler(0f, gocXoayMucTieu, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, gocXoayMoi, Time.deltaTime * tocDoXoayNhanVat);

            // Thực hiện di chuyển nhân vật tới hướng đó
            boDieuKhienNhanVat.Move(huongDiChuyen * tocDoHienTai * Time.deltaTime);
        }
    }

    // Logic thay đổi chiều cao CharacterController khi ngồi
    private void XuLyTudTheNgoi()
    {
        float chieuCaoMucTieu = dangNhanNgoi ? chieuCaoNgoi : chieuCaoDung;
        
        // Mượt mà thay đổi chiều cao nhân vật
        boDieuKhienNhanVat.height = Mathf.Lerp(boDieuKhienNhanVat.height, chieuCaoMucTieu, Time.deltaTime * tocDoChuyenTuThe);
    }

    // Logic thực hiện cú nhảy
    private void XuLyNhay()
    {
        if (dangDaThietLapNhay)
        {
            // Chỉ cho nhảy khi đang chạm đất và không ở tư thế ngồi
            if (kiemTraDanGiapDat && !dangNhanNgoi)
            {
                // Công thức tính vận tốc nhảy theo độ cao mong muốn: v = sqrt(h * -2 * g)
                vanTocTrongLuc.y = Mathf.Sqrt(lucNhay * -2f * trongLuc);
            }
            dangDaThietLapNhay = false; // Reset cờ nhảy
        }
    }

    // Cập nhật Slider UI
    private void CapNhatGiaoDienUI()
    {
        if (thanhTheLucUI != null)
        {
            thanhTheLucUI.value = theLucHienTai;
        }
    }
}