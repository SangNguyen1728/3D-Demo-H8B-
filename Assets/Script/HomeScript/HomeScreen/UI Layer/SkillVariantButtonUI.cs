using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class SkillVariantButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    //public Button button;
    //public TMP_Text titleText;
    //public Image iconImage;
    //public GameObject selectedHighlight;
    //public GameObject lockedOverlay;

    //public BaseSkills VariantData { get; private set; }

    //private Action<BaseSkills> onClickCallback;

    //public void Setup(BaseSkills variant, bool isUnlocked, Action<BaseSkills> callback)
    //{
    //    VariantData = variant;
    //    onClickCallback = callback;

    //    if (titleText != null) titleText.text = variant.skillName;
    //    if (iconImage != null && variant.icon != null) iconImage.sprite = variant.icon;

    //    button.interactable = isUnlocked;
    //    if (lockedOverlay != null) lockedOverlay.SetActive(!isUnlocked);

    //    button.onClick.RemoveAllListeners();
    //    if (isUnlocked)
    //        button.onClick.AddListener(() => onClickCallback?.Invoke(VariantData));

    //    SetSelected(false);
    //}

    //public void SetSelected(bool isSelected)
    //{
    //    if (selectedHighlight != null)
    //        selectedHighlight.SetActive(isSelected);
    //}

    public Button button;
    public TMP_Text titleText;
    public Image iconImage;
    public GameObject selectedHighlight;
    public GameObject lockedOverlay;
    public BaseSkills VariantData { get; private set; }

    [Header("Long Press Tooltip")]
    public float holdDurationToShow = 0.4f; // thời gian giữ trước khi hiện popup (giây)

    private Action<BaseSkills> onClickCallback;
    private Coroutine holdCoroutine;
    private bool isTooltipShowing;

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

    // ----- Long press tooltip -----

    public void OnPointerDown(PointerEventData eventData)
    {
        if (holdCoroutine != null) StopCoroutine(holdCoroutine);
        holdCoroutine = StartCoroutine(HoldRoutine(eventData.position));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ngón tay/chuột trượt ra ngoài nút trong lúc đang giữ -> huỷ luôn, tránh kẹt popup
        CancelHold();
    }

    private IEnumerator HoldRoutine(Vector2 pressPosition)
    {
        yield return new WaitForSeconds(holdDurationToShow);
        Debug.Log($"[SkillVariantButtonUI] Bấm giữ đủ lâu -> hiện tooltip: {VariantData?.skillName}");
        isTooltipShowing = true;
        SkillTooltipUI.Instance?.Show(VariantData, pressPosition);
        Debug.Log($"[SkillVariantButtonUI] Tooltip Instance null? {SkillTooltipUI.Instance == null}");
    }

    private void CancelHold()
    {
        if (holdCoroutine != null)
        {
            StopCoroutine(holdCoroutine);
            holdCoroutine = null;
        }
        if (isTooltipShowing)
        {
            isTooltipShowing = false;
            SkillTooltipUI.Instance?.Hide();
        }
    }

    private void OnDisable()
    {
        // đề phòng nút bị destroy/disable đột ngột trong lúc đang giữ (VD đổi nhân vật, đóng panel)
        CancelHold();
    }
}
