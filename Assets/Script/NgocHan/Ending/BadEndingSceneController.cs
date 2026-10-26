using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BadEndingSceneController : MonoBehaviour
{
    // =========================================================
    // PLAYER
    // =========================================================

    [Header("PLAYER")]
    [SerializeField] private Transform player;

    [Tooltip("Point where the player stands on the boat")]
    [SerializeField] private Transform playerPoint;

    [Tooltip("Player scripts that should be disabled during the cinematic")]
    [SerializeField] private Behaviour[] playerScriptsToDisable;


    // =========================================================
    // MADA
    // =========================================================

    [Header("MADA")]
    [SerializeField] private GameObject mada;

    [Tooltip("Mada position below the water")]
    [SerializeField] private Transform madaStartPoint;

    [Tooltip("Mada final position above the water")]
    [SerializeField] private Transform madaEndPoint;

    [Tooltip("How long Mada takes to rise")]
    [SerializeField] private float madaRiseDuration = 2.5f;


    // =========================================================
    // AUDIO
    // =========================================================

    [Header("AUDIO")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Ambient water / boat sound")]
    [SerializeField] private AudioClip ambienceSound;

    [Tooltip("Sound when Mada starts rising")]
    [SerializeField] private AudioClip madaRiseSound;

    [Tooltip("Horror sting when Mada is fully visible")]
    [SerializeField] private AudioClip horrorSting;


    // =========================================================
    // ENDING UI
    // =========================================================

    [Header("ENDING UI")]
    [SerializeField] private CanvasGroup endingCanvas;


    // =========================================================
    // TIMING
    // =========================================================

    [Header("TIMING")]

    [Tooltip("Time before Mada starts appearing")]
    [SerializeField] private float beforeMadaDelay = 3f;

    [Tooltip("Time to keep Mada visible before fading out")]
    [SerializeField] private float afterMadaDelay = 2f;

    [Tooltip("Fade-to-black duration")]
    [SerializeField] private float fadeDuration = 2f;


    // =========================================================
    // NEXT SCENE
    // =========================================================

    [Header("NEXT SCENE")]

    [Tooltip("Leave empty if you want the game to remain on the black screen")]
    [SerializeField] private string nextSceneName = "";


    // =========================================================
    // PRIVATE
    // =========================================================

    private bool endingStarted = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        SetupEnding();

        StartCoroutine(BadEndingSequence());
    }


    // =========================================================
    // SETUP
    // =========================================================

    private void SetupEnding()
    {
        // ---------------------------------------------
        // Place Player on the boat
        // ---------------------------------------------

        if (player != null && playerPoint != null)
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
                playerPoint.position,
                playerPoint.rotation
            );


            if (controller != null)
            {
                controller.enabled = true;
            }
        }


        // ---------------------------------------------
        // Disable Player controls
        // ---------------------------------------------

        DisablePlayerControls();


        // ---------------------------------------------
        // Put Mada below the water
        // ---------------------------------------------

        if (mada != null)
        {
            mada.SetActive(true);

            if (madaStartPoint != null)
            {
                mada.transform.SetPositionAndRotation(
                    madaStartPoint.position,
                    madaStartPoint.rotation
                );
            }
        }


        // ---------------------------------------------
        // Hide ending fade screen
        // ---------------------------------------------

        if (endingCanvas != null)
        {
            endingCanvas.alpha = 0f;
            endingCanvas.interactable = false;
            endingCanvas.blocksRaycasts = false;
        }
    }


    // =========================================================
    // BAD ENDING SEQUENCE
    // =========================================================

    private IEnumerator BadEndingSequence()
    {
        if (endingStarted)
            yield break;


        endingStarted = true;


        // ---------------------------------------------
        // Start ambience
        // ---------------------------------------------

        if (audioSource != null &&
            ambienceSound != null)
        {
            audioSource.clip = ambienceSound;
            audioSource.loop = true;
            audioSource.Play();
        }


        // Player believes they escaped
        yield return new WaitForSeconds(
            beforeMadaDelay
        );


        // ---------------------------------------------
        // Mada starts rising
        // ---------------------------------------------

        if (audioSource != null &&
            madaRiseSound != null)
        {
            audioSource.PlayOneShot(
                madaRiseSound
            );
        }


        yield return StartCoroutine(
            RiseMada()
        );


        // ---------------------------------------------
        // Horror sting
        // ---------------------------------------------

        if (audioSource != null &&
            horrorSting != null)
        {
            audioSource.PlayOneShot(
                horrorSting
            );
        }


        // Keep Mada visible
        yield return new WaitForSeconds(
            afterMadaDelay
        );


        // ---------------------------------------------
        // Fade screen to black
        // ---------------------------------------------

        yield return StartCoroutine(
            FadeToBlack()
        );


        // ---------------------------------------------
        // Optional next scene
        // ---------------------------------------------

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(
                nextSceneName
            );
        }
    }


    // =========================================================
    // MADA RISE
    // =========================================================

    private IEnumerator RiseMada()
    {
        if (mada == null ||
            madaStartPoint == null ||
            madaEndPoint == null)
        {
            yield break;
        }


        float timer = 0f;


        Vector3 startPosition =
            madaStartPoint.position;

        Vector3 endPosition =
            madaEndPoint.position;


        Quaternion startRotation =
            madaStartPoint.rotation;

        Quaternion endRotation =
            madaEndPoint.rotation;


        while (timer < madaRiseDuration)
        {
            timer += Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    timer / madaRiseDuration
                );


            // Smooth movement
            float smoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            mada.transform.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    smoothT
                );


            mada.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    smoothT
                );


            yield return null;
        }


        mada.transform.SetPositionAndRotation(
            endPosition,
            endRotation
        );
    }


    // =========================================================
    // FADE TO BLACK
    // =========================================================

    private IEnumerator FadeToBlack()
    {
        if (endingCanvas == null)
            yield break;


        endingCanvas.blocksRaycasts = true;


        float timer = 0f;


        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    timer / fadeDuration
                );


            endingCanvas.alpha = t;


            yield return null;
        }


        endingCanvas.alpha = 1f;
    }


    // =========================================================
    // DISABLE PLAYER
    // =========================================================

    private void DisablePlayerControls()
    {
        if (playerScriptsToDisable == null)
            return;


        foreach (Behaviour script in playerScriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }
}