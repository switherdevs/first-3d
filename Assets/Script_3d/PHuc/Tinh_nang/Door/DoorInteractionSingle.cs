using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class DoorInteractionSingle : MonoBehaviour
{
    [Header("--- Cấu Hình Mở Cửa ---")]
    [Tooltip("Góc quay khi cửa mở (Độ)")]
    public float gocMoMucTieu = 90f;

    [Tooltip("Tốc độ quay mượt mà của cửa")]
    public float tocDoMoCua = 5f;

    [Tooltip("Khoảng cách tối đa từ Player đến Cửa để có thể tương tác")]
    public float khoangCachTuongTac = 3f;

    [Header("--- Cấu Hình Delay / Cooldown ---")]
    [Tooltip("Thời gian (giây) người chơi phải chờ trước khi có thể bấm mở/đóng cửa lần tiếp theo")]
    public float thoiGianCooldown = 1.0f;

    [Header("--- Cấu Hình Âm Thanh ---")]
    [Tooltip("AudioSource phát âm thanh cửa")]
    public AudioSource nguonAmThanhCua;

    [Tooltip("Âm thanh khi cửa bắt đầu mở")]
    public AudioClip amThanhMoCua;
    [Range(0f, 1f)] public float amLuongMoCua = 1.0f;

    [Tooltip("Âm thanh khi cửa bắt đầu đóng")]
    public AudioClip amThanhDongCua;
    [Range(0f, 1f)] public float amLuongDongCua = 1.0f;

    [Header("--- New Input System ---")]
    [Tooltip("Kéo Action tương tác (phím E) từ file Input Action Asset vào đây")]
    public InputActionReference actionTuongTac;

    [Header("--- Trạng Thái (Tự động cập nhật) ---")]
    [Tooltip("Cửa đang mở hay đóng")]
    public bool dangMo = false;

    private Quaternion gocDongBanDau;
    private Quaternion gocMoHuongHuong;
    private Transform playerTransform;
    private float boDemCooldown = 0f;

    private void OnEnable()
    {
        if (actionTuongTac != null && actionTuongTac.action != null)
        {
            actionTuongTac.action.Enable();
            actionTuongTac.action.performed += XuLyNhanPhimTuongTac;
        }
    }

    private void OnDisable()
    {
        if (actionTuongTac != null && actionTuongTac.action != null)
        {
            actionTuongTac.action.performed -= XuLyNhanPhimTuongTac;
            actionTuongTac.action.Disable();
        }
    }

    private void Start()
    {
        // 1. Tự động lấy AudioSource nếu chưa gán
        if (nguonAmThanhCua == null)
        {
            nguonAmThanhCua = GetComponent<AudioSource>();
        }
        if (nguonAmThanhCua != null)
        {
            nguonAmThanhCua.playOnAwake = false;
            nguonAmThanhCua.loop = false;
        }

        // 2. Lưu lại góc xoay ban đầu và tính góc mở
        gocDongBanDau = transform.rotation;
        gocMoHuongHuong = gocDongBanDau * Quaternion.Euler(0f, gocMoMucTieu, 0f);

        // 3. Tự động tìm Player dựa vào Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("[DoorInteractionSingle] Không tìm thấy GameObject nào mang Tag 'Player' trong Scene!");
        }
    }

    private void Update()
    {
        // Giảm dần thời gian hồi chiêu (Cooldown) theo thời gian thực
        if (boDemCooldown > 0f)
        {
            boDemCooldown -= Time.deltaTime;
        }

        // Xử lý xoay cửa mượt mà (Lerp) mỗi Frame
        Quaternion gocQuayMucTieu = dangMo ? gocMoHuongHuong : gocDongBanDau;
        transform.rotation = Quaternion.Slerp(transform.rotation, gocQuayMucTieu, Time.deltaTime * tocDoMoCua);
    }

    // Hàm được gọi tự động khi New Input System nhận tín hiệu nhấn phím
    private void XuLyNhanPhimTuongTac(InputAction.CallbackContext context)
    {
        if (playerTransform == null) return;

        // Nếu vẫn đang trong thời gian cooldown thì bỏ qua lệnh bấm
        if (boDemCooldown > 0f) return;

        // Tính khoảng cách thực tế giữa Cửa và Player
        float khoangCach = Vector3.Distance(transform.position, playerTransform.position);

        // Nếu đứng trong phạm vi cho phép thì thực hiện đổi trạng thái cửa
        if (khoangCach <= khoangCachTuongTac)
        {
            dangMo = !dangMo;

            // Kích hoạt thời gian delay (cooldown) mới
            boDemCooldown = thoiGianCooldown;

            // Phát âm thanh tương ứng khi chuyển đổi trạng thái
            if (nguonAmThanhCua != null)
            {
                if (dangMo && amThanhMoCua != null)
                {
                    nguonAmThanhCua.PlayOneShot(amThanhMoCua, amLuongMoCua);
                }
                else if (!dangMo && amThanhDongCua != null)
                {
                    nguonAmThanhCua.PlayOneShot(amThanhDongCua, amLuongDongCua);
                }
            }

            Debug.Log(dangMo ? "Cửa đã MỞ" : "Cửa đã ĐÓNG");
        }
    }
}