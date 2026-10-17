using UnityEngine;
using TMPro;

public class CodeLock : MonoBehaviour
{
    // =========================================================
    // HIỂN THỊ 3 SỐ
    // =========================================================

    [Header("HIỂN THỊ 3 SỐ")]

    [SerializeField]
    private TMP_Text digit1Text;

    [SerializeField]
    private TMP_Text digit2Text;

    [SerializeField]
    private TMP_Text digit3Text;


    // =========================================================
    // GIÁ TRỊ HIỆN TẠI
    // =========================================================

    private int digit1 = 0;
    private int digit2 = 0;
    private int digit3 = 0;


    // =========================================================
    // THÔNG BÁO CŨ
    // =========================================================

    [Header("THÔNG BÁO - CÓ THỂ ĐỂ TRỐNG")]

    [SerializeField]
    private GameObject correctText;

    [SerializeField]
    private GameObject wrongText;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        UpdateDisplay();

        if (correctText != null)
        {
            correctText.SetActive(false);
        }

        if (wrongText != null)
        {
            wrongText.SetActive(false);
        }
    }


    // =========================================================
    // SỐ 1
    // =========================================================

    public void Up1()
    {
        digit1++;

        if (digit1 > 9)
        {
            digit1 = 0;
        }

        UpdateDisplay();
    }


    public void Down1()
    {
        digit1--;

        if (digit1 < 0)
        {
            digit1 = 9;
        }

        UpdateDisplay();
    }


    // =========================================================
    // SỐ 2
    // =========================================================

    public void Up2()
    {
        digit2++;

        if (digit2 > 9)
        {
            digit2 = 0;
        }

        UpdateDisplay();
    }


    public void Down2()
    {
        digit2--;

        if (digit2 < 0)
        {
            digit2 = 9;
        }

        UpdateDisplay();
    }


    // =========================================================
    // SỐ 3
    // =========================================================

    public void Up3()
    {
        digit3++;

        if (digit3 > 9)
        {
            digit3 = 0;
        }

        UpdateDisplay();
    }


    public void Down3()
    {
        digit3--;

        if (digit3 < 0)
        {
            digit3 = 9;
        }

        UpdateDisplay();
    }


    // =========================================================
    // UPDATE DISPLAY
    // =========================================================

    private void UpdateDisplay()
    {
        if (digit1Text != null)
        {
            digit1Text.text =
                digit1.ToString();
        }

        if (digit2Text != null)
        {
            digit2Text.text =
                digit2.ToString();
        }

        if (digit3Text != null)
        {
            digit3Text.text =
                digit3.ToString();
        }
    }


    // =========================================================
    // CHECK CODE
    // =========================================================

    public void CheckCode()
    {
        string inputCode =
            digit1.ToString() +
            digit2.ToString() +
            digit3.ToString();


        Debug.Log(
            "[CODE LOCK] Người chơi nhập: " +
            inputCode
        );


        if (Map1PuzzleManager.Instance == null)
        {
            Debug.LogError(
                "[CODE LOCK] Không tìm thấy Map1PuzzleManager!"
            );

            return;
        }


        // Gửi 3 số sang PuzzleManager kiểm tra
        bool correct =
            Map1PuzzleManager.Instance
                .CheckCode(inputCode);


        // =====================================================
        // ĐÚNG
        // =====================================================

        if (correct)
        {
            Debug.Log(
                "[CODE LOCK] MẬT MÃ ĐÚNG!"
            );


            if (correctText != null)
            {
                correctText.SetActive(true);
            }

            if (wrongText != null)
            {
                wrongText.SetActive(false);
            }
        }

        // =====================================================
        // SAI
        // =====================================================

        else
        {
            Debug.Log(
                "[CODE LOCK] SAI MẬT MÃ!"
            );


            if (wrongText != null)
            {
                wrongText.SetActive(true);
            }

            if (correctText != null)
            {
                correctText.SetActive(false);
            }
        }
    }


    // =========================================================
    // RESET CODE
    // =========================================================

    public void ResetCode()
    {
        digit1 = 0;
        digit2 = 0;
        digit3 = 0;

        UpdateDisplay();


        if (correctText != null)
        {
            correctText.SetActive(false);
        }

        if (wrongText != null)
        {
            wrongText.SetActive(false);
        }
    }
}