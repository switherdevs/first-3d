using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DoorController : MonoBehaviour
{
    [Header("--- Cấu Hình Trạng Thái Cửa ---")]
    [Tooltip("Cửa đang mở hay đóng")]
    public bool dangMo = false;

    [Tooltip("Góc quay khi cửa mở (Độ)")]
    public float gocMoMucTieu = 90f;

    [Tooltip("Tốc độ quay mượt mà của cửa")]
    public float tocDoMoCua = 5f;

    [Header("--- Cấu Hình Vùng Tương Tác ---")]
    [Tooltip("Bán kính vùng tròn tương tác quanh cửa")]
    public float banKinhVungTron = 3f;

    private Quaternion gocDongBanDau;
    private Quaternion gocMoHuongHuong;
    private SphereCollider vungTronTrigger;

    private void Start()
    {
        // 1. Lưu lại góc xoay ban đầu (Trạng thái đóng)
        gocDongBanDau = transform.rotation;
        // Tính toán góc xoay khi mở dựa trên trục Y
        gocMoHuongHuong = gocDongBanDau * Quaternion.Euler(0f, gocMoMucTieu, 0f);

        // 2. Thiết lập Sphere Collider làm vùng tròn tương tác
        vungTronTrigger = GetComponent<SphereCollider>();
        if (vungTronTrigger != null)
        {
            vungTronTrigger.isTrigger = true;
            vungTronTrigger.radius = banKinhVungTron;
        }
    }

    private void Update()
    {
        // Xử lý xoay cửa mượt mà (Lerp) giữa đóng và mở mỗi Frame
        Quaternion gocQuayMucTieu = dangMo ? gocMoHuongHuong : gocDongBanDau;
        transform.rotation = Quaternion.Slerp(transform.rotation, gocQuayMucTieu, Time.deltaTime * tocDoMoCua);
    }

    /// <summary>
    /// Hàm chuyển đổi trạng thái Đóng <-> Mở khi người chơi nhấn phím E
    /// </summary>
    public void ChuyenDoiTrangThaiCua()
    {
        dangMo = !dangMo;
        Debug.Log(dangMo ? "Cửa đã MỞ" : "Cửa đã ĐÓNG");
    }

    #region Phát hiện Player vào/ra vùng tròn Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteraction player = other.GetComponent<PlayerInteraction>();
            if (player != null)
            {
                player.DangVaoVungCua(this);
                Debug.Log("Đã bước vào vùng tương tác của cửa!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInteraction player = other.GetComponent<PlayerInteraction>();
            if (player != null)
            {
                player.RoiKhoiVungCua(this);
                Debug.Log("Đã rời khỏi vùng tương tác của cửa!");
            }
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        // Hiển thị vòng tròn phạm vi tương tác trong Editor Scene
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, banKinhVungTron);
    }
}