
using UnityEngine;

public class ThuyTrieu : MonoBehaviour
{
    [Header("Cai dat thuy trieu")]
    public KeyCode phimRutNuoc = KeyCode.T;
    public float mucNuocRut = 2f;
    public float tocDoRutNuoc = 0.5f;
    public float tocDoDangNuoc = 0.5f;

    [Header("Am thanh nuoc rut")]
    public AudioSource amThanhNuocRut;

    private Vector3 viTriBanDau;
    private Vector3 viTriMucNuocRut;

    private bool dangDiChuyen = false;
    private bool nuocDangRut = false;
    private Vector3 viTriDich;

    private void Start()
    {
        viTriBanDau = transform.position;

        viTriMucNuocRut = viTriBanDau;
        viTriMucNuocRut.y -= mucNuocRut;

        viTriDich = viTriBanDau;
    }

    private void Update()
    {
        if (Input.GetKeyDown(phimRutNuoc) && !dangDiChuyen)
        {
            if (!nuocDangRut)
            {
                // Nuoc rut xuong
                viTriDich = viTriMucNuocRut;
                nuocDangRut = true;
                dangDiChuyen = true;

                if (amThanhNuocRut != null)
                {
                    amThanhNuocRut.Play();
                }
            }
            else
            {
                // Nuoc dang len lai
                viTriDich = viTriBanDau;
                nuocDangRut = false;
                dangDiChuyen = true;
            }
        }

        if (dangDiChuyen)
        {
            float tocDo = nuocDangRut
                ? tocDoRutNuoc
                : tocDoDangNuoc;

            transform.position = Vector3.MoveTowards(
                transform.position,
                viTriDich,
                tocDo * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, viTriDich) < 0.01f)
            {
                transform.position = viTriDich;
                dangDiChuyen = false;
            }
        }
    }
}
