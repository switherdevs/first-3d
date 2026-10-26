using System.Collections;
using UnityEngine;

public class GoodEndingSceneController : MonoBehaviour
{
    [Header("PLAYER")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform sittingPoint;

    [Header("OPTIONAL ANIMATION")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string sitTriggerName = "Sit";

    [Header("PLAYER CONTROL")]
    [SerializeField] private Behaviour[] scriptsToDisable;

    [Header("CAMERA")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraPoint;

    [Header("AUDIO")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip goodEndingMusic;

    [Header("FADE")]
    [SerializeField] private CanvasGroup fadeCanvas;

    [Header("TIMING")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float endingDuration = 10f;
    [SerializeField] private float fadeOutDuration = 2f;

    private void Start()
    {
        SetupEnding();

        StartCoroutine(
            GoodEndingSequence()
        );
    }


    private void SetupEnding()
    {
        // Disable gameplay scripts
        if (scriptsToDisable != null)
        {
            foreach (Behaviour script in scriptsToDisable)
            {
                if (script != null)
                {
                    script.enabled = false;
                }
            }
        }


        // Move Player to sitting position
        if (player != null && sittingPoint != null)
        {
            CharacterController controller =
                player.GetComponent<CharacterController>();

            Rigidbody rb =
                player.GetComponent<Rigidbody>();


            if (controller != null)
            {
                controller.enabled = false;
            }


            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }


            player.SetPositionAndRotation(
                sittingPoint.position,
                sittingPoint.rotation
            );


            if (controller != null)
            {
                controller.enabled = true;
            }
        }


        // Move camera
        if (mainCamera != null && cameraPoint != null)
        {
            mainCamera.transform.SetPositionAndRotation(
                cameraPoint.position,
                cameraPoint.rotation
            );
        }


        // Play sit animation if available
        if (playerAnimator != null &&
            !string.IsNullOrEmpty(sitTriggerName))
        {
            playerAnimator.SetTrigger(
                sitTriggerName
            );
        }


        // Start completely black
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            fadeCanvas.blocksRaycasts = true;
        }
    }


    private IEnumerator GoodEndingSequence()
    {
        // Play music
        if (audioSource != null &&
            goodEndingMusic != null)
        {
            audioSource.PlayOneShot(
                goodEndingMusic
            );
        }


        // Black -> river scene
        yield return StartCoroutine(
            Fade(
                1f,
                0f,
                fadeInDuration
            )
        );


        // Enjoy the scene
        yield return new WaitForSeconds(
            endingDuration
        );


        // River scene -> black
        yield return StartCoroutine(
            Fade(
                0f,
                1f,
                fadeOutDuration
            )
        );


        Debug.Log(
            "[GOOD ENDING] Finished"
        );
    }


    private IEnumerator Fade(
        float from,
        float to,
        float duration
    )
    {
        if (fadeCanvas == null)
            yield break;


        float timer = 0f;


        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / duration
                );


            fadeCanvas.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    t
                );


            yield return null;
        }


        fadeCanvas.alpha = to;
    }
}