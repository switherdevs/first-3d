using UnityEngine;

public class OilLampController : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private LightResourceManager resources;
    [SerializeField] private GameObject oilLampHolder;
    [SerializeField] private Light oilLampLight;
    [SerializeField] private FlashlightController flashlightController;


    // =========================================================
    // AUDIO
    // =========================================================
    [Header("--- Audio ---")]

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip oilLampClick;

    // =========================================================
    // SETTINGS
    // =========================================================

    [Header("SETTINGS")]
    [SerializeField] private KeyCode toggleKey = KeyCode.G;

    [Tooltip("How many seconds before draining oil")]
    [SerializeField] private float oilDrainInterval = 10f;

    [Tooltip("How much oil percentage is drained each time")]
    [SerializeField] private float oilDrainPercent = 1f;


    // =========================================================
    // LIGHT STRENGTH
    // =========================================================

    [Header("LIGHT STRENGTH")]
    [SerializeField] private float maxIntensity = 2.5f;
    [SerializeField] private float minRange = 2f;
    [SerializeField] private float maxRange = 7f;


    // =========================================================
    // PRIVATE
    // =========================================================

    private bool isOn = false;
    private float oilTimer = 0f;

    public bool IsOn => isOn;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (resources == null)
            resources = LightResourceManager.Instance;

        // Turn off at game start without playing sound
        SetLampOff(false);
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (resources == null)
            return;


        // =====================================================
        // G TOGGLE ON / OFF
        // =====================================================

        if (Input.GetKeyDown(toggleKey))
        {
            if (isOn)
                ForceOff();
            else
                TryTurnOn();
        }


        // =====================================================
        // IF OFF -> DO NOT DRAIN OIL
        // =====================================================

        if (!isOn)
            return;


        // =====================================================
        // OIL TIMER
        // =====================================================

        oilTimer += Time.deltaTime;


        if (oilTimer >= oilDrainInterval)
        {
            oilTimer = 0f;

            DrainOil();
        }


        // Update light strength based on oil amount
        UpdateLightStrength();
    }


    // =========================================================
    // DRAIN OIL
    // =========================================================

    private void DrainOil()
    {
        float amount =
            resources.MaxOil *
            (oilDrainPercent / 100f);


        resources.ConsumeOil(amount);


        Debug.Log(
            "[OIL LAMP] Drained " +
            oilDrainPercent +
            "% oil. Remaining: " +
            resources.CurrentOil
        );


        // =====================================================
        // OUT OF OIL
        // =====================================================

        if (resources.CurrentOil <= 0f)
        {
            Debug.Log(
                "[OIL LAMP] OUT OF OIL"
            );

            ForceOff();
        }
    }


    // =========================================================
    // TURN ON OIL LAMP
    // =========================================================

    private void TryTurnOn()
    {
        if (!resources.HasOilLamp)
        {
            Debug.LogWarning(
                "[OIL LAMP] Oil lamp has not been collected!"
            );

            return;
        }


        if (resources.CurrentOil <= 0f)
        {
            Debug.LogWarning(
                "[OIL LAMP] No oil remaining!"
            );

            return;
        }


        // Turn on oil lamp -> turn off flashlight
        if (flashlightController != null)
            flashlightController.ForceOff();


        isOn = true;

        oilTimer = 0f;


        if (oilLampHolder != null)
            oilLampHolder.SetActive(true);


        if (oilLampLight != null)
            oilLampLight.enabled = true;


        UpdateLightStrength();


        // Play click sound
        PlayClickSound();


        Debug.Log(
            "[OIL LAMP] TURNED ON"
        );
    }


    // =========================================================
    // UPDATE LIGHT STRENGTH
    // =========================================================

    private void UpdateLightStrength()
    {
        if (oilLampLight == null || resources == null)
            return;


        float oilRatio =
            resources.MaxOil > 0f
            ? resources.CurrentOil /
              resources.MaxOil
            : 0f;


        oilRatio =
            Mathf.Clamp01(oilRatio);


        // Less oil -> weaker light
        oilLampLight.intensity =
            maxIntensity * oilRatio;


        oilLampLight.range =
            Mathf.Lerp(
                minRange,
                maxRange,
                oilRatio
            );
    }


    // =========================================================
    // TURN OFF OIL LAMP
    // =========================================================

    public void ForceOff()
    {
        bool wasOn = isOn;

        SetLampOff(false);

        // Only play sound if it was actually on
        if (wasOn)
        {
            PlayClickSound();
        }
    }


    // =========================================================
    // INTERNAL TURN OFF
    // =========================================================

    private void SetLampOff(bool playSound)
    {
        isOn = false;

        oilTimer = 0f;


        if (oilLampLight != null)
            oilLampLight.enabled = false;


        if (oilLampHolder != null)
            oilLampHolder.SetActive(false);


        if (playSound)
        {
            PlayClickSound();
        }
    }


    // =========================================================
    // PLAY CLICK SOUND
    // =========================================================

    private void PlayClickSound()
    {
        if (audioSource == null)
            return;

        if (oilLampClick == null)
            return;


        audioSource.PlayOneShot(oilLampClick);
    }
}