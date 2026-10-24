using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private LightResourceManager resources;
    [SerializeField] private GameObject flashlightHolder;
    [SerializeField] private Light flashlightLight;
    [SerializeField] private OilLampController oilLampController;


    // =========================================================
    // AUDIO
    // =========================================================

    [Header("--- Âm thanh (Audio) ---")]

    [Tooltip("AudioSource dùng để phát âm thanh đèn pin")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Âm thanh click khi bật / tắt đèn pin")]
    [SerializeField] private AudioClip flashlightClick;


    // =========================================================
    // SETTINGS
    // =========================================================

    [Header("SETTINGS")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    [Tooltip("Bao nhiêu giây thì trừ pin 1 lần")]
    [SerializeField] private float batteryDrainInterval = 10f;

    [Tooltip("Mỗi lần trừ bao nhiêu % pin")]
    [SerializeField] private float batteryDrainPercent = 1f;


    // =========================================================
    // PRIVATE
    // =========================================================

    private bool isOn = false;

    private float batteryTimer = 0f;

    public bool IsOn => isOn;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (resources == null)
            resources = LightResourceManager.Instance;

        // Tắt đèn lúc bắt đầu game
        // Không phát âm thanh
        SetFlashlightOff(false);
    }


    // =========================================================
    // UPDATE
    // =========================================================

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
            {
                ForceOff();
            }
            else
            {
                TryTurnOn();
            }
        }


        // =====================================================
        // KHÔNG BẬT -> KHÔNG TÍNH THỜI GIAN
        // =====================================================

        if (!isOn)
            return;


        // =====================================================
        // ĐẾM THỜI GIAN TRỪ PIN
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
        // Tính % pin dựa trên Max Battery
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
        // Chưa nhặt đèn pin
        if (!resources.HasFlashlight)
        {
            Debug.LogWarning(
                "[FLASHLIGHT] Chưa nhặt đèn pin!"
            );

            return;
        }


        // Hết pin
        if (resources.CurrentBattery <= 0f)
        {
            Debug.LogWarning(
                "[FLASHLIGHT] Hết pin!"
            );

            return;
        }


        // =====================================================
        // BẬT ĐÈN PIN -> TẮT ĐÈN DẦU
        // =====================================================

        if (oilLampController != null)
        {
            oilLampController.ForceOff();
        }


        isOn = true;

        // Bắt đầu đếm pin từ 0
        batteryTimer = 0f;


        // Bật holder
        if (flashlightHolder != null)
        {
            flashlightHolder.SetActive(true);
        }


        // Bật Light
        if (flashlightLight != null)
        {
            flashlightLight.enabled = true;
        }


        // Phát âm thanh click
        PlayClickSound();


        Debug.Log(
            "[FLASHLIGHT] BẬT ĐÈN"
        );
    }


    // =========================================================
    // TẮT ĐÈN
    // =========================================================

    public void ForceOff()
    {
        // Chỉ phát tiếng click nếu đèn đang bật
        bool wasOn = isOn;

        SetFlashlightOff(false);

        if (wasOn)
        {
            PlayClickSound();
        }
    }


    // =========================================================
    // TẮT ĐÈN NỘI BỘ
    // =========================================================

    private void SetFlashlightOff(bool playSound)
    {
        isOn = false;

        batteryTimer = 0f;


        // Tắt Light
        if (flashlightLight != null)
        {
            flashlightLight.enabled = false;
        }


        // Tắt Holder
        if (flashlightHolder != null)
        {
            flashlightHolder.SetActive(false);
        }


        // Phát âm thanh nếu yêu cầu
        if (playSound)
        {
            PlayClickSound();
        }
    }


    // =========================================================
    // ÂM THANH CLICK
    // =========================================================

    private void PlayClickSound()
    {
        if (audioSource == null)
            return;

        if (flashlightClick == null)
            return;


        audioSource.PlayOneShot(flashlightClick);
    }
}