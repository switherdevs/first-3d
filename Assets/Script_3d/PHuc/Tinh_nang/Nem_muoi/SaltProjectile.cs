using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SaltProjectile : MonoBehaviour
{
    [Header("--- Cấu Hình Hiệu Ứng & Biến Mất ---")]
    [Tooltip("Prefab hiệu ứng (nổ muối/văng tóe) khi chạm mục tiêu")]
    public GameObject hieuUngTrungPrefab;

    [Tooltip("Thời gian trễ trước khi viên muối biến mất hoàn toàn sau khi va chạm (giây)")]
    public float thoiGianBienMatSauKhiVaCham = 0.05f;

    [Tooltip("Thời gian tự hủy tối đa nếu bay đi quá xa mà không trúng gì (giây)")]
    public float thoiGianTonTaiToiDa = 5f;

    private bool daVaCham = false;

    private void Start()
    {
        // Bỏ qua va chạm giữa viên muối và Player ngay khi vừa spawn để tránh tự bắn vào mình
        Collider colVienMuoi = GetComponent<Collider>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Collider colPlayer = player.GetComponent<Collider>();
            if (colPlayer != null && colVienMuoi != null)
            {
                Physics.IgnoreCollision(colVienMuoi, colPlayer, true);
            }
        }

        // Tự hủy nếu bay quá lâu
        Destroy(gameObject, thoiGianTonTaiToiDa);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (daVaCham) return;

        if (!collision.collider.isTrigger)
        {
            daVaCham = true;

            ContactPoint diemVaCham = collision.contacts[0];
            Vector3 viTriVaCham = diemVaCham.point;
            Quaternion gocXoayVaCham = Quaternion.LookRotation(diemVaCham.normal);

            if (hieuUngTrungPrefab != null)
            {
                Instantiate(hieuUngTrungPrefab, viTriVaCham, gocXoayVaCham);
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            Renderer rend = GetComponent<Renderer>();
            if (rend != null) rend.enabled = false;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Destroy(gameObject, thoiGianBienMatSauKhiVaCham);
        }
    }
}