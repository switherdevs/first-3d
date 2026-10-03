using UnityEngine;
using TMPro;

public class CodeInputUI : MonoBehaviour
{
    public TMP_InputField number1;
    public TMP_InputField number2;
    public TMP_InputField number3;

    public void CheckCode()
    {
        string code =
            number1.text +
            number2.text +
            number3.text;

        if (Map1PuzzleManager.Instance != null)
        {
            Map1PuzzleManager.Instance.CheckCode(code);
        }
    }

    public void ClearCode()
    {
        number1.text = "";
        number2.text = "";
        number3.text = "";

        number1.Select();
        number1.ActivateInputField();
    }
}