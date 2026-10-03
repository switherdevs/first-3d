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

    [Header("Loại vật thể Puzzle")]
    public PuzzleType puzzleType;

    public void Interact()
    {
        if (Map1PuzzleManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy Map1PuzzleManager!");
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
                Map1PuzzleManager.Instance.GoToMap2();
                break;
        }
    }
}