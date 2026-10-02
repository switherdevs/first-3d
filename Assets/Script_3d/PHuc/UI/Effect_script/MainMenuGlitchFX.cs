using System.Collections;
using UnityEngine;

public class MainMenuGlitchFX : MonoBehaviour
{
    [Header("--- Cấu hình Hiệu ứng Rung Giật (Glitch) ---")]
    [Tooltip("Khoảng thời gian tối thiểu giữa 2 lần giật (giây)")]
    public float thoiGianChoToiThieu = 4f;

    [Tooltip("Khoảng thời gian tối đa giữa 2 lần giật (giây)")]
    public float thoiGianChoToiDa = 8f;

    [Tooltip("Độ lệch vị trí Camera khi bị giật")]
    public float doLechLieuMang = 0.15f;

    [Tooltip("Thời gian diễn ra 1 cú giật (giây)")]
    public float thoiGianGiat = 0.12f;

    private void Start()
    {
        StartCoroutine(LuongRungGiatNgauNhien());
    }

    private IEnumerator LuongRungGiatNgauNhien()
    {
        while (true)
        {
            // Chờ một khoảng thời gian ngẫu nhiên
            float thoiGianCho = Random.Range(thoiGianChoToiThieu, thoiGianChoToiDa);
            yield return new WaitForSeconds(thoiGianCho);

            // Kích hoạt giật nhẹ Camera
            Vector3 viTriGoc = transform.localPosition;
            float demThoiGian = 0f;

            while (demThoiGian < thoiGianGiat)
            {
                Vector3 viTriLienTuc = viTriGoc + (Vector3)Random.insideUnitCircle * doLechLieuMang;
                transform.localPosition = viTriLienTuc;

                demThoiGian += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = viTriGoc;
        }
    }
}