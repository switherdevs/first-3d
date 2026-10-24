using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEndingController : MonoBehaviour
{
    [Header("ENDING UI")]
    [SerializeField] private CanvasGroup endingCanvas;

    [Header("SCENE")]
    [SerializeField] private string nextSceneName;

    [Header("TIMING")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float endingDuration = 3f;

    [Header("PLAYER")]
    [SerializeField] private Behaviour[] scriptsToDisable;

    [Header("AUDIO")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip endingSound;

    [Header("TEST")]
    [SerializeField] private KeyCode testKey = KeyCode.P;

    private bool endingStarted = false;

    private void Start()
    {
        if (endingCanvas != null)
        {
            endingCanvas.alpha = 0f;
            endingCanvas.interactable = false;
            endingCanvas.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartEnding();
        }
    }

    public void StartEnding()
    {
        if (endingStarted)
            return;

        endingStarted = true;

        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        DisablePlayer();

        if (audioSource != null && endingSound != null)
        {
            audioSource.PlayOneShot(endingSound);
        }

        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(endingDuration);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator FadeIn()
    {
        if (endingCanvas == null)
            yield break;

        endingCanvas.blocksRaycasts = true;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            endingCanvas.alpha =
                Mathf.Lerp(
                    0f,
                    1f,
                    timer / fadeDuration
                );

            yield return null;
        }

        endingCanvas.alpha = 1f;
    }

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