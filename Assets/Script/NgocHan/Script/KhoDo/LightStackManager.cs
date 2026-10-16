using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LightStackManager : MonoBehaviour
{
    public static LightStackManager Instance;

    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("HOTBAR")]
    [Tooltip("Kéo Kho_do_Ui vào đây")]
    [SerializeField]
    private Transform hotbarRoot;

    [Header("ITEM ICON")]
    [Tooltip("Đúng sprite cục pin đang dùng trong SearchItem")]
    [SerializeField]
    private Sprite batteryIcon;

    [Tooltip("Đúng sprite can dầu đang dùng trong SearchItem")]
    [SerializeField]
    private Sprite oilIcon;


    // =========================================================
    // COUNT
    // =========================================================

    private int batteryCount = 0;
    private int oilCount = 0;

    public int BatteryCount => batteryCount;
    public int OilCount => oilCount;


    // =========================================================
    // TEXT ĐƯỢC TẠO TỰ ĐỘNG
    // =========================================================

    private TMP_Text batteryCountText;
    private TMP_Text oilCountText;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        Instance = this;
    }


    // =========================================================
    // ADD BATTERY
    // =========================================================

    public void AddBattery()
    {
        batteryCount++;

        RefreshBatteryUI();

        Debug.Log(
            "[STACK] Battery x" +
            batteryCount
        );
    }


    // =========================================================
    // ADD OIL
    // =========================================================

    public void AddOil()
    {
        oilCount++;

        RefreshOilUI();

        Debug.Log(
            "[STACK] Oil x" +
            oilCount
        );
    }


    // =========================================================
    // USE BATTERY
    // =========================================================

    public bool UseBattery()
    {
        if (batteryCount <= 0)
        {
            Debug.Log(
                "[STACK] Không còn pin."
            );

            return false;
        }

        if (LightResourceManager.Instance == null)
            return false;

        bool success =
            LightResourceManager.Instance
                .AddBattery(25f);

        // Pin đang đầy
        // thì không mất cục pin
        if (!success)
            return false;

        batteryCount--;

        RefreshBatteryUI();

        return true;
    }


    // =========================================================
    // USE OIL
    // =========================================================

    public bool UseOil()
    {
        if (oilCount <= 0)
        {
            Debug.Log(
                "[STACK] Không còn can dầu."
            );

            return false;
        }

        if (LightResourceManager.Instance == null)
            return false;

        bool success =
            LightResourceManager.Instance
                .AddOil(25f);

        // Dầu đầy
        // thì không mất can dầu
        if (!success)
            return false;

        oilCount--;

        RefreshOilUI();

        return true;
    }


    // =========================================================
    // BATTERY UI
    // =========================================================

    private void RefreshBatteryUI()
    {
        Image icon =
            FindItemIcon(
                batteryIcon
            );

        if (icon == null)
        {
            Debug.LogWarning(
                "[STACK] Chưa tìm thấy icon pin trong kho."
            );

            return;
        }


        // Nếu chưa có chữ
        // hoặc pin đã chuyển sang slot khác
        if (
            batteryCountText == null ||
            batteryCountText.transform.parent !=
            icon.transform.parent
        )
        {
            if (batteryCountText != null)
            {
                Destroy(
                    batteryCountText.gameObject
                );
            }

            batteryCountText =
                CreateCountText(
                    icon,
                    "BatteryCount"
                );
        }


        if (batteryCount <= 0)
        {
            batteryCountText.text = "";
            batteryCountText.gameObject
                .SetActive(false);

            return;
        }


        batteryCountText.gameObject
            .SetActive(true);

        batteryCountText.text =
            "x" + batteryCount;
    }


    // =========================================================
    // OIL UI
    // =========================================================

    private void RefreshOilUI()
    {
        Image icon =
            FindItemIcon(
                oilIcon
            );

        if (icon == null)
        {
            Debug.LogWarning(
                "[STACK] Chưa tìm thấy icon can dầu trong kho."
            );

            return;
        }


        if (
            oilCountText == null ||
            oilCountText.transform.parent !=
            icon.transform.parent
        )
        {
            if (oilCountText != null)
            {
                Destroy(
                    oilCountText.gameObject
                );
            }

            oilCountText =
                CreateCountText(
                    icon,
                    "OilCount"
                );
        }


        if (oilCount <= 0)
        {
            oilCountText.text = "";
            oilCountText.gameObject
                .SetActive(false);

            return;
        }


        oilCountText.gameObject
            .SetActive(true);

        oilCountText.text =
            "x" + oilCount;
    }


    // =========================================================
    // TÌM ICON TRONG HOTBAR
    // =========================================================

    private Image FindItemIcon(
        Sprite targetSprite
    )
    {
        if (hotbarRoot == null)
        {
            Debug.LogError(
                "[STACK] Chưa kéo Kho_do_Ui vào Hotbar Root!"
            );

            return null;
        }

        if (targetSprite == null)
            return null;


        Image[] images =
            hotbarRoot.GetComponentsInChildren<Image>(
                true
            );


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
    // TỰ TẠO x1 x2 x3
    // =========================================================

    private TMP_Text CreateCountText(
        Image icon,
        string objectName
    )
    {
        GameObject textObject =
            new GameObject(
                objectName
            );


        // Cho chữ đi theo đúng slot chứa icon
        textObject.transform.SetParent(
            icon.transform.parent,
            false
        );


        RectTransform rect =
            textObject.AddComponent<RectTransform>();


        // Neo góc phải dưới
        rect.anchorMin =
            new Vector2(
                1f,
                0f
            );

        rect.anchorMax =
            new Vector2(
                1f,
                0f
            );

        rect.pivot =
            new Vector2(
                1f,
                0f
            );


        rect.anchoredPosition =
            new Vector2(
                -4f,
                4f
            );


        rect.sizeDelta =
            new Vector2(
                55f,
                30f
            );


        TextMeshProUGUI text =
            textObject.AddComponent<TextMeshProUGUI>();


        text.text = "";

        text.fontSize = 20f;

        text.fontStyle =
            FontStyles.Bold;

        text.alignment =
            TextAlignmentOptions.BottomRight;

        text.raycastTarget = false;


        // Đưa chữ lên trên icon
        textObject.transform
            .SetAsLastSibling();


        return text;
    }


    // =========================================================
    // REFRESH THỦ CÔNG NẾU CẦN
    // =========================================================

    public void RefreshAll()
    {
        if (batteryCount > 0)
        {
            RefreshBatteryUI();
        }

        if (oilCount > 0)
        {
            RefreshOilUI();
        }
    }
}