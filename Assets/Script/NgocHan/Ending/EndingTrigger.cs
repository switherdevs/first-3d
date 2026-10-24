using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private MapEndingController endingController;

    [Header("SETTINGS")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (endingController == null)
            return;

        endingController.StartEnding();
    }
}