using UnityEngine;

public class Map1Interact : MonoBehaviour
{
    public enum PuzzleType
    {
        HuyPhoto,   // Ảnh Huy đeo dây chuyền
        Letter,     // Bức thư manh mối

        Number4,    // Cân cũ
        Number1,    // Đèn dầu
        Number7,    // Mảnh gương vỡ

        Box,        // Hộp gỗ

        Key,        // Chìa khóa trong hộp
        BoxClue,    // Manh mối trong hộp

        Exit        // Lối thoát
    }

    [Header("Puzzle Type")]
    public PuzzleType puzzleType;

    [Header("Nội dung khi nhấn R")]
    public string displayName = "Manh moi";

    [TextArea(3, 8)]
    public string inspectText = "Khong co noi dung.";

    // ==================================================
    // E = TƯƠNG TÁC
    // ==================================================

    public void Interact()
    {
        if (Map1PuzzleManager.Instance == null)
        {
            Debug.LogError(
                "Khong tim thay Map1PuzzleManager!"
            );

            return;
        }

        switch (puzzleType)
        {
            // ==========================================
            // ẢNH HUY
            // ==========================================

            case PuzzleType.HuyPhoto:

                Map1PuzzleManager.Instance
                    .FindHuyPhoto();

                break;

            // ==========================================
            // BỨC THƯ
            // ==========================================

            case PuzzleType.Letter:

                Map1PuzzleManager.Instance
                    .FindLetter();

                break;

            // ==========================================
            // CÂN - SỐ 4
            // ==========================================

            case PuzzleType.Number4:

                Map1PuzzleManager.Instance
                    .FindNumber4();

                break;

            // ==========================================
            // ĐÈN DẦU - SỐ 1
            // ==========================================

            case PuzzleType.Number1:

                Map1PuzzleManager.Instance
                    .FindNumber1();

                break;

            // ==========================================
            // GƯƠNG VỠ - SỐ 7
            // ==========================================

            case PuzzleType.Number7:

                Map1PuzzleManager.Instance
                    .FindNumber7();

                break;

            // ==========================================
            // HỘP GỖ
            // ==========================================

            case PuzzleType.Box:

                Map1PuzzleManager.Instance
                    .OpenCodePanel();

                break;

            // ==========================================
            // CHÌA KHÓA TRONG HỘP
            // ==========================================

            case PuzzleType.Key:

                Map1PuzzleManager.Instance
                    .FindKey();

                break;

            // ==========================================
            // MANH MỐI TRONG HỘP
            // ==========================================

            case PuzzleType.BoxClue:

                Map1PuzzleManager.Instance
                    .FindBoxClue();

                break;

            // ==========================================
            // LỐI THOÁT
            // ==========================================

            case PuzzleType.Exit:

                Map1PuzzleManager.Instance
                    .GoToExit();

                break;
        }
    }

    // ==================================================
    // R = XEM NỘI DUNG
    // ==================================================

    public void Inspect()
    {
        if (Map1PuzzleManager.Instance == null)
        {
            Debug.LogError(
                "Khong tim thay Map1PuzzleManager!"
            );

            return;
        }

        Map1PuzzleManager.Instance.ShowClue(
            displayName +
            "\n\n" +
            inspectText
        );
    }
}