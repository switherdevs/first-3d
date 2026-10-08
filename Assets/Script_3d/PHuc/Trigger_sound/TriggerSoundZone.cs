using System.Collections;
using UnityEngine;

public class TriggerSoundZone : MonoBehaviour
{
    [Header("--- Tham chiếu Âm Thanh ---")]
    [Tooltip("Component AudioSource để phát âm thanh (Nếu để trống script sẽ tự tìm)")]
    public AudioSource amThanhSource;

    [Tooltip("File âm thanh sẽ phát khi Player bước vào")]
    public AudioClip amThanhPhat;

    [Header("--- Cấu Hình Cooldown & Tag ---")]
    [Tooltip("Tag của đối tượng kích hoạt (thường là Player)")]
    public string tagNhanVat = "Player";

    [Tooltip("Thời gian hồi giữa các lần phát âm thanh (tính bằng giây)")]
    public float thoiGianHoi = 5f;

    private bool dangCoHoi = false;

    private void Start()
    {
        // Tự động lấy AudioSource nếu chưa kéo vào Inspector
        if (amThanhSource == null)
        {
            amThanhSource = GetComponent<AudioSource>();
            if (amThanhSource == null)
            {
                amThanhSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Đảm bảo AudioSource không tự động phát khi vào game
        amThanhSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra đúng Tag Player và vùng trigger không trong thời gian hồi
        if (!dangCoHoi && other.CompareTag(tagNhanVat))
        {
            PhatAmThanhVaBatHoi();
        }
    }

    private void PhatAmThanhVaBatHoi()
    {
        // Phát âm thanh 1 lần duy nhất bằng PlayOneShot
        if (amThanhSource != null && amThanhPhat != null)
        {
            amThanhSource.PlayOneShot(amThanhPhat);
        }

        // Bắt đầu đếm ngược thời gian hồi
        StartCoroutine(CoDemThoiGianHoi());
    }

    private IEnumerator CoDemThoiGianHoi()
    {
        dangCoHoi = true;

        // Chờ đúng số giây cấu hình ở thoiGianHoi
        yield return new WaitForSeconds(thoiGianHoi);

        dangCoHoi = false;
    }
}