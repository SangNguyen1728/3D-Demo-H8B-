using UnityEngine;

public class PanelToggleUI : MonoBehaviour
{
    [Tooltip("Panel muốn bật/tắt — kéo BallSelectionPanel vào đây")]
    public GameObject targetPanel;

    [Tooltip("Panel có hiện sẵn ngay lúc mới vào trận hay không")]
    public bool startVisible = false;

    void Start()
    {
        if (targetPanel != null)
            targetPanel.SetActive(startVisible);
    }

    // Gọi hàm này từ OnClick() của nút bấm
    public void TogglePanel()
    {
        if (targetPanel == null) return;

        bool newState = !targetPanel.activeSelf;
        targetPanel.SetActive(newState);

        Debug.Log($"[PanelToggleUI] Panel chọn bi: {(newState ? "BẬT" : "TẮT")}");
    }
}
