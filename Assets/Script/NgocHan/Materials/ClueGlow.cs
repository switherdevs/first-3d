using UnityEngine;

public class ClueGlow : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Glow")]
    public float glowDistance = 4f;

    [Header("Renderer")]
    public Renderer targetRenderer;

    [Header("Materials")]
    public Material normalMaterial;
    public Material glowMaterial;

    private bool isGlowing = false;

    private void Start()
    {
        if (targetRenderer == null)
        {
            targetRenderer =
                GetComponentInChildren<Renderer>();
        }

        if (targetRenderer != null &&
            normalMaterial == null)
        {
            normalMaterial =
                targetRenderer.material;
        }

        DisableGlow();
    }

    private void Update()
    {
        if (player == null ||
            targetRenderer == null)
            return;

        float distance =
            Vector3.Distance(
                player.position,
                transform.position
            );

        if (distance <= glowDistance)
        {
            EnableGlow();
        }
        else
        {
            DisableGlow();
        }
    }

    private void EnableGlow()
    {
        if (isGlowing)
            return;

        isGlowing = true;

        if (glowMaterial != null)
        {
            targetRenderer.material =
                glowMaterial;
        }
    }

    private void DisableGlow()
    {
        if (!isGlowing &&
            targetRenderer != null &&
            normalMaterial != null)
        {
            targetRenderer.material =
                normalMaterial;

            return;
        }

        isGlowing = false;

        if (targetRenderer != null &&
            normalMaterial != null)
        {
            targetRenderer.material =
                normalMaterial;
        }
    }
}