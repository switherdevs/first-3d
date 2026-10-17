using UnityEngine;

public class Evidence : MonoBehaviour
{
    [Header("Evidence Information")]
    public string evidenceID;

    [TextArea(2, 5)]
    public string evidenceHint;

    [Header("Next Evidence")]
    public Evidence nextEvidence;

    [Header("Status")]
    public bool isCompleted = false;

    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        SetEvidenceVisible(false);
    }

    public void SetEvidenceVisible(bool visible)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }

    public void CompleteEvidence()
    {
        isCompleted = true;

        SetEvidenceVisible(false);

        if (nextEvidence != null)
        {
            nextEvidence.SetEvidenceVisible(true);
        }
    }
}