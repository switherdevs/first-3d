using UnityEngine;
using UnityEngine.Events;

public class UniversalInteractable : MonoBehaviour
{
    public enum InteractionType
    {
        InspectOnly,
        Pickup,
        InspectAndPickup
    }


    // =========================================================
    // THÔNG TIN VẬT PHẨM
    // =========================================================

    [Header("THÔNG TIN VẬT PHẨM")]

    [SerializeField] private string itemID;

    [SerializeField] private string itemName;

    [TextArea(2, 8)]
    [SerializeField] private string itemHint;

    [SerializeField] private Sprite itemIcon;


    // =========================================================
    // MANH MỐI
    // =========================================================

    [Header("MANH MỐI")]

    [Tooltip("Bật nếu vật này phải lưu vào bảng Manh Mối")]
    [SerializeField] private bool saveAsClue = false;


    // =========================================================
    // KIỂU TƯƠNG TÁC
    // =========================================================

    [Header("KIỂU TƯƠNG TÁC")]

    [SerializeField]
    private InteractionType interactionType =
        InteractionType.InspectOnly;

    [Tooltip("Bật nếu muốn tương tác nhiều lần")]
    [SerializeField] private bool repeatable = false;

    [Tooltip("Nhặt xong thì vật biến mất")]
    [SerializeField] private bool hideAfterPickup = true;


    // =========================================================
    // PUZZLE
    // =========================================================

    [Header("PUZZLE - KHÔNG BẮT BUỘC")]

    [SerializeField]
    private IslandPuzzleManager puzzleManager;

    [Tooltip("Thứ tự: 0, 1, 2...")]
    [SerializeField]
    private int stepIndex = 0;


    // =========================================================
    // UI PHÍM E
    // =========================================================

    [Header("UI TƯƠNG TÁC")]

    [SerializeField]
    private GameObject worldPrompt;


    // =========================================================
    // EVENT
    // =========================================================

    [Header("EVENT")]

    public UnityEvent onInteract;

    public UnityEvent onPickup;


    // =========================================================
    // RUNTIME
    // =========================================================

    private bool hasInteracted = false;


    // =========================================================
    // INTERACT
    // =========================================================

    public void Interact()
    {
        // Không cho tương tác lại
        if (hasInteracted && !repeatable)
            return;


        // =====================================================
        // KIỂM TRA THỨ TỰ
        // =====================================================

        if (puzzleManager != null)
        {
            if (!puzzleManager.CanInteract(stepIndex))
            {
                puzzleManager.ShowMessage(
                    "Có vẻ mình chưa cần thứ này..."
                );

                return;
            }
        }


        // =====================================================
        // XỬ LÝ THEO LOẠI
        // =====================================================

        switch (interactionType)
        {
            case InteractionType.InspectOnly:

                ShowHint();

                CompletePuzzleStep();

                break;


            case InteractionType.Pickup:

                PickupItem();

                CompletePuzzleStep();

                break;


            case InteractionType.InspectAndPickup:

                ShowHint();

                PickupItem();

                CompletePuzzleStep();

                break;
        }


        hasInteracted = true;

        onInteract?.Invoke();
    }


    // =========================================================
    // HIỆN MANH MỐI
    // =========================================================

    private void ShowHint()
    {
        // =========================================
        // VẬT NÀY LÀ MANH MỐI
        // =========================================

        if (saveAsClue)
        {
            if (ClueJournalManager.Instance != null)
            {
                ClueJournalManager.Instance.AddClue(
                    itemID,
                    itemName,
                    itemHint,
                    itemIcon
                );
            }
            else
            {
                Debug.LogError(
                    "Không tìm thấy ClueJournalManager trong Scene!"
                );
            }

            return;
        }


        // =========================================
        // KHÔNG LƯU VÀO SỔ MANH MỐI
        // =========================================

        Debug.Log(
            itemName + ": " + itemHint
        );
    }


    // =========================================================
    // NHẶT
    // =========================================================

    private void PickupItem()
    {
        Debug.Log(
            "Đã nhặt: " + itemName
        );


        onPickup?.Invoke();


        if (hideAfterPickup)
        {
            gameObject.SetActive(false);
        }
    }


    // =========================================================
    // HOÀN THÀNH STEP
    // =========================================================

    private void CompletePuzzleStep()
    {
        if (puzzleManager == null)
            return;


        puzzleManager.CompleteStep(
            stepIndex,
            itemName,
            ""
        );
    }


    // =========================================================
    // UI PHÍM E
    // =========================================================

    public void SetPrompt(bool show)
    {
        if (worldPrompt != null)
        {
            worldPrompt.SetActive(show);
        }
    }


    // =========================================================
    // COLLIDER
    // =========================================================

    public Collider GetItemCollider()
    {
        Collider col =
            GetComponent<Collider>();


        if (col != null)
            return col;


        return GetComponentInChildren<Collider>();
    }
}