using UnityEngine;
using TMPro;

public class PlayerMap1Interaction : MonoBehaviour
{
    // =========================================================
    // INTERACTION
    // =========================================================

    [Header("INTERACTION")]
    [SerializeField] private float interactionDistance = 2f;


    // =========================================================
    // TARGET
    // =========================================================

    private Transform targetClue;
    private string currentHint = "";


    // =========================================================
    // MANH MỐI UI
    // =========================================================

    [Header("MANH MỐI UI")]
    [SerializeField] private GameObject itemNameGroup;
    [SerializeField] private TMP_Text itemNameText;


    // =========================================================
    // DISTANCE UI
    // =========================================================

    [Header("DISTANCE UI")]
    [SerializeField] private GameObject distanceGroup;
    [SerializeField] private TMP_Text distanceText;


    // =========================================================
    // CURRENT INTERACTION
    // =========================================================

    private Map1Interact currentInteractable;
    private PuzzleWorldPrompt currentPrompt;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (itemNameGroup != null)
            itemNameGroup.SetActive(false);

        if (distanceGroup != null)
            distanceGroup.SetActive(false);

        HideAllPrompts();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        UpdateTargetUI();
        UpdateInteraction();
    }


    // =========================================================
    // TARGET UI
    // =========================================================

    private void UpdateTargetUI()
    {
        if (targetClue == null)
        {
            if (itemNameGroup != null)
                itemNameGroup.SetActive(false);

            if (distanceGroup != null)
                distanceGroup.SetActive(false);

            return;
        }


        // MANH MỐI
        if (itemNameGroup != null)
            itemNameGroup.SetActive(true);


        if (itemNameText != null)
        {
            itemNameText.text =
                "Manh mối: " + currentHint;
        }


        // KHOẢNG CÁCH
        float distance =
            Vector3.Distance(
                transform.position,
                targetClue.position
            );


        if (distanceGroup != null)
            distanceGroup.SetActive(true);


        if (distanceText != null)
        {
            distanceText.text =
                "Khoảng cách: " +
                distance.ToString("0.0") +
                " m";
        }
    }


    // =========================================================
    // INTERACTION
    // =========================================================

    private void UpdateInteraction()
    {
        if (targetClue == null)
        {
            ClearCurrentInteraction();
            return;
        }


        float distance =
            Vector3.Distance(
                transform.position,
                targetClue.position
            );


        // XA HƠN 2M
        if (distance > interactionDistance)
        {
            ClearCurrentInteraction();
            return;
        }


        // =====================================================
        // TÌM INTERACT CỦA ĐÚNG TARGET
        // =====================================================

        Map1Interact interactable =
            FindInteractable(targetClue);


        PuzzleWorldPrompt prompt =
            FindPrompt(targetClue);


        if (interactable == null)
        {
            Debug.LogWarning(
                "[PlayerMap1Interaction] Target " +
                targetClue.name +
                " không có Map1Interact."
            );

            ClearCurrentInteraction();
            return;
        }


        // Nếu đổi target prompt
        if (currentPrompt != prompt)
        {
            if (currentPrompt != null)
                currentPrompt.HidePrompt();

            currentPrompt = prompt;
        }


        currentInteractable = interactable;


        // Hiện E
        if (currentPrompt != null)
        {
            currentPrompt.ShowPrompt();
        }


        // =====================================================
        // NHẤN E
        // =====================================================

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(
                "[PLAYER] E -> Target = " +
                targetClue.name +
                " | Interact = " +
                currentInteractable.gameObject.name
            );


            currentInteractable.Interact();
        }
    }


    // =========================================================
    // FIND INTERACTABLE
    // =========================================================

    private Map1Interact FindInteractable(
        Transform target
    )
    {
        if (target == null)
            return null;


        // 1. Ưu tiên chính target
        Map1Interact interactable =
            target.GetComponent<Map1Interact>();


        if (interactable != null)
            return interactable;


        // 2. Tìm trong child của target
        interactable =
            target.GetComponentInChildren<Map1Interact>(
                true
            );


        if (interactable != null)
            return interactable;


        // KHÔNG tìm parent.
        // Tránh BoxClue / Key lấy nhầm Map1Interact của LockBox.
        return null;
    }


    // =========================================================
    // FIND PROMPT
    // =========================================================

    private PuzzleWorldPrompt FindPrompt(
        Transform target
    )
    {
        if (target == null)
            return null;


        // Component trên target
        PuzzleWorldPrompt prompt =
            target.GetComponent<PuzzleWorldPrompt>();


        if (prompt != null)
            return prompt;


        // Component trong child
        prompt =
            target.GetComponentInChildren<PuzzleWorldPrompt>(
                true
            );


        return prompt;
    }


    // =========================================================
    // CLEAR CURRENT
    // =========================================================

    private void ClearCurrentInteraction()
    {
        if (currentPrompt != null)
        {
            currentPrompt.HidePrompt();
        }


        currentPrompt = null;
        currentInteractable = null;
    }


    // =========================================================
    // HIDE ALL PROMPTS
    // =========================================================

    private void HideAllPrompts()
    {
        PuzzleWorldPrompt[] prompts =
            FindObjectsByType<PuzzleWorldPrompt>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );


        foreach (PuzzleWorldPrompt prompt in prompts)
        {
            if (prompt != null)
            {
                prompt.HidePrompt();
            }
        }
    }


    // =========================================================
    // SET TARGET
    // =========================================================

    public void SetTargetClue(
        Transform newTarget,
        string hint
    )
    {
        ClearCurrentInteraction();
        HideAllPrompts();


        targetClue = newTarget;
        currentHint = hint;


        if (targetClue == null)
        {
            ClearTargetClue();
            return;
        }


        Debug.Log(
            "[PLAYER] Target mới = " +
            targetClue.name
        );


        UpdateTargetUI();
    }


    // =========================================================
    // CLEAR TARGET
    // =========================================================

    public void ClearTargetClue()
    {
        ClearCurrentInteraction();
        HideAllPrompts();


        targetClue = null;
        currentHint = "";


        if (itemNameText != null)
            itemNameText.text = "";


        if (itemNameGroup != null)
            itemNameGroup.SetActive(false);


        if (distanceText != null)
            distanceText.text = "";


        if (distanceGroup != null)
            distanceGroup.SetActive(false);
    }


    // =========================================================
    // GETTERS
    // =========================================================

    public Transform GetTargetClue()
    {
        return targetClue;
    }


    public float GetDistanceToTarget()
    {
        if (targetClue == null)
            return -1f;


        return Vector3.Distance(
            transform.position,
            targetClue.position
        );
    }
}