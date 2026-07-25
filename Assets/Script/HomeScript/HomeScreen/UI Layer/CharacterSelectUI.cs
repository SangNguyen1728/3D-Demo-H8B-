using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Dữ liệu")]
    public List<CharacterSO> availableCharacters;

    [Header("Prefab nút + nơi chứa")]
    public CharacterButtonUI characterButtonPrefab;
    public Transform buttonContainer;

    [Header("Điều hướng")]
    public Button closeButton;
    public Button chooseSkillButton; // MỚI — chỉ bấm được khi đã chọn character

    private List<CharacterButtonUI> spawnedButtons = new List<CharacterButtonUI>();

    private void Start()
    {
        BuildButtons();

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        if (chooseSkillButton != null)
            chooseSkillButton.onClick.AddListener(OnChooseSkillClicked);
    }

    private void OnEnable()
    {
        RefreshSelectionVisual();
        RefreshChooseSkillButtonState();
    }

    private void BuildButtons()
    {
        foreach (Transform child in buttonContainer) Destroy(child.gameObject);
        spawnedButtons.Clear();

        foreach (CharacterSO character in availableCharacters)
        {
            CharacterButtonUI btn = Instantiate(characterButtonPrefab, buttonContainer);
            btn.Setup(character, OnCharacterSelected);
            spawnedButtons.Add(btn);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer.GetComponent<RectTransform>());

        RefreshSelectionVisual();
    }

    private void OnCharacterSelected(CharacterSO character)
    {
        Debug.Log($"[CharacterSelectUI] Chọn nhân vật: {character.displayName}");
        SceneLoader.Instance.SetSelectedCharacter(character);
        RefreshSelectionVisual();
        RefreshChooseSkillButtonState();
    }

    private void RefreshSelectionVisual()
    {
        if (SceneLoader.Instance == null) return;
        CharacterSO selected = SceneLoader.Instance.SelectedCharacter;
        foreach (var btn in spawnedButtons)
            btn.SetSelected(btn.CharacterData == selected);
    }

    private void RefreshChooseSkillButtonState() // MỚI
    {
        if (chooseSkillButton != null)
            chooseSkillButton.interactable = SceneLoader.Instance.SelectedCharacter != null;
    }

    private void OnChooseSkillClicked() // MỚI
    {
        Debug.Log("[CharacterSelectUI] Mở SkillPanel");
        UIFlowManager.Instance.ShowSkill();
    }

    private void OnCloseClicked()
    {
        Debug.Log("[CharacterSelectUI] Đóng panel, về Home");
        UIFlowManager.Instance.ShowHomeOnly();
    }
}
