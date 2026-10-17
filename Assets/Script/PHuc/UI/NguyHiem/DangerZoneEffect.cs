using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DangerZoneEffect : MonoBehaviour
{
    [Header("--- CẤU HÌNH GLOBAL VOLUME 1 (CHÍNH) ---")]
    [Tooltip("Kéo GameObject Global Volume thứ nhất trong Scene vào đây")]
    public Volume globalVolume;

    [Tooltip("Mức độ tối sầm tối đa khi tới sát tâm vùng nguy hiểm (Từ 0 đến 1)")]
    [Range(0f, 1f)] public float doToiToiDa = 0.85f;

    [Tooltip("Tốc độ chuyển đổi độ tối để tránh giật cục")]
    public float tocDoChuyenDoiUI = 3f;

    [Header("--- CẤU HÌNH GLOBAL VOLUME 2 (THỨ HAI - CHẠY SONG SONG) ---")]
    [Tooltip("Kéo GameObject Global Volume thứ hai trong Scene vào đây")]
    public Volume globalVolumeThuHai;

    [Tooltip("Mức độ trọng lượng (Weight) tối đa của Volume thứ hai khi tới sát tâm")]
    [Range(0f, 1f)] public float weightToiDaThuHai = 1f;

    [Header("--- CẤU HÌNH KHOẢNG CÁCH VÙNG NGUY HIỂM ---")]
    [Tooltip("Bán kính tối đa bắt đầu xuất hiện hiệu ứng (Đẩy ra xa hoặc thu lại gần ở đây)")]
    public float khoangCachBatDauHieuUng = 15f;

    [Tooltip("Khoảng cách tính từ tâm Danger Zone mà tại đó hiệu ứng đạt độ đậm đặc tối đa (100%)")]
    public float khoangCachDatDinhToiDa = 2f;

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
    private ChromaticAberration hieuUngChromaticAberration;
    private float demThoiGianAmThanh = 0f;
    private bool dangPhatAmThanhRandom = false;
    private AudioSource amThanhRandomSource;

    private void Start()
    {
        // 1. LẤY HIỆU ỨNG VIGNETTE CHO VOLUME 1
        if (globalVolume != null && globalVolume.profile != null)
        {
            if (globalVolume.profile.TryGet(out hieuUngVignette))
            {
                hieuUngVignette.intensity.overrideState = true;
            }
            else
            {
                Debug.LogWarning("[DangerZoneEffect] Chưa thêm hiệu ứng Vignette vào Volume Profile 1!");
            }
        }

        // 1.1 LẤY HIỆU ỨNG CHROMATIC ABERRATION CHO VOLUME 2
        if (globalVolumeThuHai != null && globalVolumeThuHai.profile != null)
        {
            if (globalVolumeThuHai.profile.TryGet(out hieuUngChromaticAberration))
            {
                hieuUngChromaticAberration.intensity.overrideState = true;
            }
            else
            {
                Debug.LogWarning("[DangerZoneEffect] Chưa thêm hiệu ứng Chromatic Aberration vào Volume Profile 2!");
            }
        }

        // 2. KHỞI CHẠY AUDIO SOURCE NỀN
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

        // 3. TẠO AUDIOSOURCE CỐ ĐỊNH PHÁT ÂM THANH RANDOM
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

    // 🎯 THUẬT TOÁN QUÉT VÙNG NGUY HIỂM VÀ TÍNH TOÁN KHOẢNG CÁCH TÙY CHỈNH
    private void QuetVungNguyHiemRealtime()
    {
        // Quét trong bán kính xa nhất được cấu hình (`khoangCachBatDauHieuUng`)
        Collider[] danhSachVaCham = Physics.OverlapSphere(
            transform.position,
            khoangCachBatDauHieuUng,
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

            // Áp dụng khoảng cách đỉnh tối đa để bóp hoặc giãn độ nhạy hiệu ứng
            float khoangCachThucTe = Mathf.Clamp(khoangCachNganNhat, khoangCachDatDinhToiDa, khoangCachBatDauHieuUng);

            // Tính toán tỷ lệ tuyến tính từ khoảng cách bắt đầu đến khoảng cách đạt đỉnh tối đa
            mucDoNguyHiemToiDa = 1f - ((khoangCachThucTe - khoangCachDatDinhToiDa) / (khoangCachBatDauHieuUng - khoangCachDatDinhToiDa));
            mucDoNguyHiemToiDa = Mathf.Clamp01(mucDoNguyHiemToiDa);
        }

        // --- XỬ LÝ GLOBAL VOLUME 1 (Vignette) ---
        float doToiMuctieu = mucDoNguyHiemToiDa * doToiToiDa;

        if (globalVolume != null)
        {
            globalVolume.weight = Mathf.Lerp(globalVolume.weight, doToiMuctieu, tocDoChuyenDoiUI * Time.deltaTime);
        }

        if (hieuUngVignette != null)
        {
            hieuUngVignette.intensity.value = Mathf.Lerp(hieuUngVignette.intensity.value, doToiMuctieu, tocDoChuyenDoiUI * Time.deltaTime);
        }

        // --- XỬ LÝ GLOBAL VOLUME 2 (Song song) ---
        float mucTieuThuHai = mucDoNguyHiemToiDa * weightToiDaThuHai;

        if (globalVolumeThuHai != null)
        {
            globalVolumeThuHai.weight = Mathf.Lerp(globalVolumeThuHai.weight, mucTieuThuHai, tocDoChuyenDoiUI * Time.deltaTime);
        }

        if (hieuUngChromaticAberration != null)
        {
            hieuUngChromaticAberration.intensity.value = Mathf.Lerp(hieuUngChromaticAberration.intensity.value, mucTieuThuHai, tocDoChuyenDoiUI * Time.deltaTime);
        }

        // --- XỬ LÝ ÂM THANH NỀN ---
        if (amThanhKinhDi != null)
        {
            float amLuongMucTieu = mucDoNguyHiemToiDa * amLuongToiDa;
            amThanhKinhDi.volume = Mathf.Lerp(amThanhKinhDi.volume, amLuongMucTieu, tocDoChuyenDoiAmThanh * Time.deltaTime);
        }
    }

    // 🎯 ĐẾM THỜI GIAN ÂM THANH RANDOM
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

    // 🎯 FADE IN / FADE OUT ÂM THANH RANDOM
    private IEnumerator FadeAmThanhCoDinh(AudioClip clip)
    {
        dangPhatAmThanhRandom = true;

        amThanhRandomSource.clip = clip;
        amThanhRandomSource.volume = 0f;
        amThanhRandomSource.Play();

        float demThoiGian = 0f;
        while (demThoiGian < thoiGianFadeIn)
        {
            demThoiGian += Time.deltaTime;
            amThanhRandomSource.volume = Mathf.Lerp(0f, 1f, demThoiGian / thoiGianFadeIn);
            yield return null;
        }
        amThanhRandomSource.volume = 1f;

        float thoiDiemFadeOut = clip.length - thoiGianFadeOut;
        while (amThanhRandomSource.isPlaying && amThanhRandomSource.time < thoiDiemFadeOut)
        {
            yield return null;
        }

        demThoiGian = 0f;
        while (demThoiGian < thoiGianFadeOut)
        {
            demThoiGian += Time.deltaTime;
            amThanhRandomSource.volume = Mathf.Lerp(1f, 0f, demThoiGian / thoiGianFadeOut);
            yield return null;
        }

        amThanhRandomSource.volume = 0f;
        amThanhRandomSource.Stop();

        dangPhatAmThanhRandom = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Vẽ 2 vòng tròn mô phỏng vùng bắt đầu và vùng đạt đỉnh tối đa trong Scene
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, khoangCachBatDauHieuUng);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, khoangCachDatDinhToiDa);
    }
}