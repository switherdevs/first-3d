using System.Collections;
using UnityEngine;

public class TombLampSequence : MonoBehaviour
{
    [Header("ĐÈN NGƯỜI CHƠI TƯƠNG TÁC")]
    [SerializeField] private TombLampUnit triggerLamp;


    [Header("HÀNG ĐÈN TRÁI")]
    [SerializeField] private TombLampUnit[] leftLamps;


    [Header("HÀNG ĐÈN PHẢI")]
    [SerializeField] private TombLampUnit[] rightLamps;


    [Header("THỜI GIAN")]
    [SerializeField] private float delayAfterTrigger = 0.4f;

    [SerializeField] private float delayBetweenPairs = 0.35f;


    [Header("MANH MỐI SAU KHI BẬT HẾT ĐÈN")]
    [SerializeField] private bool showClueAfterSequence = true;

    [SerializeField] private string clueID = "tomb_lamp_clue";

    [SerializeField] private string clueName = "Đèn dẫn hồn";

    [TextArea(2, 6)]
    [SerializeField]
    private string clueHint =
        "Lửa đã thức giữa mộ sâu,\nNhững kẻ mất đầu đang chờ được tìm.";

    [SerializeField] private Sprite clueIcon;


    private bool activated = false;


    // =========================================================
    // ĐƯỢC GỌI KHI PLAYER BẤM E
    // =========================================================
    public void StartSequence()
    {
        if (activated)
            return;

        activated = true;

        StartCoroutine(LightSequence());
    }


    // =========================================================
    // CHẠY HIỆU ỨNG BẬT ĐÈN
    // =========================================================
    private IEnumerator LightSequence()
    {
        // Bật đèn đầu tiên
        if (triggerLamp != null)
            triggerLamp.TurnOn();

        yield return new WaitForSeconds(delayAfterTrigger);


        // Tìm hàng nào có nhiều đèn hơn
        int maxCount = Mathf.Max(
            leftLamps != null ? leftLamps.Length : 0,
            rightLamps != null ? rightLamps.Length : 0
        );


        // Bật từng cặp trái + phải
        for (int i = 0; i < maxCount; i++)
        {
            if (leftLamps != null &&
                i < leftLamps.Length &&
                leftLamps[i] != null)
            {
                leftLamps[i].TurnOn();
            }


            if (rightLamps != null &&
                i < rightLamps.Length &&
                rightLamps[i] != null)
            {
                rightLamps[i].TurnOn();
            }


            yield return new WaitForSeconds(delayBetweenPairs);
        }


        // Bật hết đèn xong
        SequenceCompleted();
    }


    // =========================================================
    // HOÀN THÀNH
    // =========================================================
    private void SequenceCompleted()
    {
        Debug.Log("Đã bật toàn bộ đèn trong lăng mộ.");


        if (!showClueAfterSequence)
            return;


        if (ClueJournalManager.Instance != null)
        {
            ClueJournalManager.Instance.AddClue(
                clueID,
                clueName,
                clueHint,
                clueIcon
            );
        }
        else
        {
            Debug.LogWarning(
                "Không tìm thấy ClueJournalManager."
            );
        }
    }
}