using UnityEngine;
using TMPro;

public class PanelToggleUI : MonoBehaviour
{
    //[Tooltip("Panel muốn bật/tắt — kéo BallSelectionPanel vào đây")]
    //public GameObject targetPanel;

    //[Tooltip("Panel có hiện sẵn ngay lúc mới vào trận hay không")]
    //public bool startVisible = false;

    //void Start()
    //{
    //    if (targetPanel != null)
    //        targetPanel.SetActive(startVisible);
    //}

    //// Gọi hàm này từ OnClick() của nút bấm
    //public void TogglePanel()
    //{
    //    if (targetPanel == null) return;

    //    bool newState = !targetPanel.activeSelf;
    //    targetPanel.SetActive(newState);

    //    Debug.Log($"[PanelToggleUI] Panel chọn bi: {(newState ? "BẬT" : "TẮT")}");
    //}

    [Tooltip("Panel muốn bật/tắt — kéo BallSelectionPanel vào đây")]
    public GameObject targetPanel;

    [Tooltip("Panel có hiện sẵn ngay lúc mới vào trận hay không")]
    public bool startVisible = false;

    [Tooltip("Tự đóng Panel ngay sau khi người chơi chọn bi thành công")]
    public bool closeAfterSelect = true;

    [Tooltip("(Tuỳ chọn) Text hiện tên bi đang chọn — đặt NGOÀI Panel để vẫn thấy khi Panel đóng. Để trống nếu không cần")]
    public TMP_Text currentBallLabel;

    void Start()
    {
        if (targetPanel != null)
            targetPanel.SetActive(startVisible);

        BallInventoryManager manager = BallInventoryManager.Instance;
        if (manager == null) return;

        if (closeAfterSelect)
            manager.OnUserSelectedBall += ClosePanel;

        manager.OnBallSelected += UpdateCurrentBallLabel;
        UpdateCurrentBallLabel(manager.CurrentConfig);
    }

    void OnDestroy()
    {
        BallInventoryManager manager = BallInventoryManager.Instance;
        if (manager == null) return;

        manager.OnUserSelectedBall -= ClosePanel;
        manager.OnBallSelected -= UpdateCurrentBallLabel;
    }

    public void TogglePanel()
    {
        if (targetPanel == null) return;

        bool newState = !targetPanel.activeSelf;
        targetPanel.SetActive(newState);

        Debug.Log($"[PanelToggleUI] Panel chọn bi: {(newState ? "BẬT" : "TẮT")}");
    }

    public void ClosePanel()
    {
        if (targetPanel == null) return;

        targetPanel.SetActive(false);
        Debug.Log("[PanelToggleUI] Panel chọn bi: TẮT (tự đóng sau khi chọn)");
    }

    private void UpdateCurrentBallLabel(BallScaleConfig config)
    {
        if (currentBallLabel != null && config != null)
            currentBallLabel.text = config.displayName;
    }
}
