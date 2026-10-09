using System.Collections.Generic;
using UnityEngine;

public class RandomPuzzleSpawner : MonoBehaviour
{
    [Header("4 VẬT PHẨM ĐƯỢC RANDOM")]
    [SerializeField] private GameObject oilLamp;
    [SerializeField] private GameObject mirrorShard;
    [SerializeField] private GameObject oldScale;
    [SerializeField] private GameObject woodenBox;

    [Header("CÁC VỊ TRÍ RANDOM")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Tùy chọn")]
    [SerializeField] private bool randomRotationY = false;

    private void Start()
    {
        RandomizeItems();
    }

    public void RandomizeItems()
    {
        GameObject[] randomItems =
        {
            oilLamp,
            mirrorShard,
            oldScale,
            woodenBox
        };

        // Kiểm tra đủ vị trí hay không
        if (spawnPoints == null || spawnPoints.Length < randomItems.Length)
        {
            Debug.LogError(
                "Không đủ Spawn Point! Cần ít nhất "
                + randomItems.Length
                + " Spawn Point."
            );

            return;
        }

        // Tạo danh sách Spawn Point có thể dùng
        List<Transform> availablePoints =
            new List<Transform>(spawnPoints);

        foreach (GameObject item in randomItems)
        {
            if (item == null)
            {
                Debug.LogWarning("Có vật phẩm chưa được gắn vào RandomPuzzleSpawner!");
                continue;
            }

            // Chọn vị trí ngẫu nhiên
            int randomIndex =
                Random.Range(0, availablePoints.Count);

            Transform selectedPoint =
                availablePoints[randomIndex];

            // Đưa vật phẩm tới vị trí
            item.transform.position =
                selectedPoint.position;

            // Random hướng quay Y nếu bật
            if (randomRotationY)
            {
                Vector3 rotation =
                    item.transform.eulerAngles;

                rotation.y =
                    Random.Range(0f, 360f);

                item.transform.eulerAngles =
                    rotation;
            }

            // Xóa vị trí vừa dùng
            // để không có 2 vật phẩm cùng một chỗ
            availablePoints.RemoveAt(randomIndex);
        }
    }
}