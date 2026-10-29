using UnityEngine;
using UnityEngine.InputSystem;

public class BadEndingSceneController : MonoBehaviour
{
    [Header("PLAYER")]
    public GameObject player;

    [Header("PLAYER CONTROL")]
    public MonoBehaviour[] scriptsToDisable;

    [Header("BAD ENDING AUDIO")]
    public AudioSource badEndingAudioSource;
    public AudioClip badEndingMusic;

    private bool locked = false;

    private void Start()
    {
        LockPlayer();
    }

    public void LockPlayer()
    {
        if (locked)
            return;

        locked = true;

        // Tắt các script điều khiển
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        if (player != null)
        {
            // Khóa input
            PlayerInput input = player.GetComponent<PlayerInput>();
            if (input != null)
                input.DeactivateInput();

            // Giữ Player đứng yên
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        // Phát nhạc Bad Ending
        if (badEndingAudioSource != null && badEndingMusic != null)
        {
            badEndingAudioSource.Stop();
            badEndingAudioSource.clip = badEndingMusic;
            badEndingAudioSource.loop = false;
            badEndingAudioSource.Play();
        }

        Debug.Log("BAD ENDING: PLAYER LOCKED");
    }
}