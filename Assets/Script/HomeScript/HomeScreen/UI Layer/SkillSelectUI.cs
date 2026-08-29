using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectUI : MonoBehaviour
{
    

    [Header("Prefab hàng skill + nơi chứa")]
    public SkillSlotRowUI skillRowPrefab;
    public Transform rowContainer;

    [Header("Nút đóng panel")]
    public Button closeButton;
    public Button confirmButton;

    [Header("Tổng kết lựa chọn")]
    public SelectionSummaryUI summaryUI; // MỚI

    private List<SkillSlotRowUI> spawnedRows = new List<SkillSlotRowUI>();

    private void Start()
    {
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseClicked);
        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    private void OnEnable()
    {
        BuildRows();
    }

    private void BuildRows()
    {
        foreach (Transform child in rowContainer) Destroy(child.gameObject);
        spawnedRows.Clear();

        CharacterSO character = SceneLoader.Instance.SelectedCharacter;
        if (character == null)
        {
            Debug.LogError("[SkillSelectUI] Chưa có character nào được chọn!");
            return;
        }

        for (int i = 0; i < character.skillSlots.Count; i++)
        {
            SkillSlotRowUI row = Instantiate(skillRowPrefab, rowContainer);
            row.Setup(i, character.skillSlots[i], OnVariantSelected);
            spawnedRows.Add(row);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rowContainer.GetComponent<RectTransform>());

        RefreshConfirmButtonState();
        if (summaryUI != null) summaryUI.Refresh(); // MỚI
    }

    private void OnVariantSelected(int slotIndex, BaseSkills variant)
    {
        SceneLoader.Instance.SetSkillVariant(slotIndex, variant);
        foreach (var row in spawnedRows) row.RefreshSelectionVisual();
        RefreshConfirmButtonState();
        if (summaryUI != null) summaryUI.Refresh(); // MỚI
    }

    private void RefreshConfirmButtonState()
    {
        if (confirmButton != null)
            confirmButton.interactable = SceneLoader.Instance.SelectedSkillLoadout.IsComplete();
    }

    private void OnCloseClicked()
    {
        Debug.Log("[SkillSelectUI] Đóng panel, về Home");
        UIFlowManager.Instance.ShowHomeOnly();
    }

    private void OnConfirmClicked()
    {
        Debug.Log("[SkillSelectUI] Loadout hoàn tất, về Home");
        SaveManager.Instance.SaveCurrentSelection();
        UIFlowManager.Instance.ShowHomeOnly();
    }
}
