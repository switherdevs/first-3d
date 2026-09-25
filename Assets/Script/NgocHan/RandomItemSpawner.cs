using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Item Prefabs")]
    public GameObject[] itemPrefabs;

    [Header("Spawn Settings")]
    public int numberOfItems = 4;

    public float minDistance = 10f;
    public float maxDistance = 20f;

    public float minSpacing = 2f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;

    public float raycastHeight = 30f;
    public float raycastDistance = 100f;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private void Start()
    {
        // Automatically find Player if not assigned
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player == null)
        {
            Debug.LogError(
                "RandomItemSpawner: Player is not assigned!"
            );

            return;
        }

        SpawnItems();
    }

    private void SpawnItems()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning(
                "RandomItemSpawner: No item prefabs assigned!"
            );

            return;
        }

        for (int i = 0; i < numberOfItems; i++)
        {
            SpawnRandomItem();
        }
    }

    private void SpawnRandomItem()
    {
        int maxAttempts = 30;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 randomPosition = GetRandomPositionAroundPlayer();

            // Start raycast above the random position
            Vector3 rayStart =
                randomPosition + Vector3.up * raycastHeight;

            RaycastHit hit;

            if (Physics.Raycast(
                rayStart,
                Vector3.down,
                out hit,
                raycastDistance,
                groundLayer
            ))
            {
                Vector3 spawnPosition = hit.point;

                // Check spacing between spawned items
                if (!IsPositionValid(spawnPosition))
                {
                    continue;
                }

                // Pick a random prefab
                int randomIndex =
                    Random.Range(0, itemPrefabs.Length);

                GameObject selectedPrefab =
                    itemPrefabs[randomIndex];

                GameObject newItem = Instantiate(
                    selectedPrefab,
                    spawnPosition,
                    Quaternion.identity
                );

                spawnedItems.Add(newItem);

                float distance =
                    Vector3.Distance(
                        player.position,
                        spawnPosition
                    );

                Debug.Log(
                    "Spawned: " +
                    newItem.name +
                    " | Distance: " +
                    distance.ToString("F1") +
                    "m"
                );

                return;
            }
        }

        Debug.LogWarning(
            "Could not find a valid spawn position."
        );
    }

    private Vector3 GetRandomPositionAroundPlayer()
    {
        // Random direction around Player
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        // Random distance between 10m and 20m
        float distance =
            Random.Range(minDistance, maxDistance);

        Vector3 offset = new Vector3(
            direction.x,
            0f,
            direction.y
        );

        offset *= distance;

        return player.position + offset;
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item == null)
                continue;

            float distance =
                Vector3.Distance(
                    position,
                    item.transform.position
                );

            if (distance < minSpacing)
            {
                return false;
            }
        }

        return true;
    }
}