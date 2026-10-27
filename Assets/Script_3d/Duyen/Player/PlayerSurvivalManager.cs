using UnityEngine;

public class PlayerSurvivalManager : MonoBehaviour
{
    [Header("UI Thua Game (Game Over)")]
    [Tooltip("GameObject giao diện hoặc màn hình Thua Game cần hiển thị khi bị bắt")]
    public GameObject gameOverUI;

    [Header("Tham chiếu Player")]
    [Tooltip("GameObject của người chơi (Sẽ không bị ẩn hay xóa)")]
    public GameObject player;

    private bool daThuaGame = false;

    private void Start()
    {
        // Tự động tìm Player nếu chưa gán trên Inspector
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p;
        }

        // Ẩn bảng Thua Game khi bắt đầu vào chơi
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    /// <summary>
    /// Hàm được gọi khi Ma Da hoặc bẫy bắt được người chơi
    /// </summary>
    public void PlayerCaught()
    {
        if (daThuaGame) return;
        daThuaGame = true;

        Debug.Log("Người chơi đã bị bắt! Kích hoạt màn hình Thua Game.");

        // 1. Hiển thị giao diện Thua Game lên màn hình
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 2. Khóa di chuyển của Player (Nhân vật đứng yên tại chỗ, hoàn toàn KHÔNG bị ẩn hay xóa)
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetKhoaDiChuyen(true);
            }
        }

        // 3. Mở khóa con trỏ chuột để người chơi có thể bấm các nút như Chơi lại (Restart) hoặc Thoát (Quit) trên UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}