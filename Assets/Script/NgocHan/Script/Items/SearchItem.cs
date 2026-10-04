using UnityEngine;

public class SearchItem : MonoBehaviour
{
    [Header("Item Information")]
    public string itemID;

    public string itemName;

    [TextArea(2, 5)]
    public string itemHint;
    public Sprite itemIcon;

    [Header("Status")]
    public bool isCollected = false;

    private Renderer[] itemRenderers;
    private Material[] itemMaterials;

    private Color[] originalEmissionColors;
    private bool[] emissionSupported;

    private void Awake()
    {
        CacheMaterials();

        // Items do not glow at the beginning.
        SetGlow(false);
    }

    private void CacheMaterials()
    {
        itemRenderers =
            GetComponentsInChildren<Renderer>();

        int materialCount = 0;

        foreach (Renderer renderer in itemRenderers)
        {
            if (renderer != null)
            {
                materialCount += renderer.materials.Length;
            }
        }

        itemMaterials =
            new Material[materialCount];

        originalEmissionColors =
            new Color[materialCount];

        emissionSupported =
            new bool[materialCount];

        int index = 0;

        foreach (Renderer renderer in itemRenderers)
        {
            if (renderer == null)
                continue;

            foreach (Material material in renderer.materials)
            {
                if (material == null)
                    continue;

                itemMaterials[index] = material;

                if (material.HasProperty("_EmissionColor"))
                {
                    emissionSupported[index] = true;

                    originalEmissionColors[index] =
                        material.GetColor("_EmissionColor");
                }
                else
                {
                    emissionSupported[index] = false;
                }

                index++;
            }
        }
    }

    // =========================================================
    // GLOW
    // =========================================================

    public void SetGlow(bool enabled)
    {
        if (itemMaterials == null)
            return;

        for (int i = 0; i < itemMaterials.Length; i++)
        {
            Material material = itemMaterials[i];

            if (material == null)
                continue;

            if (!emissionSupported[i])
                continue;

            if (enabled)
            {
                material.EnableKeyword("_EMISSION");

                material.SetColor(
                    "_EmissionColor",
                    Color.yellow * 2.5f
                );
            }
            else
            {
                material.SetColor(
                    "_EmissionColor",
                    originalEmissionColors[i]
                );
            }
        }
    }

    // =========================================================
    // COLLECT
    // =========================================================

    public void CollectItem()
    {
        if (isCollected)
            return;

        isCollected = true;

        SetGlow(false);

        gameObject.SetActive(false);
    }

    // =========================================================
    // RESET
    // =========================================================

    public void ResetItem()
    {
        isCollected = false;

        gameObject.SetActive(true);

        SetGlow(false);
    }
}