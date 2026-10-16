
using UnityEngine;

public class RowingSystem : MonoBehaviour
{
    [Header("Tham chieu")]
    public BoatMovement boatMovement;

    [Header("Phim dieu khien")]
    public KeyCode tien = KeyCode.W;
    public KeyCode lui = KeyCode.S;
    public KeyCode reTrai = KeyCode.A;
    public KeyCode rePhai = KeyCode.D;

    private void Awake()
    {
        if (boatMovement == null)
            boatMovement = GetComponent<BoatMovement>();
    }

    private void Update()
    {
        if (boatMovement == null)
            return;

        if (!BoatController.PlayerDangTrenThuyen)
        {
            boatMovement.NhanDieuKhien(0f, 0f);
            return;
        }

        float tienLui = 0f;
        float re = 0f;

        if (Input.GetKey(tien)) tienLui += 1f;
        if (Input.GetKey(lui)) tienLui -= 1f;
        if (Input.GetKey(reTrai)) re -= 1f;
        if (Input.GetKey(rePhai)) re += 1f;

        boatMovement.NhanDieuKhien(tienLui, re);
    }
}
