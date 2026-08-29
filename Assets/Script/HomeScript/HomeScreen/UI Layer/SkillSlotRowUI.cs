using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillSlotRowUI : MonoBehaviour
{
    public TMP_Text slotLabelText;
    public Transform variantButtonContainer;
    public SkillVariantButtonUI variantButtonPrefab;

    private List<SkillVariantButtonUI> spawnedButtons = new List<SkillVariantButtonUI>();
    private int slotIndex;
    private Action<int, BaseSkills> onVariantSelected;

    // Giả lập level người chơi hiện tại — thay bằng hệ thống level thật sau này
    private const int CURRENT_PLAYER_LEVEL = 99;

    public void Setup(int index, SkillSlotSO slotData, Action<int, BaseSkills> callback)
    {
        slotIndex = index;
        onVariantSelected = callback;

        if (slotLabelText != null) slotLabelText.text = slotData.slotDisplayName;

        foreach (Transform child in variantButtonContainer) Destroy(child.gameObject);
        spawnedButtons.Clear();

        foreach (BaseSkills variant in slotData.variants)
        {
            bool isUnlocked = variant.requiredLevel <= CURRENT_PLAYER_LEVEL;
            SkillVariantButtonUI btn = Instantiate(variantButtonPrefab, variantButtonContainer);
            btn.Setup(variant, isUnlocked, OnVariantClicked);
            spawnedButtons.Add(btn);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(variantButtonContainer.GetComponent<RectTransform>());

        //if (slotData.variants.Count == 1)
        //{
        //    Debug.Log($"[SkillSlotRowUI] Slot {slotIndex} chỉ có 1 skill -> tự động chọn: {slotData.variants[0].skillName}");
        //    onVariantSelected?.Invoke(slotIndex, slotData.variants[0]);
        //}

        RefreshSelectionVisual();
    }

    private void OnVariantClicked(BaseSkills variant)
    {
        onVariantSelected?.Invoke(slotIndex, variant);
        RefreshSelectionVisual();
    }

    public void RefreshSelectionVisual()
    {
        if (SceneLoader.Instance == null) return;
        SkillLoadout loadout = SceneLoader.Instance.SelectedSkillLoadout;

        BaseSkills currentSelected = slotIndex switch
        {
            0 => loadout.skill1Variant,
            1 => loadout.skill2Variant,
            2 => loadout.skill3Variant,
            _ => null
        };

        foreach (var btn in spawnedButtons)
            btn.SetSelected(btn.VariantData == currentSelected);
    }
}
