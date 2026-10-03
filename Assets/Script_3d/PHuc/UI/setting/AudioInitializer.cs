using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInitializer : MonoBehaviour
{
    [Header("--- CẤU HÌNH AUDIO MIXER ---")]
    public AudioMixer mainAudioMixer;

    public string parameterMaster = "MasterVolume";
    public string parameterMusic = "MusicVolume";
    public string parameterSFX = "SFXVolume";

    // Key lưu dữ liệu
    private const string KEY_MASTER = "Save_MasterVolume";
    private const string KEY_MUSIC = "Save_MusicVolume";
    private const string KEY_SFX = "Save_SFXVolume";

    private void Awake()
    {
        // Giữ AudioManager không bị xoá khi chuyển Scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Khởi tạo âm lượng xuống Audio Mixer ngay khi vào game
        StartCoroutine(Routine_ApDungAudioBanDau());
    }

    private IEnumerator Routine_ApDungAudioBanDau()
    {
        // Chờ cuối frame đầu tiên để AudioMixer nạp hoàn tất các Parameter
        yield return new WaitForEndOfFrame();

        CapNhatAmLuongMixer();
    }

    public void CapNhatAmLuongMixer()
    {
        if (mainAudioMixer == null) return;

        // Lấy giá trị đã lưu (Mặc định 0.75f)
        float masterVal = PlayerPrefs.GetFloat(KEY_MASTER, 0.75f);
        float musicVal = PlayerPrefs.GetFloat(KEY_MUSIC, 0.75f);
        float sfxVal = PlayerPrefs.GetFloat(KEY_SFX, 0.75f);

        // Chuyển sang dB và nạp vào AudioMixer
        mainAudioMixer.SetFloat(parameterMaster, Mathf.Log10(Mathf.Clamp(masterVal, 0.0001f, 1f)) * 20f);
        mainAudioMixer.SetFloat(parameterMusic, Mathf.Log10(Mathf.Clamp(musicVal, 0.0001f, 1f)) * 20f);
        mainAudioMixer.SetFloat(parameterSFX, Mathf.Log10(Mathf.Clamp(sfxVal, 0.0001f, 1f)) * 20f);
    }
}