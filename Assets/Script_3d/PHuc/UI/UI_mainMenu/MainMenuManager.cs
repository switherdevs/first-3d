using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[System.Serializable]
public class SaveDataDataMenu
{
    public string tenMapHienTai;
    public float viTriX;
    public float viTriY;
    public float viTriZ;
}

public class MainMenuManager : MonoBehaviour
{
    [Header("--- Cấu hình Chuyển Scene ---")]
    [Tooltip("Tên Scene sẽ load khi người chơi bấm Start Game mới")]
    public string tenSceneGameMoi = "ThanhTrucLam";

    [Header("--- Cấu hình Fade Out Chuyển Map ---")]
    [Tooltip("Gán UI Image màu đen phủ kín màn hình để làm hiệu ứng mờ dần")]
    public Image anhFadeOut;

    [Tooltip("Thời gian hiệu ứng Fade Out chuyển màn (mặc định 2 giây)")]
    public float thoiGianFadeOut = 2f;

    [Header("--- UI Settings ---")]
    [Tooltip("GameObject chứa UI Setting cục bộ trong Scene")]
    public GameObject gameObjectSetting;

    [Tooltip("Animator để chạy Animation cho UI Setting")]
    public Animator animatorSetting;

    [Tooltip("Tên Trigger/State Animation xuất hiện cho Setting")]
    public string tenSettingAniIn = "ani_in";

    [Tooltip("Tên Trigger/State Animation biến mất cho Setting")]
    public string tenSettingAniOut = "ani_out";

    [Tooltip("Thời gian chờ (giây) để animation ani_out của Setting chạy hết trước khi ẩn GameObject")]
    public float thoiGianChoSettingAniOut = 0.5f;

    [Header("--- UI Main Menu Settings ---")]
    [Tooltip("GameObject chứa UI Main Menu")]
    public GameObject gameObjectMainMenu;

    [Tooltip("Animator để chạy Animation cho UI Main Menu")]
    public Animator animatorMainMenu;

    [Tooltip("Tên Trigger/State Animation xuất hiện")]
    public string tenAniIn = "ani_in";

    [Tooltip("Tên Trigger/State Animation biến mất")]
    public string tenAniOut = "ani_out";

    [Tooltip("Thời gian chờ (giây) để animation ani_out chạy hết trước khi ẩn GameObject")]
    public float thoiGianChoAniOut = 0.5f;

    [Header("--- Cấu hình Âm Thanh Click Button ---")]
    [Tooltip("AudioClip tiếng Click khi bấm các nút trên Menu")]
    public AudioClip amThanhClick;

    [Tooltip("AudioSource dùng để phát âm thanh Click")]
    public AudioSource amThanhSource;

    private string duongDanFileSave;
    private Coroutine coroutineChuyenGiaoUI;

    private void Awake()
    {
        string thuMucFirst3D = Path.Combine(Application.persistentDataPath);
        duongDanFileSave = Path.Combine(thuMucFirst3D, "savegame.txt");
    }

    private void Start()
    {
        if (anhFadeOut != null)
        {
            Color mauBanDau = anhFadeOut.color;
            mauBanDau.a = 0f;
            anhFadeOut.color = mauBanDau;
            anhFadeOut.gameObject.SetActive(false);
        }

        if (amThanhSource == null)
        {
            amThanhSource = GetComponent<AudioSource>();
            if (amThanhSource == null)
            {
                amThanhSource = gameObject.AddComponent<AudioSource>();
            }
        }

        KiemTraVaGanLienKetSetting();
        CapNhatUnscaledTimeChoAnimators();
        DamBaoHienConTroChuot();

        if (gameObjectSetting != null)
        {
            gameObjectSetting.SetActive(false);
        }

        BatTatUIMainMenu(true);
    }

    private void CapNhatUnscaledTimeChoAnimators()
    {
        if (animatorMainMenu != null)
        {
            animatorMainMenu.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        if (animatorSetting != null)
        {
            animatorSetting.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    private void KiemTraVaGanLienKetSetting()
    {
        if (gameObjectSetting != null && animatorSetting == null)
        {
            animatorSetting = gameObjectSetting.GetComponent<Animator>();
        }

        CapNhatUnscaledTimeChoAnimators();
    }

    private void Update()
    {
        XuLyPhimEscSetting();
    }

    private void XuLyPhimEscSetting()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            BatTatSetting();
        }
    }

    public void PhatAmThanhClick()
    {
        if (amThanhSource != null && amThanhClick != null)
        {
            amThanhSource.PlayOneShot(amThanhClick);
        }
    }

    public void DamBaoHienConTroChuot()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BatTatUIMainMenu(bool bat)
    {
        if (gameObjectMainMenu == null) return;

        if (bat)
        {
            gameObjectMainMenu.SetActive(true);

            if (animatorMainMenu != null && !string.IsNullOrEmpty(tenAniIn))
            {
                animatorMainMenu.Play(tenAniIn, 0, 0f);
            }

            DamBaoHienConTroChuot();
        }
        else
        {
            if (animatorMainMenu != null && !string.IsNullOrEmpty(tenAniOut))
            {
                animatorMainMenu.Play(tenAniOut, 0, 0f);
            }
        }
    }

    private IEnumerator Routine_ChuyenSceneCoFade(string tenScene)
    {
        if (anhFadeOut != null)
        {
            anhFadeOut.gameObject.SetActive(true);
            float thoiGianDaTroi = 0f;
            Color mauAnh = anhFadeOut.color;

            while (thoiGianDaTroi < thoiGianFadeOut)
            {
                thoiGianDaTroi += Time.unscaledDeltaTime;
                float tyLeAlpha = Mathf.Clamp01(thoiGianDaTroi / thoiGianFadeOut);

                mauAnh.a = tyLeAlpha;
                anhFadeOut.color = mauAnh;

                yield return null;
            }

            mauAnh.a = 1f;
            anhFadeOut.color = mauAnh;
        }

        SceneManager.LoadScene(tenScene);
    }

    public void StartGameMoi()
    {
        PhatAmThanhClick();
        Time.timeScale = 1f;

        StartCoroutine(Routine_ChuyenSceneCoFade(tenSceneGameMoi));
    }

    public void TiepTucManChoi()
    {
        PhatAmThanhClick();
        Time.timeScale = 1f;

        string mapCanLoad = tenSceneGameMoi;

        if (File.Exists(duongDanFileSave))
        {
            string chuoiJson = File.ReadAllText(duongDanFileSave);
            SaveDataDataMenu data = JsonUtility.FromJson<SaveDataDataMenu>(chuoiJson);

            if (data != null && !string.IsNullOrEmpty(data.tenMapHienTai))
            {
                Debug.Log("Đang chuyển tới Map đã save: " + data.tenMapHienTai);
                mapCanLoad = data.tenMapHienTai;
            }
            else
            {
                Debug.LogWarning("File save chưa có dữ liệu Map, chuyển sang màn chơi mới!");
            }
        }
        else
        {
            Debug.LogWarning("Chưa có file save.txt, bắt đầu màn chơi mới!");
        }

        StartCoroutine(Routine_ChuyenSceneCoFade(mapCanLoad));
    }

    public void BatTatSetting()
    {
        PhatAmThanhClick();
        KiemTraVaGanLienKetSetting();

        if (gameObjectSetting != null)
        {
            bool dangHoatDong = gameObjectSetting.activeSelf;
            if (dangHoatDong)
            {
                DongSetting();
            }
            else
            {
                MoSetting();
            }
        }
    }

    public void MoSetting()
    {
        PhatAmThanhClick();
        KiemTraVaGanLienKetSetting();

        if (!gameObject.activeInHierarchy)
        {
            if (gameObjectSetting != null)
            {
                gameObjectSetting.SetActive(true);
                if (animatorSetting != null && !string.IsNullOrEmpty(tenSettingAniIn))
                    animatorSetting.Play(tenSettingAniIn, 0, 0f);
            }
            DamBaoHienConTroChuot();
            return;
        }

        if (coroutineChuyenGiaoUI != null)
        {
            StopCoroutine(coroutineChuyenGiaoUI);
        }

        coroutineChuyenGiaoUI = StartCoroutine(Routine_MoSetting());
    }

    private IEnumerator Routine_MoSetting()
    {
        if (gameObjectSetting != null)
        {
            gameObjectSetting.SetActive(true);

            if (animatorSetting != null && !string.IsNullOrEmpty(tenSettingAniIn))
            {
                animatorSetting.Play(tenSettingAniIn, 0, 0f);
            }
        }

        DamBaoHienConTroChuot();
        coroutineChuyenGiaoUI = null;
        yield break;
    }

    public void DongSetting()
    {
        KiemTraVaGanLienKetSetting();

        if (!gameObject.activeInHierarchy)
        {
            if (gameObjectSetting != null) gameObjectSetting.SetActive(false);
            DamBaoHienConTroChuot();
            return;
        }

        if (gameObjectSetting != null && gameObjectSetting.activeSelf)
        {
            if (coroutineChuyenGiaoUI != null)
            {
                StopCoroutine(coroutineChuyenGiaoUI);
            }

            coroutineChuyenGiaoUI = StartCoroutine(Routine_DongSetting());
        }
    }

    public void AnSettingButton()
    {
        PhatAmThanhClick();
        DongSetting();
    }

    private IEnumerator Routine_DongSetting()
    {
        if (animatorSetting != null && !string.IsNullOrEmpty(tenSettingAniOut))
        {
            animatorSetting.Play(tenSettingAniOut, 0, 0f);
        }

        yield return new WaitForSecondsRealtime(thoiGianChoSettingAniOut);

        if (gameObjectSetting != null)
        {
            gameObjectSetting.SetActive(false);
        }

        DamBaoHienConTroChuot();
        coroutineChuyenGiaoUI = null;
    }

    public void ThoatGame()
    {
        PhatAmThanhClick();
        Debug.Log("Đã thoát game!");
        Application.Quit();
    }
}