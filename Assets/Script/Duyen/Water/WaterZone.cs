using UnityEngine;

public class WaterZone : MonoBehaviour
{
    public WaterSystem waterSystem;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Có vật đi vào WaterZone: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã xuống nước!");

            if (waterSystem != null)
            {
                waterSystem.PlayerEnterWater();
            }
            else
            {
                Debug.LogError("WaterSystem chưa được gán!");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (waterSystem != null)
            {
                waterSystem.PlayerEnterWater();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player rời WaterZone!");

        if (other.CompareTag("Player"))
        {
            if (waterSystem != null)
            {
                waterSystem.PlayerExitWater();
            }
        }
    }
}