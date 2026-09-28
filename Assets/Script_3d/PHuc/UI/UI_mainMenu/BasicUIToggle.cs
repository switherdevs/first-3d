using UnityEngine;
using UnityEngine.UI;

public class BasicUIToggle : MonoBehaviour
{
    [Header("--- Cấu hình GameObject UI ---")]
    [Tooltip("Kéo bảng UI (GameObject) cần bật/tắt vào đây")]
    public GameObject bangUI;

    [Header("--- Cấu hình 2 Nút Bấm ---")]
    [Tooltip("Kéo Button dùng để BẬT UI vào đây")]
    public Button nutBat;

    [Tooltip("Kéo Button dùng để TẮT UI vào đây")]
    public Button nutTat;

    private void Start()
    {
        // Gán sự kiện lắng nghe khi bấm nút Bật
        if (nutBat != null)
        {
            nutBat.onClick.AddListener(MoUI);
        }

        // Gán sự kiện lắng nghe khi bấm nút Tắt
        if (nutTat != null)
        {
            nutTat.onClick.AddListener(TatUI);
        }
    }

    // Hàm Bật UI
    public void MoUI()
    {
        if (bangUI != null)
        {
            bangUI.SetActive(true);
        }
    }

    // Hàm Tắt UI
    public void TatUI()
    {
        if (bangUI != null)
        {
            bangUI.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Dọn dẹp sự kiện để tránh lỗi rò rỉ bộ nhớ
        if (nutBat != null) nutBat.onClick.RemoveListener(MoUI);
        if (nutTat != null) nutTat.onClick.RemoveListener(TatUI);
    }
}