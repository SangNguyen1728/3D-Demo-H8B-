using TMPro;
using UnityEngine;

public class SkillTooltipUI : MonoBehaviour
{
    public static SkillTooltipUI Instance { get; private set; }

    [Header("UI References")]
    public RectTransform popupRect;   // gộp làm 1, thay cho popupRoot
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    [Header("Offset so với vị trí bấm giữ")]
    public Vector2 offset = new Vector2(0f, 60f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        if (popupRect != null) popupRect.gameObject.SetActive(false);
    }

    public void Show(BaseSkills variant, Vector2 screenPosition)
    {
        Debug.Log($"[SkillTooltipUI] Show() được gọi. popupRect null? {popupRect == null}");
        if (variant == null || popupRect == null) return;
        if (titleText != null) titleText.text = variant.skillName;
        if (descriptionText != null) descriptionText.text = variant.skillDescription;
        Debug.Log("[SkillTooltipUI] Show() SetActive(true) lúc: " + Time.frameCount);
        popupRect.gameObject.SetActive(true);
        popupRect.SetAsLastSibling();
        popupRect.position = screenPosition + offset;
    }

    public void Hide()
    {
        if (popupRect == null) return;
        Debug.Log("[SkillTooltipUI] Hide() được gọi lúc: " + Time.frameCount);
        popupRect.gameObject.SetActive(false);
    }
}
