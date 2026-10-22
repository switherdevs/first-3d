using UnityEngine;
using System.Collections;

public class DrawerInteract : MonoBehaviour
{
    [Header("Vị trí mở")]
    [Tooltip("Ngăn kéo sẽ di chuyển thêm bao nhiêu theo Local Position")]
    public Vector3 openOffset = new Vector3(0f, 0f, 0.4f);

    [Header("Tốc độ")]
    public float moveSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen = false;
    private bool isMoving = false;

    private void Start()
    {
        closedPosition = transform.localPosition;
        openPosition = closedPosition + openOffset;
    }

    public void ToggleDrawer()
    {
        if (isMoving)
            return;

        isOpen = !isOpen;

        StopAllCoroutines();
        StartCoroutine(MoveDrawer());
    }

    private IEnumerator MoveDrawer()
    {
        isMoving = true;

        Vector3 targetPosition = isOpen ? openPosition : closedPosition;

        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.001f)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.localPosition = targetPosition;

        isMoving = false;
    }
}