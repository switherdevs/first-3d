using UnityEngine;
using TMPro;

public class Map1PuzzleManager : MonoBehaviour
{
    public static Map1PuzzleManager Instance;

    [Header("UI")]
    public GameObject cluePanel;
    public TMP_Text clueText;
    public TMP_Text objectiveText;
    public TMP_Text missionText;

    [Header("Code Panel")]
    public GameObject codePanel;
    public CodeInputUI codeInputUI;

    [Header("Puzzle Numbers")]
    public GameObject number4Text;
    public GameObject number1Text;
    public GameObject number7Text;

    [Header("Clue Targets")]
    public Transform paperTarget;
    public Transform ticketTarget;
    public Transform number4Target;
    public Transform number1Target;
    public Transform number7Target;
    public Transform boxTarget;
    public Transform exitTarget;

    [Header("Player Interaction")]
    public PlayerMap1Interaction playerInteraction;

    [Header("Code Settings")]
    public int maxWrongAttempts = 3;

    private bool foundPaper = false;
    private bool foundTicket = false;

    private bool foundNumber4 = false;
    private bool foundNumber1 = false;
    private bool foundNumber7 = false;

    private bool boxOpened = false;

    private int wrongCodeAttempts = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (cluePanel != null)
            cluePanel.SetActive(false);

        if (codePanel != null)
            codePanel.SetActive(false);

        if (number4Text != null)
            number4Text.SetActive(false);

        if (number1Text != null)
            number1Text.SetActive(false);

        if (number7Text != null)
            number7Text.SetActive(false);

        SetMission(
            "Tìm mảnh giấy",
            paperTarget,
            "Trên bàn gỗ gần lối vào khu chợ"
        );
    }

    // ==================================================
    // MISSION
    // ==================================================

    private void SetMission(
        string mission,
        Transform target,
        string hint
    )
    {
        if (missionText != null)
        {
            missionText.text =
                "NHIỆM VỤ:\n" + mission;
        }

        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: " + mission;
        }

        if (playerInteraction != null)
        {
            playerInteraction.SetTargetClue(
                target,
                hint
            );
        }
    }

    // ==================================================
    // PAPER
    // ==================================================

    public void FindPaper()
    {
        if (foundPaper)
        {
            ShowClue(
                "Bạn đã kiểm tra mảnh giấy này rồi."
            );

            return;
        }

        foundPaper = true;

        ShowClue(
            "MẢNH GIẤY ƯỚT\n\n" +
            "\"Nơi người thân trở về sau mỗi chuyến đi...\""
        );

        SetMission(
            "Tìm phiếu cũ",
            ticketTarget,
            "Ở một quầy hàng cũ trong khu chợ"
        );
    }

    // ==================================================
    // TICKET
    // ==================================================

    public void FindTicket()
    {
        if (!foundPaper)
        {
            ShowClue(
                "Bạn cần tìm mảnh giấy trước."
            );

            return;
        }

        if (foundTicket)
        {
            ShowClue(
                "Bạn đã kiểm tra phiếu cũ rồi."
            );

            return;
        }

        foundTicket = true;

        ShowClue(
            "PHIẾU CŨ\n\n" +
            "Tên: Huy\n\n" +
            "\"Ba dấu hiệu sẽ mở đường.\""
        );

        UpdateNumberMission();
    }

    // ==================================================
    // NUMBER 4
    // ==================================================

    public void FindNumber4()
    {
        if (!foundTicket)
        {
            ShowClue(
                "Bạn chưa biết cần tìm gì ở chiếc cân."
            );

            return;
        }

        if (foundNumber4)
        {
            ShowClue(
                "Bạn đã tìm thấy số 4 rồi."
            );

            return;
        }

        foundNumber4 = true;

        if (number4Text != null)
            number4Text.SetActive(true);

        ShowClue(
            "CHIẾC CÂN CŨ\n\n" +
            "Dưới đáy cân có khắc số:\n\n4"
        );

        UpdateNumberMission();
    }

    // ==================================================
    // NUMBER 1
    // ==================================================

    public void FindNumber1()
    {
        if (!foundTicket)
        {
            ShowClue(
                "Bạn chưa biết cần tìm gì ở chiếc đèn."
            );

            return;
        }

        if (foundNumber1)
        {
            ShowClue(
                "Bạn đã tìm thấy số 1 rồi."
            );

            return;
        }

        foundNumber1 = true;

        if (number1Text != null)
            number1Text.SetActive(true);

        ShowClue(
            "ĐÈN DẦU CŨ\n\n" +
            "Phía sau đèn có khắc số:\n\n1"
        );

        UpdateNumberMission();
    }

    // ==================================================
    // NUMBER 7
    // ==================================================

    public void FindNumber7()
    {
        if (!foundTicket)
        {
            ShowClue(
                "Bạn chưa biết cần tìm gì ở vệt nước."
            );

            return;
        }

        if (foundNumber7)
        {
            ShowClue(
                "Bạn đã tìm thấy số 7 rồi."
            );

            return;
        }

        foundNumber7 = true;

        if (number7Text != null)
            number7Text.SetActive(true);

        ShowClue(
            "VỆT NƯỚC\n\n" +
            "Một ký hiệu hiện ra:\n\n7"
        );

        UpdateNumberMission();
    }

    // ==================================================
    // UPDATE 3 CLUES
    // ==================================================

    private void UpdateNumberMission()
    {
        int count = 0;

        if (foundNumber4)
            count++;

        if (foundNumber1)
            count++;

        if (foundNumber7)
            count++;

        // DU 3 SO
        if (count >= 3)
        {
            SetMission(
                "Tìm chiếc hộp khóa",
                boxTarget,
                "Chiếc hộp nằm ở khu vực cuối chợ"
            );

            return;
        }

        if (missionText != null)
        {
            missionText.text =
                "NHIỆM VỤ:\n" +
                "Tìm 3 manh mối (" +
                count +
                "/3)";
        }

        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: Tìm 3 manh mối (" +
                count +
                "/3)";
        }

        // Target tiep theo
        if (!foundNumber4)
        {
            SetNumberTarget(
                number4Target,
                "Chiếc cân cũ nằm tại một quầy hàng"
            );
        }
        else if (!foundNumber1)
        {
            SetNumberTarget(
                number1Target,
                "Đèn dầu cũ nằm gần khu vực giữa chợ"
            );
        }
        else if (!foundNumber7)
        {
            SetNumberTarget(
                number7Target,
                "Vệt nước nằm dưới đất cạnh một quầy cũ"
            );
        }
    }

    private void SetNumberTarget(
        Transform target,
        string hint
    )
    {
        if (playerInteraction != null)
        {
            playerInteraction.SetTargetClue(
                target,
                hint
            );
        }
    }

    // ==================================================
    // BOX
    // ==================================================

    public void OpenCodePanel()
    {
        if (!foundNumber4 ||
            !foundNumber1 ||
            !foundNumber7)
        {
            ShowClue(
                "Bạn chưa tìm đủ 3 manh mối."
            );

            return;
        }

        if (boxOpened)
        {
            ShowClue(
                "Chiếc hộp đã được mở."
            );

            return;
        }

        if (codePanel != null)
            codePanel.SetActive(true);

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        if (codeInputUI != null)
        {
            codeInputUI.ClearCode();
        }
    }

    // ==================================================
    // CHECK CODE
    // ==================================================

    public void CheckCode(string code)
    {
        // DUNG 417
        if (code == "417")
        {
            boxOpened = true;
            wrongCodeAttempts = 0;

            // XOA 3 O INPUT
            if (codeInputUI != null)
            {
                codeInputUI.ClearCode();
            }

            // DONG CODE PANEL
            if (codePanel != null)
            {
                codePanel.SetActive(false);
            }

            ShowClue(
                "MỞ KHÓA THÀNH CÔNG!\n\n" +
                "Bên trong có một tờ giấy ghi:\n\n" +
                "ĐỊA CHỈ NHÀ HUY"
            );

            SetMission(
                "Đi đến lối ra",
                exitTarget,
                "Lối ra nằm ở cuối khu chợ"
            );

            return;
        }

        // =========================
        // SAI CODE
        // =========================

        wrongCodeAttempts++;

        // Xoa input de nhap lai
        if (codeInputUI != null)
        {
            codeInputUI.ClearCode();
        }

        // Chua sai du 3 lan
        if (wrongCodeAttempts < maxWrongAttempts)
        {
            int remaining =
                maxWrongAttempts -
                wrongCodeAttempts;

            ShowClue(
                "MÃ SAI!\n\n" +
                "Bạn còn " +
                remaining +
                " lần thử."
            );

            return;
        }

        // =========================
        // SAI DU 3 LAN
        // =========================

        wrongCodeAttempts = 0;

        if (codePanel != null)
        {
            codePanel.SetActive(false);
        }

        ResetNumberClues();

        ShowClue(
            "BẠN ĐÃ NHẬP SAI 3 LẦN!\n\n" +
            "Ba manh mối đã bị mất.\n\n" +
            "Hãy tìm lại từ đầu."
        );
    }

    // ==================================================
    // RESET 3 NUMBER CLUES
    // ==================================================

    private void ResetNumberClues()
    {
        foundNumber4 = false;
        foundNumber1 = false;
        foundNumber7 = false;

        if (number4Text != null)
            number4Text.SetActive(false);

        if (number1Text != null)
            number1Text.SetActive(false);

        if (number7Text != null)
            number7Text.SetActive(false);

        if (missionText != null)
        {
            missionText.text =
                "NHIỆM VỤ:\n" +
                "Tìm 3 manh mối (0/3)";
        }

        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: Tìm 3 manh mối (0/3)";
        }

        if (playerInteraction != null)
        {
            playerInteraction.SetTargetClue(
                number4Target,
                "Chiếc cân cũ nằm tại một quầy hàng"
            );
        }
    }

    // ==================================================
    // EXIT
    // ==================================================

    public void GoToExit()
    {
        if (!boxOpened)
        {
            ShowClue(
                "Bạn chưa mở chiếc hộp."
            );

            return;
        }

        if (missionText != null)
        {
            missionText.text =
                "NHIỆM VỤ:\nMAP 1 COMPLETE";
        }

        if (objectiveText != null)
        {
            objectiveText.text =
                "MAP 1 COMPLETE";
        }

        if (playerInteraction != null)
        {
            playerInteraction.SetTargetClue(
                null,
                ""
            );
        }

        ShowClue(
            "MAP 1 HOÀN THÀNH\n\n" +
            "Điểm đến tiếp theo:\n\n" +
            "NHÀ HUY"
        );
    }

    // ==================================================
    // CLUE PANEL
    // ==================================================

    public void ShowClue(string message)
    {
        if (cluePanel == null ||
            clueText == null)
            return;

        cluePanel.SetActive(true);
        clueText.text = message;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    public void CloseClue()
    {
        if (cluePanel != null)
            cluePanel.SetActive(false);

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }
}