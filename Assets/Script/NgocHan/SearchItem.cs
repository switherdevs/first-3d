using UnityEngine;

public enum SearchItemType
{
    Key,
    Phone,
    Photo,
    Radio
}

public class SearchItem : MonoBehaviour
{
    [Header("Item Information")]
    public SearchItemType itemType;

    public string itemName;

    [TextArea(2, 5)]
    public string itemHint;

    [HideInInspector]
    public bool isCollected = false;

    [Header("Glow Settings")]
    public Color glowColor = Color.yellow;
    public float glowIntensity = 2f;

    private Renderer[] itemRenderers;
    private Material[] runtimeMaterials;
    private Color[] originalEmissionColors;
    private bool[] supportsEmission;

    private void Awake()
    {
        SetupMaterials();
    }

    private void SetupMaterials()
    {
        itemRenderers = GetComponentsInChildren<Renderer>(true);

        int materialCount = 0;

        foreach (Renderer renderer in itemRenderers)
        {
            materialCount += renderer.materials.Length;
        }

        runtimeMaterials = new Material[materialCount];
        originalEmissionColors = new Color[materialCount];
        supportsEmission = new bool[materialCount];

        int index = 0;

        foreach (Renderer renderer in itemRenderers)
        {
            Material[] materials = renderer.materials;

            foreach (Material material in materials)
            {
                runtimeMaterials[index] = material;

                if (material != null &&
                    material.HasProperty("_EmissionColor"))
                {
                    supportsEmission[index] = true;

                    originalEmissionColors[index] =
                        material.GetColor("_EmissionColor");
                }
                else
                {
                    supportsEmission[index] = false;
                }

                index++;
            }
        }
    }

    public void SetGlow(bool enabled)
    {
        if (runtimeMaterials == null)
            return;

        for (int i = 0; i < runtimeMaterials.Length; i++)
        {
            Material material = runtimeMaterials[i];

            if (material == null || !supportsEmission[i])
                continue;

            if (enabled)
            {
                material.EnableKeyword("_EMISSION");

                Color finalGlowColor =
                    glowColor * glowIntensity;

                material.SetColor(
                    "_EmissionColor",
                    finalGlowColor
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

    public void CollectItem()
    {
        if (isCollected)
            return;

        isCollected = true;

        SetGlow(false);

        Debug.Log(
            "Collected item: " +
            itemName +
            " | Type: " +
            itemType
        );

        gameObject.SetActive(false);
    }
}