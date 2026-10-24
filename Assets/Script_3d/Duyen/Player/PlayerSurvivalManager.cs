
using System.Collections;
using UnityEngine;

public class PlayerSurvivalManager : MonoBehaviour
{
    [Header("Player và Checkpoint")]
    public GameObject player;
    public Transform checkpoint;

    [Header("Thời gian hồi sinh")]
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

        // Player khi bị Ma Da bắt
        player.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        //  Đưa Player về Checkpoint
        player.transform.position = checkpoint.position;
        player.transform.rotation = checkpoint.rotation;

        // Hiện Player trở lạii
        player.SetActive(true);

        isRespawning = false;

        Debug.Log("Player đã hồi sinh tại Checkpoint!");

        // Đặt lại trạng thái nước nguy hiểm
        PlayerMovement playerMovement =
            player.GetComponent<PlayerMovement>();

        //if (playerMovement != null)
        //{
        //    playerMovement.DatLaiTrangThaiNuocNguyHiem();
        //}
    }
}