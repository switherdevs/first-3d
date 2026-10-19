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

    [Tooltip("Camera chính trong Game (Nếu để trống script sẽ tự tìm MainCamera)")]
    public Transform cameraChinh;

    [Header("--- Animator Tham Chiếu ---")]
    [Tooltip("Animator của nhân vật để điều khiển hoạt ảnh")]
    public Animator animatorNhanVat;

    [Header("--- Hệ thống Âm thanh Đa Lớp (2 AudioSources) ---")]
    [Tooltip("Nguồn âm thanh CHUYÊN DÙNG CHO BƯỚC CHÂN (Đi bộ, Chạy, Ngồi, Dưới nước)")]
    public AudioSource nguonAmThanhBuocChan;

    [Tooltip("Nguồn âm thanh CHUYÊN DÙNG CHO HIỆU ỨNG (Nhảy, Tiếp đất)")]
    public AudioSource nguonAmThanhHieuUng;

    [Header("--- Cấu hình Audio Clips & Âm Lượng ---")]
    [Tooltip("File âm thanh bước chân thông thường trên cạn")]
    public AudioClip amThanhBuocChan;
    [Range(0f, 1f)] public float amLuongBuocChan = 0.7f;

    [Tooltip("File âm thanh chạy nhanh trên cạn")]
    public AudioClip amThanhChay;
    [Range(0f, 1f)] public float amLuongChay = 1.0f;

    [Tooltip("File âm thanh khi ngồi khi giữ Ctrl")]
    public AudioClip amThanhNgoi;
    [Range(0f, 1f)] public float amLuongNgoi = 0.35f;

    [Tooltip("File âm thanh lội nước khi đi bộ trong vùng nước (Tag Water)")]
    public AudioClip amThanhDuoiNuoc;
    [Range(0f, 1f)] public float amLuongDuoiNuoc = 0.8f;

    [Tooltip("File âm thanh lội nước khi chạy trong vùng nước (Tag Water)")]
    public AudioClip amThanhChayDuoiNuoc;
    [Range(0f, 1f)] public float amLuongChayDuoiNuoc = 1.0f;

    [Tooltip("File âm thanh khi nhân vật nhảy")]
    public AudioClip amThanhNhay;
    [Range(0f, 1f)] public float amLuongNhay = 0.8f;

    [Tooltip("File âm thanh khi nhân vật tiếp đất")]
    public AudioClip amThanhTiepDat;
    [Range(0f, 1f)] public float amLuongTiepDat = 0.8f;

    [Header("--- Cấu hình Nhịp bước chân (Thời gian delay giữa các bước) ---")]
    public float thoiGianBuocDiBo = 0.55f;
    public float thoiGianBuocChay = 0.3f;
    public float thoiGianBuocNgoi = 0.65f;
    public float thoiGianBuocDuoiNuoc = 0.6f;
    public float thoiGianBuocChayDuoiNuoc = 0.35f;

    [Header("--- Tốc độ di chuyển ---")]
    public float tocDoDiBo = 4f;
    public float tocDoChay = 7f;
    public float tocDoNgoi = 1.5f;
    public float tocDoXoayNhanVat = 12f;

    [Header("--- Vùng nước nguy hiểm & Vùng nước chung ---")]
    public bool dangTrongVungNuocNguyHiem = false;
    [Range(0.1f, 1f)]
    public float heSoGiamTocDoNuocNguyHiem = 0.5f;
    public string tagVungNuoc = "Water";

    [Header("--- Cấu hình Nhảy & Kiểm tra chạm đất ---")]
    public float lucNhay = 5f;
    public Transform diemKiemTraChan;
    public float banKinhKiemTraDat = 0.2f;
    public LayerMask lopMatDat;

    [Header("--- Cấu hình Tư thế Ngồi & Kiểm tra đầu ---")]
    public float chieuCaoDung = 2.0f;
    public float chieuCaoNgoi = 1.0f;
    public float tocDoChuyenTuThe = 8f;
    public Transform diemKiemTraDau;
    public float banKinhKiemTraDau = 0.2f;

    [Header("--- Cấu hình Thể lực ---")]
    public float theLucToiDa = 100f;
    public float theLucTieuHaoMoiGiay = 20f;
    public float theLucHoiPhucMoiGiay = 15f;

    // --- Biến nội bộ quản lý trạng thái ---
    private float theLucHienTai;
    private float tocDoHienTai;
    private Vector2 giaTriDiChuyenDauVao;

    private bool dangNhanChay = false;
    private bool dangNhanNgoi = false;
    private bool dangDaThietLapNhay = false;
    private bool kiemTagDanGiapDat = false;
    private bool dangOTrongNuoc = false;
    private bool dangOTrenKhong = false;
    private float demThoiGianBuocChan = 0f;

    // Biến trạng thái khóa di chuyển khi vào hội thoại
    private bool dangKhoaDiChuyen = false;

    public bool DangDiChuyen => !dangKhoaDiChuyen && giaTriDiChuyenDauVao.sqrMagnitude > 0.01f;

    private void Start()
    {
        if (boDieuKhienNhanVat == null)
            boDieuKhienNhanVat = GetComponent<Rigidbody>();

        if (vaChamNhanVat == null)
            vaChamNhanVat = GetComponent<CapsuleCollider>();

        if (animatorNhanVat == null)
            animatorNhanVat = GetComponentInChildren<Animator>();

        AudioSource[] danhSachAudio = GetComponents<AudioSource>();
        if (nguonAmThanhBuocChan == null)
        {
            nguonAmThanhBuocChan = danhSachAudio.Length > 0 ? danhSachAudio[0] : gameObject.AddComponent<AudioSource>();
        }
        if (nguonAmThanhHieuUng == null)
        {
            nguonAmThanhHieuUng = danhSachAudio.Length > 1 ? danhSachAudio[1] : gameObject.AddComponent<AudioSource>();
        }

        CauHinhAudioSource(nguonAmThanhBuocChan);
        CauHinhAudioSource(nguonAmThanhHieuUng);

        if (cameraChinh == null && Camera.main != null)
            cameraChinh = Camera.main.transform;

        if (boDieuKhienNhanVat != null)
        {
            boDieuKhienNhanVat.freezeRotation = true;
            boDieuKhienNhanVat.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        theLucHienTai = theLucToiDa;

        if (thanhTheLucUI != null)
        {
            thanhTheLucUI.maxValue = theLucToiDa;
            thanhTheLucUI.value = theLucHienTai;
        }
        dangTrongVungNuocNguyHiem = false;
    }

    private void CauHinhAudioSource(AudioSource source)
    {
        if (source != null)
        {
            source.clip = null;
            source.playOnAwake = false;
            source.loop = false;
        }
    }

    private void Update()
    {
        if (diemKiemTraChan != null)
        {
            kiemTagDanGiapDat = Physics.CheckSphere(
                diemKiemTraChan.position,
                banKinhKiemTraDat,
                lopMatDat
            );
        }
        else
        {
            kiemTagDanGiapDat = Physics.Raycast(
                transform.position,
                Vector3.down,
                1.1f,
                lopMatDat
            );
        }

        XuLyAmThanhTiepDatLapTuc();
        XuLyTheLucVaTocDo();
        XuLyTudTheNgoi();
        XuLyAmThanhBuocChan();
        CapNhatAnimator();
        CapNhatGiaoDienUI();
    }

    private void FixedUpdate()
    {
        XuLyDiChuyenTheoCamera();
        XuLyNhay();
    }

    #region Trigger Events (Phát hiện vùng nước)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagVungNuoc))
        {
            dangOTrongNuoc = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagVungNuoc))
        {
            dangOTrongNuoc = false;
        }
    }
    #endregion

    #region Input Events

    public void OnMove(InputAction.CallbackContext context)
    {
        if (dangKhoaDiChuyen)
        {
            giaTriDiChuyenDauVao = Vector2.zero;
            return;
        }
        giaTriDiChuyenDauVao = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (dangKhoaDiChuyen) return;

        if (context.started || context.performed)
        {
            dangNhanChay = true;
            demThoiGianBuocChan = 0f;
        }
        else if (context.canceled)
        {
            dangNhanChay = false;
            demThoiGianBuocChan = 0f;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (dangKhoaDiChuyen) return;

        if (context.started || context.performed)
            dangNhanNgoi = true;
        else if (context.canceled)
            dangNhanNgoi = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (dangKhoaDiChuyen) return;

        if (context.performed)
        {
            if (kiemTagDanGiapDat && !dangNhanNgoi)
            {
                dangDaThietLapNhay = true;

                if (animatorNhanVat != null)
                {
                    animatorNhanVat.SetTrigger("Jump");
                }

                if (nguonAmThanhHieuUng != null && amThanhNhay != null)
                {
                    nguonAmThanhHieuUng.pitch = 1.0f;
                    nguonAmThanhHieuUng.PlayOneShot(amThanhNhay, amLuongNhay);
                }
            }
        }
    }

    #endregion

    private void XuLyTheLucVaTocDo()
    {
        if (dangKhoaDiChuyen) return;

        bool dangDiChuyen = DangDiChuyen;

        if (dangNhanChay && dangDiChuyen &&
            theLucHienTai > 0 && !dangNhanNgoi)
        {
            tocDoHienTai = tocDoChay;

            theLucHienTai -=
                theLucTieuHaoMoiGiay * Time.deltaTime;

            if (theLucHienTai < 0)
                theLucHienTai = 0;
        }
        else
        {
            if (theLucHienTai < theLucToiDa)
            {
                theLucHienTai +=
                    theLucHoiPhucMoiGiay * Time.deltaTime;

                if (theLucHienTai > theLucToiDa)
                    theLucHienTai = theLucToiDa;
            }

            if (dangNhanNgoi)
                tocDoHienTai = tocDoNgoi;
            else
                tocDoHienTai = tocDoDiBo;
        }

        if (dangTrongVungNuocNguyHiem)
        {
            tocDoHienTai *= heSoGiamTocDoNuocNguyHiem;
        }
    }

    private void XuLyDiChuyenTheoCamera()
    {
        if (dangKhoaDiChuyen)
        {
            // Triệt tiêu vận tốc ngang ngay lập tức khi bị khóa di chuyển
            boDieuKhienNhanVat.linearVelocity = new Vector3(0f, boDieuKhienNhanVat.linearVelocity.y, 0f);
            return;
        }

        if (giaTriDiChuyenDauVao.sqrMagnitude >= 0.01f)
        {
            Vector3 camForward = Vector3.forward;
            Vector3 camRight = Vector3.right;

            if (cameraChinh != null)
            {
                camForward = cameraChinh.forward;
                camRight = cameraChinh.right;

                camForward.y = 0f;
                camRight.y = 0f;

                camForward.Normalize();
                camRight.Normalize();
            }

            Vector3 huongDiChuyen =
                (camForward * giaTriDiChuyenDauVao.y) +
                (camRight * giaTriDiChuyenDauVao.x);

            huongDiChuyen.Normalize();

            Quaternion gocXoayMucTieu =
                Quaternion.LookRotation(huongDiChuyen);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                gocXoayMucTieu,
                Time.fixedDeltaTime * tocDoXoayNhanVat
            );

            Vector3 vanTocMucTieu = huongDiChuyen * tocDoHienTai;

            boDieuKhienNhanVat.linearVelocity = new Vector3(
                vanTocMucTieu.x,
                boDieuKhienNhanVat.linearVelocity.y,
                vanTocMucTieu.z
            );
        }
        else
        {
            boDieuKhienNhanVat.linearVelocity = new Vector3(
                0f,
                boDieuKhienNhanVat.linearVelocity.y,
                0f
            );
        }
    }

    private void XuLyTudTheNgoi()
    {
        if (vaChamNhanVat == null)
            return;

        bool coVatCanTrenDau = false;
        if (diemKiemTraDau != null)
        {
            coVatCanTrenDau = Physics.CheckSphere(
                diemKiemTraDau.position,
                banKinhKiemTraDau,
                lopMatDat
            );
        }

        bool thucSuDangNgoi = dangNhanNgoi || coVatCanTrenDau;
        float chieuCaoMucTieu = thucSuDangNgoi ? chieuCaoNgoi : chieuCaoDung;

        vaChamNhanVat.height = Mathf.Lerp(
            vaChamNhanVat.height,
            chieuCaoMucTieu,
            Time.deltaTime * tocDoChuyenTuThe
        );
    }

    private void XuLyAmThanhBuocChan()
    {
        if (dangKhoaDiChuyen) return;

        if (DangDiChuyen && kiemTagDanGiapDat)
        {
            demThoiGianBuocChan -= Time.deltaTime;

            if (demThoiGianBuocChan <= 0f)
            {
                float khoangCachDelay = thoiGianBuocDiBo;
                AudioClip clipSuDung = amThanhBuocChan;
                float amLuongSuDung = amLuongBuocChan;
                float doCaoPitch = 1.0f;

                bool dangChayThucSu = dangNhanChay && theLucHienTai > 0 && !dangNhanNgoi;

                if (dangOTrongNuoc)
                {
                    if (dangChayThucSu)
                    {
                        clipSuDung = amThanhChayDuoiNuoc != null ? amThanhChayDuoiNuoc : amThanhDuoiNuoc;
                        amLuongSuDung = amLuongChayDuoiNuoc;
                        khoangCachDelay = thoiGianBuocChayDuoiNuoc;
                        doCaoPitch = 1.05f;
                    }
                    else
                    {
                        clipSuDung = amThanhDuoiNuoc;
                        amLuongSuDung = amLuongDuoiNuoc;
                        khoangCachDelay = thoiGianBuocDuoiNuoc;
                        doCaoPitch = 0.9f;
                    }
                }
                else
                {
                    if (dangChayThucSu)
                    {
                        clipSuDung = amThanhChay != null ? amThanhChay : amThanhBuocChan;
                        amLuongSuDung = amLuongChay;
                        khoangCachDelay = thoiGianBuocChay;
                        doCaoPitch = 1.15f;
                    }
                    else if (dangNhanNgoi)
                    {
                        clipSuDung = amThanhNgoi != null ? amThanhNgoi : amThanhBuocChan;
                        amLuongSuDung = amLuongNgoi;
                        khoangCachDelay = thoiGianBuocNgoi;
                        doCaoPitch = 0.85f;
                    }
                    else
                    {
                        clipSuDung = amThanhBuocChan;
                        amLuongSuDung = amLuongBuocChan;
                        khoangCachDelay = thoiGianBuocDiBo;
                        doCaoPitch = 1.0f;
                    }
                }

                if (nguonAmThanhBuocChan != null && clipSuDung != null)
                {
                    nguonAmThanhBuocChan.pitch = doCaoPitch;
                    nguonAmThanhBuocChan.PlayOneShot(clipSuDung, amLuongSuDung);
                }

                demThoiGianBuocChan = khoangCachDelay;
            }
        }
        else
        {
            if (nguonAmThanhBuocChan != null && nguonAmThanhBuocChan.isPlaying)
            {
                nguonAmThanhBuocChan.Stop();
            }
            demThoiGianBuocChan = 0f;
        }
    }

    private void XuLyAmThanhTiepDatLapTuc()
    {
        if (kiemTagDanGiapDat)
        {
            if (dangOTrenKhong)
            {
                if (nguonAmThanhHieuUng != null && amThanhTiepDat != null)
                {
                    nguonAmThanhHieuUng.pitch = dangOTrongNuoc ? 0.7f : 1.0f;
                    nguonAmThanhHieuUng.PlayOneShot(amThanhTiepDat, amLuongTiepDat);
                }
                dangOTrenKhong = false;
            }
        }
        else
        {
            dangOTrenKhong = true;
        }
    }

    private void XuLyNhay()
    {
        if (dangKhoaDiChuyen) return;

        if (dangDaThietLapNhay)
        {
            if (kiemTagDanGiapDat && !dangNhanNgoi)
            {
                boDieuKhienNhanVat.linearVelocity = new Vector3(
                    boDieuKhienNhanVat.linearVelocity.x,
                    0f,
                    boDieuKhienNhanVat.linearVelocity.z
                );

                boDieuKhienNhanVat.AddForce(
                    Vector3.up * lucNhay,
                    ForceMode.Impulse
                );
            }

            dangDaThietLapNhay = false;
        }
    }

    private void CapNhatAnimator()
    {
        if (animatorNhanVat == null) return;

        float tocDoTruyenAnimator = 0f;
        if (DangDiChuyen)
        {
            bool dangChayThucSu = dangNhanChay && theLucHienTai > 0 && !dangNhanNgoi;
            tocDoTruyenAnimator = dangChayThucSu ? 1.0f : 0.5f;
        }
        animatorNhanVat.SetFloat("Speed", tocDoTruyenAnimator);

        bool coVatCanTrenDau = diemKiemTraDau != null && Physics.CheckSphere(diemKiemTraDau.position, banKinhKiemTraDau, lopMatDat);
        bool thucSuDangNgoi = dangNhanNgoi || coVatCanTrenDau;

        animatorNhanVat.SetBool("IsCrouching", thucSuDangNgoi);
        animatorNhanVat.SetBool("IsGrounded", kiemTagDanGiapDat);
        animatorNhanVat.SetBool("IsInWater", dangOTrongNuoc);
    }

    /// <summary>
    /// Hàm nhận lệnh từ DialogueManager để khóa/mở khóa di chuyển của nhân vật
    /// </summary>
    public void SetKhoaDiChuyen(bool khoa)
    {
        dangKhoaDiChuyen = khoa;
        if (khoa)
        {
            giaTriDiChuyenDauVao = Vector2.zero;
            dangNhanChay = false;
            dangNhanNgoi = false;
        }
    }

    public void DatLaiTrangThaiNuocNguyHiem()
    {
        dangTrongVungNuocNguyHiem = false;
        tocDoHienTai = tocDoDiBo;
    }

    private void CapNhatGiaoDienUI()
    {
        if (thanhTheLucUI != null)
        {
            thanhTheLucUI.value = theLucHienTai;
        }
    }
}