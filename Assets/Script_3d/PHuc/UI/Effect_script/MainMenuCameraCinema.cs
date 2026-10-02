using UnityEngine;

public class MainMenuCameraCinema : MonoBehaviour
{
    [Header("--- Vị trí Mặc định ---")]
    [Tooltip("Transform vị trí Camera gốc ở Main Menu")]
    public Transform diemGocMainMenu;

    [Tooltip("Transform vị trí Camera khi mở bảng Setting")]
    public Transform diemGocSetting;

    [Header("--- Cấu hình Xoay & Zoom theo Chuột ---")]
    [Tooltip("Góc xoay tối đa Camera nhìn theo chuột (độ)")]
    public float gocGocXoayToiDa = 5f;

    [Tooltip("Độ Zoom vào tối đa khi rê chuột ra xa tâm (mét)")]
    public float doZoomToiDa = 1.5f;

    [Tooltip("Tốc độ chuyển động của Camera (Càng nhỏ càng mượt và chậm)")]
    public float tocDoMuot = 2f;

    [Header("--- Cấu hình Đung đưa Tự nhiên ---")]
    [Tooltip("Độ đung đưa run rẩy nhẹ của Camera")]
    public float doDungDua = 0.03f;

    [Tooltip("Tốc độ đung đưa")]
    public float tocDoDungDua = 1.2f;

    [Header("--- UI Canvas ---")]
    public GameObject bangMainMenuUI;
    public GameObject bangSettingUI;

    private bool dangOSetting = false;
    private Vector3 viTriBanDauCam;
    private Quaternion gocXoayBanDauCam;

    private void Start()
    {
        if (diemGocMainMenu != null)
        {
            transform.position = diemGocMainMenu.position;
            transform.rotation = diemGocMainMenu.rotation;
        }

        if (bangMainMenuUI != null) bangMainMenuUI.SetActive(true);
        if (bangSettingUI != null) bangSettingUI.SetActive(false);
    }

    private void Update()
    {
        // 1. Xác định gốc tọa độ tham chiếu (MainMenu hoặc Setting)
        Transform gocHienTai = dangOSetting ? diemGocSetting : diemGocMainMenu;
        if (gocHienTai == null) return;

        // 2. Tính toán vị trí con trỏ chuột trên màn hình (-0.5 đến 0.5)
        float chuotDiChuyenX = Input.GetAxis("Mouse X");
        float chuotDiChuyenY = Input.GetAxis("Mouse Y");

        Vector2 toadoChuotChuanHoa = Vector2.zero;

        // Nếu người chơi có di chuyển chuột -> Tính tọa độ lệch tâm
        if (Mathf.Abs(chuotDiChuyenX) > 0.001f || Mathf.Abs(chuotDiChuyenY) > 0.001f)
        {
            toadoChuotChuanHoa.x = (Input.mousePosition.x / Screen.width) - 0.5f;
            toadoChuotChuanHoa.y = (Input.mousePosition.y / Screen.height) - 0.5f;
        }
        // Nếu chuột đứng yên -> toadoChuotChuanHoa giữ nguyên là Vector2.zero (Mặc định)

        // 3. Tính toán Góc Xoay theo chuột
        float gocXoayPitch = -toadoChuotChuanHoa.y * gocGocXoayToiDa; // Xoay lên/xuống
        float gocXoayYaw = toadoChuotChuanHoa.x * gocGocXoayToiDa;     // Xoay trái/phải
        Quaternion gocXoayMucTieu = gocHienTai.rotation * Quaternion.Euler(gocXoayPitch, gocXoayYaw, 0f);

        // 4. Tính toán Độ Zoom (Tiến sát về phía trước theo hướng Camera)
        float khoangCachTâm = toadoChuotChuanHoa.magnitude; // Độ lệch từ tâm màn hình
        Vector3 viTriZoomMucTieu = gocHienTai.position + (gocHienTai.forward * (khoangCachTâm * doZoomToiDa));

        // 5. Cộng thêm hiệu ứng đung đưa run rẩy nhẹ (Sin/Cos)
        float dungDuaY = Mathf.Sin(Time.time * tocDoDungDua) * doDungDua;
        float dungDuaX = Mathf.Cos(Time.time * tocDoDungDua * 0.8f) * doDungDua;
        Vector3 viTriDungDua = new Vector3(dungDuaX, dungDuaY, 0f);

        // 6. Thực hiện nội soy Lerp & Slerp mượt mà từ từ
        transform.position = Vector3.Lerp(transform.position, viTriZoomMucTieu + viTriDungDua, Time.deltaTime * tocDoMuot);
        transform.rotation = Quaternion.Slerp(transform.rotation, gocXoayMucTieu, Time.deltaTime * tocDoMuot);
    }

    public void MoBangSetting()
    {
        dangOSetting = true;
        if (bangMainMenuUI != null) bangMainMenuUI.SetActive(false);
        if (bangSettingUI != null) bangSettingUI.SetActive(true);
    }

    public void DongBangSetting()
    {
        dangOSetting = false;
        if (bangSettingUI != null) bangSettingUI.SetActive(false);
        if (bangMainMenuUI != null) bangMainMenuUI.SetActive(true);
    }
}