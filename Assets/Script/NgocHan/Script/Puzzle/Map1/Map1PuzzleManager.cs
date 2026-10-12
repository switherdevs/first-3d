using UnityEngine;
using TMPro;

public class Map1PuzzleManager : MonoBehaviour
{
    public static Map1PuzzleManager Instance;


    // =========================================================
    // STEP
    // =========================================================

    public enum PuzzleStep
    {
        Paper,
        Photo,

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
    private PuzzleStep currentStep =
        PuzzleStep.Paper;


    // =========================================================
    // PLAYER
    // =========================================================

    [Header("PLAYER")]
    [SerializeField]
    private PlayerMap1Interaction playerInteraction;


    // =========================================================
    // UI
    // =========================================================

    [Header("MISSION UI")]
    [SerializeField]
    private TMP_Text objectiveText;

    [SerializeField]
    private TMP_Text missionText;


    [Header("CLUE PANEL")]
    [SerializeField]
    private GameObject cluePanel;

    [SerializeField]
    private TMP_Text clueText;


    [Header("CODE PANEL")]
    [SerializeField]
    private GameObject codePanel;

    [SerializeField]
    private TMP_InputField input1;

    [SerializeField]
    private TMP_InputField input2;

    [SerializeField]
    private TMP_InputField input3;

    [SerializeField]
    private GameObject enterButton;


    // =========================================================
    // TARGET
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


    private bool boxOpened;


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
        if (cluePanel != null)
            cluePanel.SetActive(false);


        if (codePanel != null)
            codePanel.SetActive(false);


        // Thư trong hộp OFF
        if (boxClueTarget != null)
            boxClueTarget.gameObject.SetActive(false);


        // Key trong hộp OFF
        if (keyTarget != null)
            keyTarget.gameObject.SetActive(false);


        currentStep =
            PuzzleStep.Paper;


        SetMission(
            "Tìm mảnh giấy"
        );


        SetTarget(
            paperTarget,
            "Có một mảnh giấy cũ ở đâu đó trong khu vực."
        );
    }


    // =========================================================
    // PAPER
    // =========================================================

    public bool CollectPaper()
    {
        if (currentStep != PuzzleStep.Paper)
            return false;


        ShowClue(
            "MẢNH GIẤY CŨ\n\n" +
            "Trên mảnh giấy có dòng chữ:\n\n" +
            "\"Hãy tìm bức ảnh của Huy...\""
        );


        currentStep =
            PuzzleStep.Photo;


        SetMission(
            "Tìm bức ảnh của Huy"
        );


        SetTarget(
            photoTarget,
            "Hãy tìm bức ảnh cũ của Huy."
        );


        return true;
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
            "Mặt sau bức ảnh có dấu hiệu dẫn tới chiếc cân cũ."
        );


        currentStep =
            PuzzleStep.Number4;


        SetMission(
            "Kiểm tra chiếc cân cũ"
        );


        SetTarget(
            number4Target,
            "Chiếc cân cũ có một dấu khắc bất thường."
        );


        return true;
    }


    // =========================================================
    // 4
    // =========================================================

    public bool CollectNumber4()
    {
        if (currentStep != PuzzleStep.Number4)
            return false;


        ShowClue(
            "MANH MỐI\n\n" +
            "Dưới chiếc cân có khắc số:\n\n4"
        );


        currentStep =
            PuzzleStep.Number1;


        SetMission(
            "Kiểm tra chiếc đèn dầu"
        );


        SetTarget(
            number1Target,
            "Chiếc đèn dầu có thể giấu một con số."
        );


        return true;
    }


    // =========================================================
    // 1
    // =========================================================

    public bool CollectNumber1()
    {
        if (currentStep != PuzzleStep.Number1)
            return false;


        ShowClue(
            "MANH MỐI\n\n" +
            "Phía sau chiếc đèn dầu có số:\n\n1"
        );


        currentStep =
            PuzzleStep.Number7;


        SetMission(
            "Kiểm tra mảnh gương"
        );


        SetTarget(
            number7Target,
            "Mảnh gương có dấu khắc kỳ lạ."
        );


        return true;
    }


    // =========================================================
    // 7
    // =========================================================

    public bool CollectNumber7()
    {
        if (currentStep != PuzzleStep.Number7)
            return false;


        ShowClue(
            "MANH MỐI\n\n" +
            "Trên mảnh gương có số:\n\n7\n\n" +
            "Mã tìm được: 4 - 1 - 7"
        );


        currentStep =
            PuzzleStep.Box;


        SetMission(
            "Quay lại chiếc hộp"
        );


        SetTarget(
            boxTarget,
            "Mã 417 có thể mở chiếc hộp."
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


    private void OpenCodePanel()
    {
        if (cluePanel != null)
            cluePanel.SetActive(false);


        if (codePanel != null)
            codePanel.SetActive(true);


        input1.text = "";
        input2.text = "";
        input3.text = "";


        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;


        input1.Select();
        input1.ActivateInputField();
    }


    // =========================================================
    // CODE
    // =========================================================

    public void CheckCode()
    {
        if (currentStep != PuzzleStep.Box)
            return;


        string code =
            input1.text.Trim() +
            input2.text.Trim() +
            input3.text.Trim();


        if (code == "417")
        {
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


            // Hiện key
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


            Debug.Log(
                "[MAP1] Target BoxClue = " +
                (
                    boxClueTarget != null
                    ? boxClueTarget.name
                    : "NULL"
                )
            );
        }
        else
        {
            input1.text = "";
            input2.text = "";
            input3.text = "";

            input1.Select();
            input1.ActivateInputField();
        }
    }


    // =========================================================
    // BOX CLUE
    // =========================================================

    public bool CollectBoxClue()
    {
        Debug.Log(
            "[MAP1] CollectBoxClue() được gọi"
        );


        if (currentStep != PuzzleStep.BoxClue)
        {
            Debug.Log(
                "[MAP1] Sai Step: " +
                currentStep
            );

            return false;
        }


        // =====================================================
        // HIỆN NỘI DUNG BỨC THƯ
        // =====================================================

        ShowClue(
            "BỨC THƯ CŨ\n\n" +
            "Nếu con tìm thấy bức thư này...\n\n" +
            "Hãy lấy chiếc chìa khóa trong hộp.\n\n" +
            "Đừng để nó rơi vào tay người khác."
        );


        currentStep =
            PuzzleStep.Key;


        SetMission(
            "Lấy chiếc chìa khóa trong hộp"
        );


        SetTarget(
            keyTarget,
            "Trong hộp còn một chiếc chìa khóa."
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
            "Trên chìa khóa có một ký hiệu kỳ lạ.\n\n" +
            "Ký hiệu này giống với ký hiệu trên một chiếc vòng tay."
        );


        currentStep =
            PuzzleStep.Bracelet;


        SetMission(
            "Tìm chiếc vòng tay"
        );


        SetTarget(
            braceletTarget,
            "Hãy tìm chiếc vòng tay có ký hiệu tương tự."
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
            "Ký hiệu trên chiếc vòng trùng với ký hiệu trên chìa khóa.\n\n" +
            "Đây là manh mối cuối cùng ở khu vực này."
        );


        currentStep =
            PuzzleStep.Complete;


        SetMission(
            "Hoàn thành điều tra khu vực"
        );


        if (playerInteraction != null)
        {
            playerInteraction
                .ClearTargetClue();
        }


        return true;
    }


    // =========================================================
    // SHOW CLUE
    // =========================================================

    public void ShowClue(string message)
    {
        if (codePanel != null)
            codePanel.SetActive(false);


        if (cluePanel != null)
            cluePanel.SetActive(true);


        if (clueText != null)
            clueText.text = message;


        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }


    // =========================================================
    // CLOSE CLUE
    // =========================================================

    public void CloseCluePanel()
    {
        if (cluePanel != null)
            cluePanel.SetActive(false);


        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }


    public void CloseCodePanel()
    {
        if (codePanel != null)
            codePanel.SetActive(false);


        Cursor.lockState =
            CursorLockMode.Locked;

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

            playerInteraction
                .ClearTargetClue();

            return;
        }


        Debug.Log(
            "[MAP1] Target = " +
            target.name
        );


        playerInteraction
            .SetTargetClue(
                target,
                hint
            );
    }


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


    public PuzzleStep GetCurrentStep()
    {
        return currentStep;
    }


    public bool IsBoxOpened()
    {
        return boxOpened;
    }
}