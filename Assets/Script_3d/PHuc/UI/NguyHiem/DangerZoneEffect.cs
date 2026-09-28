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

    [Tooltip("Tốc độ chuyên đổi độ tối để tránh giật cục")]
    public float tocDoChuyenDoiUI = 3f;

    [Header("--- CẤU HÌNH QUÉT VÙNG NGUY HIỂM (OVERLAP SPHERE) ---")]
    [Tooltip("Bán kính quét xung quanh Player để phát hiện DangerZone từ xa")]
    public float banKinhQuet = 15f;

    [Tooltip("Layer của các GameObject Danger Zone")]
    public LayerMask layerDangerZone;

    [Header("--- CẤU HÌNH ÂM THANH KINH DỊ ---")]
    [Tooltip("Kéo AudioSource phát tiếng ghê rợn vào đây")]
    public AudioSource amThanhKinhDi;

    [Tooltip("Âm lượng tối đa khi tới sát tâm (Từ 0 đến 1)")]
    [Range(0f, 1f)] public float amLuongToiDa = 1f;

    [Tooltip("Tốc độ chuyển đổi âm thanh")]
    public float tocDoChuyenDoiAmThanh = 3f;

    // Biến nội bộ
    private Vignette hieuUngVignette;

    private void Start()
    {
        // 1. LẤY HIỆU ỨNG VÀ ÉP MỞ TRẠNG THÁI OVERRIDE
        if (globalVolume != null && globalVolume.profile != null)
        {
            if (globalVolume.profile.TryGet(out hieuUngVignette))
            {
                // Bật overrideState để Unity cho phép code can thiệp giá trị
                hieuUngVignette.intensity.overrideState = true;
            }
            else
            {
                Debug.LogWarning("[DangerZoneEffect] Chưa thêm hiệu ứng Vignette vào Volume Profile!");
            }
        }

        // 2. KHỞI CHẠY AUDIO SOURCE
        if (amThanhKinhDi != null)
        {
            amThanhKinhDi.loop = true;
            amThanhKinhDi.volume = 0f;
            if (!amThanhKinhDi.isPlaying)
            {
                amThanhKinhDi.Play();
            }
        }
    }

    private void Update()
    {
        QuetVungNguyHiemRealtime();
    }

    // 🎯 THUẬT TOÁN QUÉT VÙNG NGUY HIỂM VÀ TÍNH ĐỘ TỐI THEO THỜI GIAN THỰC
    private void QuetVungNguyHiemRealtime()
    {
        // Truyền thêm QueryTriggerInteraction.Collide để bắt buộc quét trúng cả Trigger Collider
        Collider[] danhSachVaCham = Physics.OverlapSphere(
            transform.position,
            banKinhQuet,
            layerDangerZone,
            QueryTriggerInteraction.Collide
        );

        float mucDoNguyHiemToiDa = 0f; // Mức độ nguy hiểm từ 0.0 đến 1.0

        if (danhSachVaCham.Length > 0)
        {
            float khoangCachNganNhat = float.MaxValue;

            // Duyệt qua tất cả vùng DangerZone quét được
            foreach (var col in danhSachVaCham)
            {
                // Lấy điểm gần nhất trên khối DangerZone so với vị trí Player
                Vector3 diemGanNhat = col.ClosestPoint(transform.position);
                float khoangCach = Vector3.Distance(transform.position, diemGanNhat);

                if (khoangCach < khoangCachNganNhat)
                {
                    khoangCachNganNhat = khoangCach;
                }
            }

            // Quy đổi khoảng cách ra tỷ lệ % nguy hiểm (Từ 0 đến 1)
            mucDoNguyHiemToiDa = 1f - Mathf.Clamp01(khoangCachNganNhat / banKinhQuet);
        }

        // Tính độ tối mục tiêu
        float doToiMuctieu = mucDoNguyHiemToiDa * doToiToiDa;

        // 🎯 THAY ĐỔI TRỰC TIẾP WEIGHT CỦA GLOBAL VOLUME (Thấy nhảy số trực tiếp trên Inspector)
        if (globalVolume != null)
        {
            globalVolume.weight = Mathf.Lerp(globalVolume.weight, doToiMuctieu, tocDoChuyenDoiUI * Time.deltaTime);
        }

        // Đồng bộ thêm giá trị Intensity của Vignette
        if (hieuUngVignette != null)
        {
            hieuUngVignette.intensity.value = Mathf.Lerp(hieuUngVignette.intensity.value, doToiMuctieu, tocDoChuyenDoiUI * Time.deltaTime);
        }

        // 🎯 CẬP NHẬT ÂM THANH REAL-TIME
        if (amThanhKinhDi != null)
        {
            float amLuongMucTieu = mucDoNguyHiemToiDa * amLuongToiDa;
            amThanhKinhDi.volume = Mathf.Lerp(amThanhKinhDi.volume, amLuongMucTieu, tocDoChuyenDoiAmThanh * Time.deltaTime);
        }
    }

    // Vẽ vòng tròn xem bán kính quét trực quan trong Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, banKinhQuet);
    }
}