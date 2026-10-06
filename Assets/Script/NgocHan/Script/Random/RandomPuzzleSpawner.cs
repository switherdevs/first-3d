using UnityEngine;
using System.Collections.Generic;

public class RandomPuzzleSpawner : MonoBehaviour
{
    [Header("Puzzle Objects")]
    public Transform[] puzzleObjects;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Random")]
    public bool randomOnStart = true;

    private void Start()
    {
        if (randomOnStart)
        {
            RandomizePuzzle();
        }
    }

    public void RandomizePuzzle()
    {
        if (puzzleObjects == null || spawnPoints == null)
            return;

        if (spawnPoints.Length < puzzleObjects.Length)
        {
            Debug.LogError(
                "Khong du SpawnPoint! Can it nhat " +
                puzzleObjects.Length +
                " diem."
            );
            return;
        }

        // Tao danh sach index SpawnPoint
        List<int> availablePoints = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availablePoints.Add(i);
        }

        // Random tung object
        for (int i = 0; i < puzzleObjects.Length; i++)
        {
            if (puzzleObjects[i] == null)
                continue;

            int randomListIndex =
                Random.Range(0, availablePoints.Count);

            int pointIndex =
                availablePoints[randomListIndex];

            Transform point =
                spawnPoints[pointIndex];

            puzzleObjects[i].position =
                point.position;

            puzzleObjects[i].rotation =
                point.rotation;

            // Xoa diem da dung
            // de 2 vat khong spawn trung nhau
            availablePoints.RemoveAt(randomListIndex);
        }

        Debug.Log("Da random vi tri manh moi!");
    }
}