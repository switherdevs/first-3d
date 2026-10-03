using UnityEngine;

public class WaterSystem : MonoBehaviour
{
    // Kiểm tra Player có đang ở trong nước không
    public bool playerInWater = false;

    // Player đi vào nước
    public void PlayerEnterWater()
    {
        playerInWater = true;

        Debug.Log("Player đã xuống nước!");
    }

    // Player đi ra khỏi nước
    public void PlayerExitWater()
    {
        playerInWater = false;

        Debug.Log("Player đã ra khỏi nước!");
    }
}