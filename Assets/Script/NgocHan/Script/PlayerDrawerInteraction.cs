using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDrawerInteraction : MonoBehaviour
{
    [Header("Camera người chơi")]
    public Camera playerCamera;

    [Header("Khoảng cách tương tác")]
    public float interactionDistance = 2.5f;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void Interact()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            DrawerInteract drawer = hit.collider.GetComponent<DrawerInteract>();

            if (drawer == null)
            {
                drawer = hit.collider.GetComponentInParent<DrawerInteract>();
            }

            if (drawer != null)
            {
                drawer.ToggleDrawer();
            }
        }
    }
}