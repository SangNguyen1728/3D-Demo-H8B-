using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    //[System.Serializable]
    //public class SlotUI
    //{
    //    public Button button;
    //    public Image icon;          // tùy chọn
    //    public TMP_Text nameText;   // tùy chọn
    //    public TMP_Text costText;   // tùy chọn
    //    [System.NonSerialized] public CanvasGroup group;
    //}

    //[Header("Manager (để trống sẽ tự tìm)")]
    //public SkillSelectionManager manager;

    //[Header("Root & Text")]
    //public CanvasGroup rootGroup;
    //public TMP_Text countdownText;
    //public TMP_Text messageText;
    //public float messageDuration = 1.5f;

    //[Header("Slots: đúng thứ tự Skill1, Skill2, Skill3")]
    //public SlotUI[] player1Slots = new SlotUI[3];
    //public SlotUI[] player2Slots = new SlotUI[3];

    //[Header("Look")]
    //public float litAlpha = 1f;
    //public float dimAlpha = 0.3f;
    //public float grabbedScale = 1.12f;

    //private SkillSelectionManager.State lastState = SkillSelectionManager.State.Idle;

    //private void Awake()
    //{
    //    if (manager == null) manager = FindFirstObjectByType<SkillSelectionManager>();
    //    if (manager == null) Debug.LogError("[SkillUI] Không tìm thấy SkillSelectionManager!");

    //    InitSlots(player1Slots, 1);
    //    InitSlots(player2Slots, 2);
    //    SetVisible(false);
    //    if (messageText != null) messageText.text = "";
    //}

    //private void InitSlots(SlotUI[] slots, int player)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        SlotUI slot = slots[i];
    //        if (slot == null || slot.button == null)
    //        {
    //            Debug.LogWarning($"[SkillUI] P{player} slot{i + 1} chưa gán Button!");
    //            continue;
    //        }

    //        slot.group = slot.button.GetComponent<CanvasGroup>();
    //        if (slot.group == null) slot.group = slot.button.gameObject.AddComponent<CanvasGroup>();

    //        int p = player;
    //        int s = i + 1;
    //        slot.button.onClick.AddListener(() => OnSlotClicked(p, s));
    //    }
    //}

    //private void OnSlotClicked(int player, int slot)
    //{
    //    if (manager != null) manager.RequestSkill(player, slot);
    //}

    //private void Update()
    //{
    //    if (manager == null) return;

    //    SkillSelectionManager.State st = manager.CurrentState;
    //    bool visible = st == SkillSelectionManager.State.Selecting || st == SkillSelectionManager.State.Grabbed;
    //    SetVisible(visible);

    //    if (st != lastState)
    //    {
    //        if (st == SkillSelectionManager.State.Selecting) RefreshSkillInfo();
    //        lastState = st;
    //    }

    //    if (visible)
    //    {
    //        RefreshSlots(player1Slots, 1);
    //        RefreshSlots(player2Slots, 2);
    //        UpdateCountdown(st);
    //    }

    //    if (messageText != null)
    //    {
    //        bool show = Time.unscaledTime - manager.LastMessageTime < messageDuration;
    //        messageText.text = show ? manager.LastMessage : "";
    //    }
    //}

    //private void SetVisible(bool visible)
    //{
    //    if (rootGroup == null) return;
    //    rootGroup.alpha = visible ? 1f : 0f;
    //    rootGroup.blocksRaycasts = visible;
    //    rootGroup.interactable = visible;
    //}

    //private void UpdateCountdown(SkillSelectionManager.State st)
    //{
    //    if (countdownText == null) return;

    //    if (st == SkillSelectionManager.State.Selecting)
    //        countdownText.text = $"P{manager.TurnPlayer} TURN\n{Mathf.CeilToInt(manager.TimeLeft)}";
    //    else if (st == SkillSelectionManager.State.Grabbed)
    //        countdownText.text = $"P{manager.GrabbedPlayer} GRABBED\nClick again to use";
    //}

    //private void RefreshSkillInfo()
    //{
    //    FillInfo(player1Slots, 1);
    //    FillInfo(player2Slots, 2);
    //}

    //private void FillInfo(SlotUI[] slots, int player)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        SlotUI slot = slots[i];
    //        if (slot == null || slot.button == null) continue;

    //        BaseSkills skill = manager.GetSkill(player, i + 1);
    //        slot.button.gameObject.SetActive(skill != null);
    //        if (skill == null) continue;

    //        if (slot.nameText != null) slot.nameText.text = skill.skillName;
    //        if (slot.costText != null) slot.costText.text = skill.staminaCost.ToString();
    //        if (slot.icon != null)
    //        {
    //            Sprite sp = skill.icon != null ? skill.icon : skill.skillIcon;
    //            slot.icon.sprite = sp;
    //            slot.icon.enabled = sp != null;
    //        }
    //    }
    //}

    //private void RefreshSlots(SlotUI[] slots, int player)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        SlotUI slot = slots[i];
    //        if (slot == null || slot.button == null || !slot.button.gameObject.activeSelf) continue;

    //        int s = i + 1;
    //        bool lit = manager.IsSlotLit(player, s);
    //        bool grabbed = manager.CurrentState == SkillSelectionManager.State.Grabbed
    //                       && manager.GrabbedPlayer == player && manager.GrabbedSlot == s;

    //        slot.button.interactable = lit;
    //        if (slot.group != null) slot.group.alpha = lit ? litAlpha : dimAlpha;
    //        slot.button.transform.localScale = Vector3.one * (grabbed ? grabbedScale : 1f);
    //    }
    //}

    [System.Serializable]
    public class SlotUI
    {
        public Button button;
        public Image icon;          // tùy chọn
        public TMP_Text nameText;   // tùy chọn
        public TMP_Text costText;   // tùy chọn
        [System.NonSerialized] public CanvasGroup group;
    }

    [Header("Manager (để trống sẽ tự tìm)")]
    public SkillSelectionManager manager;

    [Header("Thanh giành skill (panel DUY NHẤT)")]
    public CanvasGroup rootGroup;
    public TMP_Text countdownText;

    [Header("Slots: đúng thứ tự Skill1, Skill2, Skill3")]
    public SlotUI[] player1Slots = new SlotUI[3];
    public SlotUI[] player2Slots = new SlotUI[3];

    [Header("Chip READY (hiện sau khi người trong lượt giành được skill)")]
    public GameObject readyChip;
    public Button readyButton;
    public TMP_Text readyLabel;
    public Image readyIcon;

    [Header("Thông báo")]
    public TMP_Text messageText;
    public float messageDuration = 1.5f;

    [Header("Look")]
    public float litAlpha = 1f;
    public float dimAlpha = 0.3f;

    private SkillSelectionManager.State lastState = SkillSelectionManager.State.Idle;

    private void Awake()
    {
        if (manager == null) manager = FindFirstObjectByType<SkillSelectionManager>();
        if (manager == null) Debug.LogError("[SkillUI] Không tìm thấy SkillSelectionManager!");

        InitSlots(player1Slots, 1);
        InitSlots(player2Slots, 2);

        if (readyButton != null)
            readyButton.onClick.AddListener(() =>
            {
                if (manager != null) manager.RequestUse(manager.ReadyPlayer);
            });

        if (readyChip != null) readyChip.SetActive(false);
        SetPanelVisible(false);
        if (messageText != null) messageText.text = "";
    }

    private void InitSlots(SlotUI[] slots, int player)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            SlotUI slot = slots[i];
            if (slot == null || slot.button == null)
            {
                Debug.LogWarning($"[SkillUI] P{player} slot{i + 1} chưa gán Button!");
                continue;
            }

            slot.group = slot.button.GetComponent<CanvasGroup>();
            if (slot.group == null) slot.group = slot.button.gameObject.AddComponent<CanvasGroup>();

            int p = player;
            int s = i + 1;
            slot.button.onClick.AddListener(() =>
            {
                if (manager != null) manager.RequestSkill(p, s);
            });
        }
    }

    private void Update()
    {
        if (manager == null) return;

        SkillSelectionManager.State st = manager.CurrentState;
        bool selecting = st == SkillSelectionManager.State.Selecting;

        SetPanelVisible(selecting);

        if (st != lastState)
        {
            if (st == SkillSelectionManager.State.Selecting) RefreshSkillInfo();

            if (readyChip != null) readyChip.SetActive(st == SkillSelectionManager.State.Ready);
            if (st == SkillSelectionManager.State.Ready) RefreshReadyChip();

            lastState = st;
        }

        if (selecting)
        {
            RefreshSlots(player1Slots, 1);
            RefreshSlots(player2Slots, 2);

            if (countdownText != null)
                countdownText.text = $"P{manager.TurnPlayer}\n{Mathf.CeilToInt(manager.TimeLeft)}";
        }

        if (messageText != null)
        {
            bool show = Time.unscaledTime - manager.LastMessageTime < messageDuration;
            messageText.text = show ? manager.LastMessage : "";
        }
    }

    private void SetPanelVisible(bool visible)
    {
        if (rootGroup == null) return;
        rootGroup.alpha = visible ? 1f : 0f;
        rootGroup.blocksRaycasts = visible;
        rootGroup.interactable = visible;
    }

    private void RefreshReadyChip()
    {
        BaseSkills skill = manager.GetSkill(manager.ReadyPlayer, manager.ReadySlot);

        if (readyLabel != null)
            readyLabel.text = skill != null ? $"P{manager.ReadyPlayer}  {skill.skillName}\nTAP TO USE" : "";

        if (readyIcon != null)
        {
            Sprite sp = skill != null ? (skill.icon != null ? skill.icon : skill.skillIcon) : null;
            readyIcon.sprite = sp;
            readyIcon.enabled = sp != null;
        }
    }

    private void RefreshSkillInfo()
    {
        FillInfo(player1Slots, 1);
        FillInfo(player2Slots, 2);
    }

    private void FillInfo(SlotUI[] slots, int player)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            SlotUI slot = slots[i];
            if (slot == null || slot.button == null) continue;

            BaseSkills skill = manager.GetSkill(player, i + 1);
            slot.button.gameObject.SetActive(skill != null);
            if (skill == null) continue;

            if (slot.nameText != null) slot.nameText.text = skill.skillName;
            if (slot.costText != null) slot.costText.text = skill.staminaCost.ToString();
            if (slot.icon != null)
            {
                Sprite sp = skill.icon != null ? skill.icon : skill.skillIcon;
                slot.icon.sprite = sp;
                slot.icon.enabled = sp != null;
            }
        }
    }

    private void RefreshSlots(SlotUI[] slots, int player)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            SlotUI slot = slots[i];
            if (slot == null || slot.button == null || !slot.button.gameObject.activeSelf) continue;

            bool lit = manager.IsSlotLit(player, i + 1);
            slot.button.interactable = lit;
            if (slot.group != null) slot.group.alpha = lit ? litAlpha : dimAlpha;
        }
    }
}
