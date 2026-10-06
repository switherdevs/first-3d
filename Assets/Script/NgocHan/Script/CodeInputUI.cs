using UnityEngine;
using TMPro;

public class CodeInputUI : MonoBehaviour
{
    public TMP_InputField input1;
    public TMP_InputField input2;
    public TMP_InputField input3;

    public void CheckCode()
    {
        string code =
            input1.text +
            input2.text +
            input3.text;

        if (Map1PuzzleManager.Instance != null)
        {
            Map1PuzzleManager.Instance.CheckCode(code);
        }

        // XOA SAU KHI BAM ENTER
        ClearCode();
    }

    public void ClearCode()
    {
        if (input1 != null)
            input1.text = "";

        if (input2 != null)
            input2.text = "";

        if (input3 != null)
            input3.text = "";

        // dua con tro ve o dau
        if (input1 != null)
            input1.Select();
    }
}