using UnityEngine;

public class Map1Interact : MonoBehaviour
{
    public enum PuzzleType
    {
        // =========================
        // PUZZLE CŨ
        // =========================

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

        // =========================
        // ITEM ÁNH SÁNG
        // NHẶT TỰ DO
        // =========================

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
    // INTERACTION
    // =========================================================

    [Header("INTERACTION")]

    [Tooltip("Khoảng cách tối đa để nhặt/tương tác")]
    [SerializeField]
    private float interactDistance = 3f;

    [Tooltip("Kéo model vật phẩm vào đây để đo khoảng cách chính xác")]
    [SerializeField]
    private Transform interactionPoint;

    [Tooltip("Kéo WorldPrompt có hình E Interact vào đây")]
    [SerializeField]
    private GameObject worldPrompt;

    [SerializeField]
    private KeyCode interactKey = KeyCode.E;

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
    // LIGHT ITEM SETTINGS
    // =========================================================

    [Header("LIGHT ITEM SETTINGS")]

    [Tooltip("Pin ban đầu khi nhặt đèn pin")]
    [SerializeField]
    private float flashlightStartBattery = 25f;

    [Tooltip("Dầu ban đầu khi nhặt đèn dầu")]
    [SerializeField]
    private float oilLampStartOil = 25f;

    // =========================================================
    // PRIVATE
    // =========================================================

    private Transform player;

    private bool collected = false;

    // Chống trường hợp script cũ + script này
    // cùng gọi Interact trong một frame.
    private int lastInteractFrame = -1;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError(
                "[Map1Interact] Không tìm thấy Player có Tag Player!"
            );
        }

        // E phải ẩn khi bắt đầu
        if (worldPrompt != null)
        {
            worldPrompt.SetActive(false);
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (collected)
            return;

        if (player == null)
            return;

        bool inRange =
            IsPlayerInRange();

        // =========================
        // HIỆN / ẨN E
        // =========================

        if (worldPrompt != null)
        {
            worldPrompt.SetActive(inRange);
        }

        // =========================
        // E TƯƠNG TÁC
        // =========================

        if (inRange &&
            Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    // =========================================================
    // DISTANCE
    // =========================================================

    private bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        Vector3 itemPosition;

        // Nếu có Interaction Point
        // thì đo đúng tới model
        if (interactionPoint != null)
        {
            itemPosition =
                interactionPoint.position;
        }
        else
        {
            itemPosition =
                transform.position;
        }

        float distance =
            Vector3.Distance(
                player.position,
                itemPosition
            );

        return distance <= interactDistance;
    }

    // =========================================================
    // INTERACT
    // =========================================================

    public void Interact()
    {
        if (collected)
            return;

        // Chống gọi 2 lần trong cùng 1 frame
        if (lastInteractFrame ==
            Time.frameCount)
        {
            return;
        }

        lastInteractFrame =
            Time.frameCount;

        // Không cho tương tác ngoài 3m
        if (!IsPlayerInRange())
        {
            return;
        }

        Debug.Log(
            "[Map1Interact] E -> " +
            gameObject.name +
            " | " +
            puzzleType
        );

        // =====================================================
        // 4 ITEM MỚI
        // NHẶT BẤT CỨ LÚC NÀO
        // =====================================================

        if (IsFreeLightItem())
        {
            CollectFreeLightItem();

            return;
        }

        // =====================================================
        // PUZZLE CŨ
        // =====================================================

        if (Map1PuzzleManager.Instance == null)
        {
            Debug.LogError(
                "[Map1Interact] Không có Map1PuzzleManager!"
            );

            return;
        }

        bool success = false;

        // =====================================================
        // PUZZLE
        // =====================================================

        switch (puzzleType)
        {
            // =================================================
            // ẢNH HUY
            // =================================================

            case PuzzleType.HuyPhoto:

                success =
                    Map1PuzzleManager.Instance
                        .CollectPhoto();

                break;

            // =================================================
            // THƯ
            // =================================================

            case PuzzleType.Letter:

                success =
                    Map1PuzzleManager.Instance
                        .CollectPaper();

                break;

            // =================================================
            // CÂN -> 4
            // =================================================

            case PuzzleType.Number4:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber4();

                break;

            // =================================================
            // ĐÈN DẦU PUZZLE -> 1
            // =================================================

            case PuzzleType.Number1:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber1();

                break;

            // =================================================
            // GƯƠNG -> 7
            // =================================================

            case PuzzleType.Number7:

                success =
                    Map1PuzzleManager.Instance
                        .CollectNumber7();

                break;

            // =================================================
            // HỘP
            // =================================================

            case PuzzleType.Box:

                if (
                    Map1PuzzleManager.Instance
                        .GetCurrentStep()
                    !=
                    Map1PuzzleManager.PuzzleStep.Box
                )
                {
                    Map1PuzzleManager.Instance
                        .ShowInteractionResult(false);

                    return;
                }

                Map1PuzzleManager.Instance
                    .TryOpenBox();

                return;

            // =================================================
            // THƯ TRONG HỘP
            // =================================================

            case PuzzleType.BoxClue:

                success =
                    Map1PuzzleManager.Instance
                        .CollectBoxClue();

                break;

            // =================================================
            // CHÌA KHÓA
            // =================================================

            case PuzzleType.Key:

                success =
                    Map1PuzzleManager.Instance
                        .CollectKey();

                break;

            // =================================================
            // VÒNG TAY
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
        // SAI THỨ TỰ
        // =====================================================

        if (!success)
        {
            Debug.Log(
                "[Map1Interact] SAI THỨ TỰ -> " +
                gameObject.name
            );

            Map1PuzzleManager.Instance
                .ShowInteractionResult(false);

            // KHÔNG NHẶT
            // KHÔNG VÀO KHO
            return;
        }

        // =====================================================
        // ĐÚNG THỨ TỰ
        // =====================================================

        Debug.Log(
            "[Map1Interact] ĐÚNG -> " +
            gameObject.name
        );

        Map1PuzzleManager.Instance
            .ShowInteractionResult(true);

        // Đúng mới được thêm vào kho
        AddToInventory();
    }

    // =========================================================
    // CHECK 4 ITEM TỰ DO
    // =========================================================

    private bool IsFreeLightItem()
    {
        return
            puzzleType ==
                PuzzleType.FlashlightPickup ||

            puzzleType ==
                PuzzleType.BatteryPickup ||

            puzzleType ==
                PuzzleType.OilLampPickup ||

            puzzleType ==
                PuzzleType.OilPickup;
    }

    // =========================================================
    // COLLECT 4 ITEM MỚI
    // =========================================================

    private void CollectFreeLightItem()
    {
        // =====================================================
        // INVENTORY CŨ
        // =====================================================

        InventoryPickup pickup =
            GetComponent<InventoryPickup>();

        if (pickup == null)
        {
            Debug.LogError(
                "[Map1Interact] " +
                gameObject.name +
                " chưa có InventoryPickup!"
            );

            return;
        }

        bool added =
            pickup.Collect();

        // Không thêm được vào kho
        if (!added)
        {
            Debug.Log(
                "[Map1Interact] Không thêm được " +
                gameObject.name +
                " vào kho."
            );

            return;
        }

        collected = true;

        if (worldPrompt != null)
        {
            worldPrompt.SetActive(false);
        }

        Debug.Log(
            "[Map1Interact] ĐÃ NHẶT -> " +
            gameObject.name
        );

        // =====================================================
        // LIGHT RESOURCE
        // =====================================================

        LightResourceManager resources =
            LightResourceManager.Instance;

        // Pin và dầu vẫn được cho vào kho
        // dù manager chưa tồn tại.
        if (resources == null)
        {
            Debug.LogWarning(
                "[Map1Interact] Không tìm thấy LightResourceManager!"
            );

            return;
        }

        switch (puzzleType)
        {
            // =================================================
            // ĐÈN PIN
            // =================================================

            case PuzzleType.FlashlightPickup:

                if (!resources.HasFlashlight)
                {
                    resources.UnlockFlashlight(
                        flashlightStartBattery
                    );
                }

                Debug.Log(
                    "[LIGHT] Đã mở khóa đèn pin."
                );

                break;

            // =================================================
            // ĐÈN DẦU
            // =================================================

            case PuzzleType.OilLampPickup:

                if (!resources.HasOilLamp)
                {
                    resources.UnlockOilLamp(
                        oilLampStartOil
                    );
                }

                Debug.Log(
                    "[LIGHT] Đã mở khóa đèn dầu."
                );

                break;

            // =================================================
            // CỤC PIN
            // =================================================

            case PuzzleType.BatteryPickup:

                // Không nạp ngay.
                // Chỉ nằm trong kho.

                Debug.Log(
                    "[LIGHT] Cục pin đã vào kho."
                );

                break;

            // =================================================
            // CAN DẦU
            // =================================================

            case PuzzleType.OilPickup:

                // Không nạp ngay.
                // Chỉ nằm trong kho.

                Debug.Log(
                    "[LIGHT] Can dầu đã vào kho."
                );

                break;
        }
    }

    // =========================================================
    // ADD PUZZLE ITEM TO INVENTORY
    // =========================================================

    private void AddToInventory()
    {
        InventoryPickup pickup =
            GetComponent<InventoryPickup>();

        // Vật này không cần vào kho
        if (pickup == null)
        {
            Debug.Log(
                "[Map1Interact] " +
                gameObject.name +
                " không có InventoryPickup."
            );

            return;
        }

        bool added =
            pickup.Collect();

        if (added)
        {
            collected = true;

            if (worldPrompt != null)
            {
                worldPrompt.SetActive(false);
            }
        }

        Debug.Log(
            "[Map1Interact] InventoryPickup " +
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

    public float GetInteractDistance()
    {
        return interactDistance;
    }
}