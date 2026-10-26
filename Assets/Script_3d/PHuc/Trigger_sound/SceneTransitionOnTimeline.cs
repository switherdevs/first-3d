using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneTransitionOnTimeline : MonoBehaviour
{
    [Header("--- Tham Chiếu Playable Director (Timeline) ---")]
    [Tooltip("Kéo Component PlayableDirector của Timeline vào đây")]
    public PlayableDirector playableDirector;

    [Header("--- Cấu Hình Thời Gian Chuyển Scene ---")]
    [Tooltip("Thời điểm chính xác (tính bằng giây) trong Timeline mà bạn muốn kích hoạt chuyển scene")]
    public float thoiDiemChuyenScene = 5.0f;

    [Header("--- Cấu Hình Chuyển Scene & Fade Out ---")]
    [Tooltip("Tên của Scene tiếp theo bạn muốn load")]
    public string tenSceneTiepTheo;

    [Tooltip("GameObject làm màn hình đen Fade Out (UI Image phủ kín màn hình)")]
    public GameObject doiTuongFadeOut;

    [Tooltip("Tốc độ mờ dần của hiệu ứng Fade Out")]
    public float tocDoFade = 1.5f;

    private bool daKichHoatChuyenScene = false;

    private void Start()
    {
        // Tự động tìm PlayableDirector nếu để trống
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        // Đảm bảo ban đầu tắt hiệu ứng fade out (hoặc bật alpha = 0 tùy ý)
        if (doiTuongFadeOut != null)
        {
            doiTuongFadeOut.SetActive(false);
        }
    }

    private void Update()
    {
        // Kiểm tra nếu Timeline đang chạy và chưa được kích hoạt chuyển scene
        if (playableDirector != null && playableDirector.state == PlayState.Playing && !daKichHoatChuyenScene)
        {
            // Lấy thời gian hiện tại đang chạy của Timeline
            double thoiGianHienTai = playableDirector.time;

            // Nếu thời gian hiện tại đạt đến mốc do bạn cài đặt trên Inspector
            if (thoiGianHienTai >= thoiDiemChuyenScene)
            {
                StartCoroutine(ThucHienFadeOutVaChuyenScene());
            }
        }
    }

    /// <summary>
    /// Coroutine xử lý hiện GameObject Fade Out và đợi chuyển Scene mượt mà
    /// </summary>
    private IEnumerator ThucHienFadeOutVaChuyenScene()
    {
        daKichHoatChuyenScene = true;

        // Bật GameObject màn hình Fade Out lên
        if (doiTuongFadeOut != null)
        {
            doiTuongFadeOut.SetActive(true);

            // Nếu GameObject có UI CanvasGroup, ta có thể cho nó mờ dần alpha từ 0 lên 1
            CanvasGroup canvasGroup = doiTuongFadeOut.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                while (canvasGroup.alpha < 1f)
                {
                    canvasGroup.alpha += Time.deltaTime * tocDoFade;
                    yield return null;
                }
            }
            else
            {
                // Nếu không có CanvasGroup thì chờ một khoảng ngắn sau khi bật GameObject
                yield return new WaitForSeconds(1.0f);
            }
        }
        else
        {
            // Nếu quên gán GameObject fade, chờ 1 giây rồi chuyển luôn
            yield return new WaitForSeconds(1.0f);
        }

        // Tiến hành load Scene tiếp theo
        if (!string.IsNullOrEmpty(tenSceneTiepTheo))
        {
            SceneManager.LoadScene(tenSceneTiepTheo);
        }
        else
        {
            Debug.LogWarning("Chưa điền tên Scene tiếp theo trong Inspector!");
        }
    }
}