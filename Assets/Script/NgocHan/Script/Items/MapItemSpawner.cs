using System.Collections.Generic;
using UnityEngine;

public class MapItemSpawner : MonoBehaviour
{
    [Header("Items In This Map")]
    [Tooltip("Only items belonging to this map.")]
    public SearchItem[] mapItems;

    [Header("Spawn Points In This Map")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public bool randomizePositions = true;

    public bool randomizeRotations = true;

    private void Awake()
    {
        PrepareItems();
    }

    private void Start()
    {
        if (randomizePositions)
        {
            RandomizeItemPositions();
        }
    }

    // =========================================================
    // PREPARE ITEMS
    // =========================================================

    private void PrepareItems()
    {
        if (mapItems == null)
        {
            Debug.LogError(
                "MapItemSpawner: mapItems is null."
            );

            return;
        }

        foreach (SearchItem item in mapItems)
        {
            if (item == null)
                continue;

            item.ResetItem();
        }
    }

    // =========================================================
    // RANDOMIZE POSITIONS
    // =========================================================

    private void RandomizeItemPositions()
    {
        if (mapItems == null ||
            mapItems.Length == 0)
        {
            Debug.LogError(
                "MapItemSpawner: No items assigned."
            );

            return;
        }

        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogError(
                "MapItemSpawner: No spawn points assigned."
            );

            return;
        }

        if (spawnPoints.Length < mapItems.Length)
        {
            Debug.LogError(
                "MapItemSpawner: Not enough spawn points.\n" +
                "Items: " +
                mapItems.Length +
                "\nSpawn Points: " +
                spawnPoints.Length
            );

            return;
        }

        // Create a temporary list of spawn points.
        List<Transform> availablePoints =
            new List<Transform>(
                spawnPoints
            );

        foreach (SearchItem item in mapItems)
        {
            if (item == null)
                continue;

            if (availablePoints.Count == 0)
            {
                Debug.LogError(
                    "MapItemSpawner: No available spawn point."
                );

                break;
            }

            // Select a random spawn point.
            int randomIndex =
                Random.Range(
                    0,
                    availablePoints.Count
                );

            Transform selectedPoint =
                availablePoints[randomIndex];

            // Remove it so another item cannot use it.
            availablePoints.RemoveAt(
                randomIndex
            );

            // Move item.
            item.transform.position =
                selectedPoint.position;

            // Random rotation.
            if (randomizeRotations)
            {
                item.transform.rotation =
                    selectedPoint.rotation;
            }

            Debug.Log(
                "Spawned " +
                item.itemID +
                " at " +
                selectedPoint.name
            );
        }
    }

    // =========================================================
    // GET MAP ITEMS
    // =========================================================

    public SearchItem[] GetMapItems()
    {
        return mapItems;
    }

    // =========================================================
    // GET ITEM BY ID
    // =========================================================

    public SearchItem GetItemByID(string id)
    {
        if (mapItems == null)
            return null;

        foreach (SearchItem item in mapItems)
        {
            if (item == null)
                continue;

            if (item.itemID == id)
            {
                return item;
            }
        }

        return null;
    }
}