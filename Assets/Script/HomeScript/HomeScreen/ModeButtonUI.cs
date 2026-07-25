using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ModeButtonUI : MonoBehaviour
{
    //public Button button;
    //public Text titleText;      // hoặc TMP_Text nếu bạn dùng TextMeshPro
    //public Text descriptionText;
    //public Image iconImage;

    //private GameModeSO modeData;
    //private Action<GameModeSO> onClickCallback;

    //public void Setup(GameModeSO mode, Action<GameModeSO> callback)
    //{
    //    modeData = mode;
    //    onClickCallback = callback;

    //    if (titleText != null) titleText.text = mode.displayName;
    //    if (descriptionText != null) descriptionText.text = mode.description;
    //    if (iconImage != null && mode.icon != null) iconImage.sprite = mode.icon;

    //    button.onClick.RemoveAllListeners();
    //    button.onClick.AddListener(() => onClickCallback?.Invoke(modeData));
    //}

    public Button button;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    [Header("Hiệu ứng khi đang được chọn (VD: viền sáng, checkmark)")]
    public GameObject selectedHighlight;

    public GameModeSO ModeData { get; private set; }

    private Action<GameModeSO> onClickCallback;

    public void Setup(GameModeSO mode, Action<GameModeSO> callback)
    {
        ModeData = mode;
        onClickCallback = callback;

        if (titleText != null) titleText.text = mode.displayName;
        if (descriptionText != null) descriptionText.text = mode.description;
        if (iconImage != null && mode.icon != null) iconImage.sprite = mode.icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(ModeData));

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (selectedHighlight != null)
            selectedHighlight.SetActive(isSelected);
    }
}

