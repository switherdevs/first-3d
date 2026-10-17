using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class LightStackManager : MonoBehaviour
{
    public static LightStackManager Instance;

    // =========================================================
    // HOTBAR
    // =========================================================

    [Header("HOTBAR")]
    [SerializeField] private Transform hotbarRoot;

    [Header("ITEM ICON")]
    [SerializeField] private Sprite batteryIcon;
    [SerializeField] private Sprite oilIcon;


    // =========================================================
    // SỐ LƯỢNG PIN / DẦU DỰ PHÒNG
    // =========================================================

    [Header("STACK COUNT")]
    [SerializeField] private int batteryCount = 0;
    [SerializeField] private int oilCount = 0;

    public int BatteryCount => batteryCount;
    public int OilCount => oilCount;


    // =========================================================
    // TEXT x1 x2 x3
    // =========================================================

    private TMP_Text batteryCountText;
    private TMP_Text oilCountText;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        RefreshAll();
    }


    // =========================================================
    // UPDATE
    // B = NẠP PIN
    // O = THÊM DẦU
    // =========================================================

    private void Update()
    {
        bool pressB = false;
        bool pressO = false;

#if ENABLE_INPUT_SYSTEM

        if (Keyboard.current != null)
        {
            if (Keyboard.current.bKey.wasPressedThisFrame)
                pressB = true;

            if (Keyboard.current.oKey.wasPressedThisFrame)
                pressO = true;
        }

#endif


#if ENABLE_LEGACY_INPUT_MANAGER

        if (Input.GetKeyDown(KeyCode.B))
            pressB = true;

        if (Input.GetKeyDown(KeyCode.O))
            pressO = true;

#endif


        // B = NẠP PIN
        if (pressB)
        {
            Debug.Log("[BATTERY] Đã bấm B");

            UseBattery();
        }


        // O = THÊM DẦU
        if (pressO)
        {
            Debug.Log("[OIL] Đã bấm O");

            UseOil();
        }
    }


    // =========================================================
    // NHẶT PIN
    // =========================================================

    public void AddBattery()
    {
        batteryCount++;

        Debug.Log(
            "[BATTERY] Nhặt pin thành công. Hiện có x" +
            batteryCount
        );

        RefreshBatteryUI();
    }


    // =========================================================
    // NHẶT DẦU
    // =========================================================

    public void AddOil()
    {
        oilCount++;

        Debug.Log(
            "[OIL] Nhặt dầu thành công. Hiện có x" +
            oilCount
        );

        RefreshOilUI();
    }


    // =========================================================
    // B = DÙNG 1 CỤC PIN
    // =========================================================

    public bool UseBattery()
    {
        Debug.Log(
            "[BATTERY] Pin dự phòng hiện có: x" +
            batteryCount
        );


        // Không có pin
        if (batteryCount <= 0)
        {
            Debug.LogWarning(
                "[BATTERY] Không có pin dự phòng!"
            );

            return false;
        }


        // Không tìm thấy ResourceManager
        if (LightResourceManager.Instance == null)
        {
            Debug.LogError(
                "[BATTERY] Không tìm thấy LightResourceManager!"
            );

            return false;
        }


        LightResourceManager resources =
            LightResourceManager.Instance;


        // Chưa có đèn pin
        if (!resources.HasFlashlight)
        {
            Debug.LogWarning(
                "[BATTERY] Chưa nhặt đèn pin!"
            );

            return false;
        }


        // Pin đang đầy
        if (resources.CurrentBattery >= resources.MaxBattery)
        {
            Debug.Log(
                "[BATTERY] Pin đang đầy 100%."
            );

            return false;
        }


        // Nạp 25%
        bool success =
            resources.AddBattery(25f);


        if (!success)
        {
            Debug.LogWarning(
                "[BATTERY] Không nạp được pin!"
            );

            return false;
        }


        // Trừ 1 cục
        batteryCount--;

        Debug.Log(
            "[BATTERY] Nạp thành công. Pin còn: " +
            resources.CurrentBattery +
            "/" +
            resources.MaxBattery +
            " | Pin dự phòng: x" +
            batteryCount
        );


        RefreshBatteryUI();

        return true;
    }


    // =========================================================
    // O = DÙNG 1 CAN DẦU
    // =========================================================

    public bool UseOil()
    {
        Debug.Log(
            "[OIL] Dầu dự phòng hiện có: x" +
            oilCount
        );


        // Không có dầu
        if (oilCount <= 0)
        {
            Debug.LogWarning(
                "[OIL] Không có dầu dự phòng!"
            );

            return false;
        }


        // Không tìm thấy ResourceManager
        if (LightResourceManager.Instance == null)
        {
            Debug.LogError(
                "[OIL] Không tìm thấy LightResourceManager!"
            );

            return false;
        }


        LightResourceManager resources =
            LightResourceManager.Instance;


        // Chưa có đèn dầu
        if (!resources.HasOilLamp)
        {
            Debug.LogWarning(
                "[OIL] Chưa nhặt đèn dầu!"
            );

            return false;
        }


        // Dầu đang đầy
        if (resources.CurrentOil >= resources.MaxOil)
        {
            Debug.Log(
                "[OIL] Dầu đang đầy 100%."
            );

            return false;
        }


        // Thêm 25%
        bool success =
            resources.AddOil(25f);


        if (!success)
        {
            Debug.LogWarning(
                "[OIL] Không thêm được dầu!"
            );

            return false;
        }


        // Trừ 1 can
        oilCount--;

        Debug.Log(
            "[OIL] Thêm dầu thành công. Dầu còn: " +
            resources.CurrentOil +
            "/" +
            resources.MaxOil +
            " | Dầu dự phòng: x" +
            oilCount
        );


        RefreshOilUI();

        return true;
    }


    // =========================================================
    // UPDATE UI PIN
    // =========================================================

    private void RefreshBatteryUI()
    {
        Image icon = FindItemIcon(batteryIcon);

        if (icon == null)
        {
            Debug.LogWarning(
                "[BATTERY] Chưa tìm thấy icon pin trong kho."
            );

            return;
        }


        if (batteryCountText == null)
        {
            batteryCountText =
                CreateCountText(
                    icon,
                    "BatteryCountText"
                );
        }


        if (batteryCount <= 0)
        {
            batteryCountText.text = "";
            batteryCountText.gameObject.SetActive(false);

            return;
        }


        batteryCountText.gameObject.SetActive(true);

        batteryCountText.text =
            "x" + batteryCount;

        batteryCountText.transform.SetAsLastSibling();
    }


    // =========================================================
    // UPDATE UI DẦU
    // =========================================================

    private void RefreshOilUI()
    {
        Image icon = FindItemIcon(oilIcon);

        if (icon == null)
        {
            Debug.LogWarning(
                "[OIL] Chưa tìm thấy icon dầu trong kho."
            );

            return;
        }


        if (oilCountText == null)
        {
            oilCountText =
                CreateCountText(
                    icon,
                    "OilCountText"
                );
        }


        if (oilCount <= 0)
        {
            oilCountText.text = "";
            oilCountText.gameObject.SetActive(false);

            return;
        }


        oilCountText.gameObject.SetActive(true);

        oilCountText.text =
            "x" + oilCount;

        oilCountText.transform.SetAsLastSibling();
    }


    // =========================================================
    // TÌM ICON TRONG KHO
    // =========================================================

    private Image FindItemIcon(Sprite targetSprite)
    {
        if (hotbarRoot == null)
        {
            Debug.LogError(
                "[STACK] Chưa gắn Kho_do_Ui vào Hotbar Root!"
            );

            return null;
        }


        if (targetSprite == null)
        {
            Debug.LogError(
                "[STACK] Chưa gắn Battery Icon hoặc Oil Icon!"
            );

            return null;
        }


        Image[] images =
            hotbarRoot.GetComponentsInChildren<Image>(true);


        foreach (Image image in images)
        {
            if (image == null)
                continue;


            if (image.sprite == targetSprite)
            {
                return image;
            }
        }


        return null;
    }


    // =========================================================
    // TẠO CHỮ x1 x2 x3
    // =========================================================

    private TMP_Text CreateCountText(
        Image icon,
        string objectName
    )
    {
        GameObject textObject =
            new GameObject(
                objectName,
                typeof(RectTransform)
            );


        textObject.transform.SetParent(
            icon.transform.parent,
            false
        );


        RectTransform rect =
            textObject.GetComponent<RectTransform>();


        // Góc phải dưới của ô
        rect.anchorMin =
            new Vector2(1f, 0f);

        rect.anchorMax =
            new Vector2(1f, 0f);

        rect.pivot =
            new Vector2(1f, 0f);

        rect.anchoredPosition =
            new Vector2(-4f, 4f);

        rect.sizeDelta =
            new Vector2(55f, 30f);


        TextMeshProUGUI text =
            textObject.AddComponent<TextMeshProUGUI>();


        text.text = "";

        text.fontSize = 20f;

        text.fontStyle =
            FontStyles.Bold;

        text.alignment =
            TextAlignmentOptions.BottomRight;

        text.raycastTarget = false;


        textObject.transform.SetAsLastSibling();


        return text;
    }


    // =========================================================
    // REFRESH ALL
    // =========================================================

    public void RefreshAll()
    {
        RefreshBatteryUI();
        RefreshOilUI();
    }
}