using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LightStatusUI : MonoBehaviour
{
    [Header("RESOURCE MANAGER")]
    [SerializeField]
    private LightResourceManager resources;


    [Header("BATTERY UI")]
    [SerializeField]
    private Image batteryFill;

    [SerializeField]
    private TMP_Text batteryPercentText;


    [Header("OIL UI")]
    [SerializeField]
    private Image oilFill;

    [SerializeField]
    private TMP_Text oilPercentText;


    private void Update()
    {
        if (resources == null)
        {
            resources = LightResourceManager.Instance;

            if (resources == null)
                return;
        }

        UpdateBatteryUI();
        UpdateOilUI();
    }

    // =========================================================
    // BATTERY
    // =========================================================

    private void UpdateBatteryUI()
    {
        float percent =
            resources.CurrentBattery /
            resources.MaxBattery;

        percent = Mathf.Clamp01(percent);

        if (batteryFill != null)
            batteryFill.fillAmount = percent;

        if (batteryPercentText != null)
            batteryPercentText.text =
                Mathf.RoundToInt(percent * 100f) + "%";
    }


    // =========================================================
    // OIL
    // ========================================================= 

    private void UpdateOilUI()
    {
        float percent =
            resources.CurrentOil /
            resources.MaxOil;

        percent = Mathf.Clamp01(percent);

        if (oilFill != null)
            oilFill.fillAmount = percent;

        if (oilPercentText != null)
            oilPercentText.text =
                Mathf.RoundToInt(percent * 100f) + "%";
    }
}