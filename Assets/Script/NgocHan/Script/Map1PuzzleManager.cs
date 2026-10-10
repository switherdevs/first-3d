using UnityEngine;
using TMPro;

public class Map1PuzzleManager : MonoBehaviour
{
    public static Map1PuzzleManager Instance;


    // ======================================================
    // UI
    // ======================================================

    [Header("UI")]
    public GameObject cluePanel;
    public TMP_Text clueText;
    public TMP_Text objectiveText;
    public TMP_Text missionText;


    // ======================================================
    // CODE PANEL
    // ======================================================

    [Header("Code Panel")]
    public GameObject codePanel;
    public CodeInputUI codeInputUI;


    // ======================================================
    // PUZZLE NUMBER UI
    // ======================================================

    [Header("Puzzle Numbers")]
    public GameObject number4Text;
    public GameObject number1Text;
    public GameObject number7Text;


    // ======================================================
    // RANDOM TARGETS
    // 5 TARGET NÀY KHÔNG CẦN KÉO TRONG INSPECTOR
    // ======================================================

    [Header("Random Puzzle Targets - ĐỂ TRỐNG")]
    public Transform paperTarget;
    public Transform ticketTarget;
    public Transform number4Target;
    public Transform number1Target;
    public Transform number7Target;


    // ======================================================
    // FIXED TARGETS
    // ======================================================

    [Header("Fixed Targets")]
    public Transform boxTarget;
    public Transform exitTarget;


    // ======================================================
    // PLAYER
    // ======================================================

    [Header("Player Interaction")]
    public PlayerMap1Interaction playerInteraction;


    // ======================================================
    // SETTINGS
    // ======================================================

    [Header("Code Settings")]
    public int maxWrongAttempts = 3;


    // ======================================================
    // STATE
    // ======================================================

    private bool foundPaper = false;
    private bool foundTicket = false;

    private bool foundNumber4 = false;
    private bool foundNumber1 = false;
    private bool foundNumber7 = false;

    private bool boxOpened = false;

    private int wrongCodeAttempts = 0;

    private bool firstMissionStarted = false;


    // ======================================================
    // AWAKE
    // ======================================================

    private void Awake()
    {
        Instance = this;
    }


    // ======================================================
    // START
    // ======================================================

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

        TryStartFirstMission();
    }


    // ======================================================
    // RANDOM SPAWN TARGET REGISTRATION
    // ======================================================

    public void RegisterSpawnedTarget(
        PuzzleItemType type,
        Transform target
    )
    {
        if (target == null)
            return;

        switch (type)
        {
            case PuzzleItemType.Paper:
                paperTarget = target;
                break;

            case PuzzleItemType.Ticket:
                ticketTarget = target;
                break;

            case PuzzleItemType.Number4:
                number4Target = target;
                break;

            case PuzzleItemType.Number1:
                number1Target = target;
                break;

            case PuzzleItemType.Number7:
                number7Target = target;
                break;
        }

        Debug.Log(
            "Đã đăng ký target: " +
            type +
            " -> " +
            target.name
        );

        TryStartFirstMission();
    }


    // ======================================================
    // START FIRST MISSION
    // ======================================================

    private void TryStartFirstMission()
    {
        if (firstMissionStarted)
            return;

        if (paperTarget == null)
            return;

        firstMissionStarted = true;

        SetMission(
            "Tìm mảnh giấy",
            paperTarget,
            "Hãy tìm mảnh giấy cũ trong khu chợ."
        );
    }


    // ======================================================
    // MISSION
    // ======================================================

    private void SetMission(
        string mission,
        Transform target,
        string hint
    )
    {
        if (missionText != null)
        {
            missionText.text =
                "NHIỆM VỤ:\n" +
                mission;
        }

        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: " +
                mission;
        }

        if (playerInteraction != null)
        {
            playerInteraction.SetTargetClue(
                target,
                hint
            );
        }
    }


    // ======================================================
    // PAPER
    // ======================================================

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
            "Hãy tìm phiếu mua hàng cũ trong khu chợ."
        );
    }


    // ======================================================
    // TICKET
    // ======================================================

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


    // ======================================================
    // NUMBER 4
    // ======================================================

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
            "Trên chiếc cân có một dấu khắc:\n\n" +
            "4"
        );

        UpdateNumberMission();
    }


    // ======================================================
    // NUMBER 1
    // ======================================================

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
            "Trên chiếc đèn có một dấu khắc:\n\n" +
            "1"
        );

        UpdateNumberMission();
    }


    // ======================================================
    // NUMBER 7
    // ======================================================

    public void FindNumber7()
    {
        if (!foundTicket)
        {
            ShowClue(
                "Bạn chưa biết cần tìm gì ở mảnh gương."
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
            "MẢNH GƯƠNG VỠ\n\n" +
            "Trên mảnh gương có một dấu khắc:\n\n" +
            "7"
        );

        UpdateNumberMission();
    }


    // ======================================================
    // UPDATE 3 NUMBER CLUES
    // ======================================================

    private void UpdateNumberMission()
    {
        int count = 0;

        if (foundNumber4)
            count++;

        if (foundNumber1)
            count++;

        if (foundNumber7)
            count++;


        // ==================================================
        // ĐÃ TÌM ĐỦ 3 SỐ
        // ==================================================

        if (count >= 3)
        {
            SetMission(
                "Tìm chiếc hộp khóa",
                boxTarget,
                "Chiếc hộp nằm ở khu vực cuối chợ."
            );

            return;
        }


        // ==================================================
        // UPDATE UI
        // ==================================================

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


        // ==================================================
        // TARGET TIẾP THEO
        // ==================================================

        if (!foundNumber4)
        {
            SetNumberTarget(
                number4Target,
                "Tìm chiếc cân cũ."
            );
        }
        else if (!foundNumber1)
        {
            SetNumberTarget(
                number1Target,
                "Tìm chiếc đèn dầu cũ."
            );
        }
        else if (!foundNumber7)
        {
            SetNumberTarget(
                number7Target,
                "Tìm mảnh gương vỡ."
            );
        }
    }


    // ======================================================
    // SET NUMBER TARGET
    // ======================================================

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


    // ======================================================
    // OPEN BOX
    // ======================================================

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


    // ======================================================
    // CHECK CODE
    // ======================================================

    public void CheckCode(string code)
    {
        // MẬT KHẨU ĐÚNG
        if (code == "417")
        {
            boxOpened = true;

            wrongCodeAttempts = 0;

            if (codeInputUI != null)
            {
                codeInputUI.ClearCode();
            }

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
                "Chợ tàn sau lưng, nước tối bên mình,\n" +
                "Tìm nơi đất dứt, đường về sẽ hiện."
            );

            return;
        }


        // ==================================================
        // SAI CODE
        // ==================================================

        wrongCodeAttempts++;

        if (codeInputUI != null)
        {
            codeInputUI.ClearCode();
        }


        // ==================================================
        // CHƯA SAI ĐỦ 3 LẦN
        // ==================================================

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


        // ==================================================
        // SAI ĐỦ 3 LẦN
        // ==================================================

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


    // ======================================================
    // RESET 3 NUMBER CLUES
    // ======================================================

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
                "Tìm chiếc cân cũ."
            );
        }
    }


    // ======================================================
    // EXIT
    // ======================================================

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
                "NHIỆM VỤ:\n" +
                "MAP 1 COMPLETE";
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


    // ======================================================
    // SHOW CLUE
    // ======================================================

    public void ShowClue(string message)
    {
        if (cluePanel == null ||
            clueText == null)
        {
            return;
        }

        cluePanel.SetActive(true);

        clueText.text = message;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }


    // ======================================================
    // CLOSE CLUE
    // ======================================================

    public void CloseClue()
    {
        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }
}