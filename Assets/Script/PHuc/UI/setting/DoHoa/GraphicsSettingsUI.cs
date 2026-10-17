using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicsSettingsUI : MonoBehaviour
{
    [Header("--- Tham Chiếu Thành Phần UI ---")]
    [Tooltip("Thẻ Toggle dùng để Tắt/Mở Bloom")]
    public Toggle bloomToggle;

    [Tooltip("Thanh Slider dùng để tăng/giảm độ sáng")]
    public Slider brightnessSlider;

    [Header("--- Global Volume ---")]
    [Tooltip("Kéo Global Volume chứa hiệu ứng Post-processing vào đây")]
    public Volume globalVolume;

    // Biến lưu trữ các hiệu ứng lấy từ Volume Profile
    private Bloom bloomComponent;
    private ColorAdjustments colorAdjustmentsComponent;

    private void Start()
    {
        // 1. Kiểm tra và lấy các component hiệu ứng từ Global Volume
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out bloomComponent);
            globalVolume.profile.TryGet(out colorAdjustmentsComponent);
        }

        // 2. Thiết lập trạng thái ban đầu cho Toggle Bloom
        if (bloomToggle != null && bloomComponent != null)
        {
            bloomToggle.isOn = bloomComponent.active;
            bloomToggle.onValueChanged.AddListener(OnBloomToggleChanged);
        }

        // 3. Thiết lập giá trị ban đầu cho Slider Ánh sáng (Post Exposure)
        if (brightnessSlider != null && colorAdjustmentsComponent != null)
        {
            // Thiết lập giá trị mặc định của slider (Ví dụ từ -2 đến 2)
            brightnessSlider.value = colorAdjustmentsComponent.postExposure.value;
            brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
        }
    }

    // Hàm xử lý khi bấm Toggle Bloom
    public void OnBloomToggleChanged(bool isOn)
    {
        if (bloomComponent != null)
        {
            bloomComponent.active = isOn; // Bật hoặc tắt hiệu ứng Bloom
        }
    }

    // Hàm xử lý khi kéo Slider Ánh sáng
    public void OnBrightnessSliderChanged(float value)
    {
        if (colorAdjustmentsComponent != null)
        {
            colorAdjustmentsComponent.postExposure.value = value; // Thay đổi độ phơi sáng (Brightness)
        }
    }
}