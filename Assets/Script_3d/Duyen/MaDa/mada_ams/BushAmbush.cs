using UnityEngine;
using UnityEngine.UI; // Thêm thư viện để làm việc với UI Button

public class BushAmbushTrap : MonoBehaviour
{
    [Header("Cài đặt Bẫy Bụi Cây")]
    [Tooltip("GameObject của Ma Da cần hiện lên")]
    public GameObject maDaObject;
    [Tooltip("Vị trí cố định Ma Da sẽ xuất hiện ngay lập tức")]
    public Transform fixedSpawnPoint;
    [Tooltip("Số lần cần nhấn phím E liên tục để thoát")]
    public int soLanNhanECanThiet = 10;
    [Tooltip("Thời gian giới hạn (giây) phải nhấn đủ số lần, nếu không sẽ chết")]
    public float thoiGianGioiHan = 3f;

    [Header("Cấu hình Giao diện Canvas UI khi bị bắt")]
    [Tooltip("GameObject Canvas UI hoặc Panel hiển thị khi bị bắt (Sẽ tự động bật/tắt)")]
    public GameObject trapCanvasUI;
    [Tooltip("UI Button được chỉ định sẽ chạy hiệu ứng click mỗi lần người chơi nhấn phím E")]
    public Button actionButton;

    [Header("Cấu hình Rung Camera khi bị bắt")]
    [Tooltip("Biên độ rung lắc camera khi đang dính bẫy (Càng cao càng dữ dội)")]
    public float cuongDoRungCamera = 0.5f;

    [Header("Âm thanh")]
    public AudioClip jumpscareSound;
    public AudioSource audioSource;

    private bool daDinhTrap = false;
    private bool daThoat = false;
    private int soLanDaNhanE = 0;
    private float thoiGianConLai = 0f;

    private PlayerMovement playerMovementRef;
    private Transform mainCamTransform;
    private Vector3 cameraLocalPosBanDau;
    private bool đangRungCam = false;

    void Start()
    {
        if (maDaObject != null)
            maDaObject.SetActive(false);

        // Ban đầu ẩn bảng Canvas UI giãy giụa đi
        if (trapCanvasUI != null)
            trapCanvasUI.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (Camera.main != null)
        {
            mainCamTransform = Camera.main.transform;
            cameraLocalPosBanDau = mainCamTransform.localPosition;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (daThoat || daDinhTrap) return;

        // Bắt buộc kiểm tra đúng Tag Player
        if (other.CompareTag("Player"))
        {
            playerMovementRef = other.GetComponent<PlayerMovement>();
            KichHoatTrap();
        }
    }

    void KichHoatTrap()
    {
        daDinhTrap = true;
        soLanDaNhanE = 0;
        thoiGianConLai = thoiGianGioiHan;
        đangRungCam = true;

        // 1. Khóa di chuyển của nhân vật thông qua hàm có sẵn trong PlayerMovement
        if (playerMovementRef != null)
        {
            playerMovementRef.SetKhoaDiChuyen(true);
        }

        // 2. Hiện Canvas UI thông báo dính bẫy lên màn hình
        if (trapCanvasUI != null)
        {
            trapCanvasUI.SetActive(true);
        }

        // 3. Hiện Ma Da ngay lập tức tại vị trí cố định
        if (maDaObject != null)
        {
            if (fixedSpawnPoint != null)
            {
                maDaObject.transform.position = fixedSpawnPoint.position;
                maDaObject.transform.rotation = fixedSpawnPoint.rotation;
            }
            maDaObject.SetActive(true);
        }

        // 4. Phát âm thanh hù dọa
        if (audioSource != null && jumpscareSound != null)
        {
            audioSource.PlayOneShot(jumpscareSound);
        }

        Debug.Log("Bị bẫy bụi cây tóm gọn! Hãy nhấn liên tục phím E để thoát!");
    }

    void Update()
    {
        if (!daDinhTrap || daThoat) return;

        // Xử lý hiệu ứng rung camera liên tục khi đang bị giữ chân
        if (đangRungCam && mainCamTransform != null)
        {
            Vector3 diemRungNgauNhien = Random.insideUnitSphere * cuongDoRungCamera;
            mainCamTransform.localPosition = cameraLocalPosBanDau + diemRungNgauNhien;
        }

        // Đếm ngược thời gian sinh tử
        thoiGianConLai -= Time.deltaTime;

        // Lắng nghe phím E nhấn liên tục
        if (Input.GetKeyDown(KeyCode.E))
        {
            soLanDaNhanE++;
            Debug.Log("Đang giãy giụa thoát thân... Số lần E: " + soLanDaNhanE + "/" + soLanNhanECanThiet);

            // Kích hoạt hiệu ứng click trên UI Button mỗi lần bấm E
            KichHoatHieuUngClickButton();

            if (soLanDaNhanE >= soLanNhanECanThiet)
            {
                ThoatTrap();
                return;
            }
        }

        // Hết thời gian mà chưa nhấn đủ -> Chết
        if (thoiGianConLai <= 0f)
        {
            Chet();
        }
    }

    /// <summary>
    /// Giả lập hiệu ứng nút bấm UI được click (chuyển sang trạng thái Pressed)
    /// </summary>
    private void KichHoatHieuUngClickButton()
    {
        if (actionButton != null)
        {
            // Kích hoạt sự kiện OnClick của Button (nếu bạn muốn gọi các hàm gắn sẵn trong Inspector)
            actionButton.onClick.Invoke();

            // Gọi Coroutine để tạo hiệu ứng chuyển đổi đồ họa sang trạng thái Pressed rồi trả về Normal
            StartCoroutine(Routine_HieuUngNhanNut(actionButton));
        }
    }

    private System.Collections.IEnumerator Routine_HieuUngNhanNut(Button btn)
    {
        // Chuyển UI Button sang trạng thái Highlighted / Pressed trực tiếp qua Selectable
        btn.transform.localScale = Vector3.one * 0.9f; // Thu nhỏ nhẹ nút tạo cảm giác đang bị nhấn
        yield return new WaitForSecondsRealtime(0.08f); // Chờ ngắn trong thời gian thực
        btn.transform.localScale = Vector3.one;       // Trở lại kích thước ban đầu
    }

    void ThoatTrap()
    {
        daThoat = true;
        daDinhTrap = false;
        đangRungCam = false;

        // Trả lại vị trí camera mượt mà ban đầu và dừng rung mạnh
        if (mainCamTransform != null)
        {
            mainCamTransform.localPosition = cameraLocalPosBanDau;
        }

        // Ẩn bảng Canvas UI đi khi đã thoát
        if (trapCanvasUI != null)
        {
            trapCanvasUI.SetActive(false);
        }

        // Mở khóa di chuyển cho nhân vật
        if (playerMovementRef != null)
        {
            playerMovementRef.SetKhoaDiChuyen(false);
        }

        Debug.Log("Đã thoát khỏi bẫy thành công!");

        if (maDaObject != null)
        {
            maDaObject.SetActive(false); // Ẩn Ma Da đi
        }

        // Vô hiệu hóa bẫy vĩnh viễn
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void Chet()
    {
        daDinhTrap = false;
        đangRungCam = false;

        if (mainCamTransform != null)
        {
            mainCamTransform.localPosition = cameraLocalPosBanDau;
        }

        // Ẩn bảng Canvas UI khi chết (để nhường chỗ cho màn hình Game Over chính nếu có)
        if (trapCanvasUI != null)
        {
            trapCanvasUI.SetActive(false);
        }

        Debug.Log("Người chơi đã chết vì không giãy thoát kịp!");

        // Gọi hàm kết liễu từ PlayerSurvivalManager
        PlayerSurvivalManager survival = FindFirstObjectByType<PlayerSurvivalManager>();
        if (survival != null)
        {
            survival.PlayerCaught();
        }

        if (maDaObject != null) maDaObject.SetActive(false);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}