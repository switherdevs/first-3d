using UnityEngine;
using TMPro;

public class LightResourcePickup : MonoBehaviour
{
    public enum PickupType
    {
        Flashlight,
        Battery,
        OilLamp,
        Oil
    }

    // =========================================================
    // PICKUP
    // =========================================================

    [Header("PICKUP TYPE")]
    [SerializeField]
    private PickupType pickupType;

    [Header("RESOURCE")]
    [SerializeField]
    private float amount = 25f;

    // =========================================================
    // INTERACTION
    // =========================================================

    [Header("INTERACTION")]
    [SerializeField]
    private float interactDistance = 2f;

    [SerializeField]
    private KeyCode interactKey = KeyCode.E;

    [SerializeField]
    private GameObject worldPrompt;

    [SerializeField]
    private TMP_Text promptText;

    // =========================================================
    // PLAYER
    // =========================================================

    private Transform player;

    private LightResourceManager resources;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError(
                "[PICKUP] Không tìm thấy Player có Tag Player!"
            );
        }

        resources =
            LightResourceManager.Instance;

        if (resources == null)
        {
            resources =
                FindFirstObjectByType<LightResourceManager>();
        }

        if (worldPrompt != null)
        {
            worldPrompt.SetActive(false);
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                player.position,
                transform.position
            );

        // =============================================
        // TRONG 2M
        // =============================================

        if (distance <= interactDistance)
        {
            ShowPrompt();

            if (Input.GetKeyDown(interactKey))
            {
                TryPickup();
            }
        }

        // =============================================
        // NGOÀI 2M
        // =============================================

        else
        {
            HidePrompt();
        }
    }

    // =========================================================
    // PROMPT
    // =========================================================

    private void ShowPrompt()
    {
        if (worldPrompt != null)
        {
            worldPrompt.SetActive(true);
        }

        if (promptText != null)
        {
            promptText.text =
                GetPromptText();
        }
    }

    private void HidePrompt()
    {
        if (worldPrompt != null)
        {
            worldPrompt.SetActive(false);
        }
    }

    private string GetPromptText()
    {
        switch (pickupType)
        {
            case PickupType.Flashlight:
                return "[E] Nhặt đèn pin";

            case PickupType.Battery:
                return "[E] Nhặt pin";

            case PickupType.OilLamp:
                return "[E] Nhặt đèn dầu";

            case PickupType.Oil:
                return "[E] Nhặt dầu";
        }

        return "[E] Nhặt";
    }

    // =========================================================
    // PICKUP
    // =========================================================

    private void TryPickup()
    {
        if (resources == null)
        {
            Debug.LogError(
                "[PICKUP] Không tìm thấy LightResourceManager!"
            );

            return;
        }

        bool success = false;

        switch (pickupType)
        {
            // =============================================
            // ĐÈN PIN
            // =============================================

            case PickupType.Flashlight:

                if (!resources.HasFlashlight)
                {
                    resources.UnlockFlashlight(amount);

                    success = true;
                }

                break;

            // =============================================
            // PIN
            // =============================================

            case PickupType.Battery:

                success =
                    resources.AddBattery(amount);

                break;

            // =============================================
            // ĐÈN DẦU
            // =============================================

            case PickupType.OilLamp:

                if (!resources.HasOilLamp)
                {
                    resources.UnlockOilLamp(amount);

                    success = true;
                }

                break;

            // =============================================
            // DẦU
            // =============================================

            case PickupType.Oil:

                success =
                    resources.AddOil(amount);

                break;
        }

        if (success)
        {
            HidePrompt();

            Destroy(gameObject);
        }
    }

    // =========================================================
    // GIZMO
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            interactDistance
        );
    }
}