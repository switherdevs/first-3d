using System.Collections;
using UnityEngine;

public class DeadWaterZone : MonoBehaviour
{
    [Header("--- Cấu hình Vùng Kích Hoạt ---")]
    [Tooltip("Tag của Player để kiểm tra va chạm")]
    public string tagNhanVat = "Player";

    [Tooltip("Độ lệch Y tối đa cho phép kích hoạt (tránh bị kích hoạt từ tầng dưới)")]
    public float doLechYToiDa = 1.0f;

    [Header("--- Cấu hình Ma Da Kéo ---")]
    [Tooltip("Độ sâu sẽ bị kéo xuống (mét, tính theo giá trị âm Y)")]
    public float doSauKeo = 8f;

    [Tooltip("Tốc độ Ma Da kéo xuống (mét/giây)")]
    public float tocDoKeo = 3f;

    [Tooltip("Âm thanh hiệu ứng khi bị kéo (không bắt buộc)")]
    public AudioClip amThanhMaDa;

    private bool dangKeoNhanVat = false;

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra đúng Tag Player và chưa bị kéo
        if (!dangKeoNhanVat && other.CompareTag(tagNhanVat))
        {
            // Bổ sung kiểm tra: Tọa độ Y của Player phải nằm ở gần bề mặt vùng nước
            // Nếu Player đứng quá thấp dưới sàn (tầng dưới), bỏ qua không kích hoạt
            float doChechLechY = transform.position.y - other.transform.position.y;

            if (doChechLechY <= doLechYToiDa)
            {
                dangKeoNhanVat = true;
                StartCoroutine(CoKeoNhanVatXuong(other.gameObject));
            }
        }
    }

    private IEnumerator CoKeoNhanVatXuong(GameObject playerObj)
    {
        // 1. Vô hiệu hóa di chuyển người chơi
        PlayerMovement scriptDiChuyen = playerObj.GetComponent<PlayerMovement>();
        if (scriptDiChuyen != null)
        {
            scriptDiChuyen.enabled = false;
        }

        // Tắt trọng lực Rigidbody để tránh xung đột vật lý khi dìm xuống
        Rigidbody rb = playerObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 2. Tắt script CameraController để Camera đứng yên cố định tại vị trí hiện tại
        CameraController scriptCam = FindFirstObjectByType<CameraController>();
        if (scriptCam != null)
        {
            scriptCam.enabled = false;
        }

        // 3. Phát âm thanh Ma Da (nếu có)
        if (amThanhMaDa != null)
        {
            AudioSource.PlayClipAtPoint(amThanhMaDa, playerObj.transform.position);
        }

        // 4. Thực hiện dìm nhân vật xuống theo trục Y
        Vector3 viTriBanDau = playerObj.transform.position;
        float viTriYThapNhat = viTriBanDau.y - doSauKeo;

        while (playerObj.transform.position.y > viTriYThapNhat)
        {
            Vector3 viTriMoi = playerObj.transform.position;
            viTriMoi.y -= tocDoKeo * Time.deltaTime;

            if (viTriMoi.y < viTriYThapNhat)
            {
                viTriMoi.y = viTriYThapNhat;
            }

            playerObj.transform.position = viTriMoi;
            yield return null;
        }
    }
}