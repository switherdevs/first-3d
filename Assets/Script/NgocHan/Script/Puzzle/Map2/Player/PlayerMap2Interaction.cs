using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMap2Interaction : MonoBehaviour
{
    [Header("Khoảng cách tương tác")]
    [SerializeField] private float interactionDistance = 2f;

    // Vật phẩm hệ thống cũ
    private Map2PickupInteract currentPickupItem;

    // Vật phẩm hệ thống Universal mới
    private UniversalInteractable currentUniversalItem;


    private void Start()
    {
        HideAllPrompts();
    }


    private void Update()
    {
        FindNearestItem();

        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.eKey.wasPressedThisFrame)
            return;


        // =====================================================
        // UNIVERSAL INTERACTABLE
        // =====================================================

        if (currentUniversalItem != null)
        {
            UniversalInteractable item = currentUniversalItem;

            item.SetPrompt(false);

            currentUniversalItem = null;

            item.Interact();

            return;
        }


        // =====================================================
        // MAP 2 PICKUP CŨ
        // =====================================================

        if (currentPickupItem != null)
        {
            Map2PickupInteract item = currentPickupItem;

            item.SetPrompt(false);

            currentPickupItem = null;

            item.Interact();
        }
    }


    // =========================================================
    // TÌM OBJECT GẦN NHẤT
    // =========================================================

    private void FindNearestItem()
    {
        Map2PickupInteract nearestPickup = null;
        UniversalInteractable nearestUniversal = null;

        float nearestDistance = Mathf.Infinity;


        // =====================================================
        // TÌM UNIVERSAL INTERACTABLE
        // =====================================================

        UniversalInteractable[] universalItems =
            FindObjectsByType<UniversalInteractable>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );


        foreach (UniversalInteractable item in universalItems)
        {
            float distance = GetDistance(
                item.transform,
                item.GetItemCollider()
            );


            if (distance <= interactionDistance &&
                distance < nearestDistance)
            {
                nearestDistance = distance;

                nearestUniversal = item;
                nearestPickup = null;
            }
        }


        // =====================================================
        // TÌM MAP2 PICKUP CŨ
        // =====================================================

        Map2PickupInteract[] pickupItems =
            FindObjectsByType<Map2PickupInteract>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );


        foreach (Map2PickupInteract item in pickupItems)
        {
            float distance = GetDistance(
                item.transform,
                item.GetItemCollider()
            );


            if (distance <= interactionDistance &&
                distance < nearestDistance)
            {
                nearestDistance = distance;

                nearestPickup = item;
                nearestUniversal = null;
            }
        }


        // =====================================================
        // KHÔNG THAY ĐỔI TARGET
        // =====================================================

        if (nearestPickup == currentPickupItem &&
            nearestUniversal == currentUniversalItem)
        {
            return;
        }


        // =====================================================
        // TẮT PROMPT CŨ
        // =====================================================

        if (currentPickupItem != null)
            currentPickupItem.SetPrompt(false);

        if (currentUniversalItem != null)
            currentUniversalItem.SetPrompt(false);


        // =====================================================
        // ĐỔI TARGET
        // =====================================================

        currentPickupItem = nearestPickup;
        currentUniversalItem = nearestUniversal;


        // =====================================================
        // BẬT PROMPT MỚI
        // =====================================================

        if (currentPickupItem != null)
            currentPickupItem.SetPrompt(true);

        if (currentUniversalItem != null)
            currentUniversalItem.SetPrompt(true);
    }


    // =========================================================
    // TÍNH KHOẢNG CÁCH
    // =========================================================

    private float GetDistance(
        Transform itemTransform,
        Collider itemCollider
    )
    {
        if (itemCollider != null)
        {
            Vector3 closestPoint =
                itemCollider.ClosestPoint(transform.position);

            return Vector3.Distance(
                transform.position,
                closestPoint
            );
        }


        return Vector3.Distance(
            transform.position,
            itemTransform.position
        );
    }


    // =========================================================
    // TẮT TẤT CẢ PROMPT
    // =========================================================

    private void HideAllPrompts()
    {
        Map2PickupInteract[] pickupItems =
            FindObjectsByType<Map2PickupInteract>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );


        foreach (Map2PickupInteract item in pickupItems)
        {
            item.SetPrompt(false);
        }


        UniversalInteractable[] universalItems =
            FindObjectsByType<UniversalInteractable>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
            );


        foreach (UniversalInteractable item in universalItems)
        {
            item.SetPrompt(false);
        }
    }


    private void OnDisable()
    {
        if (currentPickupItem != null)
            currentPickupItem.SetPrompt(false);

        if (currentUniversalItem != null)
            currentUniversalItem.SetPrompt(false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            interactionDistance
        );
    }
}