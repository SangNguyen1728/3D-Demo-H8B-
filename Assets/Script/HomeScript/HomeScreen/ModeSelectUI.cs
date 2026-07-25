using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class ModeSelectUI : MonoBehaviour
{
    [Header("Danh sách mode có thể chọn")]
    public List<GameModeSO> availableModes;

    [Header("Prefab nút + nơi chứa")]
    public ModeButtonUI modeButtonPrefab;
    public Transform buttonContainer;

    [Header("Nút đóng panel")]
    public Button closeButton;

    private List<ModeButtonUI> spawnedButtons = new List<ModeButtonUI>();

    private void Start()
    {
        BuildModeButtons();

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);
    }

    private void OnEnable()
    {
        RefreshSelectionVisual();
    }

    private void BuildModeButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
        spawnedButtons.Clear();

        foreach (GameModeSO mode in availableModes)
        {
            ModeButtonUI button = Instantiate(modeButtonPrefab, buttonContainer);
            button.Setup(mode, OnModeSelected);
            spawnedButtons.Add(button);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer.GetComponent<RectTransform>());

        RefreshSelectionVisual();
    }

    private void OnModeSelected(GameModeSO mode)
    {
        Debug.Log($"[ModeSelectUI] Chọn mode: {mode.displayName}");
        SceneLoader.Instance.SetSelectedMode(mode);
        RefreshSelectionVisual();
    }

    private void RefreshSelectionVisual()
    {
        if (SceneLoader.Instance == null) return;

        GameModeSO selected = SceneLoader.Instance.SelectedMode;
        foreach (ModeButtonUI btn in spawnedButtons)
        {
            btn.SetSelected(btn.ModeData == selected);
        }
    }

    private void OnCloseClicked()
    {
        Debug.Log("[ModeSelectUI] Đóng panel, về Home");
        UIFlowManager.Instance.ShowHomeOnly();
    }
}
