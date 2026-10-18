using UnityEngine;
using UnityEngine.InputSystem;

public class SaltThrower : MonoBehaviour
{
    [Header("--- Tham Chiếu Vị Trí & Prefab ---")]
    [Tooltip("Prefab viên muối sẽ được ném ra")]
    public GameObject muoiPrefab;

    [Tooltip("Transform điểm xuất phát của muối (Phải là GameObject con nằm trên Player)")]
    public Transform viTriNemMuoi;

    [Tooltip("Camera chính dùng để lấy hướng nhìn ném muối (Tự động tìm nếu để trống)")]
    public Camera cameraChinh;

    [Header("--- Animator Tham Chiếu ---")]
    [Tooltip("Animator của nhân vật để kích hoạt hoạt ảnh ném muối")]
    public Animator animatorNhanVat;

    [Header("--- Cấu Hình Đường Bay & Tốc Độ ---")]
    [Tooltip("Tốc độ bay phóng theo hướng nhìn Camera")]
    public float tocDoBay = 12f;

    [Tooltip("Độ cao (lực hất lên trời) để tạo đường cong parabol cho đường bay")]
    public float doCaoDuongBay = 4f;

    private void Start()
    {
        // Tự động tìm Main Camera nếu chưa gán trên Inspector
        if (cameraChinh == null && Camera.main != null)
        {
            cameraChinh = Camera.main;
        }

        // Tự động tìm Animator nếu chưa gán thủ công
        if (animatorNhanVat == null)
        {
            animatorNhanVat = GetComponentInParent<Animator>();
        }
    }

    /// <summary>
    /// Hàm nhận sự kiện từ New Input System (gắn vào Player Input)
    /// </summary>
    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            ThucHienNemMuoi();
        }
    }

    /// <summary>
    /// Thuật toán khởi tạo, kích hoạt animation và ném viên muối theo hướng Camera
    /// </summary>
    private void ThucHienNemMuoi()
    {
        if (muoiPrefab == null || viTriNemMuoi == null)
        {
            Debug.LogWarning("[SaltThrower] Chưa gán Prefab muối hoặc thiếu Transform vị trí ném con của Player!");
            return;
        }

        if (cameraChinh == null)
        {
            cameraChinh = Camera.main;
            if (cameraChinh == null)
            {
                Debug.LogError("[SaltThrower] Không tìm thấy Camera chính trong Scene!");
                return;
            }
        }

        // 1. Kích hoạt Animation Trigger "ThrowSalt" trong Animator
        if (animatorNhanVat != null)
        {
            animatorNhanVat.SetTrigger("ThrowSalt");
        }

        // 2. Lấy hướng nhìn phía trước và hướng lên của Camera để tính hướng ném
        Vector3 huongNhinCam = cameraChinh.transform.forward;

        // 3. Tạo viên muối tại vị trí GameObject con (viTriNemMuoi) của Player
        Quaternion gocXoayNem = Quaternion.LookRotation(huongNhinCam);
        GameObject vienMuoi = Instantiate(muoiPrefab, viTriNemMuoi.position, gocXoayNem);

        // 4. Tính toán vận tốc bay: Kết hợp hướng nhìn Camera nhân với tốc độ + lực nâng parabol theo trục Y
        Rigidbody rbMuoi = vienMuoi.GetComponent<Rigidbody>();
        if (rbMuoi != null)
        {
            Vector3 huongBayMoi = (huongNhinCam * tocDoBay) + (Vector3.up * doCaoDuongBay);
            rbMuoi.linearVelocity = huongBayMoi;
        }
    }
}