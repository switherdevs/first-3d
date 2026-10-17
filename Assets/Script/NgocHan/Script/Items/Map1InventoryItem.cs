using UnityEngine;

public class Map1InventoryItem : MonoBehaviour
{
    [Header("HOTBAR")]
    [SerializeField]
    private HotbarItemDisplay hotbarItemDisplay;


    [Header("VẬT PHẨM")]
    [SerializeField]
    private SearchItem searchItem;


    [Header("SAU KHI NHẶT")]
    [SerializeField]
    private bool hideAfterPickup = true;


    private bool collected = false;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        // Tự tìm SearchItem
        if (searchItem == null)
        {
            searchItem =
                GetComponent<SearchItem>();
        }


        // Nếu nằm ở child
        if (searchItem == null)
        {
            searchItem =
                GetComponentInChildren<SearchItem>(true);
        }
    }


    // =========================================================
    // COLLECT
    // =========================================================

    public void Collect()
    {
        if (collected)
        {
            Debug.Log(
                "[Map1InventoryItem] Item đã nhặt rồi: " +
                gameObject.name
            );

            return;
        }


        // =====================================================
        // SEARCH ITEM
        // =====================================================

        if (searchItem == null)
        {
            Debug.LogError(
                "[Map1InventoryItem] " +
                gameObject.name +
                " chưa có SearchItem!"
            );

            return;
        }


        // =====================================================
        // HOTBAR DISPLAY
        // =====================================================

        if (hotbarItemDisplay == null)
        {
            hotbarItemDisplay =
                FindFirstObjectByType<HotbarItemDisplay>();
        }


        if (hotbarItemDisplay == null)
        {
            Debug.LogError(
                "[Map1InventoryItem] Không tìm thấy HotbarItemDisplay!"
            );

            return;
        }


        // =====================================================
        // ADD ITEM VÀO HOTBAR TRƯỚC
        // =====================================================

        hotbarItemDisplay.AddItem(
            searchItem
        );


        collected = true;


        Debug.Log(
            "[Map1InventoryItem] Đã thêm vào kho: " +
            gameObject.name
        );


        // =====================================================
        // SAU ĐÓ MỚI ẨN OBJECT
        // =====================================================

        if (hideAfterPickup)
        {
            gameObject.SetActive(false);
        }
    }


    public bool IsCollected()
    {
        return collected;
    }
}