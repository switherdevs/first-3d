
using UnityEngine;

public class DangerousWater : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("DangerousWater đã hoạt động!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player đi vào vùng nước nguy hiểm: " + other.name);

        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            player.dangTrongVungNuocNguyHiem = true;
            Debug.Log("ĐÃ BẬT GIẢM TỐC ĐỘ NƯỚC NGUY HIỂM!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player rời vùng nước: " + other.name);

        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            player.dangTrongVungNuocNguyHiem = false;
            Debug.Log("ĐÃ TẮT GIẢM TỐC ĐỘ NƯỚC NGUY HIỂM!");
        }
    }
}
