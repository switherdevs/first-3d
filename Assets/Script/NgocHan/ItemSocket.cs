using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ItemSocket : MonoBehaviour
{
    [Header("Required Item")]
    public SearchItemType requiredItemType;

    [Header("Snap Settings")]
    public Transform snapPoint;

    [Header("UI")]
    public TextMeshProUGUI interactionText;

    [Header("Events")]
    public UnityEvent onCorrectItemPlaced;

    private bool isActivated = false;

    private float messageTimer = 0f;

    private void Update()
    {
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;

            if (messageTimer <= 0f)
            {
                messageTimer = 0f;

                if (interactionText != null)
                    interactionText.text = "";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated)
            return;

        SearchItem item =
            other.GetComponentInParent<SearchItem>();

        if (item == null)
            return;

        CheckItem(item);
    }

    private void CheckItem(SearchItem item)
    {
        if (item.itemType == requiredItemType)
        {
            PlaceCorrectItem(item);
        }
        else
        {
            WrongItem(item);
        }
    }

    private void PlaceCorrectItem(SearchItem item)
    {
        if (snapPoint == null)
        {
            Debug.LogError(
                "Snap Point is not assigned!"
            );

            return;
        }

        isActivated = true;

        Transform itemTransform =
            item.transform;

        // Detach from player or another parent
        itemTransform.SetParent(null);

        // Snap item into position
        itemTransform.position =
            snapPoint.position;

        itemTransform.rotation =
            snapPoint.rotation;

        // Lock Rigidbody
        Rigidbody rb =
            item.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb =
                item.GetComponentInChildren<Rigidbody>();
        }

        if (rb != null)
        {
            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;

            rb.isKinematic = true;

            rb.useGravity = false;
        }

        // Disable colliders so the item cannot
        // be moved after placement
        Collider[] colliders =
            item.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Parent the item to the socket
        itemTransform.SetParent(
            snapPoint
        );

        if (interactionText != null)
        {
            interactionText.text =
                "Correct item placed!";

            messageTimer = 3f;
        }

        Debug.Log(
            "Correct item placed: " +
            item.itemName
        );

        // Activate mechanism
        onCorrectItemPlaced.Invoke();
    }

    private void WrongItem(SearchItem item)
    {
        Debug.LogWarning(
            "Wrong item. Required: " +
            requiredItemType +
            " | Detected: " +
            item.itemType
        );

        if (interactionText != null)
        {
            interactionText.text =
                "Wrong item: " +
                item.itemName;

            messageTimer = 3f;
        }
    }
}