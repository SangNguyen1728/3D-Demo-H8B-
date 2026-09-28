using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventorySlotUI : MonoBehaviour
{
    //public EquipmentConfig itemConfig;
    //public GameObject highlightFrame;
    //public GameObject lockedOverlay;

    //[Tooltip("Text hiển thị tên bi trên ô — để trống nếu ô này chưa có item")]
    //public TMP_Text nameText;

    //private Button button;
    //private Action<EquipmentConfig> onSlotClicked;
    //private Func<EquipmentConfig, bool> isSelectedChecker;   // 🆕 đổi kiểu — kiểm tra có nằm trong danh sách đã chọn không

    //public void Init(Action<EquipmentConfig> onClicked, Func<EquipmentConfig, bool> selectedChecker)
    //{
    //    //onSlotClicked = onClicked;
    //    //isSelectedChecker = selectedChecker;

    //    //button = GetComponent<Button>();

    //    //bool isEmpty = itemConfig == null;

    //    //if (lockedOverlay != null)
    //    //    lockedOverlay.SetActive(isEmpty);

    //    //if (button != null)
    //    //    button.interactable = !isEmpty;



    //    //if (!isEmpty)
    //    //{
    //    //    button.onClick.AddListener(() => onSlotClicked?.Invoke(itemConfig));
    //    //}

    //    //RefreshHighlight();

    //    onSlotClicked = onClicked;
    //    isSelectedChecker = selectedChecker;

    //    button = GetComponent<Button>();

    //    bool isEmpty = itemConfig == null;

    //    // 🆕 Chỉ báo lỗi cho ô CÓ bi mà lại thiếu Name Text (ô trống/khoá thì không cần)
    //    if (!isEmpty && nameText == null)
    //    {
    //        Debug.LogError($"[InventorySlotUI] '{gameObject.name}': THIẾU field Name Text!");
    //    }

    //    if (lockedOverlay != null)
    //        lockedOverlay.SetActive(isEmpty);

    //    if (button != null)
    //        button.interactable = !isEmpty;

    //    if (nameText != null)
    //    {
    //        nameText.text = isEmpty ? "" : itemConfig.displayName;
    //    }

    //    if (!isEmpty)
    //    {
    //        button.onClick.AddListener(() => onSlotClicked?.Invoke(itemConfig));
    //    }

    //    RefreshHighlight();
    //}

    //public void RefreshHighlight()
    //{
    //    if (highlightFrame == null || isSelectedChecker == null) return;

    //    highlightFrame.SetActive(itemConfig != null && isSelectedChecker(itemConfig));
    //}

    public EquipmentConfig itemConfig;
    public GameObject highlightFrame;
    public GameObject lockedOverlay;

    [Tooltip("Text hiển thị tên trang bị — nếu để trống, script tự tìm TMP_Text con đầu tiên")]
    public TMP_Text nameText;

    private Button button;
    private Action<EquipmentConfig> onSlotClicked;
    private Func<EquipmentConfig, bool> isSelectedChecker;

    public void Init(Action<EquipmentConfig> onClicked, Func<EquipmentConfig, bool> selectedChecker)
    {
        onSlotClicked = onClicked;
        isSelectedChecker = selectedChecker;

        button = GetComponent<Button>();

        bool isEmpty = itemConfig == null;

        // 🆕 Dự phòng: quên gán Name Text thì tự tìm TMP_Text con
        if (nameText == null)
            nameText = GetComponentInChildren<TMP_Text>(true);

        if (!isEmpty && nameText == null)
            Debug.LogError($"[InventorySlotUI] '{gameObject.name}': THIẾU field Name Text!");

        if (lockedOverlay != null)
            lockedOverlay.SetActive(isEmpty);

        if (button != null)
            button.interactable = !isEmpty;

        if (nameText != null)
            nameText.text = isEmpty ? "" : itemConfig.displayName;

        if (!isEmpty)
            button.onClick.AddListener(() => onSlotClicked?.Invoke(itemConfig));

        RefreshHighlight();
    }

    public void RefreshHighlight()
    {
        if (highlightFrame == null || isSelectedChecker == null) return;

        highlightFrame.SetActive(itemConfig != null && isSelectedChecker(itemConfig));
    }
}
