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

    [Tooltip("Layer đánh dấu các bề mặt được coi là mặt đất/bậc thang/trần nhà")]
    public LayerMask lopMatDat;

    [Header("--- Cấu hình Cầu Thang (Step Offset) ---")]
    [Tooltip("Chiều cao tối đa của bậc thang mà nhân vật có thể bước lên")]
    public float chieuCaoBacThangToidA = 0.5f;

    [Tooltip("Tốc độ đẩy nhân vật lên bậc thang")]
    public float tocDoLeoBacThang = 6f;

    [Header("--- Cấu hình Tư thế Ngồi & Bắt Khu Vực Hẹp ---")]
    [Tooltip("Chiều cao CapsuleCollider khi đứng")]
    public float chieuCaoDung = 2.0f;

    [Tooltip("Chiều cao CapsuleCollider khi ngồi")]
    public float chieuCaoNgoi = 1.0f;

    [Tooltip("Tốc độ chuyển đổi giữa đứng và ngồi")]
    public float tocDoChuyenTuThe = 8f;

    [Tooltip("Bán kính hình cầu kiểm tra trần nhà phía trên khi muốn đứng dậy")]
    public float banKinhKiemTraTran = 0.35f;

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

    private bool dangGiuPhimNgoi = false; // Trạng thái phím Ctrl
    private bool dangNhanNgoi = false;     // Trạng thái ngồi thực tế của nhân vật
    private bool dangNhanChay = false;
    private bool dangDaThietLapNhay = false;
    private bool kiemTraDanGiapDat = false;
    private bool dangBiVuongTran = false;  // Đánh dấu trần nhà bị kẹt

    // Biến thuộc tính khai báo công khai cho CameraController truy cập
    public bool DangDiChuyen => giaTriDiChuyenDauVao.sqrMagnitude > 0.01f;

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
        // 1. Kiểm tra vị trí chạm đất
        if (diemKiemTraChan != null)
        {
            kiemTraDanGiapDat = Physics.CheckSphere(diemKiemTraChan.position, banKinhKiemTraDat, lopMatDat);
        }
        else
        {
            kiemTraDanGiapDat = Physics.Raycast(transform.position, Vector3.down, 1.1f, lopMatDat);
        }

        // 2. KIỂM TRA TRẦN NHÀ PHÍA TRÊN (CEILING CHECK)
        KiemTraTranNhaPhiaTren();

        // 3. Xử lý tư thế ngồi & cập nhật Collider
        XuLyTuTheNgoi();

        XuLyTheLucVaTocDo();
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
        if (context.started || context.performed) dangGiuPhimNgoi = true;
        else if (context.canceled) dangGiuPhimNgoi = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) dangDaThietLapNhay = true;
    }
    #endregion

    // THUẬT TOÁN BẮT KHU VỰC HẸP PHÍA TRÊN ĐẦU
    private void KiemTraTranNhaPhiaTren()
    {
        if (vaChamNhanVat == null) return;

        // Vị trí quét từ đỉnh đầu của Collider hiện tại
        Vector3 viTriDau = transform.position + Vector3.up * (vaChamNhanVat.height - banKinhKiemTraTran);
        float khoangCachQuet = chieuCaoDung - vaChamNhanVat.height;

        if (khoangCachQuet > 0.01f)
        {
            dangBiVuongTran = Physics.SphereCast(viTriDau, banKinhKiemTraTran, Vector3.up, out _, khoangCachQuet, lopMatDat);
        }
        else
        {
            dangBiVuongTran = false;
        }

        // NẾU đè phím Ctrl HOẶC bị vướng trần -> Ép nhân vật ở trạng thái ngồi
        if (dangGiuPhimNgoi || dangBiVuongTran)
        {
            dangNhanNgoi = true;
        }
        else
        {
            dangNhanNgoi = false;
        }
    }

    private void XuLyTuTheNgoi()
    {
        if (vaChamNhanVat == null) return;

        // Chỉ thay đổi chiều cao height, giữ nguyên Center mặc định của Collider
        float chieuCaoMucTieu = dangNhanNgoi ? chieuCaoNgoi : chieuCaoDung;
        vaChamNhanVat.height = Mathf.Lerp(vaChamNhanVat.height, chieuCaoMucTieu, Time.deltaTime * tocDoChuyenTuThe);
    }

    private void XuLyTheLucVaTocDo()
    {
        if (dangNhanChay && DangDiChuyen && theLucHienTai > 0 && !dangNhanNgoi)
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
            else tocDoHienTai = tocDoDiBo;
        }
    }

    private void XuLyDiChuyen()
    {
        if (giaTriDiChuyenDauVao.sqrMagnitude >= 0.01f)
        {
            Vector3 huongTruocCam = cameraChinh != null ? cameraChinh.forward : Vector3.forward;
            Vector3 huongPhaiCam = cameraChinh != null ? cameraChinh.right : Vector3.right;

            huongTruocCam.y = 0f;
            huongPhaiCam.y = 0f;
            huongTruocCam.Normalize();
            huongPhaiCam.Normalize();

            Vector3 huongDiChuyenThucTe = (huongTruocCam * giaTriDiChuyenDauVao.y) + (huongPhaiCam * giaTriDiChuyenDauVao.x);

            Quaternion gocXoayMucTieu = Quaternion.LookRotation(huongDiChuyenThucTe);
            transform.rotation = Quaternion.Slerp(transform.rotation, gocXoayMucTieu, Time.fixedDeltaTime * tocDoXoayNhanVat);

            // Trượt tường
            RaycastHit hitTuong;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, huongDiChuyenThucTe, out hitTuong, 0.6f, lopMatDat))
            {
                if (Vector3.Angle(hitTuong.normal, Vector3.up) > 60f)
                {
                    huongDiChuyenThucTe = Vector3.ProjectOnPlane(huongDiChuyenThucTe, hitTuong.normal).normalized;
                }
            }

            Vector3 vanTocMucTieu = huongDiChuyenThucTe * tocDoHienTai;
            boDieuKhienNhanVat.linearVelocity = new Vector3(vanTocMucTieu.x, boDieuKhienNhanVat.linearVelocity.y, vanTocMucTieu.z);

            XuLyLeoCauThang(huongDiChuyenThucTe);
        }
        else
        {
            boDieuKhienNhanVat.linearVelocity = new Vector3(0f, boDieuKhienNhanVat.linearVelocity.y, 0f);
        }
    }

    private void XuLyLeoCauThang(Vector3 huongDiChuyen)
    {
        Vector3 viTriGocChan = transform.position + Vector3.up * 0.05f;
        Vector3 viTriGocDauGoi = transform.position + Vector3.up * chieuCaoBacThangToidA;

        if (Physics.Raycast(viTriGocChan, huongDiChuyen, out _, 0.6f, lopMatDat))
        {
            if (!Physics.Raycast(viTriGocDauGoi, huongDiChuyen, out _, 0.7f, lopMatDat))
            {
                boDieuKhienNhanVat.position += new Vector3(0f, tocDoLeoBacThang * Time.fixedDeltaTime, 0f);
            }
        }
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

    // Hiển thị vòng cầu quét kiểm tra trần trong tab Scene
    private void OnDrawGizmosSelected()
    {
        if (vaChamNhanVat == null) return;
        Gizmos.color = dangBiVuongTran ? Color.red : Color.yellow;
        Vector3 viTriDau = transform.position + Vector3.up * (vaChamNhanVat.height - banKinhKiemTraTran);
        float khoangCachQuet = chieuCaoDung - vaChamNhanVat.height;

        Gizmos.DrawWireSphere(viTriDau, banKinhKiemTraTran);
        if (khoangCachQuet > 0)
        {
            Gizmos.DrawWireSphere(viTriDau + Vector3.up * khoangCachQuet, banKinhKiemTraTran);
            Gizmos.DrawLine(viTriDau, viTriDau + Vector3.up * khoangCachQuet);
        }
    }
}