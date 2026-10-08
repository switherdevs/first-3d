using System.Collections.Generic;
using UnityEngine;

public class RandomPuzzleSpawner : MonoBehaviour
{
    [Header("CÁC VẬT PHẨM ĐƯỢC RANDOM")]
    [SerializeField] private GameObject oldLetter;
    [SerializeField] private GameObject purchaseTicket;
    [SerializeField] private GameObject oldScale;
    [SerializeField] private GameObject oilLamp;
    [SerializeField] private GameObject brokenMirror;

    [Header("CÁC ĐIỂM SPAWN")]
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        SpawnPuzzleItems();
    }

    private void SpawnPuzzleItems()
    {
        // Cần ít nhất 5 điểm vì có 5 vật phẩm
        if (spawnPoints == null || spawnPoints.Length < 5)
        {
            Debug.LogError(
                "RandomPuzzleSpawner: Cần ít nhất 5 Spawn Point!"
            );

            return;
        }

        // Tạo danh sách điểm có thể sử dụng
        List<Transform> availablePoints =
            new List<Transform>(spawnPoints);

        // Random từng vật phẩm
        SpawnOne(
            oldLetter,
            PuzzleItemType.Paper,
            availablePoints
        );

        SpawnOne(
            purchaseTicket,
            PuzzleItemType.Ticket,
            availablePoints
        );

        SpawnOne(
            oldScale,
            PuzzleItemType.Number4,
            availablePoints
        );

        SpawnOne(
            oilLamp,
            PuzzleItemType.Number1,
            availablePoints
        );

        SpawnOne(
            brokenMirror,
            PuzzleItemType.Number7,
            availablePoints
        );
    }

    private void SpawnOne(
        GameObject prefab,
        PuzzleItemType itemType,
        List<Transform> availablePoints
    )
    {
        if (prefab == null)
        {
            Debug.LogError(
                "Chưa gán prefab cho: " + itemType
            );

            return;
        }

        if (availablePoints == null ||
            availablePoints.Count <= 0)
        {
            Debug.LogError(
                "Không còn Spawn Point!"
            );

            return;
        }

        // Chọn 1 point ngẫu nhiên
        int randomIndex =
            Random.Range(
                0,
                availablePoints.Count
            );

        Transform selectedPoint =
            availablePoints[randomIndex];

        if (selectedPoint == null)
        {
            Debug.LogError(
                "Có Spawn Point đang bị None!"
            );

            availablePoints.RemoveAt(randomIndex);

            return;
        }

        // Spawn vật phẩm
        GameObject spawnedObject =
            Instantiate(
                prefab,
                selectedPoint.position,
                selectedPoint.rotation
            );

        // Đặt tên dễ nhìn trong Hierarchy
        spawnedObject.name =
            prefab.name + "_Spawned";

        // Báo cho Map1PuzzleManager biết
        // vật phẩm này đang nằm ở đâu
        if (Map1PuzzleManager.Instance != null)
        {
            Map1PuzzleManager.Instance
                .RegisterSpawnedTarget(
                    itemType,
                    spawnedObject.transform
                );
        }
        else
        {
            Debug.LogWarning(
                "Không tìm thấy Map1PuzzleManager."
            );
        }

        // Xóa Point khỏi danh sách
        // để vật phẩm sau không spawn trùng
        availablePoints.RemoveAt(randomIndex);

        Debug.Log(
            itemType +
            " spawn tại " +
            selectedPoint.name
        );
    }
}


// ======================================================
// LOẠI VẬT PHẨM
// ======================================================

public enum PuzzleItemType
{
    Paper,
    Ticket,
    Number4,
    Number1,
    Number7
}