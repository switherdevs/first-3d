using UnityEngine;

public class PuzzleItem : MonoBehaviour
{
    public enum ItemID
    {
        Key,
        Phone,
        Photo,
        Radio
    }

    [Header("Item Identity")]
    public ItemID itemID;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    private Rigidbody rb;
    private Collider[] itemColliders;

    [HideInInspector]
    public bool isHeld = false;

    private void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        rb = GetComponent<Rigidbody>();

        itemColliders = GetComponentsInChildren<Collider>();
    }

    // Player picks up the item
    public void PickUp(Transform holdPoint)
    {
        if (holdPoint == null)
            return;

        isHeld = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Disable colliders while holding
        SetColliders(false);

        transform.SetParent(holdPoint);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Correct item is permanently placed
    public void LockItem(Transform placementPoint)
    {
        isHeld = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        SetColliders(false);

        transform.SetParent(placementPoint);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Wrong item returns to its original position
    public void ReturnToOriginalPosition()
    {
        isHeld = false;

        transform.SetParent(originalParent);

        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (rb != null)
        {
            rb.isKinematic = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        SetColliders(true);

        gameObject.SetActive(true);
    }

    private void SetColliders(bool state)
    {
        foreach (Collider col in itemColliders)
        {
            if (col != null)
                col.enabled = state;
        }
    }
}