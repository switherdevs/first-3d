using System.Collections;
using UnityEngine;
using TMPro;

public class Map1PuzzleManager : MonoBehaviour
{
    public static Map1PuzzleManager Instance;

    // ==================================================
    // UI
    // ==================================================

    [Header("UI")]
    public GameObject cluePanel;
    public TMP_Text clueText;
    public TMP_Text objectiveText;
    public TMP_Text missionText;

    [Header("Code Panel")]
    public GameObject codePanel;
    public CodeInputUI codeInputUI;

    // ==================================================
    // PUZZLE NUMBERS
    // ==================================================

    [Header("Puzzle Numbers")]
    public GameObject number4Text;
    public GameObject number1Text;
    public GameObject number7Text;

    // ==================================================
    // CLUE TARGETS
    // ==================================================

    [Header("Clue Targets")]

    // Ảnh Huy đeo dây chuyền
    public Transform huyPhotoTarget;

    // Bức thư manh mối
    public Transform letterTarget;

    // 3 vật phẩm chứa số
    public Transform number4Target;
    public Transform number1Target;
    public Transform number7Target;

    // Hộp gỗ
    public Transform boxTarget;

    // Chìa khóa trong hộp
    public Transform keyTarget;

    // Manh mối trong hộp
    public Transform boxClueTarget;

    // Lối ra
    public Transform exitTarget;

    // ==================================================
    // PLAYER
    // ==================================================

    [Header("Player Interaction")]
    public PlayerMap1Interaction playerInteraction;

    // ==================================================
    // WOODEN BOX
    // ==================================================

    [Header("Wooden Box")]

    // Pivot của nắp hộp
    public Transform lidPivot;

    // Chìa khóa nằm trong hộp
    public GameObject keyItem;

    // Manh mối nằm trong hộp
    public GameObject boxClueItem;

    [Header("Box Opening")]
    public float openAngle = -100f;
    public float openDuration = 0.8f;

    // ==================================================
    // CODE SETTINGS
    // ==================================================

    [Header("Code Settings")]
    public string correctCode = "417";
    public int maxWrongAttempts = 3;

    // ==================================================
    // STATE
    // ==================================================

    private bool foundHuyPhoto = false;
    private bool foundLetter = false;

    private bool foundNumber4 = false;
    private bool foundNumber1 = false;
    private bool foundNumber7 = false;

    private bool boxOpened = false;
    private bool boxOpening = false;

    private bool foundKey = false;
    private bool foundBoxClue = false;

    private int wrongCodeAttempts = 0;

    private Quaternion closedLidRotation;
    private Quaternion openedLidRotation;

    // ==================================================
    // UNITY
    // ==================================================

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // ------------------------------
        // Tắt UI
        // ------------------------------

        if (cluePanel != null)
            cluePanel.SetActive(false);

        if (codePanel != null)
            codePanel.SetActive(false);

        // ------------------------------
        // Ẩn số
        // ------------------------------

        if (number4Text != null)
            number4Text.SetActive(false);

        if (number1Text != null)
            number1Text.SetActive(false);

        if (number7Text != null)
            number7Text.SetActive(false);

        // ------------------------------
        // Ẩn vật phẩm trong hộp
        // ------------------------------

        if (keyItem != null)
            keyItem.SetActive(false);

        if (boxClueItem != null)
            boxClueItem.SetActive(false);

        // ------------------------------
        // Lưu góc đóng/mở của nắp
        // ------------------------------

        if (lidPivot != null)
        {
            closedLidRotation = lidPivot.localRotation;

            openedLidRotation =
                closedLidRotation *
                Quaternion.Euler(openAngle, 0f, 0f);
        }

        // ------------------------------
        // Nhiệm vụ đầu tiên
        // ------------------------------

        SetMission(
            "Tìm bức ảnh của Huy",
            huyPhotoTarget,
            "Một bức ảnh cũ có thể còn sót lại trong khu vực"
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
    // ẢNH HUY ĐEO DÂY CHUYỀN
    // ==================================================

    public void FindHuyPhoto()
    {
        if (foundHuyPhoto)
        {
            ShowClue(
                "Bạn đã kiểm tra bức ảnh này rồi."
            );

            return;
        }

        foundHuyPhoto = true;

        ShowClue(
            "BỨC ẢNH CŨ\n\n" +
            "Trong ảnh là Huy.\n\n" +
            "Trên cổ Huy có đeo một sợi dây chuyền lạ."
        );

        SetMission(
            "Tìm bức thư manh mối",
            letterTarget,
            "Có thể còn một bức thư cũ ở đâu đó trong khu vực"
        );
    }

    // ==================================================
    // BỨC THƯ MANH MỐI
    // ==================================================

    public void FindLetter()
    {
        if (!foundHuyPhoto)
        {
            ShowClue(
                "Bạn chưa biết bức thư này có liên quan đến điều gì."
            );

            return;
        }

        if (foundLetter)
        {
            ShowClue(
                "Bạn đã đọc bức thư này rồi."
            );

            return;
        }

        foundLetter = true;

        ShowClue(
            "BỨC THƯ MANH MỐI\n\n" +
            "\"Ba dấu hiệu còn sót lại sẽ mở thứ đang bị khóa.\"\n\n" +
            "Có vẻ bạn cần tìm 3 con số."
        );

        UpdateNumberMission();
    }

    // ==================================================
    // NUMBER 4 - CÂN CŨ
    // ==================================================

    public void FindNumber4()
    {
        if (!foundLetter)
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
            "Dưới đáy cân có khắc một con số:\n\n" +
            "4"
        );

        UpdateNumberMission();
    }

    // ==================================================
    // NUMBER 1 - ĐÈN DẦU
    // ==================================================

    public void FindNumber1()
    {
        if (!foundLetter)
        {
            ShowClue(
                "Bạn chưa biết cần tìm gì ở chiếc đèn dầu."
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
            "Phía sau thân đèn có khắc một con số:\n\n" +
            "1"
        );

        UpdateNumberMission();
    }

    // ==================================================
    // NUMBER 7 - MẢNH GƯƠNG VỠ
    // ==================================================

    public void FindNumber7()
    {
        if (!foundLetter)
        {
            ShowClue(
                "Bạn chưa biết mảnh gương này có ý nghĩa gì."
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
            "Trên mặt sau của mảnh gương có một dấu khắc:\n\n" +
            "7"
        );

        UpdateNumberMission();
    }

    // ==================================================
    // UPDATE NUMBER MISSION
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

        // Đủ 3 số
        if (count >= 3)
        {
            SetMission(
                "Tìm chiếc hộp gỗ bị khóa",
                boxTarget,
                "Ba con số có thể dùng để mở một chiếc hộp"
            );

            return;
        }

        if (missionText != null)
        {
            missionText.text =
                "NHIỆM VỤ:\n" +
                "Tìm 3 con số (" +
                count +
                "/3)";
        }

        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: Tìm 3 con số (" +
                count +
                "/3)";
        }

        // Chọn vật phẩm chưa tìm làm target
        if (!foundNumber4)
        {
            SetNumberTarget(
                number4Target,
                "Hãy kiểm tra chiếc cân cũ"
            );
        }
        else if (!foundNumber1)
        {
            SetNumberTarget(
                number1Target,
                "Hãy tìm chiếc đèn dầu cũ"
            );
        }
        else if (!foundNumber7)
        {
            SetNumberTarget(
                number7Target,
                "Hãy kiểm tra một mảnh gương vỡ"
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
    // HỘP GỖ
    // ==================================================

    public void OpenCodePanel()
    {
        if (!foundNumber4 ||
            !foundNumber1 ||
            !foundNumber7)
        {
            ShowClue(
                "Bạn chưa tìm đủ 3 con số."
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

        if (boxOpening)
            return;

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
        if (boxOpened || boxOpening)
            return;

        // ==============================
        // ĐÚNG MẬT KHẨU
        // ==============================

        if (code == correctCode)
        {
            wrongCodeAttempts = 0;

            if (codeInputUI != null)
            {
                codeInputUI.ClearCode();
            }

            if (codePanel != null)
            {
                codePanel.SetActive(false);
            }

            StartCoroutine(OpenWoodenBox());

            return;
        }

        // ==============================
        // SAI MẬT KHẨU
        // ==============================

        wrongCodeAttempts++;

        if (codeInputUI != null)
        {
            codeInputUI.ClearCode();
        }

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

        // ==============================
        // SAI ĐỦ 3 LẦN
        // ==============================

        wrongCodeAttempts = 0;

        if (codePanel != null)
        {
            codePanel.SetActive(false);
        }

        ResetNumberClues();

        ShowClue(
            "BẠN ĐÃ NHẬP SAI 3 LẦN!\n\n" +
            "Bạn phải kiểm tra lại ba vật phẩm để tìm mật mã."
        );
    }

    // ==================================================
    // MỞ NẮP HỘP
    // ==================================================

    private IEnumerator OpenWoodenBox()
    {
        boxOpening = true;

        // Nếu có LidPivot thì mở nắp từ từ
        if (lidPivot != null)
        {
            Quaternion startRotation =
                lidPivot.localRotation;

            float timer = 0f;

            while (timer < openDuration)
            {
                timer += Time.deltaTime;

                float t =
                    Mathf.Clamp01(
                        timer / openDuration
                    );

                lidPivot.localRotation =
                    Quaternion.Slerp(
                        startRotation,
                        openedLidRotation,
                        t
                    );

                yield return null;
            }

            lidPivot.localRotation =
                openedLidRotation;
        }

        boxOpened = true;
        boxOpening = false;

        // Hiện chìa khóa
        if (keyItem != null)
        {
            keyItem.SetActive(true);
        }

        // Hiện manh mối trong hộp
        if (boxClueItem != null)
        {
            boxClueItem.SetActive(true);
        }

        ShowClue(
            "MỞ KHÓA THÀNH CÔNG!\n\n" +
            "Bên trong hộp có một chiếc chìa khóa " +
            "và một manh mối."
        );

        SetMission(
            "Kiểm tra vật phẩm trong hộp",
            keyTarget,
            "Hãy kiểm tra bên trong chiếc hộp vừa mở"
        );
    }

    // ==================================================
    // CHÌA KHÓA TRONG HỘP
    // ==================================================

    public void FindKey()
    {
        if (!boxOpened)
        {
            ShowClue(
                "Chiếc hộp vẫn đang khóa."
            );

            return;
        }

        if (foundKey)
        {
            ShowClue(
                "Bạn đã lấy chiếc chìa khóa."
            );

            return;
        }

        foundKey = true;

        ShowClue(
            "CHIẾC CHÌA KHÓA CŨ\n\n" +
            "Một chiếc chìa khóa được giấu bên trong hộp."
        );

        if (keyItem != null)
        {
            keyItem.SetActive(false);
        }

        UpdateBoxItemsMission();
    }

    // ==================================================
    // MANH MỐI TRONG HỘP
    // ==================================================

    public void FindBoxClue()
    {
        if (!boxOpened)
        {
            ShowClue(
                "Chiếc hộp vẫn đang khóa."
            );

            return;
        }

        if (foundBoxClue)
        {
            ShowClue(
                "Bạn đã kiểm tra manh mối này rồi."
            );

            return;
        }

        foundBoxClue = true;

        ShowClue(
            "MANH MỐI TRONG HỘP\n\n" +
            "Một dòng chữ cũ còn đọc được:\n\n" +
            "\"Hãy trở về nhà Huy.\""
        );

        if (boxClueItem != null)
        {
            boxClueItem.SetActive(false);
        }

        UpdateBoxItemsMission();
    }

    // ==================================================
    // KIỂM TRA ĐÃ LẤY ĐỒ TRONG HỘP CHƯA
    // ==================================================

    private void UpdateBoxItemsMission()
    {
        // Đã lấy cả hai
        if (foundKey && foundBoxClue)
        {
            SetMission(
                "Đi đến lối ra",
                exitTarget,
                "Mang theo chìa khóa và đi đến lối ra khỏi khu vực"
            );

            return;
        }

        // Có chìa khóa nhưng chưa lấy manh mối
        if (foundKey && !foundBoxClue)
        {
            SetMission(
                "Kiểm tra manh mối trong hộp",
                boxClueTarget,
                "Trong hộp vẫn còn một vật"
            );

            return;
        }

        // Có manh mối nhưng chưa lấy chìa khóa
        if (!foundKey && foundBoxClue)
        {
            SetMission(
                "Lấy chiếc chìa khóa",
                keyTarget,
                "Trong hộp vẫn còn chiếc chìa khóa"
            );
        }
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
                "Tìm 3 con số (0/3)";
        }

        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: Tìm 3 con số (0/3)";
        }

        if (playerInteraction != null)
        {
            playerInteraction.SetTargetClue(
                number4Target,
                "Hãy kiểm tra chiếc cân cũ"
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

        if (!foundKey)
        {
            ShowClue(
                "Bạn chưa lấy chiếc chìa khóa trong hộp."
            );

            return;
        }

        if (!foundBoxClue)
        {
            ShowClue(
                "Trong hộp vẫn còn một manh mối."
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
            "Bạn đã lấy được chìa khóa và manh mối.\n\n" +
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