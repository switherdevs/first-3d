using UnityEngine;

public class RandomPuzzleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PuzzleSpawn
    {
        public string itemName;

        public Transform item;

        public float yPosition = 1f;
    }

    [Header("Puzzle Objects")]
    public PuzzleSpawn[] puzzleObjects;

    [Header("Random Area")]
    public Vector2 xRange = new Vector2(-8f, 8f);
    public Vector2 zRange = new Vector2(3f, 20f);

    [Header("Khoảng cách tối thiểu")]
    public float minDistance = 3f;

    private void Start()
    {
        RandomizeAll();
    }

    public void RandomizeAll()
    {
        for (int i = 0; i < puzzleObjects.Length; i++)
        {
            if (puzzleObjects[i].item == null)
                continue;

            Vector3 randomPosition =
                FindRandomPosition(i);

            randomPosition.y =
                puzzleObjects[i].yPosition;

            puzzleObjects[i].item.position =
                randomPosition;

            Debug.Log(
                puzzleObjects[i].itemName +
                " random tại: " +
                randomPosition
            );
        }
    }

    private Vector3 FindRandomPosition(int index)
    {
        int attempts = 0;

        while (attempts < 100)
        {
            float randomX =
                Random.Range(xRange.x, xRange.y);

            float randomZ =
                Random.Range(zRange.x, zRange.y);

            Vector3 position =
                new Vector3(
                    randomX,
                    0f,
                    randomZ
                );

            if (!IsTooClose(position, index))
            {
                return position;
            }

            attempts++;
        }

        return Vector3.zero;
    }

    private bool IsTooClose(
        Vector3 position,
        int currentIndex)
    {
        for (int i = 0; i < currentIndex; i++)
        {
            if (puzzleObjects[i].item == null)
                continue;

            Vector3 oldPosition =
                puzzleObjects[i].item.position;

            Vector2 posA =
                new Vector2(
                    position.x,
                    position.z
                );

            Vector2 posB =
                new Vector2(
                    oldPosition.x,
                    oldPosition.z
                );

            float distance =
                Vector2.Distance(posA, posB);

            if (distance < minDistance)
            {
                return true;
            }
        }

        return false;
    }
}