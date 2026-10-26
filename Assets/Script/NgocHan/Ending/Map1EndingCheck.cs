using UnityEngine;
using UnityEngine.SceneManagement;

public class Map1EndingCheck : MonoBehaviour
{
    [Header("ENDING SCENES")]
    [SerializeField] private string goodEndingScene = "GoodEnding_Market";
    [SerializeField] private string badEndingScene = "BadEnding_Market";

    [Header("IMPORTANT ITEM")]
    [SerializeField] private bool hasBracelet = false;

    // Call this when the main puzzles are completed
    public void FinishMap()
    {
        if (hasBracelet)
        {
            Debug.Log("[MAP 1] GOOD ENDING");

            SceneManager.LoadScene(goodEndingScene);
        }
        else
        {
            Debug.Log("[MAP 1] BAD ENDING");

            SceneManager.LoadScene(badEndingScene);
        }
    }

    // Call this when Bracelet is collected
    public void BraceletCollected()
    {
        hasBracelet = true;

        Debug.Log("[MAP 1] Bracelet collected!");
    }
}