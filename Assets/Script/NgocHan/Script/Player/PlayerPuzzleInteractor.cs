using UnityEngine;

public class PlayerPuzzleInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform holdPoint;

    [Header("Interaction Settings")]
    public float pickupDistance = 3f;
    public float placementDistance = 3f;

    [Header("Current Item")]
    public PuzzleItem heldItem;

    private void Update()
    {
        // E = Pick up item
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldItem == null)
            {
                TryPickUpItem();
            }
        }

        // F = Place item into puzzle
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldItem != null)
            {
                TryPlaceItem();
            }
        }
    }

    private void TryPickUpItem()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            PuzzleItem item =
                hit.collider.GetComponentInParent<PuzzleItem>();

            if (item == null)
                return;

            if (item.isHeld)
                return;

            heldItem = item;

            heldItem.PickUp(holdPoint);

            Debug.Log(
                "Picked up: " + heldItem.itemID
            );
        }
    }

    private void TryPlaceItem()
    {
        if (playerCamera == null)
            return;

        if (heldItem == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, placementDistance))
        {
            PuzzleSlot slot =
                hit.collider.GetComponentInParent<PuzzleSlot>();

            if (slot == null)
            {
                Debug.Log("You are not looking at a puzzle slot.");
                return;
            }

            PuzzleItem itemToPlace = heldItem;

            // Player no longer holds the item
            heldItem = null;

            // Let the puzzle validate the item ID
            slot.TryPlaceItem(itemToPlace);
        }
    }
}