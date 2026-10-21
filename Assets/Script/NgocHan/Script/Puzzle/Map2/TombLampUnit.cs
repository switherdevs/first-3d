using UnityEngine;

public class TombLampUnit : MonoBehaviour
{
    [Header("THÀNH PHẦN ĐÈN")]
    [SerializeField] private Light lampLight;
    [SerializeField] private GameObject flame;

    [Header("TRẠNG THÁI BAN ĐẦU")]
    [SerializeField] private bool startOn = false;

    public bool IsOn { get; private set; }


    private void Awake()
    {
        if (startOn)
            TurnOn();
        else
            TurnOff();
    }


    // =========================================================
    // BẬT ĐÈN
    // =========================================================
    public void TurnOn()
    {
        IsOn = true;

        if (lampLight != null)
            lampLight.enabled = true;

        if (flame != null)
            flame.SetActive(true);
    }


    // =========================================================
    // TẮT ĐÈN
    // =========================================================
    public void TurnOff()
    {
        IsOn = false;

        if (lampLight != null)
            lampLight.enabled = false;

        if (flame != null)
            flame.SetActive(false);
    }
}