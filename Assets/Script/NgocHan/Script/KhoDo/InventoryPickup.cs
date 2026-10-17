using UnityEngine;

public class InventoryPickup : MonoBehaviour
{
    [Header("HOTBAR")]
    [SerializeField]
    private HotbarItemDisplay hotbarItemDisplay;

    [Header("ITEM")]
    [SerializeField]
    private SearchItem searchItem;

    [Header("SAU KHI NHẶT")]
    [SerializeField]
    private bool hideAfterPickup = true;

    private bool collected = false;


    private void Awake()
    {
        if (searchItem == null)
        {
            searchItem = GetComponent<SearchItem>();
        }

        if (searchItem == null)
        {
            searchItem =
                GetComponentInChildren<SearchItem>(true);
        }
    }


    public bool Collect()
    {
        if (collected)
            return false;


        if (searchItem == null)
        {
            Debug.LogError(
                "[InventoryPickup] " +
                gameObject.name +
                " chưa có SearchItem!"
            );

            return false;
        }


        if (hotbarItemDisplay == null)
        {
            hotbarItemDisplay =
                FindFirstObjectByType<HotbarItemDisplay>();
        }


        if (hotbarItemDisplay == null)
        {
            Debug.LogError(
                "[InventoryPickup] Không tìm thấy HotbarItemDisplay!"
            );

            return false;
        }


        bool added =
            hotbarItemDisplay.AddItem(searchItem);


        if (!added)
        {
            return false;
        }


        collected = true;


        Debug.Log(
            "[InventoryPickup] Đã nhặt: " +
            searchItem.itemName
        );


        if (hideAfterPickup)
        {
            gameObject.SetActive(false);
        }


        return true;
    }


    public bool IsCollected()
    {
        return collected;
    }
}