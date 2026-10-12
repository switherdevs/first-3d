using UnityEngine;

public class WorldPromptBillboard : MonoBehaviour
{
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (playerCamera == null)
            return;

        transform.rotation = Quaternion.LookRotation(
            transform.position - playerCamera.transform.position
        );
    }
}