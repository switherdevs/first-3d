using UnityEngine;
using TMPro;

public class Map1PuzzleManager : MonoBehaviour
{
    public static Map1PuzzleManager Instance;

    [Header("UI")]
    public GameObject cluePanel;
    public TMP_Text clueText;
    public TMP_Text objectiveText;

    [Header("Code Panel")]
    public GameObject codePanel;

    [Header("Puzzle Numbers")]
    public GameObject number4Text;
    public GameObject number1Text;
    public GameObject number7Text;

    // Trạng thái puzzle
    private bool foundPaper = false;
    private bool foundTicket = false;

    private bool foundNumber4 = false;
    private bool foundNumber1 = false;
    private bool foundNumber7 = false;

    private bool boxOpened = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ẩn UI lúc bắt đầu
        if (cluePanel != null)
            cluePanel.SetActive(false);

        if (codePanel != null)
            codePanel.SetActive(false);

        // Ẩn các số lúc đầu
        if (number4Text != null)
            number4Text.SetActive(false);

        if (number1Text != null)
            number1Text.SetActive(false);

        if (number7Text != null)
            number7Text.SetActive(false);

        if (objectiveText != null)
        {
            objectiveText.text =
                "Mục tiêu: Tìm mảnh giấy trong khu chợ";
        }
    }

    // =========================
    // PAPER
    // =========================

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
            "\"Nơi người thân trở về sau mỗi chuyến đi...\"\n\n" +
            "Có vẻ mảnh giấy đang dẫn Huy tới một nơi quen thuộc."
        );

        objectiveText.text =
            "Mục tiêu: Tìm phiếu cũ trong khu chợ";
    }

    // =========================
    // TICKET
    // =========================

    public void FindTicket()
    {
        if (!foundPaper)
        {
            ShowClue(
                "Bạn chưa hiểu ý nghĩa của vật này.\n\n" +
                "Hãy tìm một manh mối khác trước."
            );

            return;
        }

        if (foundTicket)
        {
            ShowClue(
                "Bạn đã kiểm tra phiếu cũ."
            );

            return;
        }

        foundTicket = true;

        ShowClue(
            "PHIẾU CŨ\n\n" +
            "Tên: Huy\n\n" +
            "Trên phiếu có ghi địa chỉ ngôi nhà cũ của gia đình.\n\n" +
            "Phía dưới còn có dòng chữ:\n" +
            "\"Ba dấu hiệu sẽ mở đường.\""
        );

        objectiveText.text =
            "Mục tiêu: Tìm 3 con số trong khu chợ";
    }

    // =========================
    // NUMBER 4
    // =========================

    public void FindNumber4()
    {
        if (!foundTicket)
        {
            ShowClue(
                "Chiếc cân trông khá cũ...\n\n" +
                "Nhưng hiện tại Huy chưa biết phải tìm gì ở đây."
            );

            return;
        }

        if (foundNumber4)
        {
            ShowClue(
                "Bạn đã tìm thấy số 4 ở chiếc cân."
            );

            return;
        }

        foundNumber4 = true;

        if (number4Text != null)
            number4Text.SetActive(true);

        ShowClue(
            "CHIẾC CÂN CŨ\n\n" +
            "Huy kiểm tra phía dưới chiếc cân.\n\n" +
            "Một con số được khắc ở đáy:\n\n" +
            "4"
        );

        UpdateNumberObjective();
    }

    // =========================
    // NUMBER 1
    // =========================

    public void FindNumber1()
    {
        if (!foundTicket)
        {
            ShowClue(
                "Chiếc đèn dầu có vẻ bình thường.\n\n" +
                "Huy chưa biết mình cần tìm gì."
            );

            return;
        }

        if (foundNumber1)
        {
            ShowClue(
                "Bạn đã tìm thấy số 1 trên đèn dầu."
            );

            return;
        }

        foundNumber1 = true;

        if (number1Text != null)
            number1Text.SetActive(true);

        ShowClue(
            "ĐÈN DẦU CŨ\n\n" +
            "Phía sau thân đèn có một ký hiệu nhỏ.\n\n" +
            "Đó là số:\n\n" +
            "1"
        );

        UpdateNumberObjective();
    }

    // =========================
    // NUMBER 7
    // =========================

    public void FindNumber7()
    {
        if (!foundTicket)
        {
            ShowClue(
                "Một vệt nước kỳ lạ trên sàn...\n\n" +
                "Có vẻ Huy vẫn thiếu một manh mối để hiểu nó."
            );

            return;
        }

        if (foundNumber7)
        {
            ShowClue(
                "Bạn đã tìm thấy số 7 ở vệt nước."
            );

            return;
        }

        foundNumber7 = true;

        if (number7Text != null)
            number7Text.SetActive(true);

        ShowClue(
            "VỆT NƯỚC\n\n" +
            "Huy quan sát kỹ vệt nước trên sàn.\n\n" +
            "Một hình dạng giống con số hiện ra:\n\n" +
            "7"
        );

        UpdateNumberObjective();
    }

    // =========================
    // UPDATE OBJECTIVE
    // =========================

    private void UpdateNumberObjective()
    {
        int count = 0;

        if (foundNumber4)
            count++;

        if (foundNumber1)
            count++;

        if (foundNumber7)
            count++;

        if (count < 3)
        {
            objectiveText.text =
                "Mục tiêu: Tìm các con số (" +
                count +
                "/3)";
        }
        else
        {
            objectiveText.text =
                "Mục tiêu: Tìm chiếc hộp khóa";
        }
    }

    // =========================
    // BOX
    // =========================

    public void OpenCodePanel()
    {
        if (!foundTicket)
        {
            ShowClue(
                "Chiếc hộp đã bị khóa.\n\n" +
                "Bạn chưa biết cách mở nó."
            );

            return;
        }

        if (!foundNumber4 ||
            !foundNumber1 ||
            !foundNumber7)
        {
            ShowClue(
                "Ổ khóa cần mã gồm 3 chữ số.\n\n" +
                "Bạn vẫn chưa tìm đủ các con số."
            );

            return;
        }

        if (boxOpened)
        {
            ShowClue(
                "Chiếc hộp đã được mở.\n\n" +
                "Bên trong là địa chỉ Nhà Huy."
            );

            return;
        }

        if (codePanel != null)
        {
            codePanel.SetActive(true);

            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }
    }

    // =========================
    // CHECK CODE
    // =========================

    public void CheckCode(string code)
    {
        if (code == "417")
        {
            boxOpened = true;

            if (codePanel != null)
                codePanel.SetActive(false);

            ShowClue(
                "MỞ KHÓA THÀNH CÔNG!\n\n" +
                "Bên trong chiếc hộp có một lá thư cũ.\n\n" +
                "\"Nếu con quay lại...\n" +
                "hãy về nhà.\"\n\n" +
                "Huy đã tìm ra địa chỉ ngôi nhà cũ."
            );

            objectiveText.text =
                "Mục tiêu: Đi đến lối ra khỏi khu chợ";
        }
        else
        {
            ShowClue(
                "MÃ KHÔNG ĐÚNG!\n\n" +
                "Hãy nhớ lại thứ tự các dấu hiệu."
            );
        }
    }

    // =========================
    // CLUE UI
    // =========================

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

    // =========================
    // EXIT
    // =========================

    public void GoToMap2()
    {
        if (!boxOpened)
        {
            ShowClue(
                "Huy vẫn chưa tìm đủ manh mối.\n\n" +
                "Không nên rời khu chợ lúc này."
            );

            return;
        }

        Debug.Log(
            "PUZZLE MAP 1 HOÀN THÀNH - SẴN SÀNG SANG MAP 2"
        );

        ShowClue(
            "MAP 1 HOÀN THÀNH\n\n" +
            "Địa điểm tiếp theo:\n\n" +
            "NHÀ HUY"
        );

        // Sau này khi có Map 2:
        // SceneManager.LoadScene("Map2_NhaHuy");
    }
}