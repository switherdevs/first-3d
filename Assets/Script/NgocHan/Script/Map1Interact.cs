using UnityEngine;

public class Map1Interact : MonoBehaviour
{
    public enum PuzzleType
    {
        HuyPhoto,
        Letter,

        Number4,
        Number1,
        Number7,

        Box,
        BoxClue,
        Key,
        Bracelet,

        Exit
    }


    [Header("PUZZLE TYPE")]
    [SerializeField]
    private PuzzleType puzzleType;


    [Header("NỘI DUNG KHI NHẤN R")]
    [SerializeField]
    private string displayName;


    [TextArea(3, 8)]
    [SerializeField]
    private string inspectText;


    // =========================================================
    // INTERACT
    // =========================================================

    public void Interact()
    {
        if (Map1PuzzleManager.Instance == null)
        {
            Debug.LogError(
                "[Map1Interact] Không có Map1PuzzleManager!"
            );

            return;
        }


        bool success = false;


        Debug.Log(
            "[Map1Interact] E -> " +
            gameObject.name +
            " | " +
            puzzleType
        );


        switch (puzzleType)
        {
            // =================================================
            // PAPER
            // =================================================

            case PuzzleType.Letter:

                success =
                    Map1PuzzleManager.Instance
                        .CollectPaper();

                break;


            // =================================================
            // PHOTO
            // =================================================

            case PuzzleType.HuyPhoto:

                success =
                    Map1PuzzleManager.Instance
                        .CollectPhoto();

                break;


            // =================================================
            // 4
            // =================================================

            case PuzzleType.Number4:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber4();

                break;


            // =================================================
            // 1
            // =================================================

            case PuzzleType.Number1:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber1();

                break;


            // =================================================
            // 7
            // =================================================

            case PuzzleType.Number7:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber7();

                break;


            // =================================================
            // BOX
            // =================================================

            case PuzzleType.Box:

                Map1PuzzleManager.Instance
                    .TryOpenBox();

                // Hộp không bỏ vào inventory
                return;


            // =================================================
            // BOX CLUE
            // =================================================

            case PuzzleType.BoxClue:

                success =
                    Map1PuzzleManager.Instance
                        .CollectBoxClue();

                break;


            // =================================================
            // KEY
            // =================================================

            case PuzzleType.Key:

                success =
                    Map1PuzzleManager.Instance
                        .CollectKey();

                break;


            // =================================================
            // BRACELET
            // =================================================

            case PuzzleType.Bracelet:

                success =
                    Map1PuzzleManager.Instance
                        .CollectBracelet();

                break;


            // =================================================
            // EXIT
            // =================================================

            case PuzzleType.Exit:

                Debug.Log(
                    "[MAP1] EXIT"
                );

                return;
        }


        // =====================================================
        // KHÔNG ĐÚNG THỨ TỰ
        // =====================================================

        if (!success)
        {
            Debug.Log(
                "[Map1Interact] Chưa thể nhặt/tương tác: " +
                gameObject.name
            );

            return;
        }


        // =====================================================
        // INVENTORY PICKUP
        // =====================================================

        InventoryPickup pickup =
            GetComponent<InventoryPickup>();


        if (pickup != null)
        {
            bool added =
                pickup.Collect();


            Debug.Log(
                "[Map1Interact] InventoryPickup " +
                gameObject.name +
                " = " +
                added
            );
        }
        else
        {
            // OldScale, OilLamp, MirrorShard
            // không phải item inventory
            Debug.Log(
                "[Map1Interact] " +
                gameObject.name +
                " không có InventoryPickup."
            );
        }
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