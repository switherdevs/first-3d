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

        [Tooltip("Gán Animator của bảng UI này vào đây (Để tự động chạy khi Pause Game)")]
        public Animator animatorUI;

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

        [Header("--- Cấu hình Con trỏ chuột ---")]
        [Tooltip("Tick chọn nếu khi tắt UI này KHÔNG muốn khóa và ẩn chuột (Dành cho Main Menu, giao diện chọn màn...)")]
        public bool khongKhoaChuotKhiTat = false;
    }

    [Header("--- THAM CHIẾU HỆ THỐNG ---")]
    [Tooltip("Gán CameraController vào đây để tự động khóa xoay góc nhìn khi mở UI")]
    public CameraController boDieuKhienCamera;

    [Header("--- CẤU HÌNH ÂM THANH CLICK ---")]
    [Tooltip("AudioClip tiếng Click nút bấm")]
    public AudioClip amThanhClick;

    [Tooltip("AudioSource phát tiếng Click (Nếu để trống script tự lấy/tạo)")]
    public AudioSource amThanhSource;

    [Header("--- DANH SÁCH CÁC BẢNG UI TRONG GAME ---")]
    public CauHinhUI[] danhSachUI;

    private void Start()
    {
        // Tự động kiểm tra và tạo AudioSource nếu bị thiếu
        if (amThanhSource == null)
        {
            amThanhSource = GetComponent<AudioSource>();
            if (amThanhSource == null)
            {
                amThanhSource = gameObject.AddComponent<AudioSource>();
            }
        }

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

    // --- HÀM PHÁT ÂM THANH CLICK 1 LẦN ---
    public void PhatAmThanhClick()
    {
        if (amThanhSource != null && amThanhClick != null)
        {
            amThanhSource.PlayOneShot(amThanhClick);
        }
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
                    PhatAmThanhClick();
                    BatBangUI(item);
                });
            }

            // Gán sự kiện cho Nút Tắt UI (Resume)
            if (item.nutTatUI != null)
            {
                item.nutTatUI.onClick.AddListener(() =>
                {
                    PhatAmThanhClick();
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

                    PhatAmThanhClick();

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

    // --- XỬ LÝ CHUYỂN ANIMATOR SANG UNSCALED TIME ---
    private void KiemTraVaCauHinhAnimatorUI(CauHinhUI item)
    {
        if (item == null) return;

        if (item.animatorUI != null)
        {
            item.animatorUI.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        if (item.bangUI != null)
        {
            Animator[] dsAnimators = item.bangUI.GetComponentsInChildren<Animator>(true);
            foreach (var ani in dsAnimators)
            {
                if (ani != null)
                {
                    ani.updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }
        }
    }

    // --- THUẬT TOÁN BẬT UI (KHÓA CAMERA & PAUSE GAME) ---
    public void BatBangUI(CauHinhUI item)
    {
        if (item == null || item.bangUI == null) return;

        KiemTraVaCauHinhAnimatorUI(item);

        item.bangUI.SetActive(true);

        if (boDieuKhienCamera != null)
        {
            boDieuKhienCamera.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

        // Kiểm tra nếu cấu hình yêu cầu KHÔNG khóa chuột thì giữ nguyên trạng thái chuột
        if (item.khongKhoaChuotKhiTat)
        {
            // Chỉ bật lại Camera để nhìn, giữ nguyên chuột hiển thị
            if (boDieuKhienCamera != null)
            {
                boDieuKhienCamera.enabled = true;
            }
        }
        else
        {
            // Mặc định gameplay thông thường: Khóa và ẩn chuột
            if (boDieuKhienCamera != null)
            {
                boDieuKhienCamera.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (item.dungThoiGianKhiMo)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnDestroy()
    {
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