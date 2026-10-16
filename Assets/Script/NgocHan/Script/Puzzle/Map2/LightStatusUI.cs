using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LightStatusUI : MonoBehaviour
{
    [Header("BATTERY")]
    [SerializeField] private Image batteryFill;
    [SerializeField] private TMP_Text batteryPercentText;

    [Header("OIL")]
    [SerializeField] private Image oilFill;
    [SerializeField] private TMP_Text oilPercentText;

    private void Update()
    {
        LightResourceManager r =
            LightResourceManager.Instance;

        if (r == null)
            return;

        // ==========================
        // PIN
        // ==========================

        float batteryPercent =
            r.MaxBattery > 0
            ? r.CurrentBattery / r.MaxBattery
            : 0f;

        batteryPercent =
            Mathf.Clamp01(batteryPercent);

        if (batteryFill != null)
            batteryFill.fillAmount =
                batteryPercent;

        if (batteryPercentText != null)
        {
            batteryPercentText.text =
                Mathf.RoundToInt(
                    batteryPercent * 100f
                ) + "%";
        }


        // ==========================
        // DẦU
        // ==========================

        float oilPercent =
            r.MaxOil > 0
            ? r.CurrentOil / r.MaxOil
            : 0f;

        oilPercent =
            Mathf.Clamp01(oilPercent);

        if (oilFill != null)
            oilFill.fillAmount =
                oilPercent;

        if (oilPercentText != null)
        {
            oilPercentText.text =
                Mathf.RoundToInt(
                    oilPercent * 100f
                ) + "%";
        }
    }
}