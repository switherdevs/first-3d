using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemSearchManager : MonoBehaviour
{
    [Header("Player and Camera")]
    public Transform player;
    public Camera playerCamera;

    [Header("Mission Items")]
    public SearchItem[] items;

    [Header("UI")]
    public TextMeshProUGUI coordinateText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI interactionText;

    [Header("Interaction Settings")]
    public float interactionDistance = 2.5f;
    public float raycastDistance = 5f;

    [Header("Debug")]
    public bool showDebugLogs = true;
    public bool drawRay = true;

    private List<SearchItem> missionList =
        new List<SearchItem>();

    private int currentMissionIndex = 0;

    private SearchItem currentItem;
    private SearchItem lookedAtItem;

    private string temporaryMessage = "";
    private float messageTimer = 0f;

    private void Start()
    {
        ClearUI();

        CreateRandomMissionList();

        StartNextMission();
    }

    private void Update()
    {
        UpdateMessageTimer();

        if (currentItem != null)
        {
            UpdateTargetInformation();

            DetectLookedAtItem();

            HandleInteraction();
        }
        else
        {
            lookedAtItem = null;
        }

        UpdateInteractionUI();
    }

    // CREATE RANDOM MISSIONS
    private void CreateRandomMissionList()
    {
        missionList.Clear();

        if (items == null)
        {
            Debug.LogError(
                "ItemSearchManager: Items array is null!"
            );

            return;
        }

        foreach (SearchItem item in items)
        {
            if (item == null)
                continue;

            item.isCollected = false;

            //Đảm bảo tất cả các vật phẩm đều bắt đầu ở trạng thái không phát sáng.
            item.SetGlow(false);

            missionList.Add(item);
        }

        // Trộn ngẫu nhiên 
        for (int i = 0; i < missionList.Count; i++)
        {
            int randomIndex =
                UnityEngine.Random.Range(
                    i,
                    missionList.Count
                );

            SearchItem temp = missionList[i];

            missionList[i] =
                missionList[randomIndex];

            missionList[randomIndex] =
                temp;
        }

        currentMissionIndex = 0;

        if (showDebugLogs)
        {
            Debug.Log(
                "Random mission list created. Total missions: "
                + missionList.Count
            );

            for (int i = 0; i < missionList.Count; i++)
            {
                Debug.Log(
                    "Mission " +
                    (i + 1) +
                    ": " +
                    missionList[i].itemName +
                    " | Type: " +
                    missionList[i].itemType
                );
            }
        }
    }

    // START NEXT MISSION
    private void StartNextMission()
    {
        // Tắt hiệu ứng phát sáng của mục tiêu trước đó 
        if (currentItem != null)
        {
            currentItem.SetGlow(false);
        }

        lookedAtItem = null;

        temporaryMessage = "";
        messageTimer = 0f;

        // Đã hoàn thành tất cả nhiệm vụ 
        if (currentMissionIndex >= missionList.Count)
        {
            currentItem = null;

            if (coordinateText != null)
            {
                coordinateText.text =
                    "Coordinates: --";
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

            if (interactionText != null)
            {
                interactionText.text = "";
            }

            if (showDebugLogs)
            {
                Debug.Log(
                    "ALL MISSIONS COMPLETED!"
                );
            }

            return;
        }

        currentItem =
            missionList[currentMissionIndex];

        if (currentItem == null)
        {
            currentMissionIndex++;

            StartNextMission();

            return;
        }

        // Vật phẩm nhiệm vụ hiển thị hiệu ứng phát sáng chính xác 
        currentItem.SetGlow(true);

        UpdateMissionHint();

        if (showDebugLogs)
        {
            Debug.Log(
                "=============================="
            );

            Debug.Log(
                "NEW MISSION"
            );

            Debug.Log(
                "Find: " +
                currentItem.itemName
            );

            Debug.Log(
                "Type: " +
                currentItem.itemType
            );

            Debug.Log(
                "Object: " +
                currentItem.gameObject.name
            );

            Debug.Log(
                "================================"
            );
        }
    }

    // MISSION HINT
    private void UpdateMissionHint()
    {
        if (hintText == null ||
            currentItem == null)
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

    // COORDINATES + DISTANCE
    private void UpdateTargetInformation()
    {
        if (currentItem == null ||
            player == null)
        {
            return;
        }

        Vector3 targetPosition =
            currentItem.transform.position;

        if (coordinateText != null)
        {
            coordinateText.text =
                "Coordinates\n" +
                "X: " +
                targetPosition.x.ToString("F1") +
                " | Y: " +
                targetPosition.y.ToString("F1") +
                " | Z: " +
                targetPosition.z.ToString("F1");
        }

        float distance =
            Vector3.Distance(
                player.position,
                targetPosition
            );

        if (distanceText != null)
        {
            distanceText.text =
                "Distance: " +
                distance.ToString("F1") +
                " m";
        }
    }

    // DETECT ITEM PLAYER IS LOOKING AT
    private void DetectLookedAtItem()
    {
        lookedAtItem = null;

        if (playerCamera == null || player == null)
            return;

        float bestScreenDistance = Mathf.Infinity;
        SearchItem bestItem = null;

        foreach (SearchItem item in items)
        {
            if (item == null || item.isCollected)
                continue;

            // Khoảng cách vật lí từ người chơi đến vật phẩm 
            float worldDistance = Vector3.Distance(
                player.position,
                item.transform.position
            );

            // Chỉ kiểm tra các vât phẩm ở gần 
            if (worldDistance > interactionDistance)
                continue;

            // Chuyển đổi vị trí trong thế giới của đối tượng sang vị trí trên màn hình.
            Vector3 viewportPosition =
                playerCamera.WorldToViewportPoint(
                    item.transform.position
                );

            // Vật phẩm nằm phía sau máy ảnh.
            if (viewportPosition.z <= 0f)
                continue;

            // Center of screen = (0.5, 0.5)
            Vector2 itemScreenPosition =
                new Vector2(
                    viewportPosition.x,
                    viewportPosition.y
                );

            Vector2 screenCenter =
                new Vector2(0.5f, 0.5f);

            float screenDistance =
                Vector2.Distance(
                    itemScreenPosition,
                    screenCenter
                );

            // How close item must be to crosshair
            float aimRadius = 0.15f;

            if (screenDistance > aimRadius)
                continue;

            // Select the item closest to the crosshair
            if (screenDistance < bestScreenDistance)
            {
                bestScreenDistance = screenDistance;
                bestItem = item;
            }
        }

        lookedAtItem = bestItem;

        if (lookedAtItem != null && showDebugLogs)
        {
            Debug.Log(
                "TARGET DETECTED: " +
                lookedAtItem.itemName +
                " | Type: " +
                lookedAtItem.itemType
            );
        }
    }

    // PRESS E
    private void HandleInteraction()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (currentItem == null)
            return;

        if (player == null)
        {
            ShowTemporaryMessage(
                "Player is not assigned."
            );

            return;
        }

        if (lookedAtItem == null)
        {
            ShowTemporaryMessage(
                "No item detected."
            );

            return;
        }

        float distance =
            Vector3.Distance(
                player.position,
                lookedAtItem.transform.position
            );

        if (distance > interactionDistance)
        {
            ShowTemporaryMessage(
                "Move closer to the item."
            );

            return;
        }

        // IMPORTANT:
        // Compare ITEM TYPE instead of only GameObject reference.
        bool isCorrectItem =
            lookedAtItem.itemType ==
            currentItem.itemType;

        if (showDebugLogs)
        {
            Debug.Log(
                "INTERACTION"
            );

            Debug.Log(
                "Mission item: " +
                currentItem.itemName +
                " | Type: " +
                currentItem.itemType
            );

            Debug.Log(
                "Looked item: " +
                lookedAtItem.itemName +
                " | Type: " +
                lookedAtItem.itemType
            );

            Debug.Log(
                "Correct: " +
                isCorrectItem
            );

            Debug.Log(
                "==============================="
            );
        }

        if (isCorrectItem)
        {
            CollectCorrectItem();
        }
        else
        {
            WrongItemInteraction();
        }
    }

    // CORRECT ITEM
    private void CollectCorrectItem()
    {
        if (currentItem == null ||
            lookedAtItem == null)
        {
            return;
        }

        string collectedItemName =
            currentItem.itemName;

        if (showDebugLogs)
        {
            Debug.Log(
                "CORRECT ITEM: " +
                collectedItemName
            );
        }

        currentItem.SetGlow(false);

        // Collect the exact item the player is looking at
        lookedAtItem.CollectItem();

        currentMissionIndex++;

        // Start the next mission immediately
        StartNextMission();
    }

    // WRONG ITEM
    private void WrongItemInteraction()
    {
        if (lookedAtItem == null ||
            currentItem == null)
        {
            return;
        }

        if (showDebugLogs)
        {
            Debug.LogWarning(
                "WRONG ITEM!"
            );
        }

        ShowTemporaryMessage(
            "Wrong item: " +
            lookedAtItem.itemName +
            "\nFind: " +
            currentItem.itemName
        );
    }

    // TEMPORARY MESSAGE
    private void ShowTemporaryMessage(
        string message
    )
    {
        temporaryMessage = message;

        // Show for 3 seconds
        messageTimer = 3f;
    }

    private void UpdateMessageTimer()
    {
        if (messageTimer <= 0f)
            return;

        messageTimer -= Time.deltaTime;

        if (messageTimer <= 0f)
        {
            messageTimer = 0f;
            temporaryMessage = "";
        }
    }

    // INTERACTION UI
    private void UpdateInteractionUI()
    {
        if (interactionText == null)
            return;

        // Temporary wrong/error message
        if (messageTimer > 0f &&
            !string.IsNullOrEmpty(
                temporaryMessage
            ))
        {
            interactionText.text =
                temporaryMessage;

            return;
        }

        // Nothing shown when mission finished
        if (currentItem == null)
        {
            interactionText.text = "";
            return;
        }

        // Nothing shown when looking at nothing
        if (lookedAtItem == null)
        {
            interactionText.text = "";
            return;
        }

        if (player == null)
        {
            interactionText.text = "";
            return;
        }

        float distance =
            Vector3.Distance(
                player.position,
                lookedAtItem.transform.position
            );

        // Do not show anything if too far
        if (distance > interactionDistance)
        {
            interactionText.text = "";
            return;
        }

        bool isCorrectItem =
            lookedAtItem.itemType ==
            currentItem.itemType;

        // Correct item:
        // show interaction prompt
        if (isCorrectItem)
        {
            interactionText.text =
                lookedAtItem.itemName +
                "\nPress E to collect.";
        }
        else
        {
            // Wrong message only appears
            // AFTER player presses E.
            interactionText.text = "";
        }
    }

    // CLEAR UI
    private void ClearUI()
    {
        if (coordinateText != null)
            coordinateText.text = "";

        if (distanceText != null)
            distanceText.text = "";

        if (hintText != null)
            hintText.text = "";

        if (interactionText != null)
            interactionText.text = "";
    }
}