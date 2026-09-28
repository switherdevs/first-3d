using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("--- Thành phần tham chiếu ---")]
    [Tooltip("Nguồn sáng Light của đèn pin (đặt ở đầu súng hoặc trán nhân vật)")]
    public Light denPinLight;

    [Tooltip("Thanh Slider hiển thị lượng pin còn lại trên UI")]
    public Slider thanhPinUI;

    [Tooltip("Tham chiếu đến Camera chính để đèn pin xoay lên/xuống theo góc nhìn")]
    public Camera cameraChinh;

    [Header("--- Cấu hình Pin ---")]
    [Tooltip("Mức pin tối đa")]
    public float pinToiDa = 100f;

    [Tooltip("Lượng pin tiêu hao mỗi giây khi bật đèn")]
    public float pinTieuHaoMoiGiay = 5f;

    // --- Biến nội bộ quản lý trạng thái ---
    private float pinHienTai;
    private bool dangBatDen = false;

    private void Start()
    {
        // Khởi tạo mức pin ban đầu
        pinHienTai = pinToiDa;

        // Tự động tìm Main Camera nếu chưa kéo vào Inspector
        if (cameraChinh == null)
        {
            cameraChinh = Camera.main;
        }

        // Cấu hình thanh Slider UI nếu có tham chiếu
        if (thanhPinUI != null)
        {
            thanhPinUI.maxValue = pinToiDa;
            thanhPinUI.value = pinHienTai;
        }

        // Đảm bảo trạng thái đèn ban đầu tắt
        CapNhatTrangThaiDen(false);
    }

    private void Update()
    {
        XuLyTieuHaoPin();
        CapNhatGiaoDienUI();
    }

    // Cập nhật góc xoay ở LateUpdate để đồng bộ hoàn hảo với góc xoay của Camera
    private void LateUpdate()
    {
        DongBoGocXoayTheoCamera();
    }

    // Thuật toán đồng bộ góc xoay đèn pin theo Camera
    private void DongBoGocXoayTheoCamera()
    {
        if (dangBatDen && cameraChinh != null)
        {
            // Gán trực tiếp góc xoay của Đèn Pin bằng góc xoay của Camera
            transform.rotation = cameraChinh.transform.rotation;
        }
    }

    #region Input Event (New Input System)
    // Gọi hàm này từ Player Input Component (Action "ToggleFlashlight" gán phím F)
    public void OnToggleFlashlight(InputAction.CallbackContext context)
    {
        // Chỉ kích hoạt 1 lần khi người chơi vừa nhấn phím down xuống
        if (context.started)
        {
            // Chỉ cho phép bật nếu còn pin
            if (!dangBatDen && pinHienTai > 0f)
            {
                CapNhatTrangThaiDen(true);
            }
            else if (dangBatDen)
            {
                CapNhatTrangThaiDen(false);
            }
        }
    }
    #endregion

    // Logic rút pin theo thời gian thực
    private void XuLyTieuHaoPin()
    {
        if (dangBatDen)
        {
            // Trừ pin theo từng khung hình
            pinHienTai -= pinTieuHaoMoiGiay * Time.deltaTime;

            // Kiểm tra nếu hết pin thì tự động tắt đèn
            if (pinHienTai <= 0f)
            {
                pinHienTai = 0f;
                CapNhatTrangThaiDen(false);
            }
        }
    }

    // Bật hoặc tắt component Light
    private void CapNhatTrangThaiDen(bool trangThai)
    {
        dangBatDen = trangThai;

        if (denPinLight != null)
        {
            denPinLight.enabled = dangBatDen;
        }
    }

    // Cập nhật giá trị lên thanh Slider UI
    private void CapNhatGiaoDienUI()
    {
        if (thanhPinUI != null)
        {
            thanhPinUI.value = pinHienTai;
        }
    }

    // Hàm bổ sung: Dùng để nhặt bình pin sạc lại (gọi từ script khác nếu cần)
    public void SacPin(float luongPinSac)
    {
        pinHienTai += luongPinSac;
        if (pinHienTai > pinToiDa)
        {
            pinHienTai = pinToiDa;
        }
    }
}