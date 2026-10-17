using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // Bổ sung thư viện UI để dùng Image
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Sử dụng cho phím ESC (Unity New Input System)

// 1. Cấu trúc dữ liệu đọc từ file save.txt (Phải trùng với cấu trúc trong SaveSystem)
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
    public string tenSceneGameMoi = "ThanhTrucLam"; // Map khởi đầu

    [Header("--- Cấu hình Fade Out Chuyển Map ---")]
    [Tooltip("Gán UI Image màu đen phủ kín màn hình để làm hiệu ứng mờ dần")]
    public Image anhFadeOut;

    [Tooltip("Thời gian hiệu ứng Fade Out chuyển màn (mặc định 2 giây)")]
    public float thoiGianFadeOut = 2f;

    [Header("--- UI Settings ---")]
    [Tooltip("GameObject chứa UI Setting (Gán Game Object Bảng Setting vào đây)")]
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
        // Khởi tạo đường dẫn đến file savegame.txt cố định
        string thuMucFirst3D = Path.Combine(Application.persistentDataPath);
        duongDanFileSave = Path.Combine(thuMucFirst3D, "savegame.txt");
    }

    private void Start()
    {
        // Đảm bảo ban đầu Image Fade Out trong suốt
        if (anhFadeOut != null)
        {
            Color mauBanDau = anhFadeOut.color;
            mauBanDau.a = 0f;
            anhFadeOut.color = mauBanDau;
            anhFadeOut.gameObject.SetActive(false);
        }

        // Tự động tìm/tạo AudioSource nếu bị thiếu
        if (amThanhSource == null)
        {
            amThanhSource = GetComponent<AudioSource>();
            if (amThanhSource == null)
            {
                amThanhSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Tự động kiểm tra và liên kết lại UI Setting nếu bị Null / Missing do DontDestroyOnLoad
        KiemTraVaGanLienKetSetting();

        // Đảm bảo các Animator UI không bị đóng băng khi Time.timeScale = 0
        CapNhatUnscaledTimeChoAnimators();

        // Đảm bảo khi vào Main Menu thì luôn HIỆN con trỏ chuột để bấm bấm nút
        DamBaoHienConTroChuot();

        // Ẩn bảng Setting khi vừa vào Menu
        if (gameObjectSetting != null)
        {
            gameObjectSetting.SetActive(false);
        }

        // Khi script vừa được nạp thì tự động gọi ani_in của Main Menu ngay lập tức
        BatTatUIMainMenu(true);
    }

    // --- HÀM BỔ SUNG: ĐẢM BẢO ANIMATOR VẪN CHẠY KHI TIME.TIMESCALE = 0 ---
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
        if (gameObjectSetting == null && SettingsManager.Instance != null)
        {
            gameObjectSetting = SettingsManager.Instance.gameObject;
        }

        if (gameObjectSetting != null && animatorSetting == null)
        {
            animatorSetting = gameObjectSetting.GetComponent<Animator>();
        }

        // Đảm bảo cập nhật lại UnscaledTime sau khi tự động gắn liên kết Setting mới
        CapNhatUnscaledTimeChoAnimators();
    }

    private void Update()
    {
        // Bấm phím ESC để bật/tắt nhanh UI Setting
        XuLyPhimEscSetting();
    }

    private void XuLyPhimEscSetting()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            BatTatSetting();
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

    // --- HÀM BỔ SUNG: ĐẢM BẢO LUÔN HIỆN CON TRỎ CHUỘT ---
    public void DamBaoHienConTroChuot()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =========================================================
    // HÀM BẬT/TẮT UI MAIN MENU VÀ ANIMATION (ani_in / ani_out)
    // =========================================================
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

    // --- COROUTINE XỬ LÝ FADE OUT VÀ CHUYỂN SCENE ---
    private IEnumerator Routine_ChuyenSceneCoFade(string tenScene)
    {
        // Nếu có gán Image Fade Out thì tiến hành làm mờ dần
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

                yield return null; // Chờ sang frame tiếp theo
            }

            // Đảm bảo hoàn toàn tối đen trước khi chuyển map
            mauAnh.a = 1f;
            anhFadeOut.color = mauAnh;
        }

        // Chuyển sang Map mới
        SceneManager.LoadScene(tenScene);
    }

    // 1. HÀM START GAME (Bắt đầu game mới)
    public void StartGameMoi()
    {
        PhatAmThanhClick();
        Time.timeScale = 1f;

        // Bắt đầu Coroutine Fade Out trước khi load Scene
        StartCoroutine(Routine_ChuyenSceneCoFade(tenSceneGameMoi));
    }

    // 2. HÀM TIẾP TỤC MÀN CHƠI (Continue Game)
    public void TiepTucManChoi()
    {
        PhatAmThanhClick();
        Time.timeScale = 1f;

        string mapCanLoad = tenSceneGameMoi;

        // Kiểm tra xem đã có file save.txt hay chưa
        if (File.Exists(duongDanFileSave))
        {
            string chuoiJson = File.ReadAllText(duongDanFileSave);
            SaveDataDataMenu data = JsonUtility.FromJson<SaveDataDataMenu>(chuoiJson);

            // Kiểm tra tên Map đã được ghi lại trong file save chưa
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

        // Bắt đầu Coroutine Fade Out trước khi load Scene
        StartCoroutine(Routine_ChuyenSceneCoFade(mapCanLoad));
    }

    // 3. HÀM BẬT/TẮT UI SETTING (TOGGLE THÔNG MINH)
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

    // HÀM MỞ UI SETTING (Gán cho Button Setting ở Main Menu)
    public void MoSetting()
    {
        PhatAmThanhClick();
        KiemTraVaGanLienKetSetting();

        // Nếu GameObject script đang Inactive, bật trực tiếp không cần Coroutine
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
        // Bật Setting UI và phát Animation ani_in trực tiếp mà KHÔNG ẩn Main Menu
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

    // HÀM ĐÓNG UI SETTING (Dùng nội bộ hoặc bấm phím ESC)
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

    // HÀM BỔ SUNG: GÁN VÀO BUTTON ĐÓNG/ẨN SETTING UI
    public void AnSettingButton()
    {
        PhatAmThanhClick();
        DongSetting();
    }

    private IEnumerator Routine_DongSetting()
    {
        // 1. Gọi Animation ani_out của Setting UI
        if (animatorSetting != null && !string.IsNullOrEmpty(tenSettingAniOut))
        {
            animatorSetting.Play(tenSettingAniOut, 0, 0f);
        }

        // 2. Đợi animation ani_out của Setting chạy hết
        yield return new WaitForSecondsRealtime(thoiGianChoSettingAniOut);

        // 3. Tắt Setting UI (Main Menu vẫn giữ nguyên phía dưới)
        if (gameObjectSetting != null)
        {
            gameObjectSetting.SetActive(false);
        }

        DamBaoHienConTroChuot();
        coroutineChuyenGiaoUI = null;
    }

    // 4. HÀM THOÁT GAME
    public void ThoatGame()
    {
        PhatAmThanhClick();
        Debug.Log("Đã thoát game!");
        Application.Quit(); // Chỉ hoạt động khi đã Build ra file .exe
    }
}