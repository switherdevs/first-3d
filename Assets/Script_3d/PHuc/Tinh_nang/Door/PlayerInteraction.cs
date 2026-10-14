using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("--- Tham Chiếu Tương Tác ---")]
    [Tooltip("Cửa hiện tại mà người chơi đang đứng trong vùng tương tác")]
    private DoorController cuaHienTaiTrongVung;

    /// <summary>
    /// Hàm nhận sự kiện phím E từ New Input System (Gắn Action Interact)
    /// </summary>
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            // Nếu người chơi đang đứng trong vùng của cửa nào đó thì gọi hàm tương tác mở/đóng cửa
            if (cuaHienTaiTrongVung != null)
            {
                cuaHienTaiTrongVung.ChuyenDoiTrangThaiCua();
            }
        }
    }

    // Các hàm được gọi từ Script gắn ở Cửa khi Player bước vào/rời khỏi vùng tròn
    public void DangVaoVungCua(DoorController cua)
    {
        cuaHienTaiTrongVung = cua;
    }

    public void RoiKhoiVungCua(DoorController cua)
    {
        if (cuaHienTaiTrongVung == cua)
        {
            cuaHienTaiTrongVung = null;
        }
    }
}