using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillVariantButtonUI : MonoBehaviour
{
    public Button button;
    public TMP_Text titleText;
    public Image iconImage;
    public GameObject selectedHighlight;
    public GameObject lockedOverlay;

    public BaseSkills VariantData { get; private set; }

    private Action<BaseSkills> onClickCallback;

    public void Setup(BaseSkills variant, bool isUnlocked, Action<BaseSkills> callback)
    {
        VariantData = variant;
        onClickCallback = callback;

        if (titleText != null) titleText.text = variant.skillName;
        if (iconImage != null && variant.icon != null) iconImage.sprite = variant.icon;

        button.interactable = isUnlocked;
        if (lockedOverlay != null) lockedOverlay.SetActive(!isUnlocked);

        button.onClick.RemoveAllListeners();
        if (isUnlocked)
            button.onClick.AddListener(() => onClickCallback?.Invoke(VariantData));

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedHighlight != null)
            selectedHighlight.SetActive(isSelected);
    }
}
