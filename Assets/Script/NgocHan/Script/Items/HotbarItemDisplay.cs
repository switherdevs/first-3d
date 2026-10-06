using UnityEngine;
using UnityEngine.UI;

public class HotbarItemDisplay : MonoBehaviour
{
    [Header("References")]
    public HotbarManager hotbarManager;

    [Header("Runtime")]
    [SerializeField]
    private int nextFreeSlot = 0;

    // =====================================================
    // ADD ITEM
    // =====================================================

    public void AddItem(SearchItem item)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "[HotbarItemDisplay] SearchItem is null."
            );

            return;
        }

        if (hotbarManager == null)
        {
            Debug.LogError(
                "[HotbarItemDisplay] HotbarManager is not assigned."
            );

            return;
        }

        if (hotbarManager.danhSachOTo == null ||
            hotbarManager.danhSachOTo.Length == 0)
        {
            Debug.LogError(
                "[HotbarItemDisplay] Hotbar has no slots."
            );

            return;
        }

        // Hotbar full
        if (nextFreeSlot >=
            hotbarManager.danhSachOTo.Length)
        {
            Debug.LogWarning(
                "[HotbarItemDisplay] Hotbar is full."
            );

            return;
        }

        RectTransform slot =
            hotbarManager.danhSachOTo[
                nextFreeSlot
            ];

        if (slot == null)
        {
            Debug.LogError(
                "[HotbarItemDisplay] Slot " +
                nextFreeSlot +
                " is null."
            );

            return;
        }

        // =================================================
        // FIND ICON CHILD
        // =================================================

        Image iconImage =
            FindIconImage(slot);

        if (iconImage == null)
        {
            Debug.LogError(
                "[HotbarItemDisplay] Cannot find Icon Image inside " +
                slot.name
            );

            return;
        }

        // =================================================
        // GET ITEM ICON
        // =================================================

        ItemInventoryIcon iconData =
            item.GetComponent<ItemInventoryIcon>();

        if (iconData == null)
        {
            Debug.LogError(
                "[HotbarItemDisplay] " +
                item.name +
                " does not have ItemInventoryIcon."
            );

            return;
        }

        if (iconData.itemIcon == null)
        {
            Debug.LogWarning(
                "[HotbarItemDisplay] " +
                item.name +
                " has no icon assigned."
            );

            return;
        }

        // =================================================
        // SHOW ICON
        // =================================================

        iconImage.sprite =
            iconData.itemIcon;

        iconImage.enabled = true;

        iconImage.preserveAspect = true;

        iconImage.color =
            Color.white;

        Debug.Log(
            "[HotbarItemDisplay] Added " +
            item.itemName +
            " to " +
            slot.name
        );

        nextFreeSlot++;
    }

    // =====================================================
    // FIND ICON IMAGE
    // =====================================================

    private Image FindIconImage(
        RectTransform slot
    )
    {
        foreach (Transform child in slot)
        {
            // Accept Icon / icon / ICON
            if (child.name.Equals(
                "Icon",
                System.StringComparison.OrdinalIgnoreCase
            ))
            {
                Image image =
                    child.GetComponent<Image>();

                if (image != null)
                {
                    return image;
                }
            }
        }

        return null;
    }

    // =====================================================
    // RESET HOTBAR
    // =====================================================

    public void ClearHotbar()
    {
        if (hotbarManager == null ||
            hotbarManager.danhSachOTo == null)
        {
            return;
        }

        foreach (
            RectTransform slot
            in hotbarManager.danhSachOTo
        )
        {
            if (slot == null)
                continue;

            Image iconImage =
                FindIconImage(slot);

            if (iconImage == null)
                continue;

            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        nextFreeSlot = 0;
    }
}