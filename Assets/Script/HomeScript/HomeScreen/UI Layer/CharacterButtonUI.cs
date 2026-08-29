using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CharacterButtonUI : MonoBehaviour
{
    public Button button;
    public TMP_Text titleText;
    public Image portraitImage;
    public GameObject selectedHighlight;

    public CharacterSO CharacterData { get; private set; }

    private Action<CharacterSO> onClickCallback;

    public void Setup(CharacterSO character, Action<CharacterSO> callback)
    {
        CharacterData = character;
        onClickCallback = callback;

        if (titleText != null) titleText.text = character.displayName;
        if (portraitImage != null && character.portrait != null) portraitImage.sprite = character.portrait;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(CharacterData));

        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        Debug.Log($"[CharacterButtonUI] SetSelected({isSelected}) cho {CharacterData?.displayName}, selectedHighlight null? {selectedHighlight == null}");
        if (selectedHighlight != null)
            selectedHighlight.SetActive(isSelected);
    }
}
