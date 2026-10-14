using UnityEngine;
using UnityEngine.InputSystem;

public class Map2PickupInteract : MonoBehaviour
{
    [Header("Thông tin vật phẩm")]
    [SerializeField] private string displayName = "Vật phẩm";

    [Header("Khoảng cách tương tác")]
    [SerializeField] private float interactionDistance = 2f;

    [Header("Nhặt vật phẩm")]
    [SerializeField] private InventoryPickup inventoryPickup;

    [Header("E Interact")]
    [SerializeField] private GameObject worldInteractPrompt;

    private Transform player;
    private Collider itemCollider;
    private bool collected = false;

    private void Awake()
    {
        // Tự lấy InventoryPickup
        if (inventoryPickup == null)
            inventoryPickup = GetComponent<InventoryPickup>();

        // Tự lấy Collider
        itemCollider = GetComponent<Collider>();

        // Tự tìm WorldInteractPrompt nếu chưa kéo
        if (worldInteractPrompt == null)
        {
            Transform prompt = transform.Find("WorldInteractPrompt");

            if (prompt != null)
                worldInteractPrompt = prompt.gameObject;
        }

        // Luôn ẩn E khi bắt đầu
        SetPrompt(false);
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (collected)
            return;

        // Nếu chưa tìm thấy Player thì tìm lại
        if (player == null)
        {
            FindPlayer();
            SetPrompt(false);
            return;
        }

        float distance = GetDistanceToPlayer();

        bool playerNear = distance <= interactionDistance;

        // Gần thì hiện E, xa thì ẩn
        SetPrompt(playerNear);

        // Nhấn E để nhặt
        if (playerNear &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void FindPlayer()
    {
        // Cách 1: tìm bằng Tag Player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            return;
        }

        // Cách 2: nếu chưa Tag Player thì tìm CharacterController
        CharacterController controller =
            FindFirstObjectByType<CharacterController>();

        if (controller != null)
        {
            player = controller.transform;
        }
    }

    private float GetDistanceToPlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        // Tính khoảng cách tới MÉP Collider
        if (itemCollider != null)
        {
            Vector3 closestPoint =
                itemCollider.ClosestPoint(player.position);

            return Vector3.Distance(
                player.position,
                closestPoint
            );
        }

        // Nếu không có Collider thì tính tới tâm object
        return Vector3.Distance(
            player.position,
            transform.position
        );
    }

    public void Interact()
    {
        if (collected)
            return;

        if (inventoryPickup == null)
        {
            Debug.LogError(
                "[Map2] " + gameObject.name +
                " không có InventoryPickup!"
            );

            return;
        }

        bool success = inventoryPickup.Collect();

        if (!success)
            return;

        collected = true;

        SetPrompt(false);

        Debug.Log(
            "[Map2] Đã nhặt: " + displayName
        );
    }

    public void SetPrompt(bool show)
    {
        if (worldInteractPrompt != null)
            worldInteractPrompt.SetActive(show);
    }

    public Collider GetItemCollider()
    {
        return itemCollider;
    }

    private void OnDisable()
    {
        SetPrompt(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            interactionDistance
        );
    }
}