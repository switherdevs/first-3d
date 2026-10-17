using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class KeyRebindManager : MonoBehaviour
{
    [System.Serializable]
    public struct RebindItem
    {
        [Tooltip("Action cần đổi phím (Ví dụ: Interact, Jump...)")]
        public InputActionReference inputActionReference;
        [Tooltip("Button UI tương ứng")]
        public Button rebindButton;
        [Tooltip("Text hiển thị tên phím trên Button")]
        public TextMeshProUGUI buttonText;
    }

    [Header("--- Danh Sách Phím Cần Đổi ---")]
    public RebindItem[] rebindItems;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Start()
    {
        // Duyệt qua mảng và tự động gán sự kiện click cho từng Button
        foreach (var item in rebindItems)
        {
            var currentItem = item; // Tránh lỗi closure trong vòng lặp foreach

            if (currentItem.rebindButton != null)
            {
                currentItem.rebindButton.onClick.AddListener(() => StartRebind(currentItem));
            }

            CapNhatGiaoDienPhim(currentItem);
        }
    }

    private void StartRebind(RebindItem item)
    {
        if (item.inputActionReference == null) return;

        InputAction action = item.inputActionReference.action;
        action.Disable();

        if (item.buttonText != null)
        {
            item.buttonText.text = "...";
        }

        // Bắt đầu lắng nghe phím mới từ người dùng
        rebindingOperation = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse") // Loại trừ chuột nếu muốn
            .OnComplete(operation => CompleteRebind(item))
            .OnCancel(operation => CompleteRebind(item))
            .Start();
    }

    private void CompleteRebind(RebindItem item)
    {
        rebindingOperation.Dispose();
        item.inputActionReference.action.Enable();
        CapNhatGiaoDienPhim(item);
    }

    private void CapNhatGiaoDienPhim(RebindItem item)
    {
        if (item.inputActionReference != null && item.buttonText != null)
        {
            // Lấy tên phím hiện tại hiển thị lên Text của Button
            string tenPhim = item.inputActionReference.action.GetBindingDisplayString(0);
            item.buttonText.text = tenPhim;
        }
    }
}