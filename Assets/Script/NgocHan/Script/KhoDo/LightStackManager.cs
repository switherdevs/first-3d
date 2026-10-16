using UnityEngine;
using TMPro;

public class LightStackManager : MonoBehaviour
{
    public static LightStackManager Instance;

    [Header("BATTERY")]
    [SerializeField] private TMP_Text batteryCountText;

    [Header("OIL")]
    [SerializeField] private TMP_Text oilCountText;

    private int batteryCount = 0;
    private int oilCount = 0;

    public int BatteryCount => batteryCount;
    public int OilCount => oilCount;

    private void Awake()
    {
        Instance = this;

        UpdateBatteryUI();
        UpdateOilUI();
    }

    // =========================================================
    // BATTERY
    // =========================================================

    public void AddBattery()
    {
        batteryCount++;
        UpdateBatteryUI();
    }

    public bool UseBattery()
    {
        if (batteryCount <= 0)
            return false;

        batteryCount--;
        UpdateBatteryUI();

        return true;
    }

    public bool HasBattery()
    {
        return batteryCount > 0;
    }


    // =========================================================
    // OIL
    // =========================================================

    public void AddOil()
    {
        oilCount++;
        UpdateOilUI();
    }

    public bool UseOil()
    {
        if (oilCount <= 0)
            return false;

        oilCount--;
        UpdateOilUI();

        return true;
    }

    public bool HasOil()
    {
        return oilCount > 0;
    }


    // =========================================================
    // UI
    // =========================================================

    private void UpdateBatteryUI()
    {
        if (batteryCountText == null)
            return;

        if (batteryCount <= 0)
        {
            batteryCountText.text = "";
        }
        else
        {
            batteryCountText.text =
                "x" + batteryCount;
        }
    }

    private void UpdateOilUI()
    {
        if (oilCountText == null)
            return;

        if (oilCount <= 0)
        {
            oilCountText.text = "";
        }
        else
        {
            oilCountText.text =
                "x" + oilCount;
        }
    }
}