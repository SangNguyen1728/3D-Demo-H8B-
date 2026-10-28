using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DefaultExecutionOrder(100)]
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

    //[Header("Thanh giành skill (panel DUY NHẤT)")]
    //public CanvasGroup rootGroup;
    //public TMP_Text countdownText;

    //[Header("Slots: đúng thứ tự Skill1, Skill2, Skill3")]
    //public SlotUI[] player1Slots = new SlotUI[3];
    //public SlotUI[] player2Slots = new SlotUI[3];

    //[Header("Chip READY (hiện sau khi người trong lượt giành được skill)")]
    //public GameObject readyChip;
    //public Button readyButton;
    //public TMP_Text readyLabel;
    //public Image readyIcon;

    //[Header("Thông báo")]
    //public TMP_Text messageText;
    //public float messageDuration = 1.5f;

    //[Header("Look")]
    //public float litAlpha = 1f;
    //public float dimAlpha = 0.3f;

    //private SkillSelectionManager.State lastState = SkillSelectionManager.State.Idle;

    //private void Awake()
    //{
    //    if (manager == null) manager = FindFirstObjectByType<SkillSelectionManager>();
    //    if (manager == null) Debug.LogError("[SkillUI] Không tìm thấy SkillSelectionManager!");

    //    InitSlots(player1Slots, 1);
    //    InitSlots(player2Slots, 2);

    //    if (readyButton != null)
    //        readyButton.onClick.AddListener(() =>
    //        {
    //            if (manager != null) manager.RequestUse(manager.ReadyPlayer);
    //        });

    //    if (readyChip != null) readyChip.SetActive(false);
    //    SetPanelVisible(false);
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
    //        slot.button.onClick.AddListener(() =>
    //        {
    //            if (manager != null) manager.RequestSkill(p, s);
    //        });
    //    }
    //}

    //private void LateUpdate()
    //{
    //    if (manager == null) return;

    //    SkillSelectionManager.State st = manager.CurrentState;
    //    bool selecting = st == SkillSelectionManager.State.Selecting;

    //    SetPanelVisible(selecting);

    //    if (st != lastState)
    //    {
    //        if (st == SkillSelectionManager.State.Selecting) RefreshSkillInfo();

    //        if (readyChip != null) readyChip.SetActive(st == SkillSelectionManager.State.Ready);
    //        if (st == SkillSelectionManager.State.Ready) RefreshReadyChip();

    //        lastState = st;
    //    }

    //    if (selecting)
    //    {
    //        RefreshSlots(player1Slots, 1);
    //        RefreshSlots(player2Slots, 2);

    //        if (countdownText != null)
    //            countdownText.text = $"P{manager.TurnPlayer}\n{Mathf.CeilToInt(manager.TimeLeft)}";
    //    }

    //    if (messageText != null)
    //    {
    //        bool show = Time.unscaledTime - manager.LastMessageTime < messageDuration;
    //        messageText.text = show ? manager.LastMessage : "";
    //    }
    //}

    //private void SetPanelVisible(bool visible)
    //{
    //    if (rootGroup == null) return;
    //    rootGroup.alpha = visible ? 1f : 0f;
    //    rootGroup.blocksRaycasts = visible;
    //    rootGroup.interactable = visible;
    //}

    //private void RefreshReadyChip()
    //{
    //    BaseSkills skill = manager.GetSkill(manager.ReadyPlayer, manager.ReadySlot);

    //    if (readyLabel != null)
    //        readyLabel.text = skill != null ? $"P{manager.ReadyPlayer}  {skill.skillName}\nTAP TO USE" : "";

    //    if (readyIcon != null)
    //    {
    //        Sprite sp = skill != null ? (skill.icon != null ? skill.icon : skill.skillIcon) : null;
    //        readyIcon.sprite = sp;
    //        readyIcon.enabled = sp != null;
    //    }
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

    //        bool lit = manager.IsSlotLit(player, i + 1);
    //        slot.button.interactable = lit;
    //        if (slot.group != null) slot.group.alpha = lit ? litAlpha : dimAlpha;
    //    }
    //}

    //public enum LockedDisplayMode { Off, Flash, UntilShot }

    //[System.Serializable]
    //public class SlotUI
    //{
    //    public Button button;
    //    public Image icon;          // tùy chọn
    //    public TMP_Text nameText;   // tùy chọn
    //    public TMP_Text costText;   // tùy chọn
    //    [Tooltip("Tùy chọn: lớp phủ tối + icon khóa, bật khi skill bị khóa")]
    //    public GameObject lockOverlay;
    //    [Tooltip("Tùy chọn: viền/hiệu ứng đỏ, chỉ bật cho skill đã giành nhưng hết hạn")]
    //    public GameObject expiredHighlight;

    //    [System.NonSerialized] public CanvasGroup group;
    //    [System.NonSerialized] public Graphic graphic;
    //    [System.NonSerialized] public Color baseColor = Color.white;
    //}

    //[Header("Manager (để trống sẽ tự tìm)")]
    //public SkillSelectionManager manager;

    //[Header("Thanh giành skill (panel DUY NHẤT)")]
    //public CanvasGroup rootGroup;
    //public TMP_Text countdownText;

    //[Header("Slots: đúng thứ tự Skill1, Skill2, Skill3")]
    //public SlotUI[] player1Slots = new SlotUI[3];
    //public SlotUI[] player2Slots = new SlotUI[3];

    //[Header("Chip READY (hiện sau khi người trong lượt giành được skill)")]
    //public GameObject readyChip;
    //public Button readyButton;
    //public TMP_Text readyLabel;
    //public Image readyIcon;

    //[Header("Thông báo")]
    //public TMP_Text messageText;
    //public float messageDuration = 1.5f;

    //[Header("Look: bình thường")]
    //public float litAlpha = 1f;
    //public float dimAlpha = 0.3f;

    //[Header("Look: skill bị khóa (cooldown)")]
    //[Tooltip("Off = không hiện. Flash = hiện vài giây rồi ẩn. UntilShot = giữ tới khi bắt đầu cú đánh hoặc cửa sổ kế tiếp mở.")]
    //public LockedDisplayMode lockedDisplay = LockedDisplayMode.Flash;
    //public float lockedFlashSeconds = 2.5f;
    //public float lockedAlpha = 0.45f;
    //public Color lockedTint = new Color(0.45f, 0.45f, 0.45f, 1f);
    //public Color expiredTint = new Color(0.9f, 0.25f, 0.25f, 1f);
    //public float pulseSpeed = 6f;

    //private SkillSelectionManager.State lastState = SkillSelectionManager.State.Idle;

    //private void Awake()
    //{
    //    if (manager == null) manager = FindFirstObjectByType<SkillSelectionManager>();
    //    if (manager == null) Debug.LogError("[SkillUI] Không tìm thấy SkillSelectionManager!");

    //    InitSlots(player1Slots, 1);
    //    InitSlots(player2Slots, 2);

    //    if (readyButton != null)
    //        readyButton.onClick.AddListener(() =>
    //        {
    //            if (manager != null) manager.RequestUse(manager.ReadyPlayer);
    //        });

    //    if (readyChip != null) readyChip.SetActive(false);
    //    SetPanelVisible(false, false);
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

    //        slot.graphic = slot.button.targetGraphic;
    //        if (slot.graphic != null) slot.baseColor = slot.graphic.color;

    //        if (slot.lockOverlay != null) slot.lockOverlay.SetActive(false);
    //        if (slot.expiredHighlight != null) slot.expiredHighlight.SetActive(false);

    //        int p = player;
    //        int s = i + 1;
    //        slot.button.onClick.AddListener(() =>
    //        {
    //            if (manager != null) manager.RequestSkill(p, s);
    //        });
    //    }
    //}

    //private void LateUpdate()
    //{
    //    if (manager == null) return;

    //    SkillSelectionManager.State st = manager.CurrentState;
    //    bool selecting = st == SkillSelectionManager.State.Selecting;
    //    bool showLocked = !selecting && st != SkillSelectionManager.State.Ready && ShouldShowLocked();

    //    // Thanh chỉ nhận chạm khi đang Selecting. Ở trạng thái khóa thì chỉ để xem (không chặn input)
    //    SetPanelVisible(selecting || showLocked, selecting);

    //    if (st != lastState)
    //    {
    //        if (selecting) RefreshSkillInfo();

    //        if (readyChip != null) readyChip.SetActive(st == SkillSelectionManager.State.Ready);
    //        if (st == SkillSelectionManager.State.Ready) RefreshReadyChip();

    //        lastState = st;
    //    }

    //    if (selecting)
    //    {
    //        RefreshSlots(player1Slots, 1);
    //        RefreshSlots(player2Slots, 2);

    //        if (countdownText != null)
    //            countdownText.text = $"P{manager.TurnPlayer}\n{Mathf.CeilToInt(manager.TimeLeft)}";
    //    }
    //    else if (showLocked)
    //    {
    //        RefreshSlotsLocked(player1Slots, 1);
    //        RefreshSlotsLocked(player2Slots, 2);

    //        if (countdownText != null) countdownText.text = "LOCKED";
    //    }

    //    if (messageText != null)
    //    {
    //        bool show = Time.unscaledTime - manager.LastMessageTime < messageDuration;
    //        messageText.text = show ? manager.LastMessage : "";
    //    }
    //}

    //private bool ShouldShowLocked()
    //{
    //    if (lockedDisplay == LockedDisplayMode.Off || !manager.LockFeedbackActive) return false;
    //    if (lockedDisplay == LockedDisplayMode.UntilShot) return true;
    //    return Time.unscaledTime - manager.LockFeedbackTime < lockedFlashSeconds;
    //}

    //private void SetPanelVisible(bool visible, bool interactive)
    //{
    //    if (rootGroup == null) return;
    //    rootGroup.alpha = visible ? 1f : 0f;
    //    rootGroup.blocksRaycasts = visible && interactive;
    //    rootGroup.interactable = visible && interactive;
    //}

    //private void RefreshReadyChip()
    //{
    //    BaseSkills skill = manager.GetSkill(manager.ReadyPlayer, manager.ReadySlot);

    //    if (readyLabel != null)
    //        readyLabel.text = skill != null ? $"P{manager.ReadyPlayer}  {skill.skillName}\nTAP TO USE" : "";

    //    if (readyIcon != null)
    //    {
    //        Sprite sp = skill != null ? (skill.icon != null ? skill.icon : skill.skillIcon) : null;
    //        readyIcon.sprite = sp;
    //        readyIcon.enabled = sp != null;
    //    }
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

    //// ===== Selecting: sáng / tối theo stamina =====
    //private void RefreshSlots(SlotUI[] slots, int player)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        SlotUI slot = slots[i];
    //        if (slot == null || slot.button == null || !slot.button.gameObject.activeSelf) continue;

    //        bool lit = manager.IsSlotLit(player, i + 1);
    //        slot.button.interactable = lit;
    //        if (slot.group != null) slot.group.alpha = lit ? litAlpha : dimAlpha;

    //        // Xóa mọi dấu vết của trạng thái khóa
    //        if (slot.graphic != null) slot.graphic.color = slot.baseColor;
    //        if (slot.lockOverlay != null) slot.lockOverlay.SetActive(false);
    //        if (slot.expiredHighlight != null) slot.expiredHighlight.SetActive(false);
    //    }
    //}

    //// ===== Locked: cooldown highlight =====
    //private void RefreshSlotsLocked(SlotUI[] slots, int player)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        SlotUI slot = slots[i];
    //        if (slot == null || slot.button == null || !slot.button.gameObject.activeSelf) continue;

    //        bool expired = manager.LockedPlayer == player && manager.LockedSlot == i + 1;
    //        slot.button.interactable = false;

    //        if (expired)
    //        {
    //            float pulse = 0.85f + 0.15f * Mathf.Sin(Time.unscaledTime * pulseSpeed);
    //            if (slot.group != null) slot.group.alpha = pulse;
    //            if (slot.graphic != null) slot.graphic.color = expiredTint;
    //            if (slot.costText != null)
    //                slot.costText.text = manager.LockedPenalty > 0f ? $"LOCKED -{manager.LockedPenalty:0.#}" : "LOCKED";
    //        }
    //        else
    //        {
    //            if (slot.group != null) slot.group.alpha = lockedAlpha;
    //            if (slot.graphic != null) slot.graphic.color = lockedTint;
    //        }

    //        if (slot.lockOverlay != null) slot.lockOverlay.SetActive(true);
    //        if (slot.expiredHighlight != null) slot.expiredHighlight.SetActive(expired);
    //    }
    //}

    public enum LockedDisplayMode { Off, Flash, UntilShot }

    [System.Serializable]
    public class SlotUI
    {
        public Button button;
        public Image icon;          // tùy chọn
        public TMP_Text nameText;   // tùy chọn
        public TMP_Text costText;   // tùy chọn
        [Tooltip("Tùy chọn: lớp phủ tối + icon khóa, bật khi skill bị khóa")]
        public GameObject lockOverlay;
        [Tooltip("Tùy chọn: viền/hiệu ứng đỏ, chỉ bật cho skill đã giành nhưng hết hạn")]
        public GameObject expiredHighlight;

        [System.NonSerialized] public CanvasGroup group;
        
        [System.NonSerialized] public Graphic graphic;
        [System.NonSerialized] public Color baseColor = Color.white;
    }

    [System.Serializable]
    public class UseButtonUI
    {
        public Button button;
        [Tooltip("Text (TMP) con của nút: sẽ hiện tên skill thật của người đang giữ skill")]
        public TMP_Text label;
        [Tooltip("Tùy chọn: Image con để hiện icon skill (KHÔNG kéo Image nền của nút vào đây)")]
        public Image icon;
        [Tooltip("Tùy chọn: Image kiểu Filled / Radial 360 nằm trong nút, hiện vòng đếm ngược thời gian dùng skill")]
        public Image cooldownFill;

        [System.NonSerialized] public CanvasGroup group;
        [System.NonSerialized] public bool shown;
    }

    [Header("Manager (để trống sẽ tự tìm)")]
    public SkillSelectionManager manager;

    [Header("Thanh giành skill (panel DUY NHẤT, 6 nút)")]
    public CanvasGroup rootGroup;
    public TMP_Text countdownText;

    [Header("Slots: đúng thứ tự Skill1, Skill2, Skill3")]
    public SlotUI[] player1Slots = new SlotUI[3];
    public SlotUI[] player2Slots = new SlotUI[3];

    [Header("Nút skill HUD (3 nút tròn bên trái): chỉ hiện khi giành được skill")]
    [Tooltip("Nút dùng skill, đúng thứ tự slot 1, 2, 3. Nếu P2 không có bộ riêng thì đây là bộ DÙNG CHUNG cho cả hai người")]
    public UseButtonUI[] player1UseButtons = new UseButtonUI[3];
    [Tooltip("Nút dùng skill của P2. Để Size = 0 (trống) nếu cả hai người dùng chung bộ nút của P1")]
    public UseButtonUI[] player2UseButtons = new UseButtonUI[0];

    [Header("Thông báo")]
    public TMP_Text messageText;
    public float messageDuration = 1.5f;

    [Header("Logo phép Toggle (dưới joystick, tới khi bắt đầu cú đánh kế tiếp)")]
    [Tooltip("Object chứa logo (ẩn mặc định). Đặt nó DƯỚI joystick trong Editor.")]
    public GameObject toggleBadge;
    public Image toggleBadgeIcon;
    public TMP_Text toggleBadgeLabel;

    [Header("Look: bình thường")]
    public float litAlpha = 1f;
    public float dimAlpha = 0.3f;

    [Header("Look: skill bị khóa (cooldown) - mặc định TẮT")]
    [Tooltip("Off = không hiện gì. Flash = hiện thanh khóa vài giây. UntilShot = giữ tới khi bắt đầu cú đánh.")]
    public LockedDisplayMode lockedDisplay = LockedDisplayMode.Off;
    public float lockedFlashSeconds = 2.5f;
    public float lockedAlpha = 0.45f;
    public Color lockedTint = new Color(0.45f, 0.45f, 0.45f, 1f);
    public Color expiredTint = new Color(0.9f, 0.25f, 0.25f, 1f);
    public float pulseSpeed = 6f;

    [Header("Điều khiển cú đánh: ẩn khi đang chọn skill / đang đánh, hiện lại khi sẵn sàng đánh")]
    [Tooltip("Kéo vào: thanh lực, joystick english, panel chọn bi (và nút bật panel nếu có). Nên là object GỐC của từng thứ.")]
    public GameObject[] shotControls = new GameObject[0];
    [Tooltip("Tốc độ mờ dần / hiện dần. 0 = đổi tức thì")]
    public float controlsFadeSpeed = 10f;
    [Tooltip("Để trống sẽ tự tìm")]
    public CueStickController cueStick;

    [Header("Joystick nhường chỗ cho nút skill (cùng một vị trí)")]
    [Tooltip("Object nhường chỗ trong lúc skill READY (kéo joystick vào đây). Mỗi object ở đây PHẢI cũng nằm trong danh sách Shot Controls.")]
    public GameObject[] hideWhileSkillReady = new GameObject[0];
    [Tooltip("Mốc vị trí: kéo VÒNG NGOÀI của joystick (hình tròn cam) vào đây. Nút skill sẽ tự nhảy tới đúng tâm vòng này khi hiện.")]
    public RectTransform joystickAnchor;
    [Tooltip("true = nút skill cũng co giãn bằng đúng kích thước vòng joystick")]
    public bool matchJoystickSize = false;

    private SkillSelectionManager.State lastState = SkillSelectionManager.State.Idle;

    private CanvasGroup[] shotControlGroups = new CanvasGroup[0];
    private bool shotControlsVisibleLast = true;

    private bool[] hideInReady = new bool[0];
    private bool lastSkillReady = false;

    private bool badgeShownLast = false;

    // ================================================================
    private void Awake()
    {
        if (manager == null) manager = FindFirstObjectByType<SkillSelectionManager>();
        if (manager == null) Debug.LogError("[SkillUI] Không tìm thấy SkillSelectionManager!");

        InitSlots(player1Slots, 1);
        InitSlots(player2Slots, 2);
        InitUseButtons(player1UseButtons);
        InitUseButtons(player2UseButtons);

        if (toggleBadge != null) toggleBadge.SetActive(false);

        if (cueStick == null) cueStick = FindFirstObjectByType<CueStickController>();
        if (cueStick == null) Debug.LogWarning("[SkillUI] Không tìm thấy CueStickController: không ẩn/hiện được điều khiển cú đánh");
        InitShotControls();

        SetPanelVisible(false, false);
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

            slot.graphic = slot.button.targetGraphic;
            if (slot.graphic != null) slot.baseColor = slot.graphic.color;

            if (slot.lockOverlay != null) slot.lockOverlay.SetActive(false);
            if (slot.expiredHighlight != null) slot.expiredHighlight.SetActive(false);

            int p = player;
            int s = i + 1;
            slot.button.onClick.AddListener(() =>
            {
                if (manager != null) manager.RequestSkill(p, s);
            });
        }
    }

    private void InitUseButtons(UseButtonUI[] buttons)
    {
        if (buttons == null) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            UseButtonUI ub = buttons[i];
            if (ub == null || ub.button == null) continue;

            ub.group = ub.button.GetComponent<CanvasGroup>();
            if (ub.group == null) ub.group = ub.button.gameObject.AddComponent<CanvasGroup>();

            // An toàn: tắt mọi lệnh On Click đặt sẵn trong Inspector (vd SkillController.UI_ActivateSkillN)
            // để skill CHỈ chạy qua manager (đúng gate, đúng stamina, đúng phạt).
            int persistent = ub.button.onClick.GetPersistentEventCount();
            for (int k = 0; k < persistent; k++)
                ub.button.onClick.SetPersistentListenerState(k, UnityEventCallState.Off);
            if (persistent > 0)
                Debug.Log($"[SkillUI] Đã tắt {persistent} lệnh On Click cũ trên nút '{ub.button.name}'");

            // Chỉ nút của người đang giữ skill mới hiện, nên luôn gửi USE cho người đang READY
            ub.button.onClick.AddListener(() =>
            {
                if (manager != null) manager.RequestUse(manager.ReadyPlayer);
            });

            SetUseButtonVisible(ub, false);
        }
    }

    // ================================================================
    private void LateUpdate()
    {
        if (manager == null) return;

        SkillSelectionManager.State st = manager.CurrentState;
        bool selecting = st == SkillSelectionManager.State.Selecting;
        //bool showLocked = !selecting && st != SkillSelectionManager.State.Ready && ShouldShowLocked();
        bool showLocked = !selecting && st != SkillSelectionManager.State.Ready && st != SkillSelectionManager.State.Targeting && ShouldShowLocked();

        // Thanh chỉ nhận chạm khi đang Selecting. Ở trạng thái khóa thì chỉ để xem (không chặn input)
        SetPanelVisible(selecting || showLocked, selecting);

        UpdateUseButtons(st);

        UpdateShotControls();

        UpdateToggleBadge();

        if (st != lastState)
        {
            if (selecting) RefreshSkillInfo();
            lastState = st;
        }

        if (selecting)
        {
            RefreshSlots(player1Slots, 1);
            RefreshSlots(player2Slots, 2);

            if (countdownText != null)
                countdownText.text = $"P{manager.TurnPlayer}\n{Mathf.CeilToInt(manager.TimeLeft)}";
        }
        else if (showLocked)
        {
            RefreshSlotsLocked(player1Slots, 1);
            RefreshSlotsLocked(player2Slots, 2);

            if (countdownText != null) countdownText.text = "LOCKED";
        }

        if (messageText != null)
        {
            bool show = Time.unscaledTime - manager.LastMessageTime < messageDuration;
            messageText.text = show ? manager.LastMessage : "";
        }
    }

    private bool ShouldShowLocked()
    {
        if (lockedDisplay == LockedDisplayMode.Off || !manager.LockFeedbackActive) return false;
        if (lockedDisplay == LockedDisplayMode.UntilShot) return true;
        return Time.unscaledTime - manager.LockFeedbackTime < lockedFlashSeconds;
    }

    private void SetPanelVisible(bool visible, bool interactive)
    {
        if (rootGroup == null) return;
        rootGroup.alpha = visible ? 1f : 0f;
        rootGroup.blocksRaycasts = visible && interactive;
        rootGroup.interactable = visible && interactive;
    }

    // ================================================================
    // NÚT SKILL HUD: ẩn mặc định, chỉ hiện nút của skill vừa giành (state READY)
    // ================================================================
    private void UpdateUseButtons(SkillSelectionManager.State st)
    {
        bool ready = st == SkillSelectionManager.State.Ready;
        bool shared = !HasAnyButton(player2UseButtons); // P2 không có bộ riêng -> dùng chung bộ của P1

        ApplyUseButtons(player1UseButtons, ready, shared ? 0 : 1);
        if (!shared) ApplyUseButtons(player2UseButtons, ready, 2);
    }

    // forPlayer: 0 = bộ nút dùng chung (hiện cho bất kỳ ai đang READY), 1/2 = chỉ cho người đó
    private void ApplyUseButtons(UseButtonUI[] buttons, bool ready, int forPlayer)
    {
        if (buttons == null) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            UseButtonUI ub = buttons[i];
            if (ub == null || ub.button == null) continue;

            bool show = ready
                        && manager.ReadySlot == i + 1
                        && (forPlayer == 0 || manager.ReadyPlayer == forPlayer);

            SetUseButtonVisible(ub, show);
            if (!show) continue;

            // Tên + icon theo skill THẬT của người đang giữ (bộ nút dùng chung nên phải đổi theo người)
            BaseSkills skill = manager.GetSkill(manager.ReadyPlayer, i + 1);
            if (skill != null)
            {
                if (ub.label != null && ub.label.text != skill.skillName)
                    ub.label.text = skill.skillName;

                if (ub.icon != null)
                {
                    Sprite sp = skill.icon != null ? skill.icon : skill.skillIcon;
                    if (ub.icon.sprite != sp) ub.icon.sprite = sp;
                    ub.icon.enabled = sp != null;
                }
            }

            // Vòng đếm ngược thời gian dùng skill
            if (ub.cooldownFill != null)
            {
                float total = manager.readyDuration;
                ub.cooldownFill.fillAmount = total > 0f ? Mathf.Clamp01(manager.ReadyTimeLeft / total) : 1f;
            }
        }
    }

    private void SetUseButtonVisible(UseButtonUI ub, bool visible)
    {
        //if (ub.group == null) return;
        //ub.group.alpha = visible ? 1f : 0f;
        //ub.group.blocksRaycasts = visible;
        //ub.group.interactable = visible;
        //
        //if (ub.group == null) return;

        //if (ub.shown != visible)
        //{
        //    ub.shown = visible;
        //    Debug.Log($"[SkillUI] Nút HUD '{ub.button.name}' {(visible ? "HIỆN" : "ẨN")} | skillState={(manager != null ? manager.CurrentState.ToString() : "?")}");
        //}

        //ub.group.alpha = visible ? 1f : 0f;
        //ub.group.blocksRaycasts = visible;
        //ub.group.interactable = visible;
        //
        if (ub.group == null) return;

        if (ub.shown != visible)
        {
            ub.shown = visible;
            if (visible) SnapToJoystick(ub); // nhảy tới đúng vị trí joystick mỗi lần hiện
            Debug.Log($"[SkillUI] Nút HUD '{ub.button.name}' {(visible ? "HIỆN" : "ẨN")} | skillState={(manager != null ? manager.CurrentState.ToString() : "?")}");
        }

        ub.group.alpha = visible ? 1f : 0f;
        ub.group.blocksRaycasts = visible;
        ub.group.interactable = visible;
    }

    private void SnapToJoystick(UseButtonUI ub)
    {
        if (joystickAnchor == null || ub.button == null) return;

        RectTransform rt = ub.button.transform as RectTransform;
        if (rt == null) return;

        if (matchJoystickSize)
        {
            float sx = rt.lossyScale.x != 0f ? joystickAnchor.lossyScale.x / rt.lossyScale.x : 1f;
            float sy = rt.lossyScale.y != 0f ? joystickAnchor.lossyScale.y / rt.lossyScale.y : 1f;
            rt.sizeDelta = new Vector2(joystickAnchor.rect.width * sx, joystickAnchor.rect.height * sy);
        }

        Vector3 anchorCenter = joystickAnchor.TransformPoint(joystickAnchor.rect.center);
        Vector3 buttonCenter = rt.TransformPoint(rt.rect.center);
        rt.position += anchorCenter - buttonCenter;
    }
    private static bool HasAnyButton(UseButtonUI[] buttons)
    {
        if (buttons == null) return false;
        for (int i = 0; i < buttons.Length; i++)
            if (buttons[i] != null && buttons[i].button != null) return true;
        return false;
    }

    // ================================================================
    // THANH GIÀNH SKILL
    // ================================================================
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

    // ===== Selecting: sáng / tối theo stamina =====
    private void RefreshSlots(SlotUI[] slots, int player)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            SlotUI slot = slots[i];
            if (slot == null || slot.button == null || !slot.button.gameObject.activeSelf) continue;

            bool lit = manager.IsSlotLit(player, i + 1);
            slot.button.interactable = lit;
            if (slot.group != null) slot.group.alpha = lit ? litAlpha : dimAlpha;

            if (slot.graphic != null) slot.graphic.color = slot.baseColor;
            if (slot.lockOverlay != null) slot.lockOverlay.SetActive(false);
            if (slot.expiredHighlight != null) slot.expiredHighlight.SetActive(false);
        }
    }

    // ===== Locked: cooldown highlight (chỉ khi lockedDisplay != Off) =====
    private void RefreshSlotsLocked(SlotUI[] slots, int player)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            SlotUI slot = slots[i];
            if (slot == null || slot.button == null || !slot.button.gameObject.activeSelf) continue;

            bool expired = manager.LockedPlayer == player && manager.LockedSlot == i + 1;
            slot.button.interactable = false;

            if (expired)
            {
                float pulse = 0.85f + 0.15f * Mathf.Sin(Time.unscaledTime * pulseSpeed);
                if (slot.group != null) slot.group.alpha = pulse;
                if (slot.graphic != null) slot.graphic.color = expiredTint;
                if (slot.costText != null)
                    slot.costText.text = manager.LockedPenalty > 0f ? $"LOCKED -{manager.LockedPenalty:0.#}" : "LOCKED";
            }
            else
            {
                if (slot.group != null) slot.group.alpha = lockedAlpha;
                if (slot.graphic != null) slot.graphic.color = lockedTint;
            }

            if (slot.lockOverlay != null) slot.lockOverlay.SetActive(true);
            if (slot.expiredHighlight != null) slot.expiredHighlight.SetActive(expired);
        }
    }

    private void InitShotControls()
    {
        //if (shotControls == null) { shotControlGroups = new CanvasGroup[0]; return; }

        //shotControlGroups = new CanvasGroup[shotControls.Length];
        //for (int i = 0; i < shotControls.Length; i++)
        //{
        //    GameObject go = shotControls[i];
        //    if (go == null)
        //    {
        //        Debug.LogWarning($"[SkillUI] shotControls[{i}] đang trống");
        //        continue;
        //    }

        //    CanvasGroup cg = go.GetComponent<CanvasGroup>();
        //    if (cg == null) cg = go.AddComponent<CanvasGroup>();
        //    shotControlGroups[i] = cg;
        //}

        if (shotControls == null) shotControls = new GameObject[0];

        shotControlGroups = new CanvasGroup[shotControls.Length];
        hideInReady = new bool[shotControls.Length];

        for (int i = 0; i < shotControls.Length; i++)
        {
            GameObject go = shotControls[i];
            if (go == null)
            {
                Debug.LogWarning($"[SkillUI] shotControls[{i}] đang trống");
                continue;
            }

            CanvasGroup cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();
            shotControlGroups[i] = cg;

            if (hideWhileSkillReady != null)
            {
                for (int k = 0; k < hideWhileSkillReady.Length; k++)
                    if (hideWhileSkillReady[k] == go) hideInReady[i] = true;
            }
        }

        // Cảnh báo nếu object nằm trong hideWhileSkillReady mà không nằm trong shotControls
        if (hideWhileSkillReady != null)
        {
            for (int k = 0; k < hideWhileSkillReady.Length; k++)
            {
                GameObject h = hideWhileSkillReady[k];
                if (h == null) continue;

                bool found = false;
                for (int i = 0; i < shotControls.Length; i++)
                    if (shotControls[i] == h) found = true;

                if (!found)
                    Debug.LogWarning($"[SkillUI] '{h.name}' nằm trong Hide While Skill Ready nhưng KHÔNG nằm trong Shot Controls -> không có tác dụng. Hãy thêm nó vào Shot Controls.");
            }
        }
    }

    private void UpdateShotControls()
    {
        //if (cueStick == null || shotControlGroups == null) return;

        //bool visible = cueStick.ShotControlsVisible;

        //if (visible != shotControlsVisibleLast)
        //{
        //    shotControlsVisibleLast = visible;
        //    Debug.Log($"[SkillUI] Điều khiển cú đánh {(visible ? "HIỆN" : "ẨN")} | shotState={cueStick.shotState} | skillState={manager.CurrentState}");
        //}

        //float target = visible ? 1f : 0f;

        //for (int i = 0; i < shotControlGroups.Length; i++)
        //{
        //    CanvasGroup cg = shotControlGroups[i];
        //    if (cg == null) continue;

        //    cg.alpha = controlsFadeSpeed > 0f
        //        ? Mathf.MoveTowards(cg.alpha, target, Time.unscaledDeltaTime * controlsFadeSpeed)
        //        : target;

        //    // Ngừng nhận chạm ngay khi bắt đầu ẩn, không đợi mờ hẳn
        //    cg.blocksRaycasts = visible;
        //    cg.interactable = visible;
        //}

        if (cueStick == null || shotControlGroups == null) return;

        bool baseVisible = cueStick.ShotControlsVisible;
        bool skillReady = manager != null && manager.CurrentState == SkillSelectionManager.State.Ready;

        if (baseVisible != shotControlsVisibleLast)
        {
            shotControlsVisibleLast = baseVisible;
            Debug.Log($"[SkillUI] Điều khiển cú đánh {(baseVisible ? "HIỆN" : "ẨN")} | shotState={cueStick.shotState} | skillState={manager.CurrentState}");
        }

        if (skillReady != lastSkillReady)
        {
            lastSkillReady = skillReady;
            Debug.Log(skillReady
                ? "[SkillUI] Joystick NHƯỜNG CHỖ cho nút skill"
                : "[SkillUI] Joystick được phép HIỆN lại (nút skill đã ẩn)");
        }

        for (int i = 0; i < shotControlGroups.Length; i++)
        {
            CanvasGroup cg = shotControlGroups[i];
            if (cg == null) continue;

            // Joystick (hideInReady): nhường chỗ cho nút skill trong lúc skill đang READY
            bool visible = baseVisible && !(hideInReady[i] && skillReady);

            float target = visible ? 1f : 0f;
            cg.alpha = controlsFadeSpeed > 0f
                ? Mathf.MoveTowards(cg.alpha, target, Time.unscaledDeltaTime * controlsFadeSpeed)
                : target;

            // Ngừng nhận chạm ngay khi bắt đầu ẩn, không đợi mờ hẳn
            cg.blocksRaycasts = visible;
            cg.interactable = visible;
        }
    }

    private void UpdateToggleBadge()
    {
        if (toggleBadge == null || manager == null) return;

        BaseSkills skill = manager.BadgeSkill;
        bool show = skill != null;

        if (show != badgeShownLast)
        {
            badgeShownLast = show;
            toggleBadge.SetActive(show);
            Debug.Log($"[SkillUI] Logo bật tắt {(show ? "HIỆN: " + skill.skillName : "ẨN")}");
        }

        if (!show) return;

        if (toggleBadgeIcon != null)
        {
            Sprite sp = skill.icon != null ? skill.icon : skill.skillIcon;
            toggleBadgeIcon.sprite = sp;
            toggleBadgeIcon.enabled = sp != null;
        }

        if (toggleBadgeLabel != null && toggleBadgeLabel.text != skill.skillName)
            toggleBadgeLabel.text = skill.skillName;
    }
}
