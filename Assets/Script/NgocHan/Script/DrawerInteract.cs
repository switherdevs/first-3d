using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawerInteract : MonoBehaviour
{
    private static readonly List<DrawerInteract> allDrawers = new List<DrawerInteract>();

    [Header("PLAYER")]
    [Tooltip("Có thể để trống, code sẽ tự tìm object có Tag Player")]
    public Transform player;

    [Header("KHOẢNG CÁCH TƯƠNG TÁC")]
    public float interactionDistance = 1.5f;

    [Header("VỊ TRÍ MỞ")]
    public Vector3 openOffset = new Vector3(0f, 0f, 0.4f);

    [Header("TỐC ĐỘ")]
    public float moveSpeed = 2f;

    [Header("UI HIỆN PHÍM E")]
    public GameObject ePrompt;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen = false;

    private void OnEnable()
    {
        if (!allDrawers.Contains(this))
            allDrawers.Add(this);
    }

    private void OnDisable()
    {
        allDrawers.Remove(this);
    }

    private void Start()
    {
        closedPosition = transform.localPosition;
        openPosition = closedPosition + openOffset;

        // Tự tìm Player nếu chưa kéo vào Inspector
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (ePrompt != null)
            ePrompt.SetActive(false);
    }

    private void Update()
    {
        // Di chuyển ngăn kéo
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );

        if (player == null)
            return;

        DrawerInteract nearestDrawer = GetNearestDrawer();

        bool canInteract =
            nearestDrawer == this &&
            Vector3.Distance(player.position, transform.position) <= interactionDistance;

        // Chỉ ngăn gần nhất hiện E
        if (ePrompt != null)
            ePrompt.SetActive(canInteract);

        // Bấm E mở / đóng
        if (canInteract &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleDrawer();
        }
    }

    private DrawerInteract GetNearestDrawer()
    {
        DrawerInteract nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (DrawerInteract drawer in allDrawers)
        {
            if (drawer == null || drawer.player == null)
                continue;

            float distance = Vector3.Distance(
                drawer.player.position,
                drawer.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = drawer;
            }
        }

        return nearest;
    }

    public void ToggleDrawer()
    {
        isOpen = !isOpen;
    }
}