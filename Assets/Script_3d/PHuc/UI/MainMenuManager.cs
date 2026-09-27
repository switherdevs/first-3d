using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("--- UI Settings ---")]
    [Tooltip("GameObject Panel chứa UI Setting")]
    public GameObject panelSetting;

    private string duongDanFileSave;

    private void Awake()
    {
        // Khởi tạo đường dẫn đến file savegame.txt cố định
        string thuMucFirst3D = Path.Combine(Application.persistentDataPath);
        duongDanFileSave = Path.Combine(thuMucFirst3D, "savegame.txt");
    }

    private void Start()
    {
        // Ẩn bảng Setting khi vừa vào Menu
        if (panelSetting != null)
        {
            panelSetting.SetActive(false);
        }
    }

    // 1. HÀM START GAME (Bắt đầu game mới)
    public void StartGameMoi()
    {
        // Nếu chọn chơi mới, có thể xóa file save cũ (nếu muốn) hoặc load thẳng scene đầu tiên
        SceneManager.LoadScene(tenSceneGameMoi);
    }

    // 2. HÀM TIẾP TỤC MÀN CHƠI (Continue Game)
    public void TiepTucManChoi()
    {
        // Kiểm tra xem đã có file save.txt hay chưa
        if (File.Exists(duongDanFileSave))
        {
            string chuoiJson = File.ReadAllText(duongDanFileSave);
            SaveDataDataMenu data = JsonUtility.FromJson<SaveDataDataMenu>(chuoiJson);

            // Kiểm tra tên Map đã được ghi lại trong file save chưa
            if (data != null && !string.IsNullOrEmpty(data.tenMapHienTai))
            {
                Debug.Log("Đang chuyển tới Map đã save: " + data.tenMapHienTai);
                SceneManager.LoadScene(data.tenMapHienTai);
            }
            else
            {
                Debug.LogWarning("File save chưa có dữ liệu Map, chuyển sang màn chơi mới!");
                SceneManager.LoadScene(tenSceneGameMoi);
            }
        }
        else
        {
            Debug.LogWarning("Chưa có file save.txt, bắt đầu màn chơi mới!");
            SceneManager.LoadScene(tenSceneGameMoi);
        }
    }

    // 3. HÀM MỞ UI SETTING
    public void MoPanelSetting()
    {
        if (panelSetting != null)
        {
            panelSetting.SetActive(true);
        }
    }

    // HÀM ĐÓNG UI SETTING (Gán thêm cho nút X/Close trong Setting)
    public void DongPanelSetting()
    {
        if (panelSetting != null)
        {
            panelSetting.SetActive(false);
        }
    }

    // 4. HÀM THOÁT GAME
    public void ThoatGame()
    {
        Debug.Log("Đã thoát game!");
        Application.Quit(); // Chỉ hoạt động khi đã Build ra file .exe
    }
}