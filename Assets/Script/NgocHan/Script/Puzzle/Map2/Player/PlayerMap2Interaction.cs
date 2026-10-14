using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMap2Interaction : MonoBehaviour
{
    [Header("Khoảng cách tương tác")]
    [SerializeField] private float interactionDistance = 2f;

    private Map2PickupInteract currentItem;

    private void Start()
    {
        HideAllPrompts();
    }

    private void Update()
    {
        FindNearestItem();

        if (currentItem != null &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Map2PickupInteract itemToPickup = currentItem;

            currentItem.SetPrompt(false);
            currentItem = null;

            itemToPickup.Interact();
        }
    }

    private void FindNearestItem()
    {
        Map2PickupInteract[] items =
            FindObjectsByType<Map2PickupInteract>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );

        Map2PickupInteract nearestItem = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Map2PickupInteract item in items)
        {
            Collider itemCollider = item.GetItemCollider();

            float distance;

            if (itemCollider != null)
            {
                Vector3 closestPoint =
                    itemCollider.ClosestPoint(transform.position);

                distance = Vector3.Distance(
                    transform.position,
                    closestPoint
                );
            }
            else
            {
                distance = Vector3.Distance(
                    transform.position,
                    item.transform.position
                );
            }

            if (distance <= interactionDistance &&
                distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestItem = item;
            }
        }

        // Không thay đổi vật phẩm
        if (nearestItem == currentItem)
            return;

        // Tắt E item cũ
        if (currentItem != null)
            currentItem.SetPrompt(false);

        currentItem = nearestItem;

        // Bật E item mới
        if (currentItem != null)
            currentItem.SetPrompt(true);
    }

    private void HideAllPrompts()
    {
        Map2PickupInteract[] items =
            FindObjectsByType<Map2PickupInteract>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );

        foreach (Map2PickupInteract item in items)
            item.SetPrompt(false);
    }

    private void OnDisable()
    {
        if (currentItem != null)
            currentItem.SetPrompt(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            interactionDistance
        );
    }
}