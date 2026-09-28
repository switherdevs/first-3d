using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem; // Sử dụng cho phím ESC

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

    [Header("--- BỔ SUNG: ANIMATION & RESUME UI ---")]
    [Tooltip("Animator để chạy Animation cho UI Setting")]
    public Animator animatorSetting;

    [Tooltip("Tên Trigger/State Animation xuất hiện cho Setting")]
    public string tenAniIn = "ani_in";

    [Tooltip("Tên Trigger/State Animation biến mất cho Setting")]
    public string tenAniOut = "ani_out";

    [Tooltip("Thời gian chờ (giây) để animation ani_out chạy hết trước khi ẩn GameObject")]
    public float thoiGianChoAniOut = 0.5f;

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

    private Coroutine coroutineChuyenGiaoUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có Instance tồn tại, chuyển toàn bộ liên kết UI mới sang Instance cũ
            Instance.CapNhatLienKetUIMoi(this);
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        KhoiTaoSettings();
    }

    private void Update()
    {
        // Nhấn ESC để Toggle/Resume Setting khi đang mở
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (gameObject.activeSelf)
            {
                ResumeGame();
            }
        }
    }

    public void KhoiTaoSettings()
    {
        // 1. Tự động lấy Animator nếu chưa kéo vào Inspector
        if (animatorSetting == null)
        {
            animatorSetting = GetComponent<Animator>();
        }

        // 2. Cấu hình khoảng Min/Max cho Slider
        CauHinhSlider(sliderMaster, 0.0001f, 1f);
        CauHinhSlider(sliderMusic, 0.0001f, 1f);
        CauHinhSlider(sliderSFX, 0.0001f, 1f);
        CauHinhSlider(sliderSensitivity, 0.05f, 2.0f);

        // 3. Lắng nghe sự kiện kéo Slider từ UI
        DangKySuKienSliders();

        // 4. Tải và áp dụng cài đặt đã lưu
        LoadAllSettings();
    }

    // Cập nhật lại các tham chiếu UI khi reload scene mới
    public void CapNhatLienKetUIMoi(SettingsManager newSettings)
    {
        this.sliderMaster = newSettings.sliderMaster;
        this.sliderMusic = newSettings.sliderMusic;
        this.sliderSFX = newSettings.sliderSFX;
        this.sliderSensitivity = newSettings.sliderSensitivity;

        this.textMasterValue = newSettings.textMasterValue;
        this.textMusicValue = newSettings.textMusicValue;
        this.textSFXValue = newSettings.textSFXValue;
        this.textSensitivityValue = newSettings.textSensitivityValue;

        this.animatorSetting = newSettings.animatorSetting;
        this.tenAniIn = newSettings.tenAniIn;
        this.tenAniOut = newSettings.tenAniOut;
        this.thoiGianChoAniOut = newSettings.thoiGianChoAniOut;

        KhoiTaoSettings();
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
        {
            sliderMaster.onValueChanged.RemoveAllListeners();
            sliderMaster.onValueChanged.AddListener(SetMasterVolume);
        }

        if (sliderMusic != null)
        {
            sliderMusic.onValueChanged.RemoveAllListeners();
            sliderMusic.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.RemoveAllListeners();
            sliderSFX.onValueChanged.AddListener(SetSFXVolume);
        }

        if (sliderSensitivity != null)
        {
            sliderSensitivity.onValueChanged.RemoveAllListeners();
            sliderSensitivity.onValueChanged.AddListener(SetMouseSensitivity);
        }
    }

    // =========================================================
    // HÀM MỞ / ĐÓNG (RESUME) SETTING VỚI ANIMATION IN/OUT
    // =========================================================

    // 🎯 HÀM MỞ SETTING (Gán cho nút Setting hoặc phím Pause)
    public void MoSettingUI()
    {
        gameObject.SetActive(true);

        if (animatorSetting != null && !string.IsNullOrEmpty(tenAniIn))
        {
            animatorSetting.Play(tenAniIn, 0, 0f);
        }

        // Hiện chuột để thao tác UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 🎯 HÀM BỔ SUNG: NÚT RESUME (Gán vào Button Resume/Đóng trong Setting UI)
    public void ResumeGame()
    {
        // An toàn chống lỗi Coroutine nếu GameObject bị Inactive
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        if (coroutineChuyenGiaoUI != null)
        {
            StopCoroutine(coroutineChuyenGiaoUI);
        }

        coroutineChuyenGiaoUI = StartCoroutine(Routine_ResumeGame());
    }

    private IEnumerator Routine_ResumeGame()
    {
        // 1. Chạy Animation ani_out biến mất
        if (animatorSetting != null && !string.IsNullOrEmpty(tenAniOut))
        {
            animatorSetting.Play(tenAniOut, 0, 0f);
        }

        // 2. Chờ thời gian chạy hết animation ani_out (Realtime không bị ảnh hưởng bởi Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(thoiGianChoAniOut);

        // 3. Tắt GameObject Setting UI
        gameObject.SetActive(false);

        // 4. Khôi phục lại thời gian Game
        Time.timeScale = 1f;

        coroutineChuyenGiaoUI = null;
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