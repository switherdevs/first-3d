using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ItemSocket : MonoBehaviour
{
    [Header("Required Item")]
    [Tooltip("Example: KEY_01")]
    public string requiredItemID;

    [Header("Snap Settings")]
    public Transform snapPoint;

    [Header("Player")]
    public PlayerInventory playerInventory;

    [Header("Interaction")]
    public float interactionDistance = 2.5f;

    public KeyCode interactionKey = KeyCode.E;

    [Header("UI")]
    public TextMeshProUGUI interactionText;

    [Header("Events")]
    public UnityEvent onCorrectItemPlaced;

    private bool isActivated = false;

    private float messageTimer = 0f;

    private void Start()
    {
        // Automatically find PlayerInventory
        // if it has not been assigned.
        if (playerInventory == null)
        {
            playerInventory =
                FindFirstObjectByType<PlayerInventory>();
        }

        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    private void Update()
    {
        UpdateMessageTimer();

        if (isActivated)
        {
            return;
        }

        CheckInteraction();
    }

    // =========================================================
    // CHECK PLAYER INTERACTION
    // =========================================================

    private void CheckInteraction()
    {
        if (playerInventory == null)
        {
            return;
        }

        if (playerInventory.transform == null)
        {
            return;
        }

        float distance = Vector3.Distance(
            playerInventory.transform.position,
            transform.position
        );

        if (distance > interactionDistance)
        {
            return;
        }

        if (Input.GetKeyDown(interactionKey))
        {
            TryUseRequiredItem();
        }
    }

    // =========================================================
    // USE REQUIRED ITEM
    // =========================================================

    public void TryUseRequiredItem()
    {
        if (isActivated)
        {
            return;
        }

        if (playerInventory == null)
        {
            ShowMessage(
                "Player inventory is missing."
            );

            return;
        }

        if (string.IsNullOrEmpty(requiredItemID))
        {
            Debug.LogError(
                "ItemSocket: Required Item ID is empty."
            );

            ShowMessage(
                "Required item is not configured."
            );

            return;
        }

        // Check if player has the required item.
        if (!playerInventory.HasItem(
            requiredItemID))
        {
            ShowMessage(
                "You need: " +
                requiredItemID
            );

            Debug.Log(
                "Player does not have required item: " +
                requiredItemID
            );

            return;
        }

        SearchItem item =
            playerInventory.GetItem(
                requiredItemID
            );

        if (item == null)
        {
            ShowMessage(
                "Item could not be found."
            );

            return;
        }

        // =====================================================
        // PLACE ITEM
        // =====================================================

        if (snapPoint != null)
        {
            item.transform.position =
                snapPoint.position;

            item.transform.rotation =
                snapPoint.rotation;
        }
        else
        {
            item.transform.position =
                transform.position;
        }

        item.gameObject.SetActive(true);

        // Turn off glow.
        item.SetGlow(false);

        // =====================================================
        // REMOVE FROM INVENTORY
        // =====================================================

        playerInventory.RemoveItem(item);

        // =====================================================
        // ACTIVATE SOCKET
        // =====================================================

        isActivated = true;

        ShowMessage(
            "Correct item placed!"
        );

        Debug.Log(
            "Correct item placed in socket: " +
            requiredItemID
        );

        // Call UnityEvent.
        if (onCorrectItemPlaced != null)
        {
            onCorrectItemPlaced.Invoke();
        }
    }

    // =========================================================
    // UI MESSAGE
    // =========================================================

    private void ShowMessage(
        string message)
    {
        if (interactionText == null)
        {
            return;
        }

        interactionText.text = message;

        messageTimer = 3f;
    }

    // =========================================================
    // MESSAGE TIMER
    // =========================================================

    private void UpdateMessageTimer()
    {
        if (messageTimer <= 0f)
        {
            return;
        }

        messageTimer -= Time.deltaTime;

        if (messageTimer <= 0f)
        {
            messageTimer = 0f;

            if (interactionText != null)
            {
                interactionText.text = "";
            }
        }
    }

    // =========================================================
    // RESET SOCKET
    // =========================================================

    public void ResetSocket()
    {
        isActivated = false;

        messageTimer = 0f;

        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    // =========================================================
    // STATUS
    // =========================================================

    public bool IsActivated()
    {
        return isActivated;
    }
}