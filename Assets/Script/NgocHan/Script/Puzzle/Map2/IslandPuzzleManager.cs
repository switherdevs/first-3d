using UnityEngine;

public class IslandPuzzleManager : MonoBehaviour
{
    // =========================================================
    // THÔNG TIN CÙ LAO
    // =========================================================

    [Header("THÔNG TIN CÙ LAO")]
    [SerializeField] private string islandName = "Cù lao 1";


    // =========================================================
    // PUZZLE
    // =========================================================

    [Header("PUZZLE")]
    [SerializeField] private int totalSteps = 3;

    [Tooltip("Nhiệm vụ tương ứng Step 0, Step 1, Step 2...")]
    [TextArea(2, 4)]
    [SerializeField] private string[] stepObjectives;


    // =========================================================
    // NHIỆM VỤ SAU KHI XONG CÙ LAO
    // =========================================================

    [Header("SAU KHI HOÀN THÀNH CÙ LAO")]

    [TextArea(2, 4)]
    [SerializeField]
    private string afterCompleteObjective =
        "Đi đến khu vực tiếp theo.";


    // =========================================================
    // KHỞI ĐỘNG NHIỆM VỤ
    // =========================================================

    [Header("NHIỆM VỤ BAN ĐẦU")]

    [Tooltip("Chỉ bật ở cù lao đầu tiên của map.")]
    [SerializeField] private bool showMissionOnStart = false;

    [SerializeField] private string missionTitle = "NHIỆM VỤ";


    // =========================================================
    // RUNTIME
    // =========================================================

    private int currentStep = 0;
    private bool completed = false;


    public int CurrentStep => currentStep;
    public bool Completed => completed;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (!showMissionOnStart)
            return;


        if (MissionUIManager.Instance == null)
        {
            Debug.LogWarning(
                islandName +
                ": Không tìm thấy MissionUIManager."
            );

            return;
        }


        string firstObjective =
            GetObjectiveForStep(0);


        MissionUIManager.Instance.ShowMission(
            missionTitle,
            firstObjective
        );
    }


    // =========================================================
    // KIỂM TRA STEP
    // =========================================================

    public bool CanInteract(int stepIndex)
    {
        if (completed)
            return false;


        return stepIndex == currentStep;
    }


    // =========================================================
    // HOÀN THÀNH STEP
    // =========================================================

    public bool CompleteStep(
        int stepIndex,
        string objectName,
        string nextHint
    )
    {
        // Cù lao đã xong
        if (completed)
            return false;


        // Sai thứ tự
        if (stepIndex != currentStep)
        {
            ShowMessage(
                "Có vẻ mình chưa cần thứ này..."
            );

            return false;
        }


        Debug.Log(
            islandName +
            " | Step " +
            currentStep +
            " hoàn thành: " +
            objectName
        );


        // Chuyển step
        currentStep++;


        // =====================================================
        // CÒN STEP TIẾP THEO
        // =====================================================

        if (currentStep < totalSteps)
        {
            string objective =
                GetObjectiveForStep(
                    currentStep
                );


            UpdateMissionObjective(
                objective
            );


            // nextHint vẫn hỗ trợ nếu Owner đang dùng
            if (!string.IsNullOrEmpty(nextHint))
            {
                ShowMessage(nextHint);
            }


            return true;
        }


        // =====================================================
        // HOÀN THÀNH CÙ LAO
        // =====================================================

        completed = true;


        Debug.Log(
            "HOÀN THÀNH " +
            islandName
        );


        if (!string.IsNullOrEmpty(
            afterCompleteObjective
        ))
        {
            UpdateMissionObjective(
                afterCompleteObjective
            );
        }


        return true;
    }


    // =========================================================
    // LẤY OBJECTIVE THEO STEP
    // =========================================================

    private string GetObjectiveForStep(
        int step
    )
    {
        if (stepObjectives != null &&
            step >= 0 &&
            step < stepObjectives.Length &&
            !string.IsNullOrEmpty(
                stepObjectives[step]
            ))
        {
            return stepObjectives[step];
        }


        return "Tiếp tục tìm manh mối.";
    }


    // =========================================================
    // UPDATE UI NHIỆM VỤ
    // =========================================================

    private void UpdateMissionObjective(
        string objective
    )
    {
        if (MissionUIManager.Instance == null)
        {
            Debug.LogWarning(
                "Không tìm thấy MissionUIManager!"
            );

            return;
        }


        MissionUIManager.Instance.UpdateObjective(
            objective
        );
    }


    // =========================================================
    // THÔNG BÁO
    // =========================================================

    public void ShowMessage(
        string message
    )
    {
        Debug.Log(message);
    }
}