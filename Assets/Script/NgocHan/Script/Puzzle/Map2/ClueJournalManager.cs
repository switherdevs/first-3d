using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class ClueJournalManager : MonoBehaviour
{
    public static ClueJournalManager Instance { get; private set; }


    [System.Serializable]
    public class ClueData
    {
        public string id;
        public string itemName;

        [TextArea(2, 8)]
        public string hint;

        public Sprite icon;
    }


    // =========================================================
    // UI
    // =========================================================

    [Header("UI MANH MỐI")]
    [SerializeField] private GameObject cluePanel;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private TMP_Text pageText;

    [SerializeField] private Image itemIcon;


    [Header("BUTTON")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;


    // =========================================================
    // DATA
    // =========================================================

    private readonly List<ClueData> clues =
        new List<ClueData>();

    private int currentIndex = 0;
    private bool isOpen = false;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
        }


        if (previousButton != null)
        {
            previousButton.onClick.AddListener(
                PreviousClue
            );
        }


        if (nextButton != null)
        {
            nextButton.onClick.AddListener(
                NextClue
            );
        }


        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                Close
            );
        }
    }


    // =========================================================
    // INPUT
    // =========================================================

    private void Update()
    {
        if (Keyboard.current == null)
            return;


        // TAB
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                OpenJournal();
            }
        }


        if (!isOpen)
            return;


        // Trái
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            PreviousClue();
        }


        // Phải
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            NextClue();
        }


        // ESC
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
        }
    }


    // =========================================================
    // THÊM MANH MỐI
    // =========================================================

    public void AddClue(
        string id,
        string itemName,
        string hint,
        Sprite icon
    )
    {
        int index =
            clues.FindIndex(
                x => x.id == id
            );


        // Đã có → mở lại
        if (index >= 0)
        {
            currentIndex = index;

            Open();

            return;
        }


        ClueData newClue =
            new ClueData
            {
                id = id,
                itemName = itemName,
                hint = hint,
                icon = icon
            };


        clues.Add(newClue);

        currentIndex =
            clues.Count - 1;


        Open();
    }


    // =========================================================
    // TAB MỞ SỔ
    // =========================================================

    public void OpenJournal()
    {
        if (clues.Count == 0)
        {
            Debug.Log(
                "Chưa có manh mối nào."
            );

            return;
        }


        currentIndex =
            Mathf.Clamp(
                currentIndex,
                0,
                clues.Count - 1
            );


        Open();
    }


    private void Open()
    {
        if (cluePanel == null)
            return;


        cluePanel.SetActive(true);

        isOpen = true;

        ShowCurrentClue();
    }


    // =========================================================
    // ĐÓNG
    // =========================================================

    public void Close()
    {
        isOpen = false;


        if (cluePanel != null)
        {
            cluePanel.SetActive(false);
        }
    }


    // =========================================================
    // HIỂN THỊ
    // =========================================================

    private void ShowCurrentClue()
    {
        if (clues.Count == 0)
            return;


        ClueData clue =
            clues[currentIndex];


        if (titleText != null)
        {
            titleText.text =
                "MANH MỐI";
        }


        if (itemNameText != null)
        {
            itemNameText.text =
                clue.itemName;
        }


        if (hintText != null)
        {
            hintText.text =
                clue.hint;
        }


        if (itemIcon != null)
        {
            itemIcon.sprite =
                clue.icon;

            itemIcon.gameObject.SetActive(
                clue.icon != null
            );
        }


        if (pageText != null)
        {
            pageText.text =
                (currentIndex + 1)
                + " / "
                + clues.Count;
        }


        UpdateButtons();
    }


    // =========================================================
    // TRƯỚC
    // =========================================================

    public void PreviousClue()
    {
        if (clues.Count <= 1)
            return;


        currentIndex--;


        if (currentIndex < 0)
        {
            currentIndex =
                clues.Count - 1;
        }


        ShowCurrentClue();
    }


    // =========================================================
    // SAU
    // =========================================================

    public void NextClue()
    {
        if (clues.Count <= 1)
            return;


        currentIndex++;


        if (currentIndex >= clues.Count)
        {
            currentIndex = 0;
        }


        ShowCurrentClue();
    }


    private void UpdateButtons()
    {
        bool multiple =
            clues.Count > 1;


        if (previousButton != null)
        {
            previousButton.interactable =
                multiple;
        }


        if (nextButton != null)
        {
            nextButton.interactable =
                multiple;
        }
    }
}