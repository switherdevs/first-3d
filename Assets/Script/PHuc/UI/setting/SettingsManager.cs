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

    [Header("--- KHUNG GIAO DIỆN UI SETTING ---")]
    [Tooltip("Kéo GameObject Bảng Menu Setting (Panel) bị Active False vào đây")]
    public GameObject panelSettingUI;

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

    [Header("--- BỔ SUNG: BUTTON TẮT ĐUNG ĐƯA CAMERA (TOGGLE) ---")]
    [Tooltip("Kéo UI Button tắt/bật đung đưa camera vào đây")]
    public Button btnDungDuaCam;

    [Tooltip("Màu hiển thị của Nút khi ĐÃ BẤM GIỮ (Tắt đung đưa)")]
    public Color mauNutKhiBat = Color.green;

    [Tooltip("Màu hiển thị của Nút khi KHÔNG BẤM (Bật đung đưa bình thường)")]
    public Color mauNutKhiTat = Color.gray;

    [Tooltip("Kéo GameObject icon/tích xanh con nằm trong nút vào đây (Nếu có)")]
    public GameObject iconChietBaoNut;

    [Header("--- BỔ SUNG: ANIMATION & RESUME UI ---")]
    [Tooltip("Animator để chạy Animation cho UI Setting")]
    public Animator animatorSetting;

    [Tooltip("Tên Trigger/State Animation xuất hiện cho Setting")]
    public string tenAniIn = "ani_in";

    [Tooltip("Tên Trigger/State Animation biến mất cho Setting")]
    public string tenAniOut = "ani_out";

    [Tooltip("Thời gian chờ (giây) để animation ani_out chạy hết trước khi ẩn GameObject")]
    public float thoiGianChoAniOut = 0.5f;

    [Header("--- CẤU HÌNH ÂM THANH CLICK BUTTON ---")]
    [Tooltip("AudioClip tiếng Click khi bấm các nút trên Setting UI")]
    public AudioClip amThanhClick;

    [Tooltip("AudioSource dùng để phát âm thanh Click (Nếu để trống sẽ tự tạo/lấy)")]
    public AudioSource amThanhSource;

    [Header("--- CẤU HÌNH NGÔN NGỮ ---")]
    public NgonNgu ngonNguHienTai = NgonNgu.TiengViet;

    [Header("--- GIÁ TRỊ ĐỘ NHẠY CHUỘT HỆ THỐNG ---")]
    [HideInInspector] public float currentMouseSensitivity = 0.3f;

    // Biến lưu trạng thái Tắt/Bật đung đưa
    [HideInInspector] public bool isCameraBobbingDisabled = false;

    // Key lưu dữ liệu xuống máy
    private const string KEY_MASTER = "Save_MasterVolume";
    private const string KEY_MUSIC = "Save_MusicVolume";
    private const string KEY_SFX = "Save_SFXVolume";
    private const string KEY_SENSITIVITY = "Save_MouseSensitivity";
    private const string KEY_LANGUAGE = "Save_Language";
    private const string KEY_CAMERA_BOBBING = "Save_CameraBobbing"; // Key lưu tắt đung đưa

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
            if (KiemTraPanelDangActive())
            {
                ResumeGame();
            }
        }
    }

    // --- HÀM PHÁT ÂM THANH CLICK 1 LẦN ---
    public void PhatAmThanhClick()
    {
        if (amThanhSource != null && amThanhClick != null)
        {
            amThanhSource.PlayOneShot(amThanhClick);
        }
    }

    public void KhoiTaoSettings()
    {
        // Tự động tìm/tạo AudioSource nếu bị thiếu
        if (amThanhSource == null)
        {
            amThanhSource = GetComponent<AudioSource>();
            if (amThanhSource == null)
            {
                amThanhSource = gameObject.AddComponent<AudioSource>();
            }
        }

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

        // 3. Đăng ký sự kiện Slider & Nút Toggle
        DangKySuKienSliders();

        // 4. Kích hoạt Coroutine set âm lượng AudioMixer ngay lập tức lúc vào Game
        StartCoroutine(Routine_KhoiTaoAudioBanDau());
    }

    /// <summary>
    /// Coroutine chờ cuối Frame để AudioMixer nạp đủ Parameter rồi gán âm lượng đã lưu
    /// </summary>
    private IEnumerator Routine_KhoiTaoAudioBanDau()
    {
        yield return new WaitForEndOfFrame();

        LoadAllSettings();
    }

    // Cập nhật lại các tham chiếu UI khi reload scene mới
    public void CapNhatLienKetUIMoi(SettingsManager newSettings)
    {
        this.panelSettingUI = newSettings.panelSettingUI;
        this.sliderMaster = newSettings.sliderMaster;
        this.sliderMusic = newSettings.sliderMusic;
        this.sliderSFX = newSettings.sliderSFX;
        this.sliderSensitivity = newSettings.sliderSensitivity;

        this.textMasterValue = newSettings.textMasterValue;
        this.textMusicValue = newSettings.textMusicValue;
        this.textSFXValue = newSettings.textSFXValue;
        this.textSensitivityValue = newSettings.textSensitivityValue;

        this.btnDungDuaCam = newSettings.btnDungDuaCam;
        this.mauNutKhiBat = newSettings.mauNutKhiBat;
        this.mauNutKhiTat = newSettings.mauNutKhiTat;
        this.iconChietBaoNut = newSettings.iconChietBaoNut;

        this.animatorSetting = newSettings.animatorSetting;
        this.tenAniIn = newSettings.tenAniIn;
        this.tenAniOut = newSettings.tenAniOut;
        this.thoiGianChoAniOut = newSettings.thoiGianChoAniOut;

        this.amThanhClick = newSettings.amThanhClick;

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

        if (btnDungDuaCam != null)
        {
            btnDungDuaCam.onClick.RemoveAllListeners();
            btnDungDuaCam.onClick.AddListener(ToggleCameraBobbing);
        }
    }

    private bool KiemTraPanelDangActive()
    {
        if (panelSettingUI != null) return panelSettingUI.activeSelf;
        return gameObject.activeSelf;
    }

    // =========================================================
    // HÀM MỞ / ĐÓNG (RESUME) SETTING VỚI ANIMATION IN/OUT
    // =========================================================

    // 🎯 HÀM MỞ SETTING (Gán cho nút Setting hoặc phím Pause)
    public void MoSettingUI()
    {
        PhatAmThanhClick();

        // Bật Bảng Menu UI Setting lên
        if (panelSettingUI != null)
        {
            panelSettingUI.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }

        // Đồng bộ vị trí Slider & Text khi mở UI
        CapNhatGiaoDienUI();

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
        PhatAmThanhClick();

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

        // 3. Tắt Panel UI Setting
        if (panelSettingUI != null)
        {
            panelSettingUI.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        // 4. Khôi phục lại thời gian Game
        Time.timeScale = 1f;

        coroutineChuyenGiaoUI = null;
    }

    // 🎯 1. XỬ LÝ ÂM THANH MASTER
    public void SetMasterVolume(float value)
    {
        float clampedVal = Mathf.Clamp(value, 0.0001f, 1f);
        float dbValue = Mathf.Log10(clampedVal) * 20f;

        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat(parameterMaster, dbValue);
        }

        if (sliderMaster != null && sliderMaster.value != clampedVal)
        {
            sliderMaster.SetValueWithoutNotify(clampedVal);
        }

        CapNhatTextHienThi(textMasterValue, clampedVal * 100f, "%");

        PlayerPrefs.SetFloat(KEY_MASTER, clampedVal);
        PlayerPrefs.Save();
    }

    // 🎯 2. XỬ LÝ ÂM THANH MUSIC
    public void SetMusicVolume(float value)
    {
        float clampedVal = Mathf.Clamp(value, 0.0001f, 1f);
        float dbValue = Mathf.Log10(clampedVal) * 20f;

        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat(parameterMusic, dbValue);
        }

        if (sliderMusic != null && sliderMusic.value != clampedVal)
        {
            sliderMusic.SetValueWithoutNotify(clampedVal);
        }

        CapNhatTextHienThi(textMusicValue, clampedVal * 100f, "%");

        PlayerPrefs.SetFloat(KEY_MUSIC, clampedVal);
        PlayerPrefs.Save();
    }

    // 🎯 3. XỬ LÝ ÂM THANH SFX
    public void SetSFXVolume(float value)
    {
        float clampedVal = Mathf.Clamp(value, 0.0001f, 1f);
        float dbValue = Mathf.Log10(clampedVal) * 20f;

        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat(parameterSFX, dbValue);
        }

        if (sliderSFX != null && sliderSFX.value != clampedVal)
        {
            sliderSFX.SetValueWithoutNotify(clampedVal);
        }

        CapNhatTextHienThi(textSFXValue, clampedVal * 100f, "%");

        PlayerPrefs.SetFloat(KEY_SFX, clampedVal);
        PlayerPrefs.Save();
    }

    // 🎯 4. XỬ LÝ ĐỘ NHẠY CHUỘT & TRUYỀN THÔNG SỐ TỚI CAMERACONTROLLER
    public void SetMouseSensitivity(float value)
    {
        currentMouseSensitivity = value;

        if (sliderSensitivity != null && sliderSensitivity.value != value)
        {
            sliderSensitivity.SetValueWithoutNotify(value);
        }

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

    // 🎯 5. BỔ SUNG: XỬ LÝ BUTTON TOGGLE TẮT / BẬT ĐUNG ĐƯA CAMERA
    public void ToggleCameraBobbing()
    {
        PhatAmThanhClick();

        // Đảo ngược trạng thái
        isCameraBobbingDisabled = !isCameraBobbingDisabled;

        // Áp dụng trạng thái cho CameraController
        ApDungTrangThaiDungDuaCam(isCameraBobbingDisabled);

        // Lưu vào PlayerPrefs (1 = Tắt đung đưa / 0 = Bình thường)
        PlayerPrefs.SetInt(KEY_CAMERA_BOBBING, isCameraBobbingDisabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApDungTrangThaiDungDuaCam(bool isDisabled)
    {
        // Tìm CameraController trong Scene
        CameraController camControl = Object.FindFirstObjectByType<CameraController>();
        if (camControl != null)
        {
            // kichHoatDungDua = true khi isDisabled = false
            camControl.kichHoatDungDua = !isDisabled;
        }

        // Cập nhật màu sắc / icon giao diện nút bấm
        CapNhatMauNutDungDua(isDisabled);
    }

    private void CapNhatMauNutDungDua(bool isDisabled)
    {
        if (btnDungDuaCam != null)
        {
            // Đổi màu nền Nút (Nếu có Image component)
            Image imgNut = btnDungDuaCam.GetComponent<Image>();
            if (imgNut != null)
            {
                imgNut.color = isDisabled ? mauNutKhiBat : mauNutKhiTat;
            }
        }

        // Bật / Ẩn Icon chỉ báo
        if (iconChietBaoNut != null)
        {
            iconChietBaoNut.SetActive(isDisabled);
        }
    }

    // 🎯 6. HÀM ĐỔI NGÔN NGỮ (TIẾNG VIỆT / TIẾNG ANH)
    public void DoiNgonNgu(int indexNgonNgu)
    {
        PhatAmThanhClick();
        ngonNguHienTai = (NgonNgu)indexNgonNgu;
        PlayerPrefs.SetInt(KEY_LANGUAGE, indexNgonNgu);
        PlayerPrefs.Save();

        Debug.Log("<color=cyan>[SettingsManager]</color> Đã chuyển ngôn ngữ sang: " + ngonNguHienTai.ToString());
    }

    public void ChuyenDoiQuaLaiNgonNgu()
    {
        PhatAmThanhClick();
        if (ngonNguHienTai == NgonNgu.TiengViet)
        {
            DoiNgonNgu((int)NgonNgu.TiengAnh);
        }
        else
        {
            DoiNgonNgu((int)NgonNgu.TiengViet);
        }
    }

    // 🎯 7. LOAD TẤT CẢ DỮ LIỆU ĐÃ LƯU TỪ PLAYERPREFS
    public void LoadAllSettings()
    {
        float masterVal = PlayerPrefs.GetFloat(KEY_MASTER, 0.75f);
        float musicVal = PlayerPrefs.GetFloat(KEY_MUSIC, 0.75f);
        float sfxVal = PlayerPrefs.GetFloat(KEY_SFX, 0.75f);
        float sensitivityVal = PlayerPrefs.GetFloat(KEY_SENSITIVITY, 0.3f);
        int languageVal = PlayerPrefs.GetInt(KEY_LANGUAGE, (int)NgonNgu.TiengViet);

        // Lấy trạng thái Tắt đung đưa (Mặc định 0 = không tắt)
        isCameraBobbingDisabled = PlayerPrefs.GetInt(KEY_CAMERA_BOBBING, 0) == 1;

        SetMasterVolume(masterVal);
        SetMusicVolume(musicVal);
        SetSFXVolume(sfxVal);
        SetMouseSensitivity(sensitivityVal);
        ApDungTrangThaiDungDuaCam(isCameraBobbingDisabled);

        ngonNguHienTai = (NgonNgu)languageVal;
        PlayerPrefs.SetInt(KEY_LANGUAGE, languageVal);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Làm mới Slider, Text và Nút Bấm khi người chơi mở UI Setting lên
    /// </summary>
    private void CapNhatGiaoDienUI()
    {
        float masterVal = PlayerPrefs.GetFloat(KEY_MASTER, 0.75f);
        float musicVal = PlayerPrefs.GetFloat(KEY_MUSIC, 0.75f);
        float sfxVal = PlayerPrefs.GetFloat(KEY_SFX, 0.75f);
        float sensitivityVal = PlayerPrefs.GetFloat(KEY_SENSITIVITY, 0.3f);
        isCameraBobbingDisabled = PlayerPrefs.GetInt(KEY_CAMERA_BOBBING, 0) == 1;

        if (sliderMaster != null) sliderMaster.SetValueWithoutNotify(masterVal);
        if (sliderMusic != null) sliderMusic.SetValueWithoutNotify(musicVal);
        if (sliderSFX != null) sliderSFX.SetValueWithoutNotify(sfxVal);
        if (sliderSensitivity != null) sliderSensitivity.SetValueWithoutNotify(sensitivityVal);

        CapNhatTextHienThi(textMasterValue, masterVal * 100f, "%");
        CapNhatTextHienThi(textMusicValue, musicVal * 100f, "%");
        CapNhatTextHienThi(textSFXValue, sfxVal * 100f, "%");
        CapNhatTextHienThi(textSensitivityValue, sensitivityVal, "x");

        CapNhatMauNutDungDua(isCameraBobbingDisabled);
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