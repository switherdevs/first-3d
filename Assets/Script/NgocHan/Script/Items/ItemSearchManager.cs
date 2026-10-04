using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ItemSearchManager : MonoBehaviour
{
    // =====================================================
    // PLAYER
    // =====================================================

    [Header("Player")]
    public Transform player;

    // =====================================================
    // ITEMS
    // =====================================================

    [Header("Mission Items")]
    public SearchItem[] items;

    // =====================================================
    // RANDOM SPAWN
    // =====================================================

    [Header("Random Item Spawn")]
    public bool randomizeItemPositions = false;

    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 50f;

    // =====================================================
    // INVENTORY
    // =====================================================

    [Header("Inventory Display")]
    public HotbarItemDisplay hotbarItemDisplay;

    // =====================================================
    // UI
    // =====================================================

    [Header("User Interface")]
    public TextMeshProUGUI coordinateText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI interactionText;

    // =====================================================
    // E ICON
    // =====================================================

    [Header("Interaction Icon")]
    public Image eIcon;

    // =====================================================
    // INTERACTION
    // =====================================================

    [Header("Interaction Settings")]
    public float interactionDistance = 2f;

    public float messageDuration = 3f;

    // =====================================================
    // PRIVATE
    // =====================================================

    private List<SearchItem> missionList =
        new List<SearchItem>();

    private int currentMissionIndex = 0;

    private SearchItem currentItem;
    private SearchItem nearbyItem;

    private string temporaryMessage = "";

    private float messageTimer = 0f;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        ClearInteractionText();

        HideEIcon();

        TurnOffAllGlow();

        if (randomizeItemPositions)
        {
            RandomizeItemPositions();
        }

        CreateRandomMissionList();

        StartNextMission();
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        UpdateMessageTimer();

        if (currentItem == null)
        {
            nearbyItem = null;

            HideEIcon();

            UpdateInteractionUI();

            return;
        }

        UpdateTargetInformation();

        FindNearbyItem();

        HandleInteraction();

        UpdateInteractionUI();
    }

    // =====================================================
    // RANDOM POSITIONS
    // =====================================================

    private void RandomizeItemPositions()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "Player is not assigned."
            );

            return;
        }

        if (items == null)
            return;

        minSpawnDistance =
            Mathf.Max(
                0f,
                minSpawnDistance
            );

        maxSpawnDistance =
            Mathf.Clamp(
                maxSpawnDistance,
                minSpawnDistance,
                50f
            );

        foreach (SearchItem item in items)
        {
            if (item == null)
                continue;

            if (item.isCollected)
                continue;

            float angle =
                Random.Range(
                    0f,
                    360f
                );

            float radians =
                angle *
                Mathf.Deg2Rad;

            float distance =
                Random.Range(
                    minSpawnDistance,
                    maxSpawnDistance
                );

            Vector3 offset =
                new Vector3(
                    Mathf.Cos(radians) *
                    distance,

                    0f,

                    Mathf.Sin(radians) *
                    distance
                );

            Vector3 position =
                player.position +
                offset;

            position.y =
                player.position.y;

            item.transform.position =
                position;
        }
    }

    // =====================================================
    // CREATE MISSIONS
    // =====================================================

    private void CreateRandomMissionList()
    {
        missionList.Clear();

        if (items == null)
            return;

        foreach (SearchItem item in items)
        {
            if (item == null)
                continue;

            if (item.isCollected)
                continue;

            missionList.Add(item);
        }

        // Shuffle
        for (
            int i = 0;
            i < missionList.Count;
            i++
        )
        {
            int randomIndex =
                Random.Range(
                    i,
                    missionList.Count
                );

            SearchItem temp =
                missionList[i];

            missionList[i] =
                missionList[
                    randomIndex
                ];

            missionList[
                randomIndex
            ] = temp;
        }

        currentMissionIndex = 0;
    }

    // =====================================================
    // NEXT MISSION
    // =====================================================

    private void StartNextMission()
    {
        nearbyItem = null;

        temporaryMessage = "";

        messageTimer = 0f;

        HideEIcon();

        ClearInteractionText();

        if (
            currentMissionIndex >=
            missionList.Count
        )
        {
            CompleteAllMissions();

            return;
        }

        currentItem =
            missionList[
                currentMissionIndex
            ];

        if (currentItem == null)
        {
            currentMissionIndex++;

            StartNextMission();

            return;
        }

        TurnOffAllGlow();

        currentItem.SetGlow(true);

        UpdateMissionHint();

        Debug.Log(
            "NEW MISSION: " +
            currentItem.itemName
        );
    }

    // =====================================================
    // GLOW
    // =====================================================

    private void TurnOffAllGlow()
    {
        if (items == null)
            return;

        foreach (SearchItem item in items)
        {
            if (item == null)
                continue;

            if (item.isCollected)
                continue;

            item.SetGlow(false);
        }
    }

    // =====================================================
    // MISSION UI
    // =====================================================

    private void UpdateMissionHint()
    {
        if (
            hintText == null ||
            currentItem == null
        )
        {
            return;
        }

        hintText.text =
            "Mission " +
            (currentMissionIndex + 1) +
            "/" +
            missionList.Count +

            "\nFind: " +
            currentItem.itemName +

            "\nHint: " +
            currentItem.itemHint;
    }

    // =====================================================
    // COORDINATES + DISTANCE
    // =====================================================

    private void UpdateTargetInformation()
    {
        if (
            currentItem == null ||
            player == null
        )
        {
            return;
        }

        Vector3 position =
            currentItem.transform.position;

        // Coordinates
        if (coordinateText != null)
        {
            coordinateText.text =
                "Coordinates:\n" +

                "X " +
                position.x.ToString("F1") +

                "   Y " +
                position.y.ToString("F1") +

                "   Z " +
                position.z.ToString("F1");
        }

        // Distance
        float distance =
            Vector3.Distance(
                player.position,
                position
            );

        if (distanceText != null)
        {
            distanceText.text =
                "Distance: " +
                distance.ToString("F1") +
                " m";
        }
    }

    // =====================================================
    // FIND NEARBY ITEM
    // =====================================================

    private void FindNearbyItem()
    {
        nearbyItem = null;

        if (
            player == null ||
            items == null
        )
        {
            return;
        }

        float closestDistance =
            Mathf.Infinity;

        foreach (SearchItem item in items)
        {
            if (item == null)
                continue;

            if (item.isCollected)
                continue;

            float distance =
                Vector3.Distance(
                    player.position,
                    item.transform.position
                );

            if (
                distance >
                interactionDistance
            )
            {
                continue;
            }

            if (
                distance <
                closestDistance
            )
            {
                closestDistance =
                    distance;

                nearbyItem =
                    item;
            }
        }
    }

    // =====================================================
    // INPUT
    // =====================================================

    private bool InteractButtonPressed()
    {
#if ENABLE_INPUT_SYSTEM

        if (
            Keyboard.current != null &&
            Keyboard.current.eKey
                .wasPressedThisFrame
        )
        {
            return true;
        }

#endif

#if ENABLE_LEGACY_INPUT_MANAGER

        if (
            Input.GetKeyDown(
                KeyCode.E
            )
        )
        {
            return true;
        }

#endif

        return false;
    }

    // =====================================================
    // INTERACTION
    // =====================================================

    private void HandleInteraction()
    {
        if (!InteractButtonPressed())
            return;

        if (
            currentItem == null ||
            player == null
        )
        {
            return;
        }

        float targetDistance =
            Vector3.Distance(
                player.position,
                currentItem.transform.position
            );

        // Correct item
        if (
            targetDistance <=
            interactionDistance
        )
        {
            CollectCorrectItem();

            return;
        }

        // Wrong item
        if (nearbyItem != null)
        {
            ShowTemporaryMessage(
                "Wrong item: " +
                nearbyItem.itemName +

                "\nFind: " +
                currentItem.itemName
            );
        }
    }

    // =====================================================
    // COLLECT
    // =====================================================

    private void CollectCorrectItem()
    {
        if (currentItem == null)
            return;

        SearchItem collectedItem =
            currentItem;

        Debug.Log(
            "COLLECTED: " +
            collectedItem.itemName
        );

        // =================================================
        // ADD ICON TO HOTBAR
        // =================================================

        if (hotbarItemDisplay != null)
        {
            hotbarItemDisplay.AddItem(
                collectedItem
            );
        }
        else
        {
            Debug.LogWarning(
                "HotbarItemDisplay is not assigned."
            );
        }

        // =================================================
        // REMOVE FROM WORLD
        // =================================================

        HideEIcon();

        collectedItem.SetGlow(false);

        collectedItem.CollectItem();

        nearbyItem = null;

        currentMissionIndex++;

        StartNextMission();
    }

    // =====================================================
    // INTERACTION UI
    // =====================================================

    private void UpdateInteractionUI()
    {
        if (
            currentItem == null ||
            player == null
        )
        {
            HideEIcon();

            if (interactionText != null)
            {
                interactionText.text = "";
            }

            return;
        }

        // Temporary message
        if (
            messageTimer > 0f &&
            !string.IsNullOrEmpty(
                temporaryMessage
            )
        )
        {
            HideEIcon();

            if (interactionText != null)
            {
                interactionText.text =
                    temporaryMessage;
            }

            return;
        }

        float targetDistance =
            Vector3.Distance(
                player.position,
                currentItem.transform.position
            );

        // Close enough
        if (
            targetDistance <=
            interactionDistance
        )
        {
            ShowEIcon();

            if (interactionText != null)
            {
                interactionText.text =
                    "Collect " +
                    currentItem.itemName;
            }

            return;
        }

        HideEIcon();

        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    // =====================================================
    // E ICON
    // =====================================================

    private void ShowEIcon()
    {
        if (eIcon == null)
            return;

        eIcon.gameObject.SetActive(
            true
        );
    }

    private void HideEIcon()
    {
        if (eIcon == null)
            return;

        eIcon.gameObject.SetActive(
            false
        );
    }

    // =====================================================
    // MESSAGE
    // =====================================================

    private void ShowTemporaryMessage(
        string message
    )
    {
        temporaryMessage =
            message;

        messageTimer =
            messageDuration;

        HideEIcon();

        if (interactionText != null)
        {
            interactionText.text =
                temporaryMessage;
        }
    }

    private void UpdateMessageTimer()
    {
        if (messageTimer <= 0f)
            return;

        messageTimer -=
            Time.deltaTime;

        if (messageTimer <= 0f)
        {
            messageTimer = 0f;

            temporaryMessage = "";

            ClearInteractionText();
        }
    }

    private void ClearInteractionText()
    {
        if (interactionText != null)
        {
            interactionText.text = "";
        }
    }

    // =====================================================
    // COMPLETE
    // =====================================================

    private void CompleteAllMissions()
    {
        currentItem = null;
        nearbyItem = null;

        temporaryMessage = "";
        messageTimer = 0f;

        TurnOffAllGlow();

        HideEIcon();

        if (coordinateText != null)
        {
            coordinateText.text =
                "Coordinates: Completed";
        }

        if (distanceText != null)
        {
            distanceText.text =
                "Distance: --";
        }

        if (hintText != null)
        {
            hintText.text =
                "All Missions Completed!";
        }

        ClearInteractionText();

        Debug.Log(
            "ALL MISSIONS COMPLETED!"
        );
    }
}