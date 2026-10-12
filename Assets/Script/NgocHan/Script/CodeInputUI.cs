using UnityEngine;

public class CodeInputUI : MonoBehaviour
{
    public void CheckCode()
    {
        if (Map1PuzzleManager.Instance == null)
            return;


        Map1PuzzleManager.Instance
            .CheckCode();
    }
}