
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatMovement : MonoBehaviour
{
    [Header("Toc do thuyen")]
    public float tocDoTien = 4f;
    public float tocDoLui = 2f;
    public float tocDoRe = 60f;

    [Header("Huong tien cua mo hinh")]
    [Tooltip("Bat neu mui xuong nam theo truc X thay vi truc Z")]
    public bool dungTrucX = true;

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
        // Lay huong tien theo truc da chon
        Vector3 huong = dungTrucX
            ? transform.right
            : transform.forward;

        huong.y = 0f;
        huong.Normalize();

        // W tien, S lui
        float tocDo = tienLui >= 0f ? tocDoTien : tocDoLui;

        Vector3 buocDiChuyen =
        huong * -tienLui * tocDo * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + buocDiChuyen);

        // A quay trai, D quay phai
        if (Mathf.Abs(re) > 0.01f)
        {
            float gocQuay =
                re * tocDoRe * Time.fixedDeltaTime;

            rb.MoveRotation(
                rb.rotation * Quaternion.Euler(0f, gocQuay, 0f)
            );
        }
    }
}
