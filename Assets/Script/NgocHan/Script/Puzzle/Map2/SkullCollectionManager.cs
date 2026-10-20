using UnityEngine;

public class SkullCollectionManager : MonoBehaviour
{
    public static SkullCollectionManager Instance { get; private set; }

    [Header("SỐ ĐẦU LÂU")]
    [SerializeField] private int totalSkulls = 10;

    private int collectedSkulls = 0;

    private bool missionStarted = false;
    private bool completed = false;


    private void Awake()
    {
        Instance = this;
    }


    // =========================================================
    // BIA ĐÁ GỌI HÀM NÀY
    // =========================================================

    public void StartSkullMission()
    {
        if (missionStarted)
            return;

        missionStarted = true;

        if (MissionUIManager.Instance != null)
        {
            MissionUIManager.Instance.ShowMission(
                "NHIỆM VỤ",
                "Thu thập đầu lâu: 0/" + totalSkulls
            );
        }
    }


    // =========================================================
    // ĐẦU LÂU GỌI HÀM NÀY
    // =========================================================

    public void CollectSkull()
    {
        if (!missionStarted)
        {
            Debug.Log("Chưa nhận nhiệm vụ từ bia đá.");
            return;
        }

        if (completed)
            return;


        collectedSkulls++;


        if (collectedSkulls > totalSkulls)
        {
            collectedSkulls = totalSkulls;
        }


        if (MissionUIManager.Instance != null)
        {
            MissionUIManager.Instance.UpdateObjective(
                "Thu thập đầu lâu: "
                + collectedSkulls
                + "/"
                + totalSkulls
            );
        }


        // =====================================================
        // ĐỦ 10
        // =====================================================

        if (collectedSkulls >= totalSkulls)
        {
            completed = true;

            Win();
        }
    }


    private void Win()
    {
        Debug.Log("WIN - Đã thu thập đủ 10 đầu lâu");


        if (MissionUIManager.Instance != null)
        {
            MissionUIManager.Instance.UpdateObjective(
                "Đã thu thập đủ 10 đầu lâu."
            );
        }

        // Sau này gọi WinPanel ở đây.
    }


    public bool MissionStarted()
    {
        return missionStarted;
    }
}