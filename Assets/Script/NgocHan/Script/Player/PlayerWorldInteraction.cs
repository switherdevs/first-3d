using UnityEngine;

public class PlayerWorldInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private PuzzleWorldPrompt currentPrompt;

    private void Update()
    {
        CheckInteraction();
    }

    private void CheckInteraction()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            interactionDistance,
            interactableLayer,
            QueryTriggerInteraction.Ignore
        ))
        {
            PuzzleWorldPrompt prompt =
                hit.collider.GetComponentInParent<PuzzleWorldPrompt>();

            if (prompt != null)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    prompt.transform.position
                );

                // Chỉ hiện E khi thật sự đủ gần
                if (distance <= interactionDistance)
                {
                    if (currentPrompt != prompt)
                    {
                        HideCurrentPrompt();

                        currentPrompt = prompt;
                        currentPrompt.ShowPrompt();
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("Đang tương tác với: " + hit.collider.name);

                        // Gọi logic puzzle của bạn ở đây
                    }

                    return;
                }
            }
        }

        HideCurrentPrompt();
    }

    private void HideCurrentPrompt()
    {
        if (currentPrompt != null)
        {
            currentPrompt.HidePrompt();
            currentPrompt = null;
        }
    }
}