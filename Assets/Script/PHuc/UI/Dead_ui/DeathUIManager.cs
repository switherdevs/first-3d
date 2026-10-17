using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DeathUIManager : MonoBehaviour
{
    [Header("--- UI Elements ---")]
    [Tooltip("CanvasGroup của Panel Death UI để làm hiệu ứng mờ / rõ")]
    public CanvasGroup panelDeathUI;

    [Tooltip("Thời gian hiệu ứng hiện UI Death (Giây)")]
    public float thoiGianHienUI = 1.5f;

    [Header("--- Reference Components ---")]
    [Tooltip("Player Input để tắt điều khiển khi chết")]
    public PlayerInput inputNhanVat;

    private bool daChet = false;

    private void Start()
    {
        // Ban đầu ẩn hoàn toàn Canvas Death UI
        if (panelDeathUI != null)
        {
            panelDeathUI.alpha = 0f;
            panelDeathUI.interactable = false;
            panelDeathUI.blocksRaycasts = false;
        }
    }

    // Hàm gọi khi Nhân vật đụng phải Ma
    public void KichHoatNhanVatChet()
    {
        if (daChet) return;
        daChet = true;

        // 1. Khóa di chuyển nhân vật
        if (inputNhanVat != null)
        {
            inputNhanVat.DeactivateInput();
        }

        // 2. Mở chuột để tương tác UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Chạy Coroutine hiện UI mượt mà
        StartCoroutine(CoHieuUngHienUIDead());
    }

    // Coroutine làm mờ/rõ CanvasGroup
    private IEnumerator CoHieuUngHienUIDead()
    {
        float thoiGianDem = 0f;

        while (thoiGianDem < thoiGianHienUI)
        {
            thoiGianDem += Time.deltaTime;
            if (panelDeathUI != null)
            {
                panelDeathUI.alpha = Mathf.Clamp01(thoiGianDem / thoiGianHienUI);
            }
            yield return null;
        }

        if (panelDeathUI != null)
        {
            panelDeathUI.interactable = true;
            panelDeathUI.blocksRaycasts = true;
        }
    }

    #region Button Events
    // Nút Chơi Lại Màn Đó
    public void OnClickChoiLai()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Nút Trở Về Bản Đồ Lớn (Overworld)
    public void OnClickVeMapLon(string tenSceneOverworld)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(tenSceneOverworld);
    }

    // Nút Về Menu Chính
    public void OnClickVeMenuChinh()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}