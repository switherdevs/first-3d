using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleSlot : MonoBehaviour
{
    [Header("Required Item")]
    public PuzzleItem.ItemID requiredItemID;

    [Header("Placement Point")]
    public Transform placementPoint;

    [Header("Wrong Item")]
    public float wrongItemReturnDelay = 1f;

    [Header("Puzzle Event")]
    public UnityEvent onPuzzleSolved;

    private bool isCompleted = false;

    public void TryPlaceItem(PuzzleItem item)
    {
        if (item == null)
            return;

        if (isCompleted)
        {
            item.ReturnToOriginalPosition();
            return;
        }

        if (placementPoint == null)
        {
            Debug.LogWarning("Placement Point is missing.");
            item.ReturnToOriginalPosition();
            return;
        }

        // Temporarily place the item
        item.transform.SetParent(placementPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        // Check ID
        if (item.itemID == requiredItemID)
        {
            CorrectItem(item);
        }
        else
        {
            WrongItem(item);
        }
    }

    private void CorrectItem(PuzzleItem item)
    {
        Debug.Log(
            "CORRECT ITEM: " + item.itemID
        );

        isCompleted = true;

        item.LockItem(placementPoint);

        // Activate door, mechanism, etc.
        onPuzzleSolved?.Invoke();
    }

    private void WrongItem(PuzzleItem item)
    {
        Debug.LogWarning(
            "WRONG ITEM: " + item.itemID +
            " | Required: " + requiredItemID
        );

        // IMPORTANT:
        // The puzzle does NOT activate.

        StartCoroutine(
            ReturnWrongItem(item)
        );
    }

    private IEnumerator ReturnWrongItem(
        PuzzleItem item
    )
    {
        yield return new WaitForSeconds(
            wrongItemReturnDelay
        );

        if (item != null)
        {
            item.ReturnToOriginalPosition();
        }
    }
}