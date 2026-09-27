using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class CauHinhUI
    {
        [Tooltip("Tên gợi nhớ cho bảng UI (Ví dụ: Bảng Cài Đặt, Bảng Đồ Dùng...)")]
        public string tenBangUI = "Bảng UI";

        [Tooltip("GameObject Bảng UI cần Bật/Tắt")]
        public GameObject bangUI;

        [Header("--- Nút Bấm Kích Hoạt ---")]
        [Tooltip("Gán Nút (Button) dùng để BẬT/MỞ bảng UI này")]
        public Button nutBatUI;

        [Tooltip("Gán Nút (Button) dùng để TẮT/ĐÓNG/RESUME bảng UI này")]
        public Button nutTatUI;

        [Header("--- Cấu hình Menu Chính (Phím ESC) ---")]
        [Tooltip("Tick chọn nếu đây là Main Menu / Pause Menu có thể bật/tắt bằng phím ESC")]
        public bool laMainMenuGiamGame = false;

        [Tooltip("Dừng thời gian game (Pause) khi mở bảng UI này?")]
        public bool dungThoiGianKhiMo = false;
    }

    [Header("--- THAM CHIẾU HỆ THỐNG ---")]
    [Tooltip("Gán CameraController vào đây để tự động khóa xoay góc nhìn khi mở UI")]
    public CameraController boDieuKhienCamera;

    [Header("--- DANH SÁCH CÁC BẢNG UI TRONG GAME ---")]
    public CauHinhUI[] danhSachUI;

    private void Start()
    {
        // Tự động tìm CameraController nếu người chơi quên kéo thả vào Inspector
        if (boDieuKhienCamera == null)
        {
            boDieuKhienCamera = FindAnyObjectByType<CameraController>();
        }

        KhoiTaoCacBangUI();
    }

    private void Update()
    {
        XuLyKiemTraPhimEsc();
    }

    // --- CẤU HÌNH TỰ ĐỘNG GÁN SỰ KIỆN CHO CÁC NÚT BẤM ---
    private void KhoiTaoCacBangUI()
    {
        if (danhSachUI == null) return;

        foreach (var item in danhSachUI)
        {
            if (item == null) continue;

            // Đảm bảo ban đầu ẩn tất cả các bảng UI
            if (item.bangUI != null)
            {
                item.bangUI.SetActive(false);
            }

            // Gán sự kiện cho Nút Bật UI
            if (item.nutBatUI != null)
            {
                item.nutBatUI.onClick.AddListener(() =>
                {
                    BatBangUI(item);
                });
            }

            // Gán sự kiện cho Nút Tắt UI (Resume)
            if (item.nutTatUI != null)
            {
                item.nutTatUI.onClick.AddListener(() =>
                {
                    TatBangUI(item);
                });
            }
        }
    }

    // --- ĐỌC TÍN HIỆU PHÍM ESC BẰNG NEW INPUT SYSTEM ---
    private void XuLyKiemTraPhimEsc()
    {
        // Kiểm tra phím Escape vừa được bấm xuống
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (danhSachUI == null) return;

            foreach (var item in danhSachUI)
            {
                // Chỉ xử lý những phần tử được tick chọn "laMainMenuGiamGame"
                if (item != null && item.laMainMenuGiamGame && item.bangUI != null)
                {
                    bool dangActive = item.bangUI.activeSelf;

                    if (dangActive)
                    {
                        TatBangUI(item);
                    }
                    else
                    {
                        BatBangUI(item);
                    }
                }
            }
        }
    }

    // --- THUẬT TOÁN BẬT UI (KHÓA CAMERA & PAUSE GAME) ---
    public void BatBangUI(CauHinhUI item)
    {
        if (item == null || item.bangUI == null) return;

        item.bangUI.SetActive(true);

        // Vô hiệu hóa CameraController để dừng hoàn toàn việc di chuột xoay góc nhìn
        if (boDieuKhienCamera != null)
        {
            boDieuKhienCamera.enabled = false;
        }

        // Hiện và mở khóa con trỏ chuột để người chơi thao tác UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Dừng thời gian nếu cấu hình yêu cầu
        if (item.dungThoiGianKhiMo)
        {
            Time.timeScale = 0f;
        }
    }

    // --- THUẬT TOÁN TẮT UI (MỞ CAMERA & RESUME GAME) ---
    public void TatBangUI(CauHinhUI item)
    {
        if (item == null || item.bangUI == null) return;

        item.bangUI.SetActive(false);

        // Kích hoạt lại CameraController để người chơi có thể xoay chuột tiếp tục game
        if (boDieuKhienCamera != null)
        {
            boDieuKhienCamera.enabled = true;
        }

        // Khóa lại con trỏ chuột khi quay lại gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Khôi phục thời gian game chạy bình thường
        if (item.dungThoiGianKhiMo)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnDestroy()
    {
        // Dọn dẹp sự kiện để tránh Memory Leak
        if (danhSachUI != null)
        {
            foreach (var item in danhSachUI)
            {
                if (item.nutBatUI != null) item.nutBatUI.onClick.RemoveAllListeners();
                if (item.nutTatUI != null) item.nutTatUI.onClick.RemoveAllListeners();
            }
        }
    }
}