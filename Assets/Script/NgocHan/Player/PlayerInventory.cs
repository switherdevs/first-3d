using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // =========================================================
    // INVENTORY SETTINGS
    // =========================================================

    [Header("Inventory Settings")]
    [SerializeField]
    private int maxItems = 2;


    // =========================================================
    // CURRENT ITEMS
    // =========================================================

    [Header("Current Inventory")]
    [SerializeField]
    private List<SearchItem> items = new List<SearchItem>();


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (maxItems < 1)
        {
            maxItems = 2;
        }

        // Remove empty references
        items.RemoveAll(item => item == null);

        Debug.Log(
            "Player Inventory Started: " +
            items.Count +
            "/" +
            maxItems
        );
    }


    // =========================================================
    // CHECK INVENTORY SPACE
    // =========================================================

    public bool CanCarryItem()
    {
        return items.Count < maxItems;
    }


    // =========================================================
    // ADD ITEM
    // =========================================================

    public bool AddItem(SearchItem item)
    {
        if (item == null)
        {
            Debug.LogWarning(
                "PlayerInventory: Cannot add NULL item."
            );

            return false;
        }

        // Check duplicate
        if (items.Contains(item))
        {
            Debug.LogWarning(
                "Player already has this item: " +
                item.itemName
            );

            return false;
        }

        // Check inventory capacity
        if (!CanCarryItem())
        {
            Debug.LogWarning(
                "Inventory is FULL! " +
                items.Count +
                "/" +
                maxItems
            );

            return false;
        }

        // Add item
        items.Add(item);

        Debug.Log(
            "ITEM ADDED"
        );

        Debug.Log(
            "Name: " +
            item.itemName
        );

        Debug.Log(
            "ID: " +
            item.itemID
        );

        Debug.Log(
            "Inventory: " +
            items.Count +
            "/" +
            maxItems
        );

        return true;
    }


    // =========================================================
    // REMOVE ITEM
    // =========================================================

    public bool RemoveItem(SearchItem item)
    {
        if (item == null)
        {
            return false;
        }

        if (!items.Contains(item))
        {
            Debug.LogWarning(
                "Item is not in inventory: " +
                item.itemName
            );

            return false;
        }

        items.Remove(item);

        Debug.Log(
            "ITEM REMOVED: " +
            item.itemName
        );

        Debug.Log(
            "Inventory: " +
            items.Count +
            "/" +
            maxItems
        );

        return true;
    }


    // =========================================================
    // REMOVE ITEM BY ID
    // =========================================================

    public bool RemoveItemByID(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return false;
        }

        SearchItem item = GetItem(itemID);

        if (item == null)
        {
            Debug.LogWarning(
                "Cannot remove item. " +
                "Item not found: " +
                itemID
            );

            return false;
        }

        return RemoveItem(item);
    }


    // =========================================================
    // CHECK ITEM BY ID
    // =========================================================

    public bool HasItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return false;
        }

        foreach (SearchItem item in items)
        {
            if (item == null)
            {
                continue;
            }

            if (item.itemID == itemID)
            {
                return true;
            }
        }

        return false;
    }


    // =========================================================
    // GET ITEM BY ID
    // =========================================================

    public SearchItem GetItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            return null;
        }

        foreach (SearchItem item in items)
        {
            if (item == null)
            {
                continue;
            }

            if (item.itemID == itemID)
            {
                return item;
            }
        }

        return null;
    }


    // =========================================================
    // GET ITEM AT SLOT
    // =========================================================

    public SearchItem GetItemAt(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            return null;
        }

        return items[index];
    }


    // =========================================================
    // GET ALL ITEMS
    // =========================================================

    public List<SearchItem> GetItems()
    {
        return items;
    }


    // =========================================================
    // CURRENT ITEM COUNT
    // =========================================================
    //
    // Used by ItemSearchManager
    //
    // =========================================================

    public int GetCurrentItemCount()
    {
        return items.Count;
    }


    // =========================================================
    // ITEM COUNT
    // =========================================================

    public int GetItemCount()
    {
        return items.Count;
    }


    // =========================================================
    // MAX ITEMS
    // =========================================================

    public int GetMaxItems()
    {
        return maxItems;
    }


    // =========================================================
    // MAX ITEM COUNT
    // =========================================================

    public int GetMaxItemCount()
    {
        return maxItems;
    }


    // =========================================================
    // FREE SLOTS
    // =========================================================

    public int GetFreeSlots()
    {
        return maxItems - items.Count;
    }


    // =========================================================
    // HAS KEY BY STRING
    // =========================================================

    public bool HasKey(string keyID)
    {
        if (string.IsNullOrEmpty(keyID))
        {
            return false;
        }

        return HasItem(keyID);
    }


    // =========================================================
    // CONVERT KEY TYPE TO SEARCH ITEM ID
    // =========================================================

    private string GetKeyItemID(KeyItem.KeyType keyType)
    {
        switch (keyType)
        {
            case KeyItem.KeyType.RestrictedAreaKey:

                return "KEY_RESTRICTED";


            case KeyItem.KeyType.HouseRoomKey:

                return "KEY_HOUSE";


            default:

                Debug.LogWarning(
                    "Unknown KeyType: " +
                    keyType
                );

                return "";
        }
    }


    // =========================================================
    // HAS KEY BY KEY TYPE
    // =========================================================
    //
    // Used by DoorInteraction
    //
    // =========================================================

    public bool HasKey(KeyItem.KeyType keyType)
    {
        string itemID = GetKeyItemID(keyType);

        if (string.IsNullOrEmpty(itemID))
        {
            return false;
        }

        return HasItem(itemID);
    }


    // =========================================================
    // USE KEY BY STRING
    // =========================================================

    public bool UseKey(string keyID)
    {
        if (string.IsNullOrEmpty(keyID))
        {
            return false;
        }

        SearchItem key = GetItem(keyID);

        if (key == null)
        {
            Debug.LogWarning(
                "Player does not have key: " +
                keyID
            );

            return false;
        }

        bool removed = RemoveItem(key);

        if (removed)
        {
            Debug.Log(
                "KEY USED: " +
                keyID
            );

            return true;
        }

        return false;
    }


    // =========================================================
    // USE KEY BY KEY TYPE
    // =========================================================
    //
    // Used by:
    //
    // playerInventory.UseKey(requiredKey);
    //
    // =========================================================

    public bool UseKey(KeyItem.KeyType keyType)
    {
        string itemID = GetKeyItemID(keyType);

        if (string.IsNullOrEmpty(itemID))
        {
            return false;
        }

        return UseKey(itemID);
    }


    // =========================================================
    // CLEAR INVENTORY
    // =========================================================

    public void ClearInventory()
    {
        items.Clear();

        Debug.Log(
            "Player inventory cleared."
        );
    }


    // =========================================================
    // DEBUG INVENTORY
    // =========================================================

    public void PrintInventory()
    {
        Debug.Log(
            "================================"
        );

        Debug.Log(
            "PLAYER INVENTORY"
        );

        Debug.Log(
            "Items: " +
            items.Count +
            "/" +
            maxItems
        );

        if (items.Count == 0)
        {
            Debug.Log(
                "Inventory is empty."
            );
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                Debug.Log(
                    "[" +
                    i +
                    "] NULL"
                );

                continue;
            }

            Debug.Log(
                "[" +
                i +
                "] " +
                items[i].itemName +
                " | ID: " +
                items[i].itemID
            );
        }

        Debug.Log(
            "================================"
        );
    }
}