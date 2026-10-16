using UnityEngine;

public class OilLampController : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("REFERENCES")]
    [SerializeField]
    private LightResourceManager resources;

    [SerializeField]
    private GameObject oilLampHolder;

    [SerializeField]
    private Light oilLampLight;

    [SerializeField]
    private FlashlightController flashlightController;

    // =========================================================
    // SETTINGS
    // =========================================================

    [Header("SETTINGS")]
    [SerializeField]
    private KeyCode toggleKey = KeyCode.G;

    [Tooltip("Lượng dầu tiêu hao mỗi giây")]
    [SerializeField]
    private float oilUsePerSecond = 1.5f;

    [Header("LIGHT STRENGTH")]

    [SerializeField]
    private float maxIntensity = 2.5f;

    [SerializeField]
    private float minRange = 2f;

    [SerializeField]
    private float maxRange = 7f;

    private bool isOn = false;

    public bool IsOn => isOn;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (resources == null)
        {
            resources =
                LightResourceManager.Instance;
        }

        ForceOff();
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (resources == null)
            return;

        if (Input.GetKeyDown(toggleKey))
        {
            if (isOn)
            {
                ForceOff();
            }
            else
            {
                TryTurnOn();
            }
        }

        if (isOn)
        {
            DrainOil();
            UpdateLightStrength();
        }
    }

    // =========================================================
    // TURN ON
    // =========================================================

    private void TryTurnOn()
    {
        // Chưa có đèn dầu
        if (!resources.HasOilLamp)
        {
            Debug.Log(
                "[OIL LAMP] Chưa có đèn dầu."
            );

            return;
        }

        // Hết dầu
        if (resources.CurrentOil <= 0f)
        {
            Debug.Log(
                "[OIL LAMP] Hết dầu."
            );

            return;
        }

        // Tắt đèn pin
        if (flashlightController != null)
        {
            flashlightController.ForceOff();
        }

        isOn = true;

        if (oilLampHolder != null)
        {
            oilLampHolder.SetActive(true);
        }

        if (oilLampLight != null)
        {
            oilLampLight.enabled = true;
        }

        UpdateLightStrength();
    }

    // =========================================================
    // DRAIN OIL
    // =========================================================

    private void DrainOil()
    {
        resources.ConsumeOil(
            oilUsePerSecond *
            Time.deltaTime
        );

        if (resources.CurrentOil <= 0f)
        {
            Debug.Log(
                "[OIL LAMP] Dầu đã hết."
            );

            ForceOff();
        }
    }

    // =========================================================
    // LIGHT STRENGTH
    // =========================================================

    private void UpdateLightStrength()
    {
        if (oilLampLight == null)
            return;

        float oilRatio =
            resources.Oil01;

        // Dầu càng ít -> đèn càng yếu
        oilLampLight.intensity =
            maxIntensity *
            oilRatio;

        oilLampLight.range =
            Mathf.Lerp(
                minRange,
                maxRange,
                oilRatio
            );
    }

    // =========================================================
    // FORCE OFF
    // =========================================================

    public void ForceOff()
    {
        isOn = false;

        if (oilLampLight != null)
        {
            oilLampLight.enabled = false;
        }

        if (oilLampHolder != null)
        {
            oilLampHolder.SetActive(false);
        }
    }
}