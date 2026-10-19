using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [System.Serializable]
    public struct DialogueLine
    {
        [TextArea(2, 4)]
        [Tooltip("Nội dung tiếng Anh")]
        public string englishText;

        [TextArea(2, 4)]
        [Tooltip("Nội dung tiếng Việt")]
        public string vietnameseText;
    }

    [Header("--- Tham Chiếu UI ---")]
    [Tooltip("Bảng Panel chứa khung thoại")]
    public GameObject dialoguePanel;

    [Tooltip("Text hiển thị tiếng Anh")]
    public TextMeshProUGUI englishTextUI;

    [Tooltip("Text hiển thị tiếng Việt")]
    public TextMeshProUGUI vietnameseTextUI;

    [Header("--- Cài Đặt Hiệu Ứng Gõ Chữ ---")]
    [Tooltip("Tốc độ gõ chữ (càng nhỏ gõ càng nhanh)")]
    public float typingSpeed = 0.03f;

    [Header("--- Tùy Chọn Ngôn Ngữ ---")]
    [Tooltip("Tick chọn để hiển thị tiếng Anh (và ẩn tiếng Việt). Bỏ tick để hiển thị tiếng Việt (và ẩn tiếng Anh).")]
    public bool useEnglish = false;

    private DialogueLine[] currentDialogueLines;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private string fullTextToType = "";
    private TextMeshProUGUI activeTextUI;
    private Coroutine typingCoroutine;
    private bool isDialogueActive = false;

    // Tham chiếu đến script di chuyển của người chơi để khóa/mở khóa
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Tự động tìm script PlayerMovement trong Scene
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        if (isDialogueActive)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame ||
                Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                OnDialogueInputPressed();
            }
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        currentDialogueLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        // Khóa di chuyển nhân vật khi bắt đầu thoại
        if (playerMovement != null)
        {
            playerMovement.SetKhoaDiChuyen(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineIndex < currentDialogueLines.Length)
        {
            if (useEnglish)
            {
                if (englishTextUI != null) englishTextUI.gameObject.SetActive(true);
                if (vietnameseTextUI != null) vietnameseTextUI.gameObject.SetActive(false);

                fullTextToType = currentDialogueLines[currentLineIndex].englishText;
                activeTextUI = englishTextUI;
            }
            else
            {
                if (englishTextUI != null) englishTextUI.gameObject.SetActive(false);
                if (vietnameseTextUI != null) vietnameseTextUI.gameObject.SetActive(true);

                fullTextToType = currentDialogueLines[currentLineIndex].vietnameseText;
                activeTextUI = vietnameseTextUI;
            }

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeDialogueRoutine());
        }
        else
        {
            CloseDialogue();
        }
    }

    private IEnumerator TypeDialogueRoutine()
    {
        isTyping = true;
        if (activeTextUI != null)
            activeTextUI.text = "";

        string currentText = "";

        for (int i = 0; i < fullTextToType.Length; i++)
        {
            currentText += fullTextToType[i];
            if (activeTextUI != null)
                activeTextUI.text = currentText;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void OnDialogueInputPressed()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            if (activeTextUI != null)
                activeTextUI.text = fullTextToType;

            isTyping = false;
        }
        else
        {
            currentLineIndex++;
            ShowCurrentLine();
        }
    }

    public void CloseDialogue()
    {
        isDialogueActive = false;

        // Ẩn UI hội thoại
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Thả tự do người chơi (Cho phép di chuyển lại)
        if (playerMovement != null)
        {
            playerMovement.SetKhoaDiChuyen(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}