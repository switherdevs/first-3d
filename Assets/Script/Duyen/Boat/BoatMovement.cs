
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatMovement : MonoBehaviour
{
    [Header("Toc do thuyen")]
    public float tocDoTien = 4f;
    public float tocDoLui = 2f;
    public float tocDoRe = 60f;

    [Header("Huong tien cua mo hinh")]
    public bool dungTrucX = true;

    [Header("Gioi han vung nuoc")]
    public bool batGioiHan = true;
    public float minX = -20f;
    public float maxX = 20f;
    public float minZ = -20f;
    public float maxZ = 20f;

    private Rigidbody rb;
    private float tienLui;
    private float re;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void NhanDieuKhien(float inputTienLui, float inputRe)
    {
        tienLui = Mathf.Clamp(inputTienLui, -1f, 1f);
        re = Mathf.Clamp(inputRe, -1f, 1f);
    }

    private void FixedUpdate()
    {
        Vector3 huong = dungTrucX
            ? transform.right
            : transform.forward;

        huong.y = 0f;
        huong.Normalize();

        float tocDo = tienLui >= 0f
            ? tocDoTien
            : tocDoLui;

        Vector3 buocDiChuyen =
            huong * -tienLui * tocDo * Time.fixedDeltaTime;

        Vector3 viTriMoi = rb.position + buocDiChuyen;

        // Khong cho tam xuong vuot qua ranh gioi
        if (batGioiHan)
        {
            viTriMoi.x = Mathf.Clamp(viTriMoi.x, minX, maxX);
            viTriMoi.z = Mathf.Clamp(viTriMoi.z, minZ, maxZ);
        }

        rb.MovePosition(viTriMoi);

        // A quay trai, D quay phai
        if (Mathf.Abs(re) > 0.01f)
        {
            float gocQuay =
                re * tocDoRe * Time.fixedDeltaTime;

            rb.MoveRotation(
                rb.rotation *
                Quaternion.Euler(0f, gocQuay, 0f)
            );
        }
    }
}
