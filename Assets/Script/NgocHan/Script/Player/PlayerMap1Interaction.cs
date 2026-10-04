using UnityEngine;
using TMPro;

public class PlayerMap1Interaction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 2f;

    [Header("Target hien tai")]
    public Transform targetClue;

    [Header("E Icon")]
    public GameObject interactionIcon;

    [Header("R Icon")]
    public GameObject inspectIcon;

    [Header("Hint UI")]
    public GameObject hintGroup;
    public TMP_Text hintText;

    [Header("Distance UI")]
    public GameObject distanceGroup;
    public TMP_Text distanceText;

    private string currentHint = "";

    private Map1Interact currentInteract;

    private void Start()
    {
        HideInteractionIcons();

        if (hintGroup != null)
            hintGroup.SetActive(true);

        if (distanceGroup != null)
            distanceGroup.SetActive(true);
    }

    private void Update()
    {
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        // Hint
        if (hintText != null)
            hintText.text = currentHint;

        // Chua co target
        if (targetClue == null)
        {
            currentInteract = null;

            if (distanceText != null)
                distanceText.text = "-- m";

            HideInteractionIcons();
            return;
        }

        // Khoang cach
        float distance = Vector3.Distance(
            transform.position,
            targetClue.position
        );

        if (distanceText != null)
        {
            distanceText.text =
                distance.ToString("F1") + " m";
        }

        // Tim Map1Interact
        currentInteract =
            targetClue.GetComponent<Map1Interact>();

        if (currentInteract == null)
        {
            currentInteract =
                targetClue.GetComponentInChildren<Map1Interact>();
        }

        if (currentInteract == null)
        {
            currentInteract =
                targetClue.GetComponentInParent<Map1Interact>();
        }

        if (currentInteract == null)
        {
            HideInteractionIcons();
            return;
        }

        // Xa hon 2m
        if (distance > interactionDistance)
        {
            HideInteractionIcons();
            return;
        }

        // Trong 2m
        ShowInteractionIcons();

        // E
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentInteract.Interact();
        }

        // R
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentInteract.Inspect();
        }
    }

    private void ShowInteractionIcons()
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(true);

        if (inspectIcon != null)
            inspectIcon.SetActive(true);
    }

    private void HideInteractionIcons()
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(false);

        if (inspectIcon != null)
            inspectIcon.SetActive(false);
    }

    public void SetTargetClue(
        Transform newTarget,
        string newHint
    )
    {
        targetClue = newTarget;
        currentHint = newHint;
    }
}