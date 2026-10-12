using UnityEngine;

public class PuzzleWorldPrompt : MonoBehaviour
{
    [Header("WORLD PROMPT")]
    [SerializeField] private GameObject worldPrompt;


    private void Awake()
    {
        AutoFindPrompt();
        HidePrompt();
    }


    private void Start()
    {
        HidePrompt();
    }


    private void AutoFindPrompt()
    {
        if (worldPrompt != null)
            return;


        Transform[] children =
            GetComponentsInChildren<Transform>(true);


        foreach (Transform child in children)
        {
            if (child == null)
                continue;


            if (child.name == "WorldPrompt")
            {
                worldPrompt = child.gameObject;
                return;
            }
        }
    }


    public void ShowPrompt()
    {
        AutoFindPrompt();

        if (worldPrompt != null)
        {
            worldPrompt.SetActive(true);
        }
    }


    public void HidePrompt()
    {
        AutoFindPrompt();

        if (worldPrompt != null)
        {
            worldPrompt.SetActive(false);
        }
    }


    public GameObject GetWorldPrompt()
    {
        AutoFindPrompt();

        return worldPrompt;
    }
}