using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("--- Thành phần tham chiếu ---")]
    [Tooltip("Thành phần Rigidbody để xử lý di chuyển vật lý")]
    public Rigidbody boDieuKhienNhanVat;

    [Tooltip("Thành phần CapsuleCollider để chỉnh chiều cao khi ngồi")]
    public CapsuleCollider vaChamNhanVat;

    [Tooltip("Thanh Slider hiển thị thể lực trên giao diện UI")]
    public Slider thanhTheLucUI;

    [Tooltip("Camera chính dùng để tính toán hướng di chuyển theo góc nhìn")]
    public Transform cameraChinh;

    [Header("--- Tốc độ di chuyển ---")]
    [Tooltip("Tốc độ di chuyển bình thường")]
    public float tocDoDiBo = 4f;

    [Tooltip("Tốc độ khi giữ Shift (Chạy nhanh)")]
    public float tocDoChay = 7f;

    [Tooltip("Tốc độ khi giữ Alt (Đi chậm)")]
    public float tocDoDiCham = 2f;

    [Tooltip("Tốc độ khi giữ Ctrl (Ngồi)")]
    public float tocDoNgoi = 1.5f;

    [Tooltip("Tốc độ xoay thân nhân vật theo hướng Camera")]
    public float tocDoXoayNhanVat = 12f;

    [Header("--- Cấu hình Nhảy & Kiểm tra chạm đất ---")]
    [Tooltip("Lực nhảy bộc phát áp dụng vào Rigidbody")]
    public float lucNhay = 5f;

    [Tooltip("Điểm kiểm tra vị trí chân nhân vật")]
    public Transform diemKiemTraChan;

    [Tooltip("Bán kính hình cầu kiểm tra chạm đất")]
    public float banKinhKiemTraDat = 0.2f;

    [Tooltip("Layer đánh dấu các bề mặt được coi là mặt đất")]
    public LayerMask lopMatDat;

    [Header("--- Cấu hình Tư thế Ngồi ---")]
    [Tooltip("Chiều cao CapsuleCollider khi đứng")]
    public float chieuCaoDung = 2.0f;

    [Tooltip("Chiều cao CapsuleCollider khi ngồi")]
    public float chieuCaoNgoi = 1.0f;

    [Tooltip("Tốc độ chuyển đổi giữa đứng và ngồi")]
    public float tocDoChuyenTuThe = 8f;

    [Header("--- Cấu hình Thể lực ---")]
    [Tooltip("Thể lực tối đa")]
    public float theLucToiDa = 100f;

    [Tooltip("Lượng thể lực tiêu hao mỗi giây khi chạy")]
    public float theLucTieuHaoMoiGiay = 20f;

    [Tooltip("Lượng thể lực hồi phục mỗi giây")]
    public float theLucHoiPhucMoiGiay = 15f;

    // --- Biến nội bộ quản lý trạng thái ---
    private float theLucHienTai;
    private float tocDoHienTai;
    private Vector2 giaTriDiChuyenDauVao;

    private bool dangNhanChay = false;
    private bool dangNhanNgoi = false;
    private bool dangNhanDiCham = false;
    private bool dangDaThietLapNhay = false;
    private bool kiemTraDanGiapDat = false;

    private void Start()
    {
        if (boDieuKhienNhanVat == null)
        {
            boDieuKhienNhanVat = GetComponent<Rigidbody>();
        }

        if (vaChamNhanVat == null)
        {
            vaChamNhanVat = GetComponent<CapsuleCollider>();
        }

        // Tự động tìm Main Camera nếu chưa kéo trong Inspector
        if (cameraChinh == null && Camera.main != null)
        {
            cameraChinh = Camera.main.transform;
        }

        if (boDieuKhienNhanVat != null)
        {
            boDieuKhienNhanVat.freezeRotation = true;
        }

        theLucHienTai = theLucToiDa;

        if (thanhTheLucUI != null)
        {
            thanhTheLucUI.maxValue = theLucToiDa;
            thanhTheLucUI.value = theLucHienTai;
        }
    }

    private void Update()
    {
        if (diemKiemTraChan != null)
        {
            kiemTraDanGiapDat = Physics.CheckSphere(diemKiemTraChan.position, banKinhKiemTraDat, lopMatDat);
        }
        else
        {
            kiemTraDanGiapDat = Physics.Raycast(transform.position, Vector3.down, 1.1f, lopMatDat);
        }

        XuLyTheLucVaTocDo();
        XuLyTudTheNgoi();
        CapNhatGiaoDienUI();
    }

    private void FixedUpdate()
    {
        XuLyDiChuyen();
        XuLyNhay();
    }

    #region Input Events
    public void OnMove(InputAction.CallbackContext context)
    {
        giaTriDiChuyenDauVao = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started || context.performed) dangNhanChay = true;
        else if (context.canceled) dangNhanChay = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started || context.performed) dangNhanNgoi = true;
        else if (context.canceled) dangNhanNgoi = false;
    }

    public void OnWalkSlow(InputAction.CallbackContext context)
    {
        if (context.started || context.performed) dangNhanDiCham = true;
        else if (context.canceled) dangNhanDiCham = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) dangDaThietLapNhay = true;
    }
    #endregion

    private void XuLyTheLucVaTocDo()
    {
        bool dangDiChuyen = giaTriDiChuyenDauVao.sqrMagnitude > 0.01f;

        if (dangNhanChay && dangDiChuyen && theLucHienTai > 0 && !dangNhanNgoi)
        {
            tocDoHienTai = tocDoChay;
            theLucHienTai -= theLucTieuHaoMoiGiay * Time.deltaTime;
            if (theLucHienTai < 0) theLucHienTai = 0;
        }
        else
        {
            if (theLucHienTai < theLucToiDa)
            {
                theLucHienTai += theLucHoiPhucMoiGiay * Time.deltaTime;
                if (theLucHienTai > theLucToiDa) theLucHienTai = theLucToiDa;
            }

            if (dangNhanNgoi) tocDoHienTai = tocDoNgoi;
            else if (dangNhanDiCham) tocDoHienTai = tocDoDiCham;
            else tocDoHienTai = tocDoDiBo;
        }
    }

    // 🎯 THUẬT TOÁN DI CHUYỂN PHỤ THUỘC VÀO HUỚNG TẠO BỞI CAMERA
    private void XuLyDiChuyen()
    {
        if (giaTriDiChuyenDauVao.sqrMagnitude >= 0.01f)
        {
            // 1. Lấy hướng Phía Trước (Forward) và Bên Phải (Right) của Camera
            Vector3 huongTruocCam = cameraChinh != null ? cameraChinh.forward : Vector3.forward;
            Vector3 huongPhaiCam = cameraChinh != null ? cameraChinh.right : Vector3.right;

            // Triệt tiêu trục Y để nhân vật không bị di chuyển cắm đầu xuống đất hoặc bay lên trời khi nhìn lên/xuống
            huongTruocCam.y = 0f;
            huongPhaiCam.y = 0f;

            huongTruocCam.Normalize();
            huongPhaiCam.Normalize();

            // 2. Tính toán Vector hướng di chuyển thực tế dựa theo góc nhìn Camera và phím bấm WASD
            Vector3 huongDiChuyenThucTe = (huongTruocCam * giaTriDiChuyenDauVao.y) + (huongPhaiCam * giaTriDiChuyenDauVao.x);

            // 3. Xoay mặt nhân vật mượt mà về hướng di chuyển đó
            Quaternion gocXoayMucTieu = Quaternion.LookRotation(huongDiChuyenThucTe);
            transform.rotation = Quaternion.Slerp(transform.rotation, gocXoayMucTieu, Time.fixedDeltaTime * tocDoXoayNhanVat);

            // 4. Áp dụng vận tốc cho Rigidbody
            Vector3 vanTocMucTieu = huongDiChuyenThucTe * tocDoHienTai;
            boDieuKhienNhanVat.linearVelocity = new Vector3(vanTocMucTieu.x, boDieuKhienNhanVat.linearVelocity.y, vanTocMucTieu.z);
        }
        else
        {
            // Khi không nhấn phím di chuyển, dừng vận tốc ngang lại (giữ nguyên vận tốc trọng lực Y)
            boDieuKhienNhanVat.linearVelocity = new Vector3(0f, boDieuKhienNhanVat.linearVelocity.y, 0f);
        }
    }

    private void XuLyTudTheNgoi()
    {
        if (vaChamNhanVat == null) return;
        float chieuCaoMucTieu = dangNhanNgoi ? chieuCaoNgoi : chieuCaoDung;
        vaChamNhanVat.height = Mathf.Lerp(vaChamNhanVat.height, chieuCaoMucTieu, Time.deltaTime * tocDoChuyenTuThe);
    }

    private void XuLyNhay()
    {
        if (dangDaThietLapNhay)
        {
            if (kiemTraDanGiapDat && !dangNhanNgoi)
            {
                boDieuKhienNhanVat.linearVelocity = new Vector3(boDieuKhienNhanVat.linearVelocity.x, 0f, boDieuKhienNhanVat.linearVelocity.z);
                boDieuKhienNhanVat.AddForce(Vector3.up * lucNhay, ForceMode.Impulse);
            }
            dangDaThietLapNhay = false;
        }
    }

    private void CapNhatGiaoDienUI()
    {
        if (thanhTheLucUI != null)
        {
            thanhTheLucUI.value = theLucHienTai;
        }
    }
}