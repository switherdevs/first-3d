using UnityEngine;
using TMPro;

public class Map1PuzzleManager : MonoBehaviour
{
    public static Map1PuzzleManager Instance;

    // =========================================================
    // PUZZLE STEP
    // =========================================================

    public enum PuzzleStep
    {
        Photo,
        Paper,
        Riddle,

        Number4,
        Number1,
        Number7,

        Box,
        BoxClue,
        Key,
        Bracelet,

        Complete
    }

    [Header("PUZZLE STATE")]
    [SerializeField]
    private PuzzleStep currentStep = PuzzleStep.Photo;

    // =========================================================
    // PLAYER
    // =========================================================

    [Header("PLAYER")]
    [SerializeField]
    private PlayerMap1Interaction playerInteraction;

    // =========================================================
    // MISSION UI
    // =========================================================

    [Header("MISSION UI")]
    [SerializeField]
    private TMP_Text objectiveText;

    [SerializeField]
    private TMP_Text missionText;

    // =========================================================
    // CLUE PANEL
    // =========================================================

    [Header("CLUE PANEL")]
    [SerializeField]
    private GameObject cluePanel;

    [SerializeField]
    private TMP_Text clueText;

    // =========================================================
    // RIDDLE PANEL
    // =========================================================

    [Header("RIDDLE PANEL")]
    [SerializeField]
    private GameObject riddlePanel;

    [SerializeField]
    private TMP_Text riddleTitle;

    [SerializeField]
    private TMP_Text riddleText;

    [SerializeField]
    private TMP_Text riddleSlot1;

    [SerializeField]
    private TMP_Text riddleSlot2;

    [SerializeField]
    private TMP_Text riddleSlot3;

    [SerializeField]
    private TMP_Text riddleResultText;

    // =========================================================
    // RIDDLE BUTTONS
    // =========================================================

    [Header("RIDDLE BUTTONS")]
    [SerializeField]
    private GameObject canButton;

    [SerializeField]
    private GameObject denButton;

    [SerializeField]
    private GameObject guongButton;

    [SerializeField]
    private GameObject radioButton;

    [SerializeField]
    private GameObject keyAnswerButton;

    [SerializeField]
    private GameObject braceletButton;

    [SerializeField]
    private GameObject resetButton;

    [SerializeField]
    private GameObject confirmButton;

    // =========================================================
    // RIDDLE INTERACT OBJECTS
    // =========================================================

    [Header("RIDDLE INTERACT OBJECTS")]
    [SerializeField]
    private GameObject canInteract;

    [SerializeField]
    private GameObject denInteract;

    [SerializeField]
    private GameObject guongInteract;

    // =========================================================
    // NUMBER MODELS
    // =========================================================

    [Header("NUMBER MODELS")]
    [SerializeField]
    private GameObject number4Model;

    [SerializeField]
    private GameObject number1Model;

    [SerializeField]
    private GameObject number7Model;

    // =========================================================
    // RIDDLE DATA
    // =========================================================

    private int riddleIndex = 0;

    private int answer1 = -1;
    private int answer2 = -1;
    private int answer3 = -1;

    private bool riddleSolved = false;
    private bool hintPanelOpen = false;

    // Lưu nội dung câu đố gốc trong Inspector
    private string originalRiddleText = "";

    /*
        0 = Cân
        1 = Đèn dầu
        2 = Gương

        3 = Radio
        4 = Chìa khóa
        5 = Vòng tay

        Đáp án:
        CÂN -> ĐÈN DẦU -> GƯƠNG
    */

    // =========================================================
    // CODE PANEL
    // =========================================================

    [Header("CODE PANEL")]
    [SerializeField]
    private GameObject codePanel;

    [Header("CODE DIGITS")]
    [SerializeField]
    private TMP_Text digit1Text;

    [SerializeField]
    private TMP_Text digit2Text;

    [SerializeField]
    private TMP_Text digit3Text;

    [SerializeField]
    private GameObject enterButton;

    private int digit1 = 0;
    private int digit2 = 0;
    private int digit3 = 0;

    // =========================================================
    // TARGETS
    // =========================================================

    [Header("TARGETS")]

    [SerializeField]
    private Transform paperTarget;

    [SerializeField]
    private Transform photoTarget;

    [SerializeField]
    private Transform number4Target;

    [SerializeField]
    private Transform number1Target;

    [SerializeField]
    private Transform number7Target;

    [SerializeField]
    private Transform boxTarget;

    [SerializeField]
    private Transform boxClueTarget;

    [SerializeField]
    private Transform keyTarget;

    [SerializeField]
    private Transform braceletTarget;

    private bool boxOpened = false;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        Instance = this;
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Lưu lại nội dung câu đố mà Owner đã căn sẵn
        if (riddleText != null)
            originalRiddleText = riddleText.text;

        if (cluePanel != null)
            cluePanel.SetActive(false);

        if (riddlePanel != null)
            riddlePanel.SetActive(false);

        if (codePanel != null)
            codePanel.SetActive(false);

        // Chưa giải câu đố
        if (canInteract != null)
            canInteract.SetActive(false);

        if (denInteract != null)
            denInteract.SetActive(false);

        if (guongInteract != null)
            guongInteract.SetActive(false);

        // Ẩn số
        if (number4Model != null)
            number4Model.SetActive(false);

        if (number1Model != null)
            number1Model.SetActive(false);

        if (number7Model != null)
            number7Model.SetActive(false);

        // Đồ trong hộp chưa hiện
        if (boxClueTarget != null)
            boxClueTarget.gameObject.SetActive(false);

        if (keyTarget != null)
            keyTarget.gameObject.SetActive(false);

        // Bắt đầu bằng ảnh Huy
        currentStep = PuzzleStep.Photo;

        SetMission("Tìm bức ảnh của Huy");

        SetTarget(
            photoTarget,
            "Có một bức ảnh cũ của Huy ở đâu đó."
        );
    }

    // =========================================================
    // UPDATE - TAB MANH MỐI
    // =========================================================

    private void Update()
    {
        if (!riddleSolved)
            return;

        if (!Input.GetKeyDown(KeyCode.Tab))
            return;

        // Không mở khi đang đọc clue
        if (cluePanel != null && cluePanel.activeSelf)
            return;

        // Không mở khi đang nhập mật mã
        if (codePanel != null && codePanel.activeSelf)
            return;

        if (hintPanelOpen)
        {
            CloseHintPanel();
        }
        else
        {
            ShowHintPanel();
        }
    }

    // =========================================================
    // PHOTO
    // =========================================================

    public bool CollectPhoto()
    {
        if (currentStep != PuzzleStep.Photo)
            return false;

        ShowClue(
            "BỨC ẢNH CŨ\n\n" +
            "\"Nhà cũ im rồi, lời chưa ngủ,\n" +
            "Dòng mực năm xưa vẫn gọi con về.\""
        );

        currentStep = PuzzleStep.Paper;

        SetMission("Tìm bức thư ba mẹ để lại");

        SetTarget(
            paperTarget,
            "Dòng mực năm xưa vẫn đang chờ được tìm thấy."
        );

        return true;
    }

    // =========================================================
    // PAPER / LETTER
    // =========================================================

    public bool CollectPaper()
    {
        if (currentStep != PuzzleStep.Paper)
            return false;

        ShowClue(
            "BỨC THƯ CỦA BA MẸ\n\n" +
            "Chìa khóa mở lối tới nơi chưa ngủ yên.\n" +
            "Nếu nghe ai gọi phía sau... đừng quay đầu.!!!\n\n"
        );

        currentStep = PuzzleStep.Riddle;

        SetMission("Đọc bức thư");

        if (playerInteraction != null)
            playerInteraction.ClearTargetClue();

        return true;
    }

    // =========================================================
    // SHOW RIDDLE
    // =========================================================

    private void ShowRiddle()
    {
        hintPanelOpen = false;

        if (cluePanel != null)
            cluePanel.SetActive(false);

        if (codePanel != null)
            codePanel.SetActive(false);

        if (riddlePanel != null)
            riddlePanel.SetActive(true);

        if (riddleTitle != null)
            riddleTitle.text = "GIẢI CÂU ĐỐ";

        /*
         * QUAN TRỌNG:
         * Không ghi đè nội dung RiddleText nữa.
         *
         * Owner căn chữ trong Inspector thế nào
         * thì lúc Play giữ nguyên như vậy.
         */

        if (riddleText != null)
            riddleText.text = originalRiddleText;

        SetRiddleButtonsVisible(true);

        ResetRiddle();

        SetMission("Giải câu đố");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =========================================================
    // RIDDLE BUTTON VISIBILITY
    // =========================================================

    private void SetRiddleButtonsVisible(bool visible)
    {
        if (canButton != null)
            canButton.SetActive(visible);

        if (denButton != null)
            denButton.SetActive(visible);

        if (guongButton != null)
            guongButton.SetActive(visible);

        if (radioButton != null)
            radioButton.SetActive(visible);

        if (keyAnswerButton != null)
            keyAnswerButton.SetActive(visible);

        if (braceletButton != null)
            braceletButton.SetActive(visible);

        if (resetButton != null)
            resetButton.SetActive(visible);

        if (confirmButton != null)
            confirmButton.SetActive(visible);
    }

    // =========================================================
    // RIDDLE ANSWERS
    // =========================================================

    public void SelectCan()
    {
        if (currentStep != PuzzleStep.Riddle)
            return;

        SelectRiddleAnswer(0, "CÂN");
    }

    public void SelectDen()
    {
        if (currentStep != PuzzleStep.Riddle)
            return;

        SelectRiddleAnswer(1, "ĐÈN DẦU");
    }

    public void SelectGuong()
    {
        if (currentStep != PuzzleStep.Riddle)
            return;

        SelectRiddleAnswer(2, "GƯƠNG");
    }

    public void SelectRadio()
    {
        if (currentStep != PuzzleStep.Riddle)
            return;

        SelectRiddleAnswer(3, "RADIO");
    }

    public void SelectKeyAnswer()
    {
        if (currentStep != PuzzleStep.Riddle)
            return;

        SelectRiddleAnswer(4, "CHÌA KHÓA");
    }

    public void SelectBraceletAnswer()
    {
        if (currentStep != PuzzleStep.Riddle)
            return;

        SelectRiddleAnswer(5, "VÒNG TAY");
    }

    // =========================================================
    // SELECT RIDDLE ANSWER
    // =========================================================

    private void SelectRiddleAnswer(
        int value,
        string displayText
    )
    {
        if (riddleIndex >= 3)
            return;

        if (riddleResultText != null)
            riddleResultText.text = "";

        if (riddleIndex == 0)
        {
            answer1 = value;

            if (riddleSlot1 != null)
                riddleSlot1.text = displayText;
        }
        else if (riddleIndex == 1)
        {
            answer2 = value;

            if (riddleSlot2 != null)
                riddleSlot2.text = displayText;
        }
        else if (riddleIndex == 2)
        {
            answer3 = value;

            if (riddleSlot3 != null)
                riddleSlot3.text = displayText;
        }

        riddleIndex++;
    }

    // =========================================================
    // RESET RIDDLE
    // =========================================================

    public void ResetRiddle()
    {
        riddleIndex = 0;

        answer1 = -1;
        answer2 = -1;
        answer3 = -1;

        if (riddleSlot1 != null)
            riddleSlot1.text = "?";

        if (riddleSlot2 != null)
            riddleSlot2.text = "?";

        if (riddleSlot3 != null)
            riddleSlot3.text = "?";

        if (riddleResultText != null)
            riddleResultText.text = "";
    }

    // =========================================================
    // CHECK RIDDLE
    // =========================================================

    public void CheckRiddle()
    {
        if (currentStep != PuzzleStep.Riddle)
            return;

        if (riddleIndex < 3)
        {
            if (riddleResultText != null)
                riddleResultText.text =
                    "Hãy chọn đủ 3 vật.";

            return;
        }

        // CÂN -> ĐÈN DẦU -> GƯƠNG
        if (answer1 == 0 &&
            answer2 == 1 &&
            answer3 == 2)
        {
            RiddleCorrect();
        }
        else
        {
            RiddleWrong();
        }
    }

    // =========================================================
    // WRONG RIDDLE
    // =========================================================

    private void RiddleWrong()
    {
        riddleIndex = 0;

        answer1 = -1;
        answer2 = -1;
        answer3 = -1;

        if (riddleSlot1 != null)
            riddleSlot1.text = "?";

        if (riddleSlot2 != null)
            riddleSlot2.text = "?";

        if (riddleSlot3 != null)
            riddleSlot3.text = "?";

        if (riddleResultText != null)
            riddleResultText.text =
                "Thứ tự chưa đúng...";
    }

    // =========================================================
    // CORRECT RIDDLE
    // =========================================================

    private void RiddleCorrect()
    {
        riddleSolved = true;
        hintPanelOpen = false;

        if (riddlePanel != null)
            riddlePanel.SetActive(false);

        // Chỉ bật Cân trước
        if (canInteract != null)
            canInteract.SetActive(true);

        if (denInteract != null)
            denInteract.SetActive(false);

        if (guongInteract != null)
            guongInteract.SetActive(false);

        currentStep = PuzzleStep.Number4;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetMission("Kiểm tra chiếc cân cũ");

        SetTarget(
            number4Target,
            "Kẻ thứ nhất đo nặng nhẹ linh hồn, nhưng chẳng có tay."
        );
    }

    // =========================================================
    // TAB - SHOW MANH MỐI
    // =========================================================

    private void ShowHintPanel()
    {
        hintPanelOpen = true;

        if (riddlePanel != null)
            riddlePanel.SetActive(true);

        if (riddleTitle != null)
            riddleTitle.text = "MANH MỐI";

        /*
         * TAB mở lại RiddlePanel
         * nhưng lúc này nội dung đổi thành Manh Mối.
         */

        if (riddleText != null)
        {
            riddleText.text =
                "Ba vật được nhắc tới theo thứ tự:\n\n" +
                "Kẻ đo nặng nhẹ linh hồn.\n\n" +
                "Kẻ soi đường cho những thứ không còn sống.\n\n" +
                "Kẻ trả lại khuôn mặt của người nhìn.";
        }

        if (riddleSlot1 != null)
            riddleSlot1.text = "CÂN";

        if (riddleSlot2 != null)
            riddleSlot2.text = "ĐÈN DẦU";

        if (riddleSlot3 != null)
            riddleSlot3.text = "GƯƠNG";

        if (riddleResultText != null)
            riddleResultText.text =
                "Nhấn TAB để đóng";

        // Không cho giải lại
        SetRiddleButtonsVisible(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =========================================================
    // TAB - CLOSE MANH MỐI
    // =========================================================

    private void CloseHintPanel()
    {
        hintPanelOpen = false;

        if (riddlePanel != null)
            riddlePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // =========================================================
    // CÂN -> 4
    // =========================================================

    public bool CollectNumber4()
    {
        if (currentStep != PuzzleStep.Number4)
            return false;

        if (number4Model != null)
            number4Model.SetActive(true);

        if (canInteract != null)
            canInteract.SetActive(false);

        if (denInteract != null)
            denInteract.SetActive(true);

        ShowClue(
            "CHIẾC CÂN CŨ\n\n" +
            "Dưới đáy cân là những vết cào khô quắt như móng người.\n" +
            "Giữa những vết ấy hiện ra con số:\n\n4"
        );

        currentStep = PuzzleStep.Number1;

        SetMission("Kiểm tra chiếc đèn dầu");

        SetTarget(
            number1Target,
            "Kẻ thứ hai soi đường cho những thứ không còn sống, \n\n\" +" +
            "nhưng chẳng phải mặt trời. \n\n\\"
        );

        return true;
    }

    // =========================================================
    // ĐÈN -> 1
    // =========================================================

    public bool CollectNumber1()
    {
        if (currentStep != PuzzleStep.Number1)
            return false;

        if (number1Model != null)
            number1Model.SetActive(true);

        if (denInteract != null)
            denInteract.SetActive(false);

        if (guongInteract != null)
            guongInteract.SetActive(true);

        ShowClue(
            "CHIẾC ĐÈN DẦU\n\n" +
            "Sau lớp muội đen là những vết cào cháy sém.\n\n" +
            "Giữa lớp tro hiện ra con số:\n\n1"
        );

        currentStep = PuzzleStep.Number7;

        SetMission("Kiểm tra mảnh gương");

        SetTarget(
            number7Target,
            "Kẻ cuối trả lại khuôn mặt của con, \n" +
            "nhưng không phải lúc nào cũng là mặt con."
        );

        return true;
    }

    // =========================================================
    // GƯƠNG -> 7
    // =========================================================

    public bool CollectNumber7()
    {
        if (currentStep != PuzzleStep.Number7)
            return false;

        if (number7Model != null)
            number7Model.SetActive(true);

        if (guongInteract != null)
            guongInteract.SetActive(false);

        ShowClue(
            "MẢNH GƯƠNG VỠ\n\n" +
            "Mặt sau mảnh gương lạnh ngắt, \n\n" +
            "như vừa có ai áp mặt vào từ phía bên kia.\n\n" +
            "Giữa những vết nứt hiện ra con số:\n\n7\n\n" +
            "Ba con số ghép lại thành:\n" +
            "4 - 1 - 7"
        );

        currentStep = PuzzleStep.Box;

        SetMission("Quay lại chiếc hộp");

        SetTarget(
            boxTarget,
            "Ba con số đã đủ. Thứ trong chiếc hộp dường như đã biết con đang tới."
        );

        return true;
    }

    // =========================================================
    // BOX
    // =========================================================

    public void TryOpenBox()
    {
        if (currentStep != PuzzleStep.Box)
            return;

        OpenCodePanel();
    }

    // =========================================================
    // OPEN CODE PANEL
    // =========================================================

    private void OpenCodePanel()
    {
        if (cluePanel != null)
            cluePanel.SetActive(false);

        if (riddlePanel != null)
            riddlePanel.SetActive(false);

        hintPanelOpen = false;

        if (codePanel != null)
            codePanel.SetActive(true);

        digit1 = 0;
        digit2 = 0;
        digit3 = 0;

        UpdateCodeDisplay();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =========================================================
    // UPDATE CODE DISPLAY
    // =========================================================

    private void UpdateCodeDisplay()
    {
        if (digit1Text != null)
            digit1Text.text = digit1.ToString();

        if (digit2Text != null)
            digit2Text.text = digit2.ToString();

        if (digit3Text != null)
            digit3Text.text = digit3.ToString();
    }

    // =========================================================
    // DIGIT 1
    // =========================================================

    public void Up1()
    {
        digit1++;

        if (digit1 > 9)
            digit1 = 0;

        UpdateCodeDisplay();
    }

    public void Down1()
    {
        digit1--;

        if (digit1 < 0)
            digit1 = 9;

        UpdateCodeDisplay();
    }

    // =========================================================
    // DIGIT 2
    // =========================================================

    public void Up2()
    {
        digit2++;

        if (digit2 > 9)
            digit2 = 0;

        UpdateCodeDisplay();
    }

    public void Down2()
    {
        digit2--;

        if (digit2 < 0)
            digit2 = 9;

        UpdateCodeDisplay();
    }

    // =========================================================
    // DIGIT 3
    // =========================================================

    public void Up3()
    {
        digit3++;

        if (digit3 > 9)
            digit3 = 0;

        UpdateCodeDisplay();
    }

    public void Down3()
    {
        digit3--;

        if (digit3 < 0)
            digit3 = 9;

        UpdateCodeDisplay();
    }

    // =========================================================
    // CHECK CODE
    // =========================================================

    public void CheckCode()
    {
        if (digit1Text == null ||
            digit2Text == null ||
            digit3Text == null)
        {
            Debug.LogError(
                "[MAP1] Digit Text chưa được gắn!"
            );

            return;
        }

        // Đọc trực tiếp số đang HIỂN THỊ
        if (!int.TryParse(
                digit1Text.text.Trim(),
                out int d1
            ) ||
            !int.TryParse(
                digit2Text.text.Trim(),
                out int d2
            ) ||
            !int.TryParse(
                digit3Text.text.Trim(),
                out int d3
            ))
        {
            Debug.LogError(
                "[MAP1] Không đọc được mật mã!"
            );

            return;
        }

        Debug.Log(
            "[MAP1] Mật mã đang nhập: " +
            d1 + d2 + d3
        );

        if (currentStep != PuzzleStep.Box)
        {
            Debug.LogError(
                "[MAP1] Sai Step: " +
                currentStep
            );

            return;
        }

        // 417
        if (d1 == 4 &&
            d2 == 1 &&
            d3 == 7)
        {
            Debug.Log(
                "[MAP1] MẬT MÃ ĐÚNG: 417"
            );

            boxOpened = true;

            if (codePanel != null)
                codePanel.SetActive(false);

            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;

            // Hiện thư
            if (boxClueTarget != null)
            {
                boxClueTarget.gameObject
                    .SetActive(true);
            }

            // Hiện chìa khóa
            if (keyTarget != null)
            {
                keyTarget.gameObject
                    .SetActive(true);
            }

            currentStep =
                PuzzleStep.BoxClue;

            SetMission(
                "Đọc bức thư trong hộp"
            );

            SetTarget(
                boxClueTarget,
                "Có một bức thư nằm bên trong chiếc hộp."
            );
        }
        else
        {
            Debug.LogError(
                "[MAP1] SAI MẬT MÃ: " +
                d1 + d2 + d3
            );
        }
    }

    // =========================================================
    // BOX CLUE
    // =========================================================

    public bool CollectBoxClue()
    {
        if (currentStep != PuzzleStep.BoxClue)
            return false;

        ShowClue(
            "BỨC THƯ TRONG HỘP\n\n" +
            "Dưới lá thư là một chiếc chìa khóa lạnh buốt như vừa được moi lên từ mộ.\n\n" +
            "Trên thân khóa còn dính những vệt đen khô quắt, như máu đã cũ từ rất lâu."
        );

        currentStep = PuzzleStep.Key;

        SetMission(
            "Lấy chiếc chìa khóa trong hộp"
        );

        SetTarget(
            keyTarget,
            "Chiếc chìa khóa nằm trong hộp... như đang chờ một bàn tay sống chạm vào."
        );

        return true;
    }

    // =========================================================
    // KEY
    // =========================================================

    public bool CollectKey()
    {
        if (currentStep != PuzzleStep.Key)
            return false;

        ShowClue(
            "CHIẾC CHÌA KHÓA\n\n" +
            "Trên thân khóa có một ký hiệu méo mó như bị khắc bằng móng tay.\n\n" +
            "Ký hiệu ấy trùng với dấu trên một chiếc vòng tay cũ."
        );

        currentStep = PuzzleStep.Bracelet;

        SetMission("Tìm chiếc vòng tay");

        SetTarget(
            braceletTarget,
            "Chiếc vòng mang cùng ký hiệu... như thể nó từng thuộc về thứ không còn sống."
        );

        return true;
    }

    // =========================================================
    // BRACELET
    // =========================================================

    public bool CollectBracelet()
    {
        if (currentStep != PuzzleStep.Bracelet)
            return false;

        ShowClue(
            "CHIẾC VÒNG TAY\n\n" +
            "Ký hiệu trên chiếc vòng trùng khớp với ký hiệu trên chìa khóa.\n\n" +
            "Mặt trong vòng có một dòng chữ bị cào bằng móng tay:\n" +
            "\"Nếu con nhớ ra nó là ai... đừng để nó biết.\""
        );

        currentStep = PuzzleStep.Complete;

        SetMission("Rời khỏi khu vực");

        if (playerInteraction != null)
            playerInteraction.ClearTargetClue();

        return true;
    }

    // =========================================================
    // SHOW CLUE
    // =========================================================

    public void ShowClue(string message)
    {
        hintPanelOpen = false;

        if (codePanel != null)
            codePanel.SetActive(false);

        if (riddlePanel != null)
            riddlePanel.SetActive(false);

        if (cluePanel != null)
            cluePanel.SetActive(true);

        if (clueText != null)
            clueText.text = message;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =========================================================
    // CLOSE CLUE
    // =========================================================

    public void CloseCluePanel()
    {
        if (cluePanel != null)
            cluePanel.SetActive(false);

        // Đọc xong thư -> mở puzzle
        if (currentStep == PuzzleStep.Riddle &&
            !riddleSolved)
        {
            ShowRiddle();
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // =========================================================
    // CLOSE CODE
    // =========================================================

    public void CloseCodePanel()
    {
        if (codePanel != null)
            codePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // =========================================================
    // TARGET
    // =========================================================

    private void SetTarget(
        Transform target,
        string hint
    )
    {
        if (playerInteraction == null)
        {
            Debug.LogError(
                "[MAP1] PlayerInteraction = NULL"
            );

            return;
        }

        if (target == null)
        {
            Debug.LogError(
                "[MAP1] Target = NULL"
            );

            playerInteraction.ClearTargetClue();

            return;
        }

        playerInteraction.SetTargetClue(
            target,
            hint
        );
    }

    // =========================================================
    // MISSION
    // =========================================================

    private void SetMission(string text)
    {
        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: " + text;
        }

        if (missionText != null)
        {
            missionText.text =
                "NHIỆM VỤ:\n" + text;
        }
    }

    // =========================================================
    // GET STEP
    // =========================================================

    public PuzzleStep GetCurrentStep()
    {
        return currentStep;
    }

    // =========================================================
    // BOX OPENED
    // =========================================================

    public bool IsBoxOpened()
    {
        return boxOpened;
    }
}