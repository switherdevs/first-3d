using UnityEngine;

public class Map1Interact : MonoBehaviour
{
    public enum PuzzleType
    {
        Paper,
        Ticket,
        Number4,
        Number1,
        Number7,
        Box,
        Exit
    }

    [Header("Puzzle Type")]
    public PuzzleType puzzleType;

    [Header("Noi dung khi nhan R")]
    public string displayName = "Manh moi";

    [TextArea(3, 8)]
    public string inspectText = "Khong co noi dung.";

    // E = TUONG TAC
    public void Interact()
    {
        if (Map1PuzzleManager.Instance == null)
        {
            Debug.LogError("Khong tim thay Map1PuzzleManager!");
            return;
        }

        switch (puzzleType)
        {
            case PuzzleType.Paper:
                Map1PuzzleManager.Instance.FindPaper();
                break;

            case PuzzleType.Ticket:
                Map1PuzzleManager.Instance.FindTicket();
                break;

            case PuzzleType.Number4:
                Map1PuzzleManager.Instance.FindNumber4();
                break;

            case PuzzleType.Number1:
                Map1PuzzleManager.Instance.FindNumber1();
                break;

            case PuzzleType.Number7:
                Map1PuzzleManager.Instance.FindNumber7();
                break;

            case PuzzleType.Box:
                Map1PuzzleManager.Instance.OpenCodePanel();
                break;

            case PuzzleType.Exit:
                Map1PuzzleManager.Instance.GoToExit();
                break;
        }
    }

    // R = XEM NOI DUNG
    public void Inspect()
    {
        if (Map1PuzzleManager.Instance == null)
            return;

        Map1PuzzleManager.Instance.ShowClue(
            displayName +
            "\n\n" +
            inspectText
        );
    }
}