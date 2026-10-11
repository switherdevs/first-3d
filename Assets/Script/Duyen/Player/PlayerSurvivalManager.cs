
using System.Collections;
using UnityEngine;

public class PlayerSurvivalManager : MonoBehaviour
{
    [Header("Player và Checkpoint")]
    public GameObject player;
    public Transform checkpoint;

    [Header("Th?i gian h?i sinh")]
    public float respawnDelay = 3f;

    private bool isRespawning = false;

    public void PlayerCaught()
    {
        if (isRespawning) return;

        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        // ?n Player khi b? Ma Da b?t
        player.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        // ??a Player v? Checkpoint
        player.transform.position = checkpoint.position;
        player.transform.rotation = checkpoint.rotation;

        // Hi?n Player tr? l?i
        player.SetActive(true);

        isRespawning = false;

        Debug.Log("Player ?ã h?i sinh t?i Checkpoint!");
    }
}