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
    // TARGET MANH MỐI
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
    // CHỈ LO MANH MỐI + KHOẢNG CÁCH
    // KHÔNG QUYẾT ĐỊNH CÓ BẤM E ĐƯỢC HAY KHÔNG
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
        // =====================================================
        // TỰ TÌM VẬT GẦN PLAYER
        // KHÔNG PHỤ THUỘC targetClue
        // =====================================================

        Map1Interact nearest =
            FindNearestInteractable();


        // Không có vật nào trong 2m
        if (nearest == null)
        {
            ClearCurrentInteraction();
            return;
        }


        // =====================================================
        // LẤY PROMPT CỦA VẬT ĐÓ
        // =====================================================

        PuzzleWorldPrompt prompt =
            FindPrompt(
                nearest.transform
            );


        // Đổi sang vật khác
        if (currentInteractable != nearest)
        {
            if (currentPrompt != null)
            {
                currentPrompt.HidePrompt();
            }


            currentInteractable =
                nearest;


            currentPrompt =
                prompt;
        }
        else
        {
            currentPrompt =
                prompt;
        }


        // =====================================================
        // HIỆN E
        // =====================================================

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
                "[PLAYER] E -> " +
                currentInteractable.gameObject.name
            );


            currentInteractable.Interact();
        }
    }


    // =========================================================
    // TÌM VẬT GẦN NHẤT TRONG 2M
    // =========================================================

    private Map1Interact FindNearestInteractable()
    {
        Collider[] colliders =
            Physics.OverlapSphere(
                transform.position,
                interactionDistance
            );


        Map1Interact nearest = null;

        float nearestDistance =
            float.MaxValue;


        foreach (Collider col in colliders)
        {
            if (col == null)
                continue;


            Map1Interact interactable =
                col.GetComponent<Map1Interact>();


            // Nếu collider nằm ở child
            // thử tìm Map1Interact trên parent
            if (interactable == null)
            {
                interactable =
                    col.GetComponentInParent<Map1Interact>();
            }


            if (interactable == null)
                continue;


            // Không lấy Player
            if (interactable.transform == transform)
                continue;


            float distance =
                Vector3.Distance(
                    transform.position,
                    interactable.transform.position
                );


            if (distance < nearestDistance)
            {
                nearestDistance =
                    distance;

                nearest =
                    interactable;
            }
        }


        return nearest;
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


        PuzzleWorldPrompt prompt =
            target.GetComponent<PuzzleWorldPrompt>();


        if (prompt != null)
            return prompt;


        prompt =
            target.GetComponentInChildren<PuzzleWorldPrompt>(
                true
            );


        return prompt;
    }


    // =========================================================
    // CLEAR CURRENT INTERACTION
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
    // SET TARGET CLUE
    // CHỈ DÙNG CHO MANH MỐI / KHOẢNG CÁCH
    // =========================================================

    public void SetTargetClue(
        Transform newTarget,
        string hint
    )
    {
        targetClue =
            newTarget;


        currentHint =
            hint;


        if (targetClue == null)
        {
            ClearTargetClue();
            return;
        }


        Debug.Log(
            "[PLAYER] Target manh mối mới = " +
            targetClue.name
        );


        UpdateTargetUI();
    }


    // =========================================================
    // CLEAR TARGET CLUE
    // =========================================================

    public void ClearTargetClue()
    {
        targetClue = null;

        currentHint = "";


        if (itemNameText != null)
        {
            itemNameText.text = "";
        }


        if (itemNameGroup != null)
        {
            itemNameGroup.SetActive(false);
        }


        if (distanceText != null)
        {
            distanceText.text = "";
        }


        if (distanceGroup != null)
        {
            distanceGroup.SetActive(false);
        }
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


    // =========================================================
    // DEBUG VẼ BÁN KÍNH 2M
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            interactionDistance
        );
    }
}