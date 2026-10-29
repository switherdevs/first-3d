using UnityEngine;
using UnityEngine.InputSystem;

public class GoodEndingSceneController : MonoBehaviour
{
    [Header("PLAYER")]
    public GameObject player;

    [Header("PLAYER CONTROL")]
    public MonoBehaviour[] scriptsToDisable;

    [Header("GOOD ENDING AUDIO")]
    public AudioSource goodEndingAudioSource;
    public AudioClip goodEndingMusic;

    private bool locked = false;

    private void Start()
    {
        // Vừa vào scene Good Ending là khóa luôn
        LockPlayer();
    }

    public void LockPlayer()
    {
        if (locked)
            return;

        locked = true;

        // Tắt các script đã kéo vào Inspector
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        if (player != null)
        {
            // Tắt toàn bộ Player Input
            PlayerInput playerInput = player.GetComponent<PlayerInput>();

            if (playerInput != null)
                playerInput.DeactivateInput();

            // Dừng Player
            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        // Phát nhạc Good Ending
        if (goodEndingAudioSource != null && goodEndingMusic != null)
        {
            goodEndingAudioSource.Stop();
            goodEndingAudioSource.clip = goodEndingMusic;
            goodEndingAudioSource.loop = false;
            goodEndingAudioSource.Play();
        }

        Debug.Log("GOOD ENDING: PLAYER LOCKED");
    }
}