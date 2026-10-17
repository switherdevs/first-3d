using System.Collections;
using UnityEngine;
using TMPro;

public class QuestNotificationUI : MonoBehaviour
{
    [Header("--- CẤU HÌNH THÔNG TIN ---")]
    [Tooltip("Tên nhiệm vụ hiển thị thử nghiệm")]
    public string tenNhiemVu = "Khám phá bí ẩn hồ Ma Da";

    [Header("--- CẤU HÌNH COMPONENT UI ---")]
    [Tooltip("Kéo TextMeshPro - Text UI vào đây")]
    public TextMeshProUGUI questText;

    [Tooltip("Kéo RectTransform đóng vai trò là VỊ TRÍ ĐÍCH ở góc trên bên trái vào đây")]
    public RectTransform targetTopLeftPoint;

    [Header("--- CẤU HÌNH ANIMATION (HOÀN TOÀN BẰNG CODE) ---")]
    [Tooltip("Thời gian hiện rõ Text ở giữa màn hình (giây)")]
    public float thoiGianFadeIn = 1.0f;

    [Tooltip("Thời gian dừng lại ở giữa màn hình trước khi bay (giây)")]
    public float thoiGianChieuGiuaManHinh = 0.5f;

    [Tooltip("Thời gian text bay từ giữa về góc trên bên trái (giây)")]
    public float thoiGianBay = 1.2f;

    [Tooltip("Tỷ lệ thu nhỏ text khi đang trên đường bay (Ví dụ: 0.7 = thu nhỏ còn 70%)")]
    public float tyLeThuNhoKhiBay = 0.7f;

    // Biến lưu trạng thái ban đầu
    private RectTransform textRectTransform;
    private Vector2 viTriBanDauGiuaManHinh;
    private Vector3 kichThuocBanDau;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        if (questText != null)
        {
            textRectTransform = questText.GetComponent<RectTransform>();

            // Lưu lại vị trí giữa màn hình và kích thước gốc ban đầu thiết lập trên Canvas
            viTriBanDauGiuaManHinh = textRectTransform.anchoredPosition;
            kichThuocBanDau = textRectTransform.localScale;

            // Ẩn text ban đầu
            DatAnText();
        }
    }

    private void Update()
    {
        // 🎯 BỔ SUNG: Bắt phím L (Legacy Input) để test nhanh animation khi Play Game
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowQuestNotification(tenNhiemVu);
        }
    }

    // 🎯 HÀM CHÍNH ĐỂ CÁC THÀNH VIÊN KHÁC GỌI KHI NHẬN NHIỆM VỤ
    public void ShowQuestNotification(string title)
    {
        if (questText == null || targetTopLeftPoint == null)
        {
            Debug.LogError("[QuestNotificationUI] Thiếu Component questText hoặc targetTopLeftPoint trên Inspector!");
            return;
        }

        // Cập nhật nội dung text
        questText.text = title;

        // Nếu đang chạy animation cũ thì dừng lại để chạy animation mới
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Bắt đầu chuỗi Animation bằng Code
        animationCoroutine = StartCoroutine(Co_AnimateQuestTitle());
    }

    // 🎯 COROUTINE XỬ LÝ TOÀN BỘ LUỒNG ANIMATION
    private IEnumerator Co_AnimateQuestTitle()
    {
        // === BƯỚC 0: RESET VỀ TRẠNG THÁI BAN ĐẦU Ở GIỮA MÀN HÌNH ===
        textRectTransform.anchoredPosition = viTriBanDauGiuaManHinh;
        textRectTransform.localScale = kichThuocBanDau;

        Color mauText = questText.color;
        mauText.a = 0f;
        questText.color = mauText;
        questText.gameObject.SetActive(true);

        // === BƯỚC 1: FADE IN (HIỆN DẦN) Ở GIỮA MÀN HÌNH ===
        float tgHienTai = 0f;
        while (tgHienTai < thoiGianFadeIn)
        {
            tgHienTai += Time.deltaTime;
            float phanTram = tgHienTai / thoiGianFadeIn;

            mauText.a = Mathf.Lerp(0f, 1f, phanTram);
            questText.color = mauText;

            yield return null;
        }
        mauText.a = 1f;
        questText.color = mauText;

        // Dừng lại 1 chút ở giữa màn hình cho người chơi đọc
        yield return new WaitForSeconds(thoiGianChieuGiuaManHinh);

        // === BƯỚC 2: BAY VỀ GÓC TRÊN BÊN TRÁI + THU NHỎ RỒI TRỞ VỀ KÍCH THƯỚC GỐC ===
        Vector2 viTriDich = targetTopLeftPoint.anchoredPosition;
        Vector3 kichThuocThuNho = kichThuocBanDau * tyLeThuNhoKhiBay;

        tgHienTai = 0f;
        while (tgHienTai < thoiGianBay)
        {
            tgHienTai += Time.deltaTime;
            float t = tgHienTai / thoiGianBay;

            // Dùng Mathf.SmoothStep để tạo độ mượt gia tốc (Ease In Out)
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 1. Di chuyển vị trí mượt mà từ Giữa -> Góc Trái
            textRectTransform.anchoredPosition = Vector2.Lerp(viTriBanDauGiuaManHinh, viTriDich, smoothT);

            // 2. Thu nhỏ ở nửa đoạn đường đầu (t < 0.5) và phình to lại kích thước gốc ở nửa đoạn đường sau (t > 0.5)
            if (t < 0.5f)
            {
                // Từ Kích thước gốc -> Kích thước thu nhỏ
                float phanTramThuNho = t / 0.5f;
                textRectTransform.localScale = Vector3.Lerp(kichThuocBanDau, kichThuocThuNho, phanTramThuNho);
            }
            else
            {
                // Từ Kích thước thu nhỏ -> Kích thước gốc
                float phanTramPhongTo = (t - 0.5f) / 0.5f;
                textRectTransform.localScale = Vector3.Lerp(kichThuocThuNho, kichThuocBanDau, phanTramPhongTo);
            }

            yield return null;
        }

        // Đảm bảo thông số chính xác tuyệt đối ở điểm cuối
        textRectTransform.anchoredPosition = viTriDich;
        textRectTransform.localScale = kichThuocBanDau;
    }

    // Hàm phụ trợ đặt ẩn text
    private void DatAnText()
    {
        Color mauText = questText.color;
        mauText.a = 0f;
        questText.color = mauText;
    }

    // Context Menu cho phép bạn click chuột phải vào Script trên Inspector để Test nhanh ngay trong Editor
    [ContextMenu("Test Animation Chuot Phai")]
    public void TestAnimationInEditor()
    {
        ShowQuestNotification(tenNhiemVu);
    }
}