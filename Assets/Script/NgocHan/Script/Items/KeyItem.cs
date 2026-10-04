using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public enum KeyType
    {
        RestrictedAreaKey,
        HouseRoomKey
    }

    [Header("Key Information")]
    public KeyType keyType;

    public string keyName;

    [TextArea(2, 5)]
    public string keyDescription;

    private void Reset()
    {
        // Automatically set a default name
        keyName = gameObject.name;
    }

    public bool IsRestrictedAreaKey()
    {
        return keyType == KeyType.RestrictedAreaKey;
    }

    public bool IsHouseRoomKey()
    {
        return keyType == KeyType.HouseRoomKey;
    }
}