using UnityEngine;
using UnityEngine.UI;

public class HotbarItemDisplay : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField]
    private HotbarManager hotbarManager;


    [Header("RUNTIME")]
    [SerializeField]
    private int nextFreeSlot = 0;


    // =========================================================
    // ADD ITEM
    // =========================================================

    public bool AddItem(
        SearchItem item
    )
    {
        if (item == null)
        {
            Debug.LogError(
                "[Hotbar] SearchItem = NULL"
            );

            return false;
        }


        if (hotbarManager == null)
        {
            Debug.LogError(
                "[Hotbar] Chưa gắn HotbarManager!"
            );

            return false;
        }


        if (hotbarManager.danhSachOTo == null ||
            hotbarManager.danhSachOTo.Length == 0)
        {
            Debug.LogError(
                "[Hotbar] danhSachOTo đang rỗng!"
            );

            return false;
        }


        // =====================================================
        // FULL
        // =====================================================

        if (nextFreeSlot >=
            hotbarManager.danhSachOTo.Length)
        {
            Debug.LogWarning(
                "[Hotbar] Kho đồ đã đầy!"
            );

            return false;
        }


        RectTransform slot =
            hotbarManager.danhSachOTo[
                nextFreeSlot
            ];


        if (slot == null)
        {
            Debug.LogError(
                "[Hotbar] Slot " +
                nextFreeSlot +
                " = NULL"
            );

            return false;
        }


        // =====================================================
        // ICON DATA
        // =====================================================

        ItemInventoryIcon iconData =
            item.GetComponent<ItemInventoryIcon>();


        if (iconData == null)
        {
            iconData =
                item.GetComponentInChildren<ItemInventoryIcon>(
                    true
                );
        }


        if (iconData == null)
        {
            Debug.LogError(
                "[Hotbar] " +
                item.gameObject.name +
                " chưa có ItemInventoryIcon."
            );

            return false;
        }


        if (iconData.itemIcon == null)
        {
            Debug.LogError(
                "[Hotbar] " +
                item.gameObject.name +
                " chưa gắn Item Icon."
            );

            return false;
        }


        // =====================================================
        // SLOT IMAGE
        // =====================================================

        Image slotImage =
            FindSlotImage(slot);


        if (slotImage == null)
        {
            Debug.LogError(
                "[Hotbar] Không tìm thấy Image trong " +
                slot.name
            );

            return false;
        }


        // =====================================================
        // SHOW ICON
        // =====================================================

        slotImage.sprite =
            iconData.itemIcon;


        slotImage.enabled = true;

        slotImage.preserveAspect = true;

        slotImage.color =
            Color.white;


        Debug.Log(
            "[Hotbar] Đã thêm " +
            item.itemName +
            " vào " +
            slot.name
        );


        nextFreeSlot++;


        return true;
    }


    // =========================================================
    // FIND SLOT IMAGE
    // =========================================================

    private Image FindSlotImage(
        RectTransform slot
    )
    {
        if (slot == null)
            return null;


        // Nếu có child Icon
        Transform iconChild =
            slot.Find("Icon");


        if (iconChild != null)
        {
            Image icon =
                iconChild.GetComponent<Image>();


            if (icon != null)
                return icon;
        }


        // UI hiện tại của bạn:
        // kho_1 chính là Image
        Image ownImage =
            slot.GetComponent<Image>();


        if (ownImage != null)
            return ownImage;


        // Fallback
        Image[] images =
            slot.GetComponentsInChildren<Image>(
                true
            );


        foreach (Image image in images)
        {
            if (image != null)
                return image;
        }


        return null;
    }


    // =========================================================
    // CLEAR
    // =========================================================

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


            Image image =
                FindSlotImage(slot);


            if (image == null)
                continue;


            image.sprite = null;

            image.color =
                Color.white;
        }


        nextFreeSlot = 0;
    }
}