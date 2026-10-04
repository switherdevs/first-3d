using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    [Header("Hint for this location")]

    [TextArea(2, 5)]
    public string locationHint;

    [Header("Optional Settings")]
    public Vector3 positionOffset = Vector3.zero;

    public Vector3 GetSpawnPosition()
    {
        return transform.position + positionOffset;
    }

    public Quaternion GetSpawnRotation()
    {
        return transform.rotation;
    }
}