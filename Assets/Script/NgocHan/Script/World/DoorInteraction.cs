using UnityEngine;
using TMPro;

public class DoorInteraction : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Camera")]
    public Camera playerCamera;

    [Header("Inventory")]
    public PlayerInventory playerInventory;

    [Header("Door")]
    public KeyDoor door;

    [Header("UI")]
    public TextMeshProUGUI interactionText;

    [Header("Settings")]
    public float raycastDistance = 3f;

    private void Update()
    {
        DetectDoor();
    }

    private void DetectDoor()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (
            Physics.Raycast(
                ray,
                out hit,
                raycastDistance
            )
        )
        {
            KeyDoor detectedDoor =
                hit.collider.GetComponentInParent<KeyDoor>();

            if (detectedDoor != null)
            {
                HandleDoor(detectedDoor);
                return;
            }
        }

        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    private void HandleDoor(KeyDoor detectedDoor)
    {
        if (door != detectedDoor)
        {
            door = detectedDoor;
        }

        KeyItem.KeyType requiredKey =
            GetRequiredKeyType(detectedDoor);

        bool hasKey =
            playerInventory != null &&
            playerInventory.HasKey(requiredKey);

        if (hasKey)
        {
            if (interactionText != null)
            {
                interactionText.text =
                    "Press E to open.";
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenDoor(detectedDoor, requiredKey);
            }
        }
        else
        {
            if (interactionText != null)
            {
                interactionText.text =
                    detectedDoor.GetLockedMessage();
            }
        }
    }

    private KeyItem.KeyType GetRequiredKeyType(
        KeyDoor detectedDoor
    )
    {
        if (
            detectedDoor.doorType ==
            KeyDoor.DoorType.RestrictedAreaDoor
        )
        {
            return KeyItem.KeyType.RestrictedAreaKey;
        }

        return KeyItem.KeyType.HouseRoomKey;
    }

    private void OpenDoor(
        KeyDoor detectedDoor,
        KeyItem.KeyType requiredKey
    )
    {
        if (
            playerInventory.UseKey(requiredKey)
        )
        {
            detectedDoor.OpenDoor();

            if (interactionText != null)
            {
                interactionText.text =
                    "Door unlocked.";
            }
        }
    }
}