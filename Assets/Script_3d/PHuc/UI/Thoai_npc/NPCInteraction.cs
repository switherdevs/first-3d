using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
    [Header("--- Dữ Liệu Thoại Riêng Của NPC ---")]
    [Tooltip("Mảng các câu thoại song ngữ (Anh - Việt) của riêng NPC này")]
    public DialogueManager.DialogueLine[] npcDialogueLines;

    [Header("--- Cấu Hình Bán Kính Tương Tác ---")]
    [Tooltip("Bán kính (mét) để người chơi có thể tương tác với NPC")]
    public float interactionRadius = 3.0f;

    [Tooltip("Transform của Player (Nếu để trống script sẽ tự tìm theo tag Player)")]
    public Transform playerTransform;

    private bool isPlayerInRange = false;

    private void Start()
    {
        // Tự động tìm Player nếu chưa gán thủ công
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Tính khoảng cách từ NPC đến Player theo thời gian thực
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Kiểm tra xem Player có nằm trong bán kính tương tác không
        if (distanceToPlayer <= interactionRadius)
        {
            isPlayerInRange = true;

            // Nhấn phím 'E' để bật thoại khi đứng trong bán kính
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                TriggerDialogue();
            }
        }
        else
        {
            isPlayerInRange = false;
        }
    }

    private void TriggerDialogue()
    {
        if (DialogueManager.Instance != null && npcDialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(npcDialogueLines);
        }
    }

    // Vẽ vòng tròn bán kính tương tác trong cửa sổ Scene để dễ quan sát
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}