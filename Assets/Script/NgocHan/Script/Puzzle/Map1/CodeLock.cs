using UnityEngine;
using TMPro;

public class CodeLock : MonoBehaviour
{
    [Header("Hien thi 3 so")]
    public TMP_Text digit1Text;
    public TMP_Text digit2Text;
    public TMP_Text digit3Text;

    private int digit1 = 0;
    private int digit2 = 0;
    private int digit3 = 0;

    [Header("Mat ma dung")]
    public int correctDigit1 = 4;
    public int correctDigit2 = 1;
    public int correctDigit3 = 7;

    [Header("Thong bao")]
    public GameObject correctText;
    public GameObject wrongText;

    void Start()
    {
        UpdateDisplay();

        if (correctText != null)
            correctText.SetActive(false);

        if (wrongText != null)
            wrongText.SetActive(false);
    }

    // SO 1
    public void Up1()
    {
        digit1++;

        if (digit1 > 9)
            digit1 = 0;

        UpdateDisplay();
    }

    public void Down1()
    {
        digit1--;

        if (digit1 < 0)
            digit1 = 9;

        UpdateDisplay();
    }

    // SO 2
    public void Up2()
    {
        digit2++;

        if (digit2 > 9)
            digit2 = 0;

        UpdateDisplay();
    }

    public void Down2()
    {
        digit2--;

        if (digit2 < 0)
            digit2 = 9;

        UpdateDisplay();
    }

    // SO 3
    public void Up3()
    {
        digit3++;

        if (digit3 > 9)
            digit3 = 0;

        UpdateDisplay();
    }

    public void Down3()
    {
        digit3--;

        if (digit3 < 0)
            digit3 = 9;

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        digit1Text.text = digit1.ToString();
        digit2Text.text = digit2.ToString();
        digit3Text.text = digit3.ToString();
    }

    public void CheckCode()
    {
        if (digit1 == correctDigit1 &&
            digit2 == correctDigit2 &&
            digit3 == correctDigit3)
        {
            Debug.Log("MAT MA DUNG!");

            if (correctText != null)
                correctText.SetActive(true);

            if (wrongText != null)
                wrongText.SetActive(false);
        }
        else
        {
            Debug.Log("SAI MAT MA!");

            if (wrongText != null)
                wrongText.SetActive(true);

            if (correctText != null)
                correctText.SetActive(false);
        }
    }
}