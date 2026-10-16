using UnityEngine;

public class Map1Interact : MonoBehaviour
{
    public enum PuzzleType
    {
        // PUZZLE
        HuyPhoto,
        Letter,

        Number4,
        Number1,
        Number7,

        Box,
        BoxClue,
        Key,
        Bracelet,

        Exit,

        // ITEM ÁNH SÁNG
        FlashlightPickup,
        BatteryPickup,
        OilLampPickup,
        OilPickup
    }


    // =========================================================
    // TYPE
    // =========================================================

    [Header("PUZZLE / ITEM TYPE")]
    [SerializeField]
    private PuzzleType puzzleType;


    // =========================================================
    // INSPECT R
    // =========================================================

    [Header("NỘI DUNG KHI NHẤN R")]
    [SerializeField]
    private string displayName;

    [TextArea(3, 8)]
    [SerializeField]
    private string inspectText;


    // =========================================================
    // LIGHT
    // =========================================================

    [Header("LIGHT ITEM SETTINGS")]

    [SerializeField]
    private float flashlightStartBattery = 25f;

    [SerializeField]
    private float oilLampStartOil = 25f;


    // =========================================================
    // INTERACT
    // =========================================================

    public void Interact()
    {
        Debug.Log(
            "[Map1Interact] E -> " +
            gameObject.name +
            " | " +
            puzzleType
        );


        // =====================================================
        // ITEM ÁNH SÁNG
        // KHÔNG THUỘC PUZZLE
        // =====================================================

        if (IsFreeLightItem())
        {
            CollectFreeLightItem();
            return;
        }


        // =====================================================
        // PUZZLE
        // =====================================================

        if (Map1PuzzleManager.Instance == null)
        {
            Debug.LogError(
                "[Map1Interact] Không tìm thấy Map1PuzzleManager!"
            );

            return;
        }


        bool success = false;


        switch (puzzleType)
        {
            // ẢNH HUY
            case PuzzleType.HuyPhoto:

                success =
                    Map1PuzzleManager.Instance
                        .CollectPhoto();

                break;


            // MẢNH GIẤY
            case PuzzleType.Letter:

                success =
                    Map1PuzzleManager.Instance
                        .CollectPaper();

                break;


            // CÂN -> 4
            case PuzzleType.Number4:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber4();

                break;


            // ĐÈN DẦU -> 1
            case PuzzleType.Number1:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber1();

                break;


            // GƯƠNG -> 7
            case PuzzleType.Number7:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber7();

                break;


            // HỘP KHÓA
            case PuzzleType.Box:

                Map1PuzzleManager.Instance
                    .TryOpenBox();

                return;


            // MANH MỐI TRONG HỘP
            case PuzzleType.BoxClue:

                success =
                    Map1PuzzleManager.Instance
                        .CollectBoxClue();

                break;


            // CHÌA KHÓA
            case PuzzleType.Key:

                success =
                    Map1PuzzleManager.Instance
                        .CollectKey();

                break;


            // VÒNG TAY
            case PuzzleType.Bracelet:

                success =
                    Map1PuzzleManager.Instance
                        .CollectBracelet();

                break;


            // EXIT
            case PuzzleType.Exit:

                Debug.Log("[MAP1] EXIT");

                return;
        }


        // =====================================================
        // SAI
        // =====================================================

        if (!success)
        {
            Debug.Log(
                "[Map1Interact] SAI -> " +
                gameObject.name
            );


            Map1PuzzleManager.Instance
                .ShowInteractionResult(false);


            return;
        }


        // =====================================================
        // ĐÚNG
        // =====================================================

        Debug.Log(
            "[Map1Interact] ĐÚNG -> " +
            gameObject.name
        );


        Map1PuzzleManager.Instance
            .ShowInteractionResult(true);


        // Đúng mới vào kho
        AddToInventory();
    }


    // =========================================================
    // CHECK LIGHT ITEM
    // =========================================================

    private bool IsFreeLightItem()
    {
        return
            puzzleType == PuzzleType.FlashlightPickup ||
            puzzleType == PuzzleType.BatteryPickup ||
            puzzleType == PuzzleType.OilLampPickup ||
            puzzleType == PuzzleType.OilPickup;
    }


    // =========================================================
    // COLLECT LIGHT ITEM
    // =========================================================

    private void CollectFreeLightItem()
    {
        InventoryPickup pickup =
            GetComponent<InventoryPickup>();


        if (pickup == null)
        {
            Debug.LogError(
                "[Map1Interact] " +
                gameObject.name +
                " thiếu InventoryPickup!"
            );

            return;
        }


        // =====================================================
        // FLASHLIGHT
        // =====================================================

        if (puzzleType ==
            PuzzleType.FlashlightPickup)
        {
            bool added =
                pickup.Collect();


            if (!added)
                return;


            LightResourceManager resources =
                LightResourceManager.Instance;


            if (resources != null &&
                !resources.HasFlashlight)
            {
                resources.UnlockFlashlight(
                    flashlightStartBattery
                );
            }


            Debug.Log(
                "[LIGHT] Đã nhặt đèn pin."
            );


            return;
        }


        // =====================================================
        // OIL LAMP
        // =====================================================

        if (puzzleType ==
            PuzzleType.OilLampPickup)
        {
            bool added =
                pickup.Collect();


            if (!added)
                return;


            LightResourceManager resources =
                LightResourceManager.Instance;


            if (resources != null &&
                !resources.HasOilLamp)
            {
                resources.UnlockOilLamp(
                    oilLampStartOil
                );
            }


            Debug.Log(
                "[LIGHT] Đã nhặt đèn dầu."
            );


            return;
        }


        // =====================================================
        // BATTERY
        // =====================================================

        if (puzzleType ==
            PuzzleType.BatteryPickup)
        {
            if (LightStackManager.Instance == null)
            {
                Debug.LogError(
                    "[BATTERY] Không có LightStackManager!"
                );

                return;
            }


            LightStackManager stack =
                LightStackManager.Instance;


            // PIN ĐẦU TIÊN
            if (stack.BatteryCount <= 0)
            {
                bool added =
                    pickup.Collect();


                if (!added)
                    return;


                stack.AddBattery();


                Debug.Log(
                    "[BATTERY] x" +
                    stack.BatteryCount
                );


                return;
            }


            // PIN THỨ 2 TRỞ ĐI
            stack.AddBattery();


            Debug.Log(
                "[BATTERY] x" +
                stack.BatteryCount
            );


            gameObject.SetActive(false);


            return;
        }


        // =====================================================
        // OIL
        // =====================================================

        if (puzzleType ==
            PuzzleType.OilPickup)
        {
            if (LightStackManager.Instance == null)
            {
                Debug.LogError(
                    "[OIL] Không có LightStackManager!"
                );

                return;
            }


            LightStackManager stack =
                LightStackManager.Instance;


            // CAN ĐẦU TIÊN
            if (stack.OilCount <= 0)
            {
                bool added =
                    pickup.Collect();


                if (!added)
                    return;


                stack.AddOil();


                Debug.Log(
                    "[OIL] x" +
                    stack.OilCount
                );


                return;
            }


            // CAN THỨ 2 TRỞ ĐI
            stack.AddOil();


            Debug.Log(
                "[OIL] x" +
                stack.OilCount
            );


            gameObject.SetActive(false);


            return;
        }
    }


    // =========================================================
    // ADD PUZZLE ITEM TO INVENTORY
    // =========================================================

    private void AddToInventory()
    {
        InventoryPickup pickup =
            GetComponent<InventoryPickup>();


        if (pickup == null)
        {
            Debug.LogWarning(
                "[Map1Interact] " +
                gameObject.name +
                " không có InventoryPickup."
            );

            return;
        }


        bool added =
            pickup.Collect();


        Debug.Log(
            "[INVENTORY] " +
            gameObject.name +
            " = " +
            added
        );
    }


    // =========================================================
    // GETTERS
    // =========================================================

    public PuzzleType GetPuzzleType()
    {
        return puzzleType;
    }


    public string GetDisplayName()
    {
        return displayName;
    }


    public string GetInspectText()
    {
        return inspectText;
    }
}