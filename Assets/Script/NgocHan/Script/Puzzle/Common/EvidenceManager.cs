using UnityEngine;
using TMPro;

public class EvidenceManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Evidence")]
    public Evidence firstEvidence;

    [Header("UI")]
    public TextMeshProUGUI evidenceText;
    public TextMeshProUGUI interactionText;

    [Header("Settings")]
    public float raycastDistance = 5f;
    public float interactionDistance = 2.5f;

    private Evidence currentEvidence;
    private Evidence lookedAtEvidence;

    private string temporaryMessage = "";
    private float messageTimer = 0f;

    private void Start()
    {
        StartEvidenceChain();
    }

    private void Update()
    {
        UpdateMessageTimer();

        if (currentEvidence == null)
            return;

        DetectEvidence();

        HandleInteraction();

        UpdateUI();
    }

    private void StartEvidenceChain()
    {
        if (firstEvidence == null)
        {
            Debug.LogWarning("First Evidence is not assigned.");
            return;
        }

        currentEvidence = firstEvidence;

        currentEvidence.SetEvidenceVisible(true);

        UpdateEvidenceUI();
    }

    private void DetectEvidence()
    {
        lookedAtEvidence = null;

        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            raycastDistance
        ))
        {
            Evidence evidence =
                hit.collider.GetComponentInParent<Evidence>();

            if (evidence != null)
            {
                lookedAtEvidence = evidence;
            }
        }
    }

    private void HandleInteraction()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (lookedAtEvidence == null)
        {
            return;
        }

        float distance = Vector3.Distance(
            player.position,
            lookedAtEvidence.transform.position
        );

        if (distance > interactionDistance)
        {
            ShowMessage(
                "Move closer to the evidence."
            );

            return;
        }

        if (lookedAtEvidence == currentEvidence)
        {
            ConfirmEvidence();
        }
        else
        {
            ShowMessage(
                "This is not the correct evidence."
            );
        }
    }

    private void ConfirmEvidence()
    {
        string completedID =
            currentEvidence.evidenceID;

        Debug.Log(
            "Evidence confirmed: " + completedID
        );

        ShowMessage(
            "Evidence confirmed!\n" +
            currentEvidence.evidenceHint
        );

        Evidence nextEvidence =
            currentEvidence.nextEvidence;

        currentEvidence.CompleteEvidence();

        currentEvidence = nextEvidence;

        if (currentEvidence != null)
        {
            currentEvidence.SetEvidenceVisible(true);

            UpdateEvidenceUI();
        }
        else
        {
            CompleteEvidenceChain();
        }
    }

    private void UpdateEvidenceUI()
    {
        if (currentEvidence == null)
            return;

        if (evidenceText != null)
        {
            evidenceText.text =
                "Evidence: " +
                currentEvidence.evidenceID +
                "\n" +
                currentEvidence.evidenceHint;
        }
    }

    private void UpdateUI()
    {
        if (interactionText == null)
            return;

        if (messageTimer > 0f)
        {
            interactionText.text = temporaryMessage;
            return;
        }

        if (lookedAtEvidence == null)
        {
            interactionText.text = "";
            return;
        }

        float distance = Vector3.Distance(
            player.position,
            lookedAtEvidence.transform.position
        );

        if (distance > interactionDistance)
        {
            interactionText.text = "";
            return;
        }

        if (lookedAtEvidence == currentEvidence)
        {
            interactionText.text =
                "Evidence found!\n" +
                "Press E to investigate.";
        }
        else
        {
            interactionText.text = "";
        }
    }

    private void ShowMessage(string message)
    {
        temporaryMessage = message;
        messageTimer = 3f;
    }

    private void UpdateMessageTimer()
    {
        if (messageTimer <= 0f)
            return;

        messageTimer -= Time.deltaTime;

        if (messageTimer <= 0f)
        {
            messageTimer = 0f;
            temporaryMessage = "";
        }
    }

    private void CompleteEvidenceChain()
    {
        currentEvidence = null;

        if (evidenceText != null)
        {
            evidenceText.text =
                "Evidence chain completed!";
        }

        ShowMessage(
            "You followed the evidence trail."
        );

        Debug.Log(
            "Evidence chain completed!"
        );
    }
}