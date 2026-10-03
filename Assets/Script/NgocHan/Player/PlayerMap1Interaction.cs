using UnityEngine;
using TMPro;

public class PlayerMap1Interaction : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;

    [Header("Interaction")]
    public float interactionDistance = 3f;

    [Header("UI")]
    public GameObject interactionIcon;
    public TMP_Text itemInfoText;

    private Map1Interact currentInteract;

    private void Start()
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(false);

        if (itemInfoText != null)
            itemInfoText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckInteraction();
    }

    private void CheckInteraction()
    {
        currentInteract = null;

        // Mỗi frame đều ẩn trước
        if (interactionIcon != null)
            interactionIcon.SetActive(false);

        if (itemInfoText != null)
            itemInfoText.gameObject.SetActive(false);

        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        Debug.DrawRay(
            ray.origin,
            ray.direction * interactionDistance,
            Color.green
        );

        if (Physics.Raycast(
            ray,
            out hit,
            interactionDistance
        ))
        {
            Map1Interact interact =
                hit.collider.GetComponent<Map1Interact>();

            // Nếu collider nằm ở object con
            if (interact == null)
            {
                interact =
                    hit.collider.GetComponentInParent<Map1Interact>();
            }

            if (interact != null)
            {
                currentInteract = interact;

                // -------------------------
                // HIỆN ICON E
                // -------------------------

                if (interactionIcon != null)
                {
                    interactionIcon.SetActive(true);
                }

                // -------------------------
                // KHOẢNG CÁCH + TỌA ĐỘ
                // -------------------------

                if (itemInfoText != null)
                {
                    Vector3 itemPosition =
                        interact.transform.position;

                    float distance =
                        Vector3.Distance(
                            playerCamera.transform.position,
                            itemPosition
                        );

                    itemInfoText.gameObject.SetActive(true);

                    itemInfoText.text =
                        "Manh moi: " +
                        interact.gameObject.name +
                        "\n" +
                        "Khoang cach: " +
                        distance.ToString("F1") +
                        " m" +
                        "\n" +
                        "Toa do: X " +
                        itemPosition.x.ToString("F1") +
                        " | Y " +
                        itemPosition.y.ToString("F1") +
                        " | Z " +
                        itemPosition.z.ToString("F1");
                }

                // -------------------------
                // BẤM E
                // -------------------------

                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentInteract.Interact();
                }
            }
        }
    }
}