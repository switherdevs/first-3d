using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("--- Thành phần tham chiếu ---")]
    [Tooltip("Nguồn sáng Light của đèn pin")]
    public Light denPinLight;

    [Tooltip("Thanh Slider hiển thị lượng pin còn lại trên UI")]
    public Slider thanhPinUI;

    [Tooltip("Tham chiếu đến Camera chính")]
    public Camera cameraChinh;

    [Header("--- Âm thanh (Audio) ---")]
    [Tooltip("Component AudioSource để phát âm thanh bật/tắt")]
    public AudioSource amThanhSource;

    [Tooltip("File âm thanh tiếng click bật/tắt đèn pin")]
    public AudioClip tiengClickDenPin;

    [Header("--- Cấu hình Pin ---")]
    [Tooltip("Mức pin tối đa")]
    public float pinToiDa = 200f;

    [Tooltip("Lượng pin tiêu hao mỗi giây khi bật đèn")]
    public float pinTieuHaoMoiGiay = 1f;

    // --- Biến nội bộ quản lý trạng thái ---
    private float pinHienTai;
    private bool dangBatDen = false;

    private void Start()
    {
        pinHienTai = pinToiDa;

        if (cameraChinh == null)
        {
            cameraChinh = Camera.main;
        }

        // Tự động tìm AudioSource trên GameObject này hoặc con nếu chưa kéo
        if (amThanhSource == null)
        {
            amThanhSource = GetComponentInChildren<AudioSource>();
        }

        if (thanhPinUI != null)
        {
            thanhPinUI.maxValue = pinToiDa;
            thanhPinUI.value = pinHienTai;
        }

        // Khởi tạo trạng thái tắt ban đầu, KHÔNG phát âm thanh khi vào game
        CapNhatTrangThaiDen(false, false);
    }

    private void Update()
    {
        XuLyTieuHaoPin();
        CapNhatGiaoDienUI();
    }

    private void LateUpdate()
    {
        DongBoGocXoayTheoCamera();
    }

    private void DongBoGocXoayTheoCamera()
    {
        // Chỉ xoay vị trí của nguồn sáng Light (denPinLight) theo Camera chứ KHÔNG xoay toàn bộ Player
        if (dangBatDen && denPinLight != null && cameraChinh != null)
        {
            denPinLight.transform.rotation = cameraChinh.transform.rotation;
        }
    }

    #region Input Event (New Input System)
    public void OnToggleFlashlight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!dangBatDen && pinHienTai > 0f)
            {
                CapNhatTrangThaiDen(true, true);
            }
            else if (dangBatDen)
            {
                CapNhatTrangThaiDen(false, true);
            }
        }
    }
    #endregion

    private void XuLyTieuHaoPin()
    {
        if (dangBatDen)
        {
            pinHienTai -= pinTieuHaoMoiGiay * Time.deltaTime;

            if (pinHienTai <= 0f)
            {
                pinHienTai = 0f;
                CapNhatTrangThaiDen(false, true); // Hết pin sập nguồn cũng phát tiếng click
            }
        }
    }

    private void CapNhatTrangThaiDen(bool trangThai, bool phatAmThanh = true)
    {
        dangBatDen = trangThai;

        if (denPinLight != null)
        {
            denPinLight.enabled = dangBatDen;
        }

        // Kiểm tra và phát âm thanh tiếng click bật/tắt
        if (phatAmThanh && amThanhSource != null && tiengClickDenPin != null)
        {
            amThanhSource.PlayOneShot(tiengClickDenPin);
        }
    }

    private void CapNhatGiaoDienUI()
    {
        if (thanhPinUI != null)
        {
            thanhPinUI.value = pinHienTai;
        }
    }

    public void SacPin(float luongPinSac)
    {
        pinHienTai += luongPinSac;
        if (pinHienTai > pinToiDa)
        {
            pinHienTai = pinToiDa;
        }
    }
}