using System.Collections;
using UnityEngine;
using TMPro;

public class MissionUIManager : MonoBehaviour
{
    public static MissionUIManager Instance { get; private set; }


    // =========================================================
    // UI
    // =========================================================

    [Header("UI")]
    [SerializeField] private RectTransform missionPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text objectiveText;


    // =========================================================
    // VỊ TRÍ
    // =========================================================

    [Header("VỊ TRÍ")]
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform displayPoint;


    // =========================================================
    // THỜI GIAN
    // =========================================================

    [Header("THỜI GIAN")]
    [SerializeField] private float fadeInDuration = 0.35f;
    [SerializeField] private float waitBeforeMove = 0.5f;
    [SerializeField] private float moveDuration = 0.65f;
    [SerializeField] private float fadeOutDuration = 0.4f;


    // =========================================================
    // NHIỆM VỤ BAN ĐẦU
    // =========================================================

    [Header("NHIỆM VỤ BAN ĐẦU")]
    [SerializeField] private bool showOnStart = true;

    [SerializeField] private string startTitle = "NHIỆM VỤ";

    [TextArea(2, 4)]
    [SerializeField]
    private string startObjective =
        "Tìm manh mối";


    private Coroutine currentRoutine;

    private bool missionActive = false;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;


        // Canvas Group
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }


        // Mission UI
        if (missionPanel != null)
        {
            missionPanel.gameObject.SetActive(false);
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (showOnStart)
        {
            ShowMission(
                startTitle,
                startObjective
            );
        }
    }


    // =========================================================
    // HIỆN NHIỆM VỤ
    // =========================================================

    public void ShowMission(
        string title,
        string objective
    )
    {
        // Kiểm tra setup
        if (missionPanel == null)
        {
            Debug.LogError(
                "MissionUIManager: Chưa gắn Mission Panel!"
            );

            return;
        }


        if (canvasGroup == null)
        {
            Debug.LogError(
                "MissionUIManager: Chưa gắn Canvas Group!"
            );

            return;
        }


        if (startPoint == null)
        {
            Debug.LogError(
                "MissionUIManager: Chưa gắn MissionStartPoint!"
            );

            return;
        }


        if (displayPoint == null)
        {
            Debug.LogError(
                "MissionUIManager: Chưa gắn MissionDisplayPoint!"
            );

            return;
        }


        // Dừng animation cũ
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }


        // Nội dung
        if (titleText != null)
        {
            titleText.text = title;
        }


        if (objectiveText != null)
        {
            objectiveText.text = objective;
        }


        currentRoutine =
            StartCoroutine(
                ShowMissionRoutine()
            );
    }


    // =========================================================
    // ANIMATION NHẬN NHIỆM VỤ
    // =========================================================

    private IEnumerator ShowMissionRoutine()
    {
        missionActive = true;


        // Bật UI
        missionPanel.gameObject.SetActive(true);


        // Không cho UI chặn chuột
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;


        // Bắt đầu trong suốt
        canvasGroup.alpha = 0f;


        // =====================================================
        // ĐƯA UI TỚI VỊ TRÍ BAN ĐẦU
        // =====================================================

        missionPanel.position =
            startPoint.position;


        // =====================================================
        // FADE IN
        // =====================================================

        float timer = 0f;


        while (timer < fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;


            float t = Mathf.Clamp01(
                timer / fadeInDuration
            );


            canvasGroup.alpha = t;


            yield return null;
        }


        canvasGroup.alpha = 1f;


        // =====================================================
        // ĐỨNG GIỮA MÀN HÌNH MỘT CHÚT
        // =====================================================

        yield return new WaitForSecondsRealtime(
            waitBeforeMove
        );


        // =====================================================
        // BAY TỚI VỊ TRÍ HIỂN THỊ
        // =====================================================

        Vector3 from =
            missionPanel.position;


        Vector3 to =
            displayPoint.position;


        timer = 0f;


        while (timer < moveDuration)
        {
            timer += Time.unscaledDeltaTime;


            float t = Mathf.Clamp01(
                timer / moveDuration
            );


            // Chuyển động mượt
            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );


            missionPanel.position =
                Vector3.Lerp(
                    from,
                    to,
                    t
                );


            yield return null;
        }


        missionPanel.position = to;


        currentRoutine = null;
    }


    // =========================================================
    // ĐỔI MỤC TIÊU
    // =========================================================

    public void UpdateObjective(
        string newObjective
    )
    {
        if (objectiveText == null)
            return;


        objectiveText.text =
            newObjective;
    }


    // =========================================================
    // ĐỔI CẢ TITLE + OBJECTIVE
    // =========================================================

    public void UpdateMission(
        string title,
        string objective
    )
    {
        if (titleText != null)
        {
            titleText.text = title;
        }


        if (objectiveText != null)
        {
            objectiveText.text = objective;
        }
    }


    // =========================================================
    // HOÀN THÀNH NHIỆM VỤ
    // =========================================================

    public void CompleteMission()
    {
        if (!missionActive)
            return;


        if (missionPanel == null ||
            canvasGroup == null)
        {
            return;
        }


        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }


        currentRoutine =
            StartCoroutine(
                FadeOutRoutine()
            );
    }


    // =========================================================
    // FADE OUT
    // =========================================================

    private IEnumerator FadeOutRoutine()
    {
        float startAlpha =
            canvasGroup.alpha;


        float timer = 0f;


        while (timer < fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;


            float t = Mathf.Clamp01(
                timer / fadeOutDuration
            );


            canvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    0f,
                    t
                );


            yield return null;
        }


        canvasGroup.alpha = 0f;


        missionPanel.gameObject.SetActive(false);


        missionActive = false;


        currentRoutine = null;
    }


    // =========================================================
    // ẨN NGAY
    // =========================================================

    public void HideImmediately()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }


        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }


        if (missionPanel != null)
        {
            missionPanel.gameObject.SetActive(false);
        }


        missionActive = false;
    }
}