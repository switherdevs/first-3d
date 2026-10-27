using UnityEngine;
using System.Collections;

public class BlackShadowDash : MonoBehaviour
{
    [Header("Cài đặt Phát hiện (Trigger Zone)")]
    [Tooltip("Bán kính vùng phát hiện quanh điểm đặt Trigger")]
    public float detectionRadius = 5f;

    [Header("Điểm xuất phát và kết thúc cố định")]
    public Transform pointA;
    public Transform pointB;

    [Header("Cài đặt bóng đen")]
    public GameObject shadowObject; // GameObject bóng đen
    public float tocDoVut = 25f;    // Tốc độ lướt vụt qua

    [Header("Âm thanh giật mình")]
    public AudioClip jumpScareSound;
    public AudioSource audioSource;

    private bool daKichHoat = false;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Ẩn bóng đen lúc đầu
        if (shadowObject != null)
        {
            shadowObject.SetActive(false);
        }
    }

    void Update()
    {
        if (daKichHoat) return;

        // Dùng Physics.OverlapSphere để quét các đối tượng xung quanh bán kính
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (Collider hit in hits)
        {
            // BẮT BUỘC: Kiểm tra đối tượng phải có Tag là "Player"
            if (hit.CompareTag("Player"))
            {
                daKichHoat = true;
                StartCoroutine(VutQuaDiem());
                break;
            }
        }
    }

    IEnumerator VutQuaDiem()
    {
        if (shadowObject == null || pointA == null || pointB == null) yield break;

        // Đặt vị trí bắt đầu tại Điểm A và kích hoạt hiện lên
        shadowObject.transform.position = pointA.position;
        shadowObject.SetActive(true);

        // Phát âm thanh giật mình (âm thanh giật 1)
        if (audioSource != null && jumpScareSound != null)
        {
            audioSource.PlayOneShot(jumpScareSound);
        }

        // Tính toán thời gian lướt từ A sang B
        float khoangCach = Vector3.Distance(pointA.position, pointB.position);
        float thoiGianDiChuyen = khoangCach / tocDoVut;
        float thoiGianDaChay = 0f;

        Vector3 viTriBatDau = pointA.position;
        Vector3 viTriKetThuc = pointB.position;

        while (thoiGianDaChay < thoiGianDiChuyen)
        {
            shadowObject.transform.position = Vector3.Lerp(viTriBatDau, viTriKetThuc, thoiGianDaChay / thoiGianDiChuyen);
            thoiGianDaChay += Time.deltaTime;
            yield return null;
        }

        shadowObject.transform.position = viTriKetThuc;

        // Ẩn bóng đen sau khi vụt xong đến điểm B
        shadowObject.SetActive(false);
    }

    // Vẽ bán kính quét kiểm tra Tag Player trong cửa sổ Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}