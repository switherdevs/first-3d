using UnityEngine;

public class MaDaDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 8f;

    [Header("References")]
    public Transform playerTransform;
    public WaterSystem waterSystem;

    public bool playerDetected = false;

    private void Start()
    {
        // Tự tìm Player
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // Tự tìm WaterSystem
        if (waterSystem == null)
        {
            waterSystem = FindFirstObjectByType<WaterSystem>();
        }
    }

    private void Update()
    {
        CheckPlayerDetection();
    }

    void CheckPlayerDetection()
    {
        playerDetected = false;

        if (playerTransform == null || waterSystem == null)
            return;

        // Player phải đang ở dưới nước
        if (!waterSystem.playerInWater)
            return;

        // Chỉ tính khoảng cách ngang X/Z
        Vector3 maDaPosition = transform.position;
        Vector3 playerPosition = playerTransform.position;

        maDaPosition.y = 0;
        playerPosition.y = 0;

        float distance = Vector3.Distance(maDaPosition, playerPosition);

        if (distance <= detectionRadius)
        {
            playerDetected = true;

            Debug.Log("Ma Da đã phát hiện Player!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}