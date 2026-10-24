using System.Collections;
using UnityEngine;

public class FinalEndingManager : MonoBehaviour
{
    [Header("ENDING UI")]
    [SerializeField] private CanvasGroup endingCanvas;

    [SerializeField] private GameObject goodEndingPanel;
    [SerializeField] private GameObject badEndingPanel;


    [Header("ENDING TIMING")]
    [SerializeField] private float fadeDuration = 2f;


    [Header("PLAYER")]
    [Tooltip("Drag PlayerMovement, MouseLook, FlashlightController... here")]
    [SerializeField] private Behaviour[] scriptsToDisable;


    [Header("AUDIO")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip goodEndingSound;
    [SerializeField] private AudioClip badEndingSound;


    [Header("DEMO TEST")]
    [SerializeField] private bool enableTestKeys = true;

    [SerializeField] private KeyCode goodEndingKey = KeyCode.P;
    [SerializeField] private KeyCode badEndingKey = KeyCode.O;


    private bool endingStarted = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Hide the ending UI at the beginning
        if (endingCanvas != null)
        {
            endingCanvas.alpha = 0f;
            endingCanvas.interactable = false;
            endingCanvas.blocksRaycasts = false;
        }


        // Hide both ending panels
        if (goodEndingPanel != null)
        {
            goodEndingPanel.SetActive(false);
        }


        if (badEndingPanel != null)
        {
            badEndingPanel.SetActive(false);
        }
    }


    // =========================================================
    // TEST
    // =========================================================

    private void Update()
    {
        if (!enableTestKeys)
            return;


        if (endingStarted)
            return;


        // P = Good Ending
        if (Input.GetKeyDown(goodEndingKey))
        {
            PlayGoodEnding();
        }


        // O = Bad Ending
        if (Input.GetKeyDown(badEndingKey))
        {
            PlayBadEnding();
        }
    }


    // =========================================================
    // GOOD ENDING
    // =========================================================

    public void PlayGoodEnding()
    {
        if (endingStarted)
            return;


        endingStarted = true;


        if (goodEndingPanel != null)
        {
            goodEndingPanel.SetActive(true);
        }


        if (badEndingPanel != null)
        {
            badEndingPanel.SetActive(false);
        }


        StartCoroutine(
            EndingSequence(goodEndingSound)
        );
    }


    // =========================================================
    // BAD ENDING
    // =========================================================

    public void PlayBadEnding()
    {
        if (endingStarted)
            return;


        endingStarted = true;


        if (goodEndingPanel != null)
        {
            goodEndingPanel.SetActive(false);
        }


        if (badEndingPanel != null)
        {
            badEndingPanel.SetActive(true);
        }


        StartCoroutine(
            EndingSequence(badEndingSound)
        );
    }


    // =========================================================
    // ENDING SEQUENCE
    // =========================================================

    private IEnumerator EndingSequence(AudioClip endingSound)
    {
        // Disable player controls
        DisablePlayer();


        // Play ending sound
        if (audioSource != null &&
            endingSound != null)
        {
            audioSource.PlayOneShot(
                endingSound
            );
        }


        // Fade the screen
        yield return StartCoroutine(
            FadeInEnding()
        );
    }


    // =========================================================
    // FADE
    // =========================================================

    private IEnumerator FadeInEnding()
    {
        if (endingCanvas == null)
            yield break;


        endingCanvas.blocksRaycasts = true;


        float timer = 0f;


        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;


            float alpha =
                Mathf.Clamp01(
                    timer / fadeDuration
                );


            endingCanvas.alpha = alpha;


            yield return null;
        }


        endingCanvas.alpha = 1f;
    }


    // =========================================================
    // DISABLE PLAYER
    // =========================================================

    private void DisablePlayer()
    {
        foreach (Behaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }
}