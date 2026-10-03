using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Sử dụng cho New Input System

public class DisclaimerController : MonoBehaviour
{
    [Header("--- UI Cảnh Báo (Warning Image) ---")]
    [Tooltip("Image Cảnh báo (chứa component Image để chỉnh mờ dần)")]
    public Image imageWarning;

    [Tooltip("Tốc độ mờ dần khi nhấn đồng ý (Giá trị càng lớn mờ càng nhanh)")]
    public float tocDoMo = 2.0f;

    [Header("--- Nút Bấm ---")]
    [Tooltip("Nút Đồng ý (Accept)")]
    public Button nutAccept;

    [Tooltip("Nút Từ chối (Deny)")]
    public Button nutDeny;

    [Header("--- Xử Lý Từ Chối (Deny) ---")]
    [Tooltip("GameObject sẽ hiện ra khi người chơi nhấn Deny")]
    public GameObject thongBaoDenyObject;

    [Tooltip("Thời gian (giây) thông báo Deny hiển thị trước khi tự biến mất")]
    public float thoiGianHienThongBao = 3.0f;

    private bool daXacNhan = false;
    private Coroutine coroutineDangChay;

    private void Start()
    {
        // Đăng ký sự kiện Click cho nút Accept và Deny
        if (nutAccept != null)
        {
            nutAccept.onClick.AddListener(DongY);
        }

        if (nutDeny != null)
        {
            nutDeny.onClick.AddListener(TuChoi);
        }

        // Đảm bảo ban đầu Image Warning bật và Thông báo Deny ẩn
        if (imageWarning != null) imageWarning.gameObject.SetActive(true);
        if (thongBaoDenyObject != null) thongBaoDenyObject.SetActive(false);
    }

    private void Update()
    {
        // Nếu đã xác nhận xong thì không nhận phím nữa
        if (daXacNhan) return;

        // Bắt phím Enter / Space để Đồng ý (Accept)
        bool nhanEnter = Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame ||
             Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
             Keyboard.current.spaceKey.wasPressedThisFrame);

        if (nhanEnter)
        {
            DongY();
        }
    }

    /// <summary>
    /// Hàm xử lý khi người chơi nhấn Đồng ý (Accept)
    /// </summary>
    public void DongY()
    {
        if (daXacNhan) return;
        daXacNhan = true;

        // Vô hiệu hóa tương tác nút bấm để tránh click nhiều lần
        if (nutAccept != null) nutAccept.interactable = false;
        if (nutDeny != null) nutDeny.interactable = false;

        // Bắt đầu tiến trình làm mờ dần
        StartCoroutine(MoDanVaAnImage());
    }

    /// <summary>
    /// Hàm xử lý khi người chơi nhấn Từ chối (Deny)
    /// </summary>
    public void TuChoi()
    {
        if (daXacNhan) return;

        // Nếu đang hiện thông báo Deny cũ thì dừng lại để đếm thời gian mới
        if (coroutineDangChay != null)
        {
            StopCoroutine(coroutineDangChay);
        }

        coroutineDangChay = StartCoroutine(HienThongBaoTuChoi());
    }

    /// <summary>
    /// Tiến trình Coroutine làm mờ dần Alpha của Image Warning rồi ẩn (active false)
    /// </summary>
    private IEnumerator MoDanVaAnImage()
    {
        if (imageWarning != null)
        {
            Color mauHienTai = imageWarning.color;

            // Vòng lặp giảm độ trong suốt (Alpha) từ giá trị hiện tại về 0
            while (mauHienTai.a > 0f)
            {
                mauHienTai.a -= tocDoMo * Time.deltaTime;
                imageWarning.color = mauHienTai;
                yield return null; // Chờ sang frame tiếp theo
            }

            // Đảm bảo alpha về đúng 0 và ẩn GameObject
            mauHienTai.a = 0f;
            imageWarning.color = mauHienTai;
            imageWarning.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Tiến trình Coroutine bật GameObject thông báo Deny, chờ rồi tự ẩn
    /// </summary>
    private IEnumerator HienThongBaoTuChoi()
    {
        if (thongBaoDenyObject != null)
        {
            thongBaoDenyObject.SetActive(true);

            // Chờ trong khoảng thời gian chỉ định
            yield return new WaitForSeconds(thoiGianHienThongBao);

            thongBaoDenyObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Hủy lắng nghe sự kiện
        if (nutAccept != null) nutAccept.onClick.RemoveListener(DongY);
        if (nutDeny != null) nutDeny.onClick.RemoveListener(TuChoi);
    }
}