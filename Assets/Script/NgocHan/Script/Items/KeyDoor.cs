using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    public enum DoorType
    {
        RestrictedAreaDoor,
        HouseRoomDoor
    }

    [Header("Door Information")]
    public DoorType doorType;

    [Header("Door Settings")]
    public Transform doorObject;

    public float openAngle = 90f;
    public float openSpeed = 3f;

    [Header("Interaction")]
    public string lockedMessage = "This door is locked.";

    private bool isOpen = false;
    private bool isOpening = false;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    private void Start()
    {
        if (doorObject == null)
        {
            doorObject = transform;
        }

        closedRotation = doorObject.localRotation;

        openedRotation =
            closedRotation *
            Quaternion.Euler(0f, openAngle, 0f);
    }

    private void Update()
    {
        if (!isOpening)
            return;

        doorObject.localRotation = Quaternion.Slerp(
            doorObject.localRotation,
            openedRotation,
            Time.deltaTime * openSpeed
        );

        if (
            Quaternion.Angle(
                doorObject.localRotation,
                openedRotation
            ) < 0.5f
        )
        {
            doorObject.localRotation = openedRotation;
            isOpening = false;
        }
    }

    public bool CanOpen(KeyItem key)
    {
        if (key == null)
            return false;

        if (
            doorType ==
            DoorType.RestrictedAreaDoor
        )
        {
            return key.IsRestrictedAreaKey();
        }

        if (
            doorType ==
            DoorType.HouseRoomDoor
        )
        {
            return key.IsHouseRoomKey();
        }

        return false;
    }

    public void OpenDoor()
    {
        if (isOpen)
            return;

        isOpen = true;
        isOpening = true;
    }

    public string GetLockedMessage()
    {
        return lockedMessage;
    }
}