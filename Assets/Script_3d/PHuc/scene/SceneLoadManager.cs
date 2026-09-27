using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    [System.Serializable]
    public class ChuyenSceneCoXacNhan
    {
        [Tooltip("Gán nút Button kích hoạt mở bảng hỏi")]
        public Button nutMoBangHoi;

        [Tooltip("Gán GameObject UI Bảng hỏi xáp nhận (Popup Confirm UI)")]
        public GameObject bangHoiXacNhanUI;

        [Tooltip("Gán nút ĐỒNG Ý trong Bảng hỏi để chuyển Scene")]
        public Button nutDongYChuyenScene;

        [Tooltip("Gán nút HỦY BỎ trong Bảng hỏi để đóng bảng")]
        public Button nutHuyBo;

        [Tooltip("Tên Scene mục tiêu muốn chuyển đến")]
        public string tenSceneMucTieu;
    }

    [System.Serializable]
    public class ChuyenSceneThang
    {
        [Tooltip("Gán nút Button chuyển Scene ngay lập tức")]
        public Button nutChuyenScene;

        [Tooltip("Tên Scene mục tiêu muốn chuyển đến")]
        public string tenSceneMucTieu;
    }

    [Header("--- MỤC 1: CHUYỂN SCENE CÓ BẢNG HỎI XÁC NHẬN ---")]
    public ChuyenSceneCoXacNhan[] danhSachChuyenSceneCoXacNhan;

    [Header("--- MỤC 2: CHUYỂN SCENE TRỰC TIẾP ---")]
    public ChuyenSceneThang[] danhSachChuyenSceneThang;

    private void Start()
    {
        KhoiTaoMuc1_CoXacNhan();
        KhoiTaoMuc2_ChuyenThang();
    }

    // --- CẤU HÌNH MỤC 1 ---
    private void KhoiTaoMuc1_CoXacNhan()
    {
        if (danhSachChuyenSceneCoXacNhan == null) return;

        foreach (var item in danhSachChuyenSceneCoXacNhan)
        {
            if (item == null) continue;

            // An bang hoi UI ban dau
            if (item.bangHoiXacNhanUI != null)
            {
                item.bangHoiXacNhanUI.SetActive(false);
            }

            // Gan su kien cho Nut Mo Bang Hoi
            if (item.nutMoBangHoi != null)
            {
                item.nutMoBangHoi.onClick.AddListener(() =>
                {
                    if (item.bangHoiXacNhanUI != null)
                    {
                        item.bangHoiXacNhanUI.SetActive(true);
                        
                        // Hien va mo khoa con tro chuot de nguoi choi click nut UI
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                });
            }

            // Gan su kien cho Nut Dong Y
            if (item.nutDongYChuyenScene != null)
            {
                item.nutDongYChuyenScene.onClick.AddListener(() =>
                {
                    ThucHienChuyenScene(item.tenSceneMucTieu);
                });
            }

            // Gan su kien cho Nut Huy Bo
            if (item.nutHuyBo != null)
            {
                item.nutHuyBo.onClick.AddListener(() =>
                {
                    if (item.bangHoiXacNhanUI != null)
                    {
                        item.bangHoiXacNhanUI.SetActive(false);
                        
                        // Khoa lai con tro chuot neu quay lai game
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                });
            }
        }
    }

    // --- CẤU HÌNH MỤC 2 ---
    private void KhoiTaoMuc2_ChuyenThang()
    {
        if (danhSachChuyenSceneThang == null) return;

        foreach (var item in danhSachChuyenSceneThang)
        {
            if (item == null) continue;

            if (item.nutChuyenScene != null)
            {
                item.nutChuyenScene.onClick.AddListener(() =>
                {
                    ThucHienChuyenScene(item.tenSceneMucTieu);
                });
            }
        }
    }

    // --- THUẬT TOÁN CHUYỂN SCENE CHÍNH ---
    public void ThucHienChuyenScene(string tenScene)
    {
        if (string.IsNullOrEmpty(tenScene))
        {
            Debug.LogError("[SceneLoadManager] Tên Scene đang bị để trống!");
            return;
        }

        // dam bao thoi gian game khong bi khung neu dang pause
        Time.timeScale = 1f;

        // Load scene theo ten
        SceneManager.LoadScene(tenScene);
    }

    private void OnDestroy()
    {
        // Dọn dẹp các sự kiện Listener để tránh rò rỉ bộ nhớ (Memory Leak)
        XoaToanBoListeners();
    }

    private void XoaToanBoListeners()
    {
        if (danhSachChuyenSceneCoXacNhan != null)
        {
            foreach (var item in danhSachChuyenSceneCoXacNhan)
            {
                if (item.nutMoBangHoi != null) item.nutMoBangHoi.onClick.RemoveAllListeners();
                if (item.nutDongYChuyenScene != null) item.nutDongYChuyenScene.onClick.RemoveAllListeners();
                if (item.nutHuyBo != null) item.nutHuyBo.onClick.RemoveAllListeners();
            }
        }

        if (danhSachChuyenSceneThang != null)
        {
            foreach (var item in danhSachChuyenSceneThang)
            {
                if (item.nutChuyenScene != null) item.nutChuyenScene.onClick.RemoveAllListeners();
            }
        }
    }
}