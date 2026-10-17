using System.Collections;
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

        Number4,
        Number1,
        Number7,

        Box,
        BoxClue,
        Key,
        Bracelet,

        Complete
    }


    // =========================================================
    // STATE
    // =========================================================

    [Header("PUZZLE STATE")]
    [SerializeField]
    private PuzzleStep currentStep =
        PuzzleStep.Photo;


    // =========================================================
    // ĐÚNG / SAI
    // =========================================================

    [Header("INTERACTION RESULT")]

    [SerializeField]
    private TMP_Text interactionResultText;

    [SerializeField]
    private float resultDisplayTime = 1f;

    [SerializeField]
    private Color correctColor = Color.green;

    [SerializeField]
    private Color wrongColor = Color.red;


    private Coroutine interactionResultCoroutine;


    // =========================================================
    // PLAYER
    // =========================================================

    [Header("PLAYER")]

    [SerializeField]
    private PlayerMap1Interaction playerInteraction;


    // =========================================================
    // MISSION
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
    // CODE
    // =========================================================

    [Header("CODE PANEL")]

    [SerializeField]
    private GameObject codePanel;

    [SerializeField]
    private string correctCode = "417";


    // =========================================================
    // RIDDLE
    // =========================================================

    [Header("RIDDLE PANEL")]

    [SerializeField]
    private GameObject riddlePanel;

    [SerializeField]
    private TMP_Text riddleTitle;

    [SerializeField]
    private TMP_Text riddleText;


    [Header("RIDDLE ANSWER SLOTS")]

    [SerializeField]
    private TMP_Text riddleSlot1;

    [SerializeField]
    private TMP_Text riddleSlot2;

    [SerializeField]
    private TMP_Text riddleSlot3;


    private bool riddleUnlocked = false;

    private int riddleIndex = 0;

    private int answer1 = -1;
    private int answer2 = -1;
    private int answer3 = -1;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // ĐÚNG / SAI
        if (interactionResultText != null)
        {
            interactionResultText.text = "";
            interactionResultText.gameObject.SetActive(false);
        }


        // CLUE
        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
        }


        // CODE
        if (codePanel != null)
        {
            codePanel.SetActive(false);
        }


        // RIDDLE
        if (riddlePanel != null)
        {
            riddlePanel.SetActive(false);
        }


        riddleUnlocked = false;


        ResetRiddle();


        UpdateMissionUI();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // TAB = xem lại câu đố
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleRiddlePanel();
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
    // PHOTO
    // =========================================================

    public bool CollectPhoto()
    {
        if (currentStep != PuzzleStep.Photo)
            return false;


        currentStep =
            PuzzleStep.Paper;


        ShowClue(
            "Bức ảnh cũ của Huy.\n" +
            "Có lẽ còn một mảnh giấy ở đâu đó."
        );


        UpdateMissionUI();


        return true;
    }


    // =========================================================
    // PAPER
    // =========================================================

    public bool CollectPaper()
    {
        if (currentStep != PuzzleStep.Paper)
            return false;


        currentStep =
            PuzzleStep.Number4;


        // Mở khóa câu đố
        riddleUnlocked = true;


        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
        }


        if (riddlePanel != null)
        {
            riddlePanel.SetActive(true);
        }


        UpdateMissionUI();


        return true;
    }


    // =========================================================
    // NUMBER 4
    // =========================================================

    public bool CollectNumber4()
    {
        if (currentStep != PuzzleStep.Number4)
            return false;


        currentStep =
            PuzzleStep.Number1;


        ShowClue(
            "Chiếc cân mang dấu số 4."
        );


        UpdateMissionUI();


        return true;
    }


    // =========================================================
    // NUMBER 1
    // =========================================================

    public bool CollectNumber1()
    {
        if (currentStep != PuzzleStep.Number1)
            return false;


        currentStep =
            PuzzleStep.Number7;


        ShowClue(
            "Chiếc đèn dầu mang dấu số 1."
        );


        UpdateMissionUI();


        return true;
    }


    // =========================================================
    // NUMBER 7
    // =========================================================

    public bool CollectNumber7()
    {
        if (currentStep != PuzzleStep.Number7)
            return false;


        currentStep =
            PuzzleStep.Box;


        ShowClue(
            "Mảnh gương mang dấu số 7.\n" +
            "Mật mã là 4 - 1 - 7."
        );


        UpdateMissionUI();


        return true;
    }


    // =========================================================
    // BOX
    // =========================================================

    public void TryOpenBox()
    {
        if (currentStep != PuzzleStep.Box)
        {
            ShowInteractionResult(false);
            return;
        }


        if (cluePanel != null)
            cluePanel.SetActive(false);


        if (riddlePanel != null)
            riddlePanel.SetActive(false);


        if (codePanel != null)
        {
            codePanel.SetActive(true);
        }
    }


    // =========================================================
    // CHECK CODE
    // =========================================================

    public bool CheckCode(string inputCode)
    {
        if (currentStep != PuzzleStep.Box)
        {
            ShowInteractionResult(false);
            return false;
        }


        if (inputCode != null)
        {
            inputCode = inputCode.Trim();
        }


        if (inputCode == correctCode)
        {
            currentStep =
                PuzzleStep.BoxClue;


            if (codePanel != null)
            {
                codePanel.SetActive(false);
            }


            ShowInteractionResult(true);


            ShowClue(
                "Mật mã 417 chính xác.\n" +
                "Chiếc hộp đã mở."
            );


            UpdateMissionUI();


            return true;
        }


        ShowInteractionResult(false);


        return false;
    }


    public bool CheckCode(int inputCode)
    {
        return CheckCode(
            inputCode.ToString()
        );
    }


    public void CloseCodePanel()
    {
        if (codePanel != null)
        {
            codePanel.SetActive(false);
        }
    }


    // =========================================================
    // BOX CLUE
    // =========================================================

    public bool CollectBoxClue()
    {
        if (currentStep != PuzzleStep.BoxClue)
            return false;


        currentStep =
            PuzzleStep.Key;


        ShowClue(
            "Manh mối trong hộp dẫn đến chiếc chìa khóa."
        );


        UpdateMissionUI();


        return true;
    }


    // =========================================================
    // KEY
    // =========================================================

    public bool CollectKey()
    {
        if (currentStep != PuzzleStep.Key)
            return false;


        currentStep =
            PuzzleStep.Bracelet;


        ShowClue(
            "Đã tìm được chìa khóa.\n" +
            "Hãy tìm chiếc vòng tay."
        );


        UpdateMissionUI();


        return true;
    }


    // =========================================================
    // BRACELET
    // =========================================================

    public bool CollectBracelet()
    {
        if (currentStep != PuzzleStep.Bracelet)
            return false;


        currentStep =
            PuzzleStep.Complete;


        ShowClue(
            "Đã tìm đủ các manh mối."
        );


        UpdateMissionUI();


        return true;
    }


    // =========================================================
    // SHOW ĐÚNG / SAI
    // =========================================================

    public void ShowInteractionResult(
        bool correct
    )
    {
        if (interactionResultText == null)
        {
            Debug.LogError(
                "[PUZZLE] InteractionResultText chưa gắn!"
            );

            return;
        }


        if (interactionResultCoroutine != null)
        {
            StopCoroutine(
                interactionResultCoroutine
            );
        }


        interactionResultCoroutine =
            StartCoroutine(
                ShowInteractionResultRoutine(
                    correct
                )
            );
    }


    private IEnumerator ShowInteractionResultRoutine(
        bool correct
    )
    {
        interactionResultText.gameObject
            .SetActive(true);


        if (correct)
        {
            interactionResultText.text =
                "ĐÚNG";

            interactionResultText.color =
                correctColor;
        }
        else
        {
            interactionResultText.text =
                "SAI";

            interactionResultText.color =
                wrongColor;
        }


        yield return new WaitForSeconds(
            resultDisplayTime
        );


        interactionResultText.text = "";

        interactionResultText.gameObject
            .SetActive(false);


        interactionResultCoroutine = null;
    }


    // =========================================================
    // CLUE
    // =========================================================

    private void ShowClue(
        string message
    )
    {
        if (riddlePanel != null)
        {
            riddlePanel.SetActive(false);
        }


        if (codePanel != null)
        {
            codePanel.SetActive(false);
        }


        if (clueText != null)
        {
            clueText.text = message;
        }


        if (cluePanel != null)
        {
            cluePanel.SetActive(true);
        }
    }


    public void CloseCluePanel()
    {
        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
        }
    }


    // =========================================================
    // RIDDLE TAB
    // =========================================================

    public void ToggleRiddlePanel()
    {
        if (!riddleUnlocked)
            return;


        if (riddlePanel == null)
            return;


        if (riddlePanel.activeSelf)
        {
            riddlePanel.SetActive(false);
            return;
        }


        if (codePanel != null &&
            codePanel.activeSelf)
        {
            return;
        }


        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
        }


        riddlePanel.SetActive(true);
    }


    public void CloseRiddlePanel()
    {
        if (riddlePanel != null)
        {
            riddlePanel.SetActive(false);
        }
    }


    // =========================================================
    // RIDDLE BUTTONS
    // =========================================================

    public void SelectCan()
    {
        SelectRiddleAnswer(
            0,
            "CÂN"
        );
    }


    public void SelectDen()
    {
        SelectRiddleAnswer(
            1,
            "ĐÈN DẦU"
        );
    }


    public void SelectGuong()
    {
        SelectRiddleAnswer(
            2,
            "GƯƠNG"
        );
    }


    public void SelectRadio()
    {
        SelectRiddleAnswer(
            3,
            "RADIO"
        );
    }


    public void SelectKeyAnswer()
    {
        SelectRiddleAnswer(
            4,
            "CHÌA KHÓA"
        );
    }


    public void SelectBraceletAnswer()
    {
        SelectRiddleAnswer(
            5,
            "VÒNG TAY"
        );
    }


    // =========================================================
    // SELECT RIDDLE ANSWER
    // =========================================================

    private void SelectRiddleAnswer(
        int value,
        string label
    )
    {
        if (riddleIndex >= 3)
            return;


        if (riddleIndex == 0)
        {
            answer1 = value;

            if (riddleSlot1 != null)
            {
                riddleSlot1.text =
                    label;
            }
        }


        else if (riddleIndex == 1)
        {
            answer2 = value;

            if (riddleSlot2 != null)
            {
                riddleSlot2.text =
                    label;
            }
        }


        else if (riddleIndex == 2)
        {
            answer3 = value;

            if (riddleSlot3 != null)
            {
                riddleSlot3.text =
                    label;
            }
        }


        riddleIndex++;
    }


    // =========================================================
    // RESET RIDDLE
    // =========================================================

    public void ResetRiddle()
    {
        answer1 = -1;
        answer2 = -1;
        answer3 = -1;

        riddleIndex = 0;


        if (riddleSlot1 != null)
            riddleSlot1.text = "[ ? ]";


        if (riddleSlot2 != null)
            riddleSlot2.text = "[ ? ]";


        if (riddleSlot3 != null)
            riddleSlot3.text = "[ ? ]";
    }


    // =========================================================
    // CHECK RIDDLE
    // =========================================================

    public void CheckRiddle()
    {
        if (riddleIndex < 3)
        {
            ShowInteractionResult(false);
            return;
        }


        // ĐÁP ÁN:
        // CÂN -> ĐÈN DẦU -> GƯƠNG

        bool correct =
            answer1 == 0 &&
            answer2 == 1 &&
            answer3 == 2;


        if (correct)
        {
            ShowInteractionResult(true);


            if (riddlePanel != null)
            {
                riddlePanel.SetActive(false);
            }


            return;
        }


        ShowInteractionResult(false);


        ResetRiddle();
    }


    // =========================================================
    // MISSION
    // =========================================================

    private void UpdateMissionUI()
    {
        string objective = "";


        switch (currentStep)
        {
            case PuzzleStep.Photo:

                objective =
                    "Tìm bức ảnh của Huy";

                break;


            case PuzzleStep.Paper:

                objective =
                    "Tìm mảnh giấy";

                break;


            case PuzzleStep.Number4:

                objective =
                    "Tìm chiếc cân";

                break;


            case PuzzleStep.Number1:

                objective =
                    "Tìm chiếc đèn dầu";

                break;


            case PuzzleStep.Number7:

                objective =
                    "Tìm mảnh gương vỡ";

                break;


            case PuzzleStep.Box:

                objective =
                    "Tìm chiếc hộp và nhập mật mã";

                break;


            case PuzzleStep.BoxClue:

                objective =
                    "Kiểm tra manh mối trong hộp";

                break;


            case PuzzleStep.Key:

                objective =
                    "Tìm chiếc chìa khóa";

                break;


            case PuzzleStep.Bracelet:

                objective =
                    "Tìm chiếc vòng tay";

                break;


            case PuzzleStep.Complete:

                objective =
                    "Hoàn thành";

                break;
        }


        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: " +
                objective;
        }


        if (missionText != null)
        {
            missionText.text =
                "NHIỆM VỤ:\n" +
                objective;
        }
    }
}