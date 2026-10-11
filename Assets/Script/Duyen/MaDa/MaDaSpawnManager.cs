
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaDaSpawnManager : MonoBehaviour
{
    [Header("Ma Da Prefab")]
    public GameObject maDaPrefab;

    [Header("Spawn Points - v? trí xu?t hi?n")]
    public Transform[] spawnPoints;

    [Header("Cài ??t Spawn / Despawn")]
    public float spawnInterval = 15f;
    public float despawnAfter = 30f;
    public int maxActiveMaDa = 2;

    private readonly List<GameObject> activeMaDa = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private void Start()
    {
        if (maDaPrefab == null)
        {
            Debug.LogWarning("Ch?a gán Ma Da Prefab vào MaDaSpawnManager!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Ch?a gán Spawn Points!");
            return;
        }

        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            activeMaDa.RemoveAll(maDa => maDa == null);

            if (activeMaDa.Count < maxActiveMaDa)
            {
                SpawnMaDa();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMaDa()
    {
        List<Transform> validPoints = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                validPoints.Add(point);
            }
        }

        if (validPoints.Count == 0)
        {
            Debug.LogWarning("Không có Spawn Point h?p l?!");
            return;
        }

        Transform selectedPoint =
            validPoints[Random.Range(0, validPoints.Count)];

        GameObject maDa = Instantiate(
            maDaPrefab,
            selectedPoint.position,
            selectedPoint.rotation
        );

        activeMaDa.Add(maDa);

        Debug.Log("Ma Da ?ã Spawn t?i: " + selectedPoint.name);

        StartCoroutine(DespawnAfterTime(maDa));
    }

    private IEnumerator DespawnAfterTime(GameObject maDa)
    {
        yield return new WaitForSeconds(despawnAfter);

        if (maDa != null)
        {
            activeMaDa.Remove(maDa);
            Destroy(maDa);
            Debug.Log("Ma Da ?ã Despawn!");
        }
    }
}