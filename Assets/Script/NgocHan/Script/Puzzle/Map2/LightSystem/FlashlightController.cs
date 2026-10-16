using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("REFERENCES")]
    [SerializeField]
    private LightResourceManager resources;

    [SerializeField]
    private GameObject flashlightHolder;

    [SerializeField]
    private Light flashlightLight;

    [SerializeField]
    private OilLampController oilLampController;

    // =========================================================
    // SETTINGS
    // =========================================================

    [Header("SETTINGS")]
    [SerializeField]
    private KeyCode toggleKey = KeyCode.F;

    [Tooltip("Số pin tiêu hao mỗi giây")]
    [SerializeField]
    private float batteryUsePerSecond = 2f;

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

        // F bật / tắt
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

        // Đang bật -> tiêu hao pin
        if (isOn)
        {
            DrainBattery();
        }
    }

    // =========================================================
    // TURN ON
    // =========================================================

    private void TryTurnOn()
    {
        // Chưa nhặt đèn pin
        if (!resources.HasFlashlight)
        {
            Debug.Log(
                "[FLASHLIGHT] Chưa có đèn pin."
            );

            return;
        }

        // Hết pin
        if (resources.CurrentBattery <= 0f)
        {
            Debug.Log(
                "[FLASHLIGHT] Hết pin."
            );

            return;
        }

        // Tắt đèn dầu nếu đang dùng
        if (oilLampController != null)
        {
            oilLampController.ForceOff();
        }

        isOn = true;

        if (flashlightHolder != null)
        {
            flashlightHolder.SetActive(true);
        }

        if (flashlightLight != null)
        {
            flashlightLight.enabled = true;
        }
    }

    // =========================================================
    // BATTERY
    // =========================================================

    private void DrainBattery()
    {
        resources.ConsumeBattery(
            batteryUsePerSecond *
            Time.deltaTime
        );

        // Hết pin -> tự tắt
        if (resources.CurrentBattery <= 0f)
        {
            Debug.Log(
                "[FLASHLIGHT] Pin đã hết."
            );

            ForceOff();
        }
    }

    // =========================================================
    // FORCE OFF
    // =========================================================

    public void ForceOff()
    {
        isOn = false;

        if (flashlightLight != null)
        {
            flashlightLight.enabled = false;
        }

        if (flashlightHolder != null)
        {
            flashlightHolder.SetActive(false);
        }
    }
}