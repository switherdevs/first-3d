using UnityEngine;

public class LightResourceManager : MonoBehaviour
{
    public static LightResourceManager Instance;

    // =========================================================
    // FLASHLIGHT
    // =========================================================

    [Header("FLASHLIGHT")]
    [SerializeField] private bool hasFlashlight = false;

    [SerializeField] private float maxBattery = 100f;

    [SerializeField] private float currentBattery = 0f;


    // =========================================================
    // OIL LAMP
    // =========================================================

    [Header("OIL LAMP")]
    [SerializeField] private bool hasOilLamp = false;

    [SerializeField] private float maxOil = 100f;

    [SerializeField] private float currentOil = 0f;


    // =========================================================
    // GETTERS
    // =========================================================

    public bool HasFlashlight => hasFlashlight;
    public bool HasOilLamp => hasOilLamp;

    public float CurrentBattery => currentBattery;
    public float MaxBattery => maxBattery;

    public float CurrentOil => currentOil;
    public float MaxOil => maxOil;


    public float Battery01
    {
        get
        {
            if (maxBattery <= 0f)
                return 0f;

            return Mathf.Clamp01(
                currentBattery / maxBattery
            );
        }
    }


    public float Oil01
    {
        get
        {
            if (maxOil <= 0f)
                return 0f;

            return Mathf.Clamp01(
                currentOil / maxOil
            );
        }
    }


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentBattery =
            Mathf.Clamp(
                currentBattery,
                0f,
                maxBattery
            );

        currentOil =
            Mathf.Clamp(
                currentOil,
                0f,
                maxOil
            );
    }


    // =========================================================
    // NHẶT ĐÈN PIN
    // =========================================================

    public void UnlockFlashlight(
        float startingBattery
    )
    {
        hasFlashlight = true;

        currentBattery =
            Mathf.Clamp(
                Mathf.Max(
                    currentBattery,
                    startingBattery
                ),
                0f,
                maxBattery
            );

        Debug.Log(
            "[LIGHT] Đã nhặt đèn pin. Pin = " +
            currentBattery + "%"
        );
    }


    // =========================================================
    // TRỪ PIN
    // =========================================================

    public void ConsumeBattery(float amount)
    {
        if (!hasFlashlight)
            return;

        currentBattery -= amount;

        currentBattery =
            Mathf.Clamp(
                currentBattery,
                0f,
                maxBattery
            );
    }


    // =========================================================
    // NẠP PIN
    // =========================================================

    public bool AddBattery(float percent)
    {
        // Chưa có đèn pin
        if (!hasFlashlight)
        {
            Debug.Log(
                "[LIGHT] Chưa có đèn pin."
            );

            return false;
        }

        // Pin đầy
        if (currentBattery >= maxBattery)
        {
            Debug.Log(
                "[LIGHT] Pin đã đầy."
            );

            return false;
        }

        float amount =
            maxBattery *
            (percent / 100f);

        currentBattery += amount;

        currentBattery =
            Mathf.Clamp(
                currentBattery,
                0f,
                maxBattery
            );

        Debug.Log(
            "[LIGHT] Nạp +" +
            percent +
            "% pin. Pin hiện tại = " +
            currentBattery + "%"
        );

        return true;
    }


    // =========================================================
    // NHẶT ĐÈN DẦU
    // =========================================================

    public void UnlockOilLamp(
        float startingOil
    )
    {
        hasOilLamp = true;

        currentOil =
            Mathf.Clamp(
                Mathf.Max(
                    currentOil,
                    startingOil
                ),
                0f,
                maxOil
            );

        Debug.Log(
            "[LIGHT] Đã nhặt đèn dầu. Dầu = " +
            currentOil + "%"
        );
    }


    // =========================================================
    // TRỪ DẦU
    // =========================================================

    public void ConsumeOil(float amount)
    {
        if (!hasOilLamp)
            return;

        currentOil -= amount;

        currentOil =
            Mathf.Clamp(
                currentOil,
                0f,
                maxOil
            );
    }


    // =========================================================
    // THÊM DẦU
    // =========================================================

    public bool AddOil(float percent)
    {
        // Chưa có đèn dầu
        if (!hasOilLamp)
        {
            Debug.Log(
                "[LIGHT] Chưa có đèn dầu."
            );

            return false;
        }

        // Dầu đầy
        if (currentOil >= maxOil)
        {
            Debug.Log(
                "[LIGHT] Dầu đã đầy."
            );

            return false;
        }

        float amount =
            maxOil *
            (percent / 100f);

        currentOil += amount;

        currentOil =
            Mathf.Clamp(
                currentOil,
                0f,
                maxOil
            );

        Debug.Log(
            "[LIGHT] Thêm +" +
            percent +
            "% dầu. Dầu hiện tại = " +
            currentOil + "%"
        );

        return true;
    }
}