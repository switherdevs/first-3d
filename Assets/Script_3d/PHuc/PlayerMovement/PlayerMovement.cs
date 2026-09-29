using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("--- Thành phần tham chiếu ---")]
    [Tooltip("Thành phần Rigidbody để xử lý di chuyển vật lý")]
    public Rigidbody boDieuKhienNhanVat;

    [Tooltip("Thành phần CapsuleCollider để chỉnh chiều cao khi ngồi")]
    public CapsuleCollider vaChamNhanVat;

    [Tooltip("Model Mesh hiển thị nhân vật để co giãn Scale Y riêng biệt")]
    public Transform moHinhNhanVat;

    [Tooltip("Thanh Slider hiển thị thể lực trên giao diện UI")]
    public Slider thanhTheLucUI;

    [Tooltip("Camera chính dùng để tính toán hướng di chuyển theo góc nhìn")]
    public Transform cameraChinh;

    [Tooltip("Thành phần AudioSource để phát âm thanh bước chân")]
    public AudioSource nguonAmThanh;

    [Tooltip("File âm thanh tiếng bước chân dùng chung")]
    public AudioClip amThanhBuocChan;

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

    [Header("--- Kiểm tra Trần Nhà Độc Lập ---")]
    [Tooltip("Transform điểm check trần (đặt ở vị trí đỉnh đầu khi đứng)")]
    public Transform diemKiemTraTran;

    [Tooltip("Bán kính quả cầu kiểm tra vật cản ở đỉnh đầu")]
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
    private float scaleYMoHinhGoc = 1f;

    private bool dangGiuPhimNgoi = false;
    private bool dangNhanNgoi = false;
    private bool dangNhanChay = false;
    private bool dangDaThietLapNhay = false;
    private bool kiemTraDanGiapDat = false;
    private bool dangBiVuongTran = false;

    // Biến đếm thời gian cho nhịp phát âm thanh bước chân
    private float demThoiGianBuocChan = 0f;

    private Collider[] vaChamKiemTraTranBuffer = new Collider[5];

    public bool DangDiChuyen => giaTriDiChuyenDauVao.sqrMagnitude > 0.01f;

    private void Start()
    {
        if (boDieuKhienNhanVat == null) boDieuKhienNhanVat = GetComponent<Rigidbody>();
        if (vaChamNhanVat == null) vaChamNhanVat = GetComponent<CapsuleCollider>();
        if (nguonAmThanh == null) nguonAmThanh = GetComponent<AudioSource>();

        if (cameraChinh == null && Camera.main != null) cameraChinh = Camera.main.transform;

        if (boDieuKhienNhanVat != null) boDieuKhienNhanVat.freezeRotation = true;
        if (moHinhNhanVat != null) scaleYMoHinhGoc = moHinhNhanVat.localScale.y;

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

        // 2. Kiểm tra trần nhà
        KiemTraTranNhaPhiaTren();

        // 3. Xử lý tư thế ngồi & âm thanh
        XuLyTuTheNgoi();
        XuLyTheLucVaTocDo();
        XuLyAmThanhBuocChan();
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

    private Vector3 LayViTriKiemTraTran()
    {
        if (diemKiemTraTran != null) return diemKiemTraTran.position;
        return transform.position + Vector3.up * (chieuCaoDung * 0.8f);
    }

    private void KiemTraTranNhaPhiaTren()
    {
        Vector3 viTriCheck = LayViTriKiemTraTran();
        dangBiVuongTran = false;

        int soLuongVaCham = Physics.OverlapSphereNonAlloc(viTriCheck, banKinhKiemTraTran, vaChamKiemTraTranBuffer, lopMatDat);

        for (int i = 0; i < soLuongVaCham; i++)
        {
            if (vaChamKiemTraTranBuffer[i] != vaChamNhanVat)
            {
                dangBiVuongTran = true;
                break;
            }
        }

        if (dangGiuPhimNgoi || dangBiVuongTran) dangNhanNgoi = true;
        else dangNhanNgoi = false;
    }

    private void XuLyTuTheNgoi()
    {
        if (vaChamNhanVat == null) return;

        float chieuCaoMucTieu = dangNhanNgoi ? chieuCaoNgoi : chieuCaoDung;
        vaChamNhanVat.height = Mathf.Lerp(vaChamNhanVat.height, chieuCaoMucTieu, Time.deltaTime * tocDoChuyenTuThe);
        vaChamNhanVat.center = Vector3.zero;

        if (moHinhNhanVat != null)
        {
            float tiLeScale = vaChamNhanVat.height / chieuCaoDung;
            Vector3 scaleHienTai = moHinhNhanVat.localScale;
            moHinhNhanVat.localScale = new Vector3(scaleHienTai.x, scaleYMoHinhGoc * tiLeScale, scaleHienTai.z);
        }
    }

    // TỐI ƯU ÂM THANH BƯỚC CHÂN CHẠY VÀ ĐI BỘ DÙNG CHUNG 1 CLIP
    private void XuLyAmThanhBuocChan()
    {
        // Chỉ phát âm thanh khi nhân vật đang di chuyển thực sự trên mặt đất
        if (DangDiChuyen && kiemTraDanGiapDat)
        {
            demThoiGianBuocChan -= Time.deltaTime;

            if (demThoiGianBuocChan <= 0f)
            {
                float thoiGianKhoangCach;
                float doCaoPitch;
                float amLuongVolume;

                // TH1: Đang chạy nhanh
                if (tocDoHienTai == tocDoChay)
                {
                    thoiGianKhoangCach = 0.28f; // Nhịp bước chân dồn dập (ngắn hơn)
                    doCaoPitch = 1.25f;         // Tăng pitch giúp tiếng bước nhanh và giật hơn
                    amLuongVolume = 1.0f;       // Âm thanh to, đầm chân
                }
                // TH2: Đang ngồi (Rón rén)
                else if (dangNhanNgoi)
                {
                    thoiGianKhoangCach = 0.65f; // Nhịp bước chân chậm rãi
                    doCaoPitch = 0.85f;         // Tông giọng trầm hơn
                    amLuongVolume = 0.35f;      // Nhỏ nhẹ
                }
                // TH3: Đi bộ bình thường
                else
                {
                    thoiGianKhoangCach = 0.45f; // Nhịp tiêu chuẩn
                    doCaoPitch = 1.0f;          // Âm gốc
                    amLuongVolume = 0.7f;
                }

                // Áp dụng các thông số đã tính toán vào AudioSource
                if (nguonAmThanh != null && amThanhBuocChan != null)
                {
                    nguonAmThanh.pitch = doCaoPitch;
                    nguonAmThanh.PlayOneShot(amThanhBuocChan, amLuongVolume);
                }

                // Reset timer cho nhịp bước chân tiếp theo
                demThoiGianBuocChan = thoiGianKhoangCach;
            }
        }
        else
        {
            // Reset timer về 0 để khi vừa di chuyển là phát ngay tiếng bước chân đầu tiên
            demThoiGianBuocChan = 0f;
        }
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
            if (kiemTraDanGiapDat && !dangNhanNgoi && !dangBiVuongTran)
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

    private void OnDrawGizmosSelected()
    {
        Vector3 viTriCheck = LayViTriKiemTraTran();
        Gizmos.color = dangBiVuongTran ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(viTriCheck, banKinhKiemTraTran);
    }
}