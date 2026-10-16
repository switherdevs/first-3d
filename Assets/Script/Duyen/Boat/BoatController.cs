
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Tham chieu")]
    public Transform viTriNguoiChoi;
    public Transform viTriLenThuyen;

    [Header("Cai dat")]
    public KeyCode phimTuongTac = KeyCode.E;
    public float khoangCachXuongThuyen = 2f;

    private bool nguoiChoiGanThuyen;
    private bool dangTrenThuyen;

    private Transform chaBanDau;
    private Vector3 scaleBanDau;
    private bool gravityBanDau;
    private bool isKinematicBanDau;

    private PlayerMovement playerMovement;
    private Rigidbody rbPlayer;
    private Collider colliderPlayer;

    public static bool PlayerDangTrenThuyen { get; private set; }

    private void Start()
    {
        if (viTriNguoiChoi == null)
        {
            GameObject player =
                GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                viTriNguoiChoi = player.transform;
        }

        if (viTriNguoiChoi == null)
        {
            Debug.LogError("Chua gan Player vao BoatController!");
            return;
        }

        chaBanDau = viTriNguoiChoi.parent;
        scaleBanDau = viTriNguoiChoi.localScale;

        playerMovement =
            viTriNguoiChoi.GetComponent<PlayerMovement>();

        rbPlayer =
            viTriNguoiChoi.GetComponent<Rigidbody>();

        colliderPlayer =
            viTriNguoiChoi.GetComponent<Collider>();

        if (rbPlayer != null)
        {
            gravityBanDau = rbPlayer.useGravity;
            isKinematicBanDau = rbPlayer.isKinematic;
        }
    }

    private void Update()
    {
        if (nguoiChoiGanThuyen &&
            Input.GetKeyDown(phimTuongTac))
        {
            if (!dangTrenThuyen)
                LenThuyen();
            else
                XuongThuyen();
        }
    }

    public bool DangTrenThuyen()
    {
        return dangTrenThuyen;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
            nguoiChoiGanThuyen = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null
            && !dangTrenThuyen)
        {
            nguoiChoiGanThuyen = false;
        }
    }

    private void LenThuyen()
    {
        if (viTriNguoiChoi == null || viTriLenThuyen == null)
        {
            Debug.LogError(
                "Chua gan Player hoac ViTriLenThuyen!"
            );
            return;
        }

        // Luu kich thuoc hien thi va vi tri len thuyen
        Vector3 scaleGoc = viTriNguoiChoi.lossyScale;
        Vector3 viTriDung = viTriLenThuyen.position;
        Quaternion huongDung = viTriLenThuyen.rotation;

        // Tam dung dieu khien nhan vat
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Tam dung vat ly cua Player
        if (rbPlayer != null)
        {
            rbPlayer.linearVelocity = Vector3.zero;
            rbPlayer.angularVelocity = Vector3.zero;
            rbPlayer.useGravity = false;
            rbPlayer.isKinematic = true;
        }

        // Gan Player vao thuyen
        viTriNguoiChoi.SetParent(transform, true);

        // Dat Player tai diem len thuyen
        viTriNguoiChoi.SetPositionAndRotation(
            viTriDung,
            huongDung
        );

        // Giu kich thuoc hien thi ban dau
        Vector3 scaleXuong = transform.lossyScale;

        if (Mathf.Abs(scaleXuong.x) > 0.0001f &&
            Mathf.Abs(scaleXuong.y) > 0.0001f &&
            Mathf.Abs(scaleXuong.z) > 0.0001f)
        {
            viTriNguoiChoi.localScale = new Vector3(
                scaleGoc.x / scaleXuong.x,
                scaleGoc.y / scaleXuong.y,
                scaleGoc.z / scaleXuong.z
            );
        }

        dangTrenThuyen = true;
        nguoiChoiGanThuyen = true;
        PlayerDangTrenThuyen = true;

        Debug.Log(
            "Len thuyen: Player parent = " +
            viTriNguoiChoi.parent.name
        );
    }

    private void XuongThuyen()
    {
        if (viTriNguoiChoi == null)
            return;

        Vector3 viTriXuong =
            transform.position -
            transform.right * khoangCachXuongThuyen;

        // Tach Player khoi thuyen
        viTriNguoiChoi.SetParent(chaBanDau, true);
        viTriNguoiChoi.position = viTriXuong;
        viTriNguoiChoi.localScale = scaleBanDau;

        dangTrenThuyen = false;
        nguoiChoiGanThuyen = false;
        PlayerDangTrenThuyen = false;

        // Khoi phuc vat ly ban dau
        if (rbPlayer != null)
        {
            rbPlayer.isKinematic = isKinematicBanDau;
            rbPlayer.useGravity = gravityBanDau;
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        Debug.Log("Player da xuong thuyen!");
    }
}
