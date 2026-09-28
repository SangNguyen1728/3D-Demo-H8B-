using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySelectionPanelUI : MonoBehaviour
{
    //[Header("Panel")]
    //public GameObject inventoryPanel;
    //public Button openButton;
    //public Button closeButton;

    //[Header("Ball")]
    //public BallScaleConfig normalBallConfig;
    //public List<InventorySlotUI> ballSlots = new List<InventorySlotUI>();

    //[Header("Loadout")]
    //[Tooltip("Số bi tối đa được chọn cho 1 trận")]
    //public int maxBallLoadout = 3;

    //private List<BallScaleConfig> selectedBalls = new List<BallScaleConfig>();

    //void Start()
    //{
    //    if (inventoryPanel != null)
    //        inventoryPanel.SetActive(false);

    //    if (openButton != null)
    //        openButton.onClick.AddListener(OpenPanel);

    //    if (closeButton != null)
    //        closeButton.onClick.AddListener(ClosePanel);

    //    if (SaveManager.Instance != null)
    //        SaveManager.Instance.LoadSavedSelection();

    //    selectedBalls = SceneLoader.Instance != null && SceneLoader.Instance.SelectedBallLoadout != null && SceneLoader.Instance.SelectedBallLoadout.Count > 0
    //        ? new List<BallScaleConfig>(SceneLoader.Instance.SelectedBallLoadout)
    //        : new List<BallScaleConfig> { normalBallConfig };

    //    if (SceneLoader.Instance != null)
    //        SceneLoader.Instance.SetSelectedBallLoadout(selectedBalls);

    //    foreach (var slot in ballSlots)
    //    {
    //        slot.Init(OnBallSlotClicked, (c) => selectedBalls.Contains(c as BallScaleConfig));
    //    }
    //}

    //private void OnBallSlotClicked(EquipmentConfig config)
    //{
    //    BallScaleConfig ball = config as BallScaleConfig;
    //    if (ball == null) return;

    //    if (selectedBalls.Contains(ball))
    //    {
    //        selectedBalls.Remove(ball);
    //        Debug.Log($"[InventorySelectionPanelUI] Bỏ chọn: {ball.displayName}");
    //    }
    //    else
    //    {
    //        if (selectedBalls.Count >= maxBallLoadout)
    //        {
    //            Debug.Log($"[InventorySelectionPanelUI] Đã chọn đủ {maxBallLoadout} bi — bỏ chọn bớt trước khi chọn thêm");
    //            return;
    //        }

    //        selectedBalls.Add(ball);
    //        Debug.Log($"[InventorySelectionPanelUI] Đã chọn: {ball.displayName}");
    //    }

    //    if (SceneLoader.Instance != null)
    //        SceneLoader.Instance.SetSelectedBallLoadout(selectedBalls);

    //    foreach (var slot in ballSlots)
    //        slot.RefreshHighlight();
    //}

    //public void OpenPanel()
    //{
    //    if (inventoryPanel != null)
    //        inventoryPanel.SetActive(true);
    //}

    //public void ClosePanel()
    //{
    //    if (inventoryPanel != null)
    //        inventoryPanel.SetActive(false);

    //    if (SaveManager.Instance != null)
    //        SaveManager.Instance.SaveCurrentSelection();
    //}

    [Header("Panel")]
    public GameObject inventoryPanel;
    public Button openButton;
    public Button closeButton;

    [Header("Ball")]
    public BallScaleConfig normalBallConfig;
    public List<InventorySlotUI> ballSlots = new List<InventorySlotUI>();

    [Header("Loadout")]
    [Tooltip("Số bi tối đa được chọn cho 1 trận")]
    public int maxBallLoadout = 3;

    [Header("Cue (chưa làm — để dành cho tương lai)")]
    public List<InventorySlotUI> cueSlots = new List<InventorySlotUI>();   // 🆕

    private List<BallScaleConfig> selectedBalls = new List<BallScaleConfig>();

    void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (openButton != null)
            openButton.onClick.AddListener(OpenPanel);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        if (SaveManager.Instance != null)
            SaveManager.Instance.LoadSavedSelection();

        selectedBalls = SceneLoader.Instance != null && SceneLoader.Instance.SelectedBallLoadout != null && SceneLoader.Instance.SelectedBallLoadout.Count > 0
            ? new List<BallScaleConfig>(SceneLoader.Instance.SelectedBallLoadout)
            : new List<BallScaleConfig> { normalBallConfig };

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.SetSelectedBallLoadout(selectedBalls);

        foreach (var slot in ballSlots)
        {
            slot.Init(OnBallSlotClicked, (c) => selectedBalls.Contains(c as BallScaleConfig));
        }

        // 🆕 Ô Cơ: chưa có item nào nên tất cả tự động thành ô khoá
        foreach (var slot in cueSlots)
        {
            slot.Init(OnCueSlotClicked, (c) => false);
        }
    }

    private void OnBallSlotClicked(EquipmentConfig config)
    {
        BallScaleConfig ball = config as BallScaleConfig;
        if (ball == null) return;

        if (selectedBalls.Contains(ball))
        {
            selectedBalls.Remove(ball);
            Debug.Log($"[InventorySelectionPanelUI] Bỏ chọn: {ball.displayName}");
        }
        else
        {
            if (selectedBalls.Count >= maxBallLoadout)
            {
                Debug.Log($"[InventorySelectionPanelUI] Đã chọn đủ {maxBallLoadout} bi — bỏ chọn bớt trước khi chọn thêm");
                return;
            }

            selectedBalls.Add(ball);
            Debug.Log($"[InventorySelectionPanelUI] Đã chọn: {ball.displayName}");
        }

        if (SceneLoader.Instance != null)
            SceneLoader.Instance.SetSelectedBallLoadout(selectedBalls);

        foreach (var slot in ballSlots)
            slot.RefreshHighlight();
    }

    // 🆕 Placeholder cho phần Cơ — hiện chưa ô nào bấm được nên hàm này chưa bao giờ chạy
    private void OnCueSlotClicked(EquipmentConfig config)
    {
        Debug.Log("[InventorySelectionPanelUI] Phần Cơ chưa được hỗ trợ");
    }

    public void OpenPanel()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveCurrentSelection();
    }
}
