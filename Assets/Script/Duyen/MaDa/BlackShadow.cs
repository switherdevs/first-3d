
using UnityEngine;

public class BlackShadow : MonoBehaviour
{
    [Header("Chuyen dong bong den")]
    public float khoangLuot = 2f;
    public float tocDoLuot = 0.5f;

    private Vector3 viTriBanDau;
    private float tienTrinh;

    void Start()
    {
        viTriBanDau = transform.localPosition;
    }

    void Update()
    {
        tienTrinh += Time.deltaTime * tocDoLuot;

        float lech = Mathf.Sin(tienTrinh) * khoangLuot;

        transform.localPosition = viTriBanDau
            + Vector3.right * lech;
    }
}
