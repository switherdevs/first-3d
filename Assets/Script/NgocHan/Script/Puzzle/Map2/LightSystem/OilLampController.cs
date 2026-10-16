
using UnityEngine;

public class OilLampController : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private LightResourceManager resources;
    [SerializeField] private GameObject oilLampHolder;
    [SerializeField] private Light oilLampLight;
    [SerializeField] private FlashlightController flashlightController;

    [Header("SETTINGS")]
    [SerializeField] private KeyCode toggleKey = KeyCode.G;

    [Tooltip("Bao nhiêu giây thì trừ dầu 1 lần")]
    [SerializeField] private float oilDrainInterval = 10f;

    [Tooltip("Mỗi lần trừ bao nhiêu % dầu")]
    [SerializeField] private float oilDrainPercent = 1f;

    [Header("LIGHT STRENGTH")]
    [SerializeField] private float maxIntensity = 2.5f;
    [SerializeField] private float minRange = 2f;
    [SerializeField] private float maxRange = 7f;

    private bool isOn = false;
    private float oilTimer = 0f;

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
        // G BẬT / TẮT
        // =====================================================

        if (Input.GetKeyDown(toggleKey))
        {
            if (isOn)
                ForceOff();
            else
                TryTurnOn();
        }


        // =====================================================
        // KHÔNG BẬT -> KHÔNG HAO DẦU
        // =====================================================

        if (!isOn)
            return;


        // =====================================================
        // ĐẾM THỜI GIAN
        // =====================================================

        oilTimer += Time.deltaTime;


        if (oilTimer >= oilDrainInterval)
        {
            oilTimer = 0f;

            DrainOil();
        }


        // Cập nhật độ sáng theo lượng dầu
        UpdateLightStrength();
    }


    // =========================================================
    // TRỪ DẦU
    // =========================================================

    private void DrainOil()
    {
        // 1% của Max Oil
        float amount =
            resources.MaxOil *
            (oilDrainPercent / 100f);


        resources.ConsumeOil(amount);


        Debug.Log(
            "[OIL LAMP] Trừ " +
            oilDrainPercent +
            "% dầu. Còn: " +
            resources.CurrentOil
        );


        // =====================================================
        // HẾT DẦU
        // =====================================================

        if (resources.CurrentOil <= 0f)
        {
            Debug.Log(
                "[OIL LAMP] HẾT DẦU"
            );

            ForceOff();
        }
    }


    // =========================================================
    // BẬT ĐÈN DẦU
    // =========================================================

    private void TryTurnOn()
    {
        if (!resources.HasOilLamp)
        {
            Debug.LogWarning(
                "[OIL LAMP] Chưa nhặt đèn dầu!"
            );

            return;
        }


        if (resources.CurrentOil <= 0f)
        {
            Debug.LogWarning(
                "[OIL LAMP] Hết dầu!"
            );

            return;
        }


        // Bật đèn dầu -> tắt đèn pin
        if (flashlightController != null)
            flashlightController.ForceOff();


        isOn = true;

        oilTimer = 0f;


        if (oilLampHolder != null)
            oilLampHolder.SetActive(true);


        if (oilLampLight != null)
            oilLampLight.enabled = true;


        UpdateLightStrength();


        Debug.Log(
            "[OIL LAMP] BẬT ĐÈN"
        );
    }


    // =========================================================
    // ĐỘ SÁNG THEO DẦU
    // =========================================================

    private void UpdateLightStrength()
    {
        if (oilLampLight == null)
            return;


        float oilRatio =
            resources.MaxOil > 0f
            ? resources.CurrentOil /
              resources.MaxOil
            : 0f;


        oilRatio =
            Mathf.Clamp01(oilRatio);


        // Dầu càng ít -> đèn càng yếu
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
    // TẮT ĐÈN
    // =========================================================

    public void ForceOff()
    {
        isOn = false;

        oilTimer = 0f;


        if (oilLampLight != null)
            oilLampLight.enabled = false;


        if (oilLampHolder != null)
            oilLampHolder.SetActive(false);
    }
}