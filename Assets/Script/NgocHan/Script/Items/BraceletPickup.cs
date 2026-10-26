using UnityEngine;

public class BraceletPickup : MonoBehaviour
{
    [SerializeField] private Map1EndingCheck map1EndingCheck;

    public void CollectBracelet()
    {
        if (map1EndingCheck != null)
        {
            map1EndingCheck.BraceletCollected();
        }

        gameObject.SetActive(false);
    }
}