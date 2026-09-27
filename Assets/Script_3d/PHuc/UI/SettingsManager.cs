using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public enum NgonNgu
{
    TiengViet = 0,
    TiengAnh = 1
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("--- CẤU HÌNH AUDIO MIXER ---")]
    [Tooltip("Kéo file Audio Mixer của bạn vào đây")]
    public AudioMixer mainAudioMixer;

    [Header("--- TÊN BIẾN TRONG AUDIO MIXER ---")]
    public string parameterMaster = "MasterVolume";
    public string parameterMusic = "MusicVolume";
    public string parameterSFX = "SFXVolume";

    [Header("--- GIAO DIỆN UI SLIDERS (KÉO VÀO INSPECTOR) ---")]
    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSFX;
    public Slider sliderSensitivity;

    [Header("--- GIAO DIỆN UI TEXT (TEXT MESH PRO) ---")]
    public TextMeshProUGUI textMasterValue;
    public TextMeshProUGUI textMusicValue;
    public TextMeshProUGUI textSFXValue;
    public TextMeshProUGUI textSensitivityValue;

    [Header("--- CẤU HÌNH NGÔN NGỮ ---")]
    public NgonNgu ngonNguHienTai = NgonNgu.TiengViet;

    [Header("--- GIÁ TRỊ ĐỘ NHẠY CHUỘT HỆ THỐNG ---")]
    [HideInInspector] public float currentMouseSensitivity = 0.3f;

    // Key lưu dữ liệu xuống máy
    private const string KEY_MASTER = "Save_MasterVolume";
    private const string KEY_MUSIC = "Save_MusicVolume";
    private const string KEY_SFX = "Save_SFXVolume";
    private const string KEY_SENSITIVITY = "Save_MouseSensitivity";
    private const string KEY_LANGUAGE = "Save_Language";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 1. Cấu hình khoảng Min/Max cho Slider
        CauHinhSlider(sliderMaster, 0.0001f, 1f);
        CauHinhSlider(sliderMusic, 0.0001f, 1f);
        CauHinhSlider(sliderSFX, 0.0001f, 1f);
        CauHinhSlider(sliderSensitivity, 0.05f, 2.0f); // Khoảng độ nhạy chuột phù hợp cho CameraController

        // 2. Lắng nghe sự kiện kéo Slider từ UI
        DangKySuKienSliders();

        // 3. Tải và áp dụng cài đặt đã lưu
        LoadAllSettings();
    }

    private void CauHinhSlider(Slider slider, float min, float max)
    {
        if (slider != null)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }
    }

    private void DangKySuKienSliders()
    {
        if (sliderMaster != null)
            sliderMaster.onValueChanged.AddListener(SetMasterVolume);

        if (sliderMusic != null)
            sliderMusic.onValueChanged.AddListener(SetMusicVolume);

        if (sliderSFX != null)
            sliderSFX.onValueChanged.AddListener(SetSFXVolume);

        if (sliderSensitivity != null)
            sliderSensitivity.onValueChanged.AddListener(SetMouseSensitivity);
    }

    // 🎯 1. XỬ LÝ ÂM THANH MASTER
    public void SetMasterVolume(float value)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat(parameterMaster, dbValue);
        }

        if (sliderMaster != null && sliderMaster.value != value) sliderMaster.value = value;
        CapNhatTextHienThi(textMasterValue, value * 100f, "%");

        PlayerPrefs.SetFloat(KEY_MASTER, value);
        PlayerPrefs.Save();
    }

    // 🎯 2. XỬ LÝ ÂM THANH MUSIC
    public void SetMusicVolume(float value)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat(parameterMusic, dbValue);
        }

        if (sliderMusic != null && sliderMusic.value != value) sliderMusic.value = value;
        CapNhatTextHienThi(textMusicValue, value * 100f, "%");

        PlayerPrefs.SetFloat(KEY_MUSIC, value);
        PlayerPrefs.Save();
    }

    // 🎯 3. XỬ LÝ ÂM THANH SFX
    public void SetSFXVolume(float value)
    {
        float dbValue = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat(parameterSFX, dbValue);
        }

        if (sliderSFX != null && sliderSFX.value != value) sliderSFX.value = value;
        CapNhatTextHienThi(textSFXValue, value * 100f, "%");

        PlayerPrefs.SetFloat(KEY_SFX, value);
        PlayerPrefs.Save();
    }

    // 🎯 4. XỬ LÝ ĐỘ NHẠY CHUỘT & TRUYỀN THÔNG SỐ TỚI CAMERACONTROLLER
    public void SetMouseSensitivity(float value)
    {
        currentMouseSensitivity = value;

        if (sliderSensitivity != null && sliderSensitivity.value != value) sliderSensitivity.value = value;
        CapNhatTextHienThi(textSensitivityValue, value, "x");

        // Liên kết trực tiếp sang CameraController trong Scene
        CameraController camControl = Object.FindFirstObjectByType<CameraController>();
        if (camControl != null)
        {
            camControl.doNhayNgang = value;
            camControl.doNhayDoc = value;
        }

        PlayerPrefs.SetFloat(KEY_SENSITIVITY, value);
        PlayerPrefs.Save();
    }

    // 🎯 5. HÀM ĐỔI NGÔN NGỮ (TIẾNG VIỆT / TIẾNG ANH)
    public void DoiNgonNgu(int indexNgonNgu)
    {
        ngonNguHienTai = (NgonNgu)indexNgonNgu;
        PlayerPrefs.SetInt(KEY_LANGUAGE, indexNgonNgu);
        PlayerPrefs.Save();

        Debug.Log("<color=cyan>[SettingsManager]</color> Đã chuyển ngôn ngữ sang: " + ngonNguHienTai.ToString());

        // Có thể phát sự kiện để các UI khác cập nhật văn bản nếu cần
    }

    public void ChuyenDoiQuaLaiNgonNgu()
    {
        if (ngonNguHienTai == NgonNgu.TiengViet)
        {
            DoiNgonNgu((int)NgonNgu.TiengAnh);
        }
        else
        {
            DoiNgonNgu((int)NgonNgu.TiengViet);
        }
    }

    // 🎯 6. LOAD TẤT CẢ DỮ LIỆU ĐÃ LƯU TỪ PLAYERPREFS
    public void LoadAllSettings()
    {
        float masterVal = PlayerPrefs.GetFloat(KEY_MASTER, 0.75f);
        float musicVal = PlayerPrefs.GetFloat(KEY_MUSIC, 0.75f);
        float sfxVal = PlayerPrefs.GetFloat(KEY_SFX, 0.75f);
        float sensitivityVal = PlayerPrefs.GetFloat(KEY_SENSITIVITY, 0.3f);
        int languageVal = PlayerPrefs.GetInt(KEY_LANGUAGE, (int)NgonNgu.TiengViet);

        SetMasterVolume(masterVal);
        SetMusicVolume(musicVal);
        SetSFXVolume(sfxVal);
        SetMouseSensitivity(sensitivityVal);
        DoiNgonNgu(languageVal);
    }

    private void CapNhatTextHienThi(TextMeshProUGUI textComp, float value, string unit)
    {
        if (textComp != null)
        {
            if (unit == "%")
            {
                textComp.text = Mathf.RoundToInt(value).ToString() + "%";
            }
            else
            {
                textComp.text = value.ToString("F2") + unit;
            }
        }
    }
}