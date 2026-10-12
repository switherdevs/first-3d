using UnityEngine;

[CreateAssetMenu(
    fileName = "InventoryItem",
    menuName = "Game/Inventory Item"
)]
public class InventoryItemData : ScriptableObject
{
    [Header("THÔNG TIN")]
    public string itemId;
    public string itemName;

    [TextArea(2, 5)]
    public string itemHint;

    [Header("ICON")]
    public Sprite itemIcon;
}