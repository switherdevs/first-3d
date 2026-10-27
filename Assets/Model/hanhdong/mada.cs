
using UnityEngine;
using System.Collections;

public class MadaAI : MonoBehaviour
{
    [Header("Vùng bắt chân")]
    public float grabDuration = 3f;
    public float pullSpeed = 1.5f;

    private bool isGrabbing = false;
    private Transform player;
    private CharacterController playerController;
    private Rigidbody playerRb;

    private void OnTriggerEnter(Collider other)
    {
        if (isGrabbing || !other.CompareTag("Player"))
            return;

        player = other.transform;
        playerController = player.GetComponent<CharacterController>();
        playerRb = player.GetComponent<Rigidbody>();

        StartCoroutine(GrabPlayer());
    }

    private IEnumerator GrabPlayer()
    {
        isGrabbing = true;

        if (playerController != null)
            playerController.enabled = false;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        float timer = 0f;

        while (player != null && timer < grabDuration)
        {
            // Kéo Player về phía Ma Da
            Vector3 target = transform.position;
            target.y = player.position.y;

            player.position = Vector3.MoveTowards(
                player.position,
                target,
                pullSpeed * Time.deltaTime
            );

            timer += Time.deltaTime;
            yield return null;
        }

        if (playerController != null)
            playerController.enabled = true;

        if (playerRb != null)
            playerRb.isKinematic = false;

        isGrabbing = false;
        player = null;
    }
}
