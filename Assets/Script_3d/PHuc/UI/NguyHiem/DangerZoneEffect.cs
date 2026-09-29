using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DangerZoneEffect : MonoBehaviour
{
    [Header("--- CẤU HÌNH GLOBAL VOLUME ---")]
    [Tooltip("Kéo GameObject Global Volume trong Scene vào đây")]
    public Volume globalVolume;

    [Tooltip("Mức độ tối sầm tối đa khi tới sát tâm vùng nguy hiểm (Từ 0 đến 1)")]
    [Range(0f, 1f)] public float doToiToiDa = 0.85f;

    [Tooltip("Tốc độ chuyển đổi độ tối để tránh giật cục")]
    public float tocDoChuyenDoiUI = 3f;

    [Header("--- CẤU HÌNH QUÉT VÙNG NGUY HIỂM (OVERLAP SPHERE) ---")]
    [Tooltip("Bán kính quét xung quanh Player để phát hiện DangerZone từ xa")]
    public float banKinhQuet = 15f;

    [Tooltip("Layer của các GameObject Danger Zone")]
    public LayerMask layerDangerZone;

    [Header("--- CẤU HÌNH ÂM THANH KINH DỊ NỀN ---")]
    [Tooltip("AudioSource phát tiếng rên/rè nền liên tục")]
    public AudioSource amThanhKinhDi;

    [Tooltip("Âm lượng tối đa khi tới sát tâm (Từ 0 đến 1)")]
    [Range(0f, 1f)] public float amLuongToiDa = 1f;

    [Tooltip("Tốc độ chuyển đổi âm thanh nền")]
    public float tocDoChuyenDoiAmThanh = 3f;

    [Header("--- CẤU HÌNH ÂM THANH KINH DỊ RANDOM ---")]
    [Tooltip("Danh sách âm thanh kinh dị phát ngẫu nhiên")]
    public AudioClip[] dsAmThanhKinhDi = new AudioClip[3];

    [Tooltip("Khoảng thời gian cố định phát âm thanh random (Giây)")]
    public float thoiGianPhatRandom = 45f;

    [Header("--- CẤU HÌNH FADE IN / FADE OUT ÂM THANH RANDOM ---")]
    [Tooltip("Thời gian âm thanh ngẫu nhiên to dần lên (Giây)")]
    public float thoiGianFadeIn = 2f;

    [Tooltip("Thời gian âm thanh ngẫu nhiên nhỏ dần rồi tắt hẳn (Giây)")]
    public float thoiGianFadeOut = 2f;

    // Biến nội bộ
    private Vignette hieuUngVignette;
    private float demThoiGianAmThanh = 0f;
    private bool dangPhatAmThanhRandom = false;
    private AudioSource amThanhRandomSource; // AudioSource cố định dành riêng cho âm thanh random

    private void Start()
    {
        // 1. LẤY HIỆU ỨNG VÀ ÉP MỞ TRẠNG THÁI OVERRIDE
        if (globalVolume != null && globalVolume.profile != null)
        {
            if (globalVolume.profile.TryGet(out hieuUngVignette))
            {
                hieuUngVignette.intensity.overrideState = true;
            }
            else
            {
                Debug.LogWarning("[DangerZoneEffect] Chưa thêm hiệu ứng Vignette vào Volume Profile!");
            }
        }

        // 2. KHỞI CHẠY AUDIO SOURCE NỀN DANGER ZONE
        if (amThanhKinhDi == null)
        {
            amThanhKinhDi = GetComponent<AudioSource>();
        }

        if (amThanhKinhDi != null)
        {
            amThanhKinhDi.loop = true;
            amThanhKinhDi.volume = 0f;
            if (!amThanhKinhDi.isPlaying)
            {
                amThanhKinhDi.Play();
            }
        }

        // 3. TẠO TỰ ĐỘNG AUDIOSOURCE CỐ ĐỊNH PHÁT ÂM THANH RANDOM (KHÔNG CẦN KÉO TAY)
        amThanhRandomSource = gameObject.AddComponent<AudioSource>();
        amThanhRandomSource.loop = false;
        amThanhRandomSource.playOnAwake = false;
        amThanhRandomSource.volume = 0f;
    }

    private void Update()
    {
        QuetVungNguyHiemRealtime();
        XuLyAmThanhKinhDiNgauNhien();
    }

    // 🎯 THUẬT TOÁN QUÉT VÙNG NGUY HIỂM VÀ TÍNH ĐỘ TỐI / ÂM THANH NỀN REALTIME
    private void QuetVungNguyHiemRealtime()
    {
        Collider[] danhSachVaCham = Physics.OverlapSphere(
            transform.position,
            banKinhQuet,
            layerDangerZone,
            QueryTriggerInteraction.Collide
        );

        float mucDoNguyHiemToiDa = 0f;

        if (danhSachVaCham.Length > 0)
        {
            float khoangCachNganNhat = float.MaxValue;

            foreach (var col in danhSachVaCham)
            {
                Vector3 diemGanNhat = col.ClosestPoint(transform.position);
                float khoangCach = Vector3.Distance(transform.position, diemGanNhat);

                if (khoangCach < khoangCachNganNhat)
                {
                    khoangCachNganNhat = khoangCach;
                }
            }

            mucDoNguyHiemToiDa = 1f - Mathf.Clamp01(khoangCachNganNhat / banKinhQuet);
        }

        float doToiMuctieu = mucDoNguyHiemToiDa * doToiToiDa;

        if (globalVolume != null)
        {
            globalVolume.weight = Mathf.Lerp(globalVolume.weight, doToiMuctieu, tocDoChuyenDoiUI * Time.deltaTime);
        }

        if (hieuUngVignette != null)
        {
            hieuUngVignette.intensity.value = Mathf.Lerp(hieuUngVignette.intensity.value, doToiMuctieu, tocDoChuyenDoiUI * Time.deltaTime);
        }

        // Cập nhật volume cho âm thanh nền Danger Zone
        if (amThanhKinhDi != null)
        {
            float amLuongMucTieu = mucDoNguyHiemToiDa * amLuongToiDa;
            amThanhKinhDi.volume = Mathf.Lerp(amThanhKinhDi.volume, amLuongMucTieu, tocDoChuyenDoiAmThanh * Time.deltaTime);
        }
    }

    // 🎯 THUẬT TOÁN ĐẾM THỜI GIAN LẮP ÂM THANH RANDOM
    private void XuLyAmThanhKinhDiNgauNhien()
    {
        if (dsAmThanhKinhDi == null || dsAmThanhKinhDi.Length == 0 || dangPhatAmThanhRandom) return;

        demThoiGianAmThanh += Time.deltaTime;

        if (demThoiGianAmThanh >= thoiGianPhatRandom)
        {
            int chiSoNgauNhien = Random.Range(0, dsAmThanhKinhDi.Length);

            if (dsAmThanhKinhDi[chiSoNgauNhien] != null)
            {
                StartCoroutine(FadeAmThanhCoDinh(dsAmThanhKinhDi[chiSoNgauNhien]));
            }

            demThoiGianAmThanh = 0f;
        }
    }

    // 🎯 COROUTINE ĐIỀU CHỈNH FADE IN / FADE OUT TRÊN AUDIOSOURCE CỐ ĐỊNH
    private IEnumerator FadeAmThanhCoDinh(AudioClip clip)
    {
        dangPhatAmThanhRandom = true;

        // 1. Gán clip và chuẩn bị phát từ volume = 0
        amThanhRandomSource.clip = clip;
        amThanhRandomSource.volume = 0f;
        amThanhRandomSource.Play();

        // 2. FADE IN (Tăng volume từ 0 lên 1)
        float demThoiGian = 0f;
        while (demThoiGian < thoiGianFadeIn)
        {
            demThoiGian += Time.deltaTime;
            amThanhRandomSource.volume = Mathf.Lerp(0f, 1f, demThoiGian / thoiGianFadeIn);
            yield return null;
        }
        amThanhRandomSource.volume = 1f;

        // 3. CHỜ PHÁT ĐOẠN GIỮA (Phát tới điểm bắt đầu Fade Out)
        float thoiDiemFadeOut = clip.length - thoiGianFadeOut;
        while (amThanhRandomSource.isPlaying && amThanhRandomSource.time < thoiDiemFadeOut)
        {
            yield return null;
        }

        // 4. FADE OUT (Giảm volume từ 1 về 0)
        demThoiGian = 0f;
        while (demThoiGian < thoiGianFadeOut)
        {
            demThoiGian += Time.deltaTime;
            amThanhRandomSource.volume = Mathf.Lerp(1f, 0f, demThoiGian / thoiGianFadeOut);
            yield return null;
        }

        // 5. NGẮT ÂM THANH HOÀN TOÀN KHI KẾT THÚC
        amThanhRandomSource.volume = 0f;
        amThanhRandomSource.Stop();

        dangPhatAmThanhRandom = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, banKinhQuet);
    }
}