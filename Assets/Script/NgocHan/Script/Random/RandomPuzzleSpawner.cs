using System.Collections.Generic;
using UnityEngine;

public class RandomPuzzleSpawner : MonoBehaviour
{
    [Header("4 VAT PHAM DUOC RANDOM")]

    [SerializeField]
    private GameObject oilLamp;

    [SerializeField]
    private GameObject mirrorShard;

    [SerializeField]
    private GameObject oldScale;

    [SerializeField]
    private GameObject woodenBox;


    [Header("CAC VI TRI RANDOM")]

    [SerializeField]
    private Transform[] spawnPoints;


    [Header("TUY CHON")]

    [SerializeField]
    private bool randomRotationY = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        RandomizeItems();
    }


    // =========================================================
    // RANDOM
    // =========================================================

    public void RandomizeItems()
    {
        GameObject[] randomItems =
        {
            oilLamp,
            mirrorShard,
            oldScale,
            woodenBox
        };


        if (spawnPoints == null ||
            spawnPoints.Length < randomItems.Length)
        {
            Debug.LogError(
                "Khong du Spawn Point!"
            );

            return;
        }


        List<Transform> availablePoints =
            new List<Transform>();


        foreach (Transform point in spawnPoints)
        {
            if (point != null)
            {
                availablePoints.Add(point);
            }
        }


        if (availablePoints.Count <
            randomItems.Length)
        {
            Debug.LogError(
                "Co Spawn Point dang bi Null!"
            );

            return;
        }


        foreach (GameObject item in randomItems)
        {
            if (item == null)
            {
                Debug.LogWarning(
                    "Co item chua duoc gan!"
                );

                continue;
            }


            // ---------------------------------------------
            // TẮT E
            // ---------------------------------------------

            HideItemPrompt(item);


            // ---------------------------------------------
            // RANDOM POINT
            // ---------------------------------------------

            int randomIndex =
                Random.Range(
                    0,
                    availablePoints.Count
                );


            Transform point =
                availablePoints[randomIndex];


            // ---------------------------------------------
            // POSITION
            // ---------------------------------------------

            item.transform.position =
                point.position;


            // ---------------------------------------------
            // ROTATION
            // ---------------------------------------------

            if (randomRotationY)
            {
                Vector3 rotation =
                    item.transform.eulerAngles;


                rotation.y =
                    Random.Range(
                        0f,
                        360f
                    );


                item.transform.eulerAngles =
                    rotation;
            }


            // ---------------------------------------------
            // TẮT E LẦN NỮA
            // ---------------------------------------------

            HideItemPrompt(item);


            // ---------------------------------------------
            // XÓA POINT ĐÃ DÙNG
            // ---------------------------------------------

            availablePoints.RemoveAt(
                randomIndex
            );


            Debug.Log(
                item.name
                + " duoc dat tai "
                + point.name
            );
        }
    }


    // =========================================================
    // HIDE PROMPT
    // =========================================================

    private void HideItemPrompt(
        GameObject item
    )
    {
        if (item == null)
            return;


        PuzzleWorldPrompt[] prompts =
            item.GetComponentsInChildren
            <PuzzleWorldPrompt>(true);


        foreach (PuzzleWorldPrompt prompt in prompts)
        {
            if (prompt != null)
            {
                prompt.HidePrompt();
            }
        }
    }
}