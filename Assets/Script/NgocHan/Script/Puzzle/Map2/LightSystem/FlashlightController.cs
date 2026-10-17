using UnityEngine;

namespace NgocHan.Map2
{
public class FlashlightController : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private LightResourceManager resources;
    [SerializeField] private GameObject flashlightHolder;
    [SerializeField] private Light flashlightLight;
    [SerializeField] private OilLampController oilLampController;

    [Header("SETTINGS")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    [Tooltip("Bao nhiêu giây thì trừ pin 1 lần")]
    [SerializeField] private float batteryDrainInterval = 10f;

    [Tooltip("Mỗi lần trừ bao nhiêu % pin")]
    [SerializeField] private float batteryDrainPercent = 1f;

    private bool isOn = false;

    private float batteryTimer = 0f;

    public bool IsOn => isOn;


    private void Start()
    {
        if (resources == null)
            resources = LightResourceManager.Instance;

        ForceOff();
    }


    private void Update()
    {
        if (resources == null)
            return;


        // =====================================================
        // F BẬT / TẮT
        // =====================================================

        if (Input.GetKeyDown(toggleKey))
        {
            if (isOn)
                ForceOff();
            else
                TryTurnOn();
        }


        // =====================================================
        // KHÔNG BẬT -> KHÔNG TÍNH THỜI GIAN
        // =====================================================

        if (!isOn)
            return;


        // =====================================================
        // ĐẾM 10 GIÂY
        // =====================================================

        batteryTimer += Time.deltaTime;


        if (batteryTimer >= batteryDrainInterval)
        {
            batteryTimer = 0f;

            DrainBattery();
        }
    }


    // =========================================================
    // TRỪ PIN
    // =========================================================

    private void DrainBattery()
    {
        // 1% của Max Battery
        float amount =
            resources.MaxBattery *
            (batteryDrainPercent / 100f);


        resources.ConsumeBattery(amount);


        Debug.Log(
            "[FLASHLIGHT] Trừ " +
            batteryDrainPercent +
            "% pin. Còn: " +
            resources.CurrentBattery
        );


        // =====================================================
        // HẾT PIN
        // =====================================================

        if (resources.CurrentBattery <= 0f)
        {
            Debug.Log(
                "[FLASHLIGHT] HẾT PIN"
            );

            ForceOff();
        }
    }


    // =========================================================
    // BẬT ĐÈN
    // =========================================================

    private void TryTurnOn()
    {
        if (!resources.HasFlashlight)
        {
            Debug.LogWarning(
                "[FLASHLIGHT] Chưa nhặt đèn pin!"
            );

            return;
        }


        if (resources.CurrentBattery <= 0f)
        {
            Debug.LogWarning(
                "[FLASHLIGHT] Hết pin!"
            );

            return;
        }


        // Bật đèn pin -> tắt đèn dầu
        if (oilLampController != null)
            oilLampController.ForceOff();


        isOn = true;

        // Bắt đầu đếm từ 0
        batteryTimer = 0f;


        if (flashlightHolder != null)
            flashlightHolder.SetActive(true);


        if (flashlightLight != null)
            flashlightLight.enabled = true;


        Debug.Log(
            "[FLASHLIGHT] BẬT ĐÈN"
        );
    }


    // =========================================================
    // TẮT ĐÈN
    // =========================================================

    public void ForceOff()
    {
        isOn = false;

        batteryTimer = 0f;


        if (flashlightLight != null)
            flashlightLight.enabled = false;


        if (flashlightHolder != null)
            flashlightHolder.SetActive(false);
    }
}
}
