using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    [Header("--- CẤU HÌNH KHO ĐỒ HOTBAR ---")]
    [Tooltip("Danh sách các ô UI Hotbar (Kéo từ 1 tới tối đa 9 RectTransform ô đồ vào đây)")]
    public RectTransform[] danhSachOTo;

    [Tooltip("GameObject hiển thị khung chọn (Chỉ 1 cái duy nhất)")]
    public RectTransform khungChonSelect;

    [Header("--- TRẠNG THÁI Ô ĐANG CHỌN ---")]
    [Tooltip("Chỉ số ô đang chọn (0 đến Số ô - 1)")]
    public int oDangChonIndex = 0;

    private void Start()
    {
        // Kiểm tra giới hạn tối đa 9 ô đồ
        if (danhSachOTo != null && danhSachOTo.Length > 9)
        {
            Debug.LogWarning("[HotbarManager] Số ô vượt quá 9! Hệ thống sẽ tự cắt chỉ lấy 9 ô đầu tiên.");
            System.Array.Resize(ref danhSachOTo, 9);
        }

        // Cập nhật vị trí khung Select ngay khi vào game
        CapNhatViTriKhungChon();
    }

    private void Update()
    {
        XuLyLienKetPhimSo();
        XuLyLanChuot();
    }

    // 🎯 1. XỬ LÝ ĐỌC PHÍM SỐ (1 -> 9) BẰNG NEW INPUT SYSTEM
    private void XuLyLienKetPhimSo()
    {
        if (Keyboard.current == null || danhSachOTo == null || danhSachOTo.Length == 0) return;

        // Quét các phím từ Key1 đến Key9
        for (int i = 0; i < danhSachOTo.Length; i++)
        {
            Key phimTuongUng = Key.Digit1 + i;
            if (Keyboard.current[phimTuongUng].wasPressedThisFrame)
            {
                ChonODo(i);
                break;
            }
        }
    }

    // 🎯 2. XỬ LÝ LĂN CHUỘT (SCROLL WHEEL)
    private void XuLyLanChuot()
    {
        if (Mouse.current == null || danhSachOTo == null || danhSachOTo.Length == 0) return;

        float giaTriLan = Mouse.current.scroll.ReadValue().y;

        if (giaTriLan > 0f) // Lăn lên -> Chuyển ô kế tiếp
        {
            int chiSoMoi = (oDangChonIndex + 1) % danhSachOTo.Length;
            ChonODo(chiSoMoi);
        }
        else if (giaTriLan < 0f) // Lăn xuống -> Chuyển ô phía trước
        {
            int chiSoMoi = (oDangChonIndex - 1 + danhSachOTo.Length) % danhSachOTo.Length;
            ChonODo(chiSoMoi);
        }
    }

    // 🎯 3. THUẬT TOÁN ĐỔI Ô VÀ DI CHUYỂN KHUNG SELECT TỰ ĐỘNG
    public void ChonODo(int indexMoi)
    {
        if (danhSachOTo == null || indexMoi < 0 || indexMoi >= danhSachOTo.Length) return;

        oDangChonIndex = indexMoi;
        CapNhatViTriKhungChon();
    }

    private void CapNhatViTriKhungChon()
    {
        if (khungChonSelect == null || danhSachOTo == null || danhSachOTo.Length == 0) return;

        // Lấy RectTransform của ô đồ được chọn
        RectTransform oHienTai = danhSachOTo[oDangChonIndex];

        if (oHienTai != null)
        {
            // Tự động gán vị trí của Khung Select đè lên vị trí của ô đồ trong UI Canvas
            khungChonSelect.position = oHienTai.position;
        }
    }
}