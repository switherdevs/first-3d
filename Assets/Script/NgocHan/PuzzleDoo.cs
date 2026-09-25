using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    public void OpenDoor()
    {
        transform.Rotate(
            0f,
            90f,
            0f
        );

        Debug.Log("Door opened.");
    }
}