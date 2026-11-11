using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallHealthHUD : MonoBehaviour
{
    [Header("Refs")]
    public PocketTowPs pocket;
    [Tooltip("1 = Player 1, 2 = Player 2 (mỗi HUD phải khác nhau)")]
    public int playerIndex = 1;
    [Tooltip("Container riêng của HUD này (HealthBars_P1 / HealthBars_P2)")]
    public RectTransform container;

    [Header("Vị trí (tự đặt theo mốc)")]
    public bool autoPlace = true;
    [Tooltip("Mốc để căn (HealthAnchor_P1 / HealthAnchor_P2). Thanh sẽ nằm đúng chỗ mốc")]
    public RectTransform alignTo;
    [Tooltip("Tự bám theo mốc mỗi frame (bền khi UI trượt/đổi độ phân giải)")]
    public bool liveFollow = true;
    [Tooltip("Điểm trên mốc để bám. (0.5,0.5) = tâm mốc")]
    public Vector2 targetPoint = new Vector2(0.5f, 0.5f);
    [Tooltip("Điểm của HealthBars dùng để bám. (0.5,0.5) = tâm")]
    public Vector2 selfPoint = new Vector2(0.5f, 0.5f);
    [Tooltip("Dịch thêm so với mốc (đơn vị Canvas)")]
    public Vector2 nudge = Vector2.zero;

    [Header("Icon bi (index = số bi 1..15, ô 0 bỏ trống). Ô trống = hiện số bi")]
    public Sprite[] ballIcons = new Sprite[16];
    [Tooltip("Nếu mảng trên trống, lấy icon từ HUD này (đỡ phải gán 2 lần)")]
    public BallHealthHUD shareIconsWith;

    [Header("Kích thước (mỗi slot: icon ở trên, thanh máu ở dưới)")]
    public float slotWidth = 26f;
    public float iconSize = 22f;
    public float barHeight = 5f;
    public float gap = 2f;
    public float spacing = 3f;
    public TextAnchor alignment = TextAnchor.MiddleCenter;

    [Header("Màu máu")]
    [Range(0f, 1f)] public float yellowBelow = 0.5f;
    [Range(0f, 1f)] public float redBelow = 0.25f;
    public Color greenColor = new Color32(76, 217, 100, 255);
    public Color yellowColor = new Color32(255, 214, 10, 255);
    public Color redColor = new Color32(255, 59, 48, 255);
    public Color backColor = new Color(0f, 0f, 0f, 0.6f);

    [Header("Trạng thái bi")]
    [Tooltip("TẮT = luôn hiện đủ slot, bi chết thì mờ (phương án B). BẬT = bi chết thì ẩn")]
    public bool hideDeadSlots = false;
    [Range(0f, 1f)] public float deadAlpha = 0.25f;
    [Tooltip("BẬT = bi 8 luôn hiện (mờ tới khi hết bi nhóm). TẮT = ẩn tới khi hết bi nhóm")]
    public bool showLockedEight = true;
    [Range(0f, 1f)] public float lockedAlpha = 0.4f;

    [Header("Animation")]
    public float smoothSpeed = 10f;
    public float refreshInterval = 0.1f;

    [Header("Debug")]
    [Tooltip("Tô xanh lá mờ vùng container để căn vị trí")]
    public bool showContainerBounds = false;

    [Tooltip("BẬT = khi đã hết bi nhóm, ẨN toàn bộ slot bi nhóm, chỉ còn bi 8")]
    public bool hideGroupWhenCleared = true;
    private class Slot
    {
        public BallNo ball;
        public BallHealth health;
        public GameObject root;
        public CanvasGroup group;
        public RectTransform fill;
        public Image fillImage;
        public float shown = 1f;
        public bool alive = true;
        public bool visible = true;
        public float alpha = 1f;
    }

    private readonly List<Slot> groupSlots = new List<Slot>();
    private Slot eightSlot;
    private float refreshTimer;
    private bool isCleared;
    bool UseClearedPlacement => isCleared && hideGroupWhenCleared && clearedAnchor != null;
    RectTransform CurrentTarget => UseClearedPlacement ? clearedAnchor : alignTo;
    private bool subscribed;

    private static readonly Vector3[] corners = new Vector3[4];
    private static readonly Dictionary<int, BallHealthHUD> byIndex = new Dictionary<int, BallHealthHUD>();
    private static readonly Dictionary<RectTransform, BallHealthHUD> byContainer = new Dictionary<RectTransform, BallHealthHUD>();

    string Tag => $"[BallHealthHUD P{playerIndex}/{name}]";

    [Tooltip("BẬT = luôn hiện đủ 7 bi nhóm + bi 8 cho cả 2 người chơi (bỏ qua 2 ô bên dưới). Bi chết thì mờ, không biến mất")]
    public bool alwaysShowAllSlots = true;

    [Tooltip("BẬT = đảo thứ tự slot (dùng cho Player 1 để đối xứng gương với Player 2)")]
    public bool reverseOrder = false;
    [Tooltip("BẬT = thanh máu tụt từ trái sang phải (đối xứng gương)")]
    public bool mirrorFill = false;

    [Tooltip("BẬT = số bi nằm DƯỚI thanh máu. TẮT = số bi nằm trên thanh máu")]
    public bool numberBelowBar = true;

    [Header("Khi chỉ còn bi 8 (đã hết bi nhóm)")]
    [Tooltip("Mốc khi chỉ còn bi 8, thường là icon nhóm (Solid/Striped). Trống = vẫn dùng Align To")]
    public RectTransform clearedAnchor;
    [Tooltip("Điểm trên mốc để bám. (0.5, 0) = giữa-đáy icon")]
    public Vector2 clearedTargetPoint = new Vector2(0.5f, 0f);
    [Tooltip("Điểm của HealthBars dùng để bám. (0.5, 1) = giữa-đỉnh, nên thanh treo ngay DƯỚI icon")]
    public Vector2 clearedSelfPoint = new Vector2(0.5f, 1f);
    [Tooltip("Dịch thêm (Y âm = thấp xuống)")]
    public Vector2 clearedNudge = new Vector2(0f, -2f);

    // ================= LIFECYCLE =================
    void Awake()
    {
        if (pocket == null) pocket = FindFirstObjectByType<PocketTowPs>();
        ResolveContainer();
    }

    void OnEnable()
    {
        if (pocket == null)
        {
            Debug.LogError($"{Tag} Không tìm thấy PocketTowPs!");
            return;
        }

        if (pocket.gameMode != PocketTowPs.PoolGameMode.EightBall)
        {
            Debug.Log($"{Tag} Không phải chế độ 8 bi -> HUD máu không chạy");
            return;
        }

        if (!RegisterOwners()) return;

        pocket.OnGroupAssigned += Rebuild;
        subscribed = true;

        if (pocket.groupAssigned) Rebuild();
    }

    void OnDisable()
    {
        if (subscribed && pocket != null) pocket.OnGroupAssigned -= Rebuild;
        subscribed = false;

        if (byIndex.TryGetValue(playerIndex, out var a) && a == this) byIndex.Remove(playerIndex);
        if (container != null && byContainer.TryGetValue(container, out var b) && b == this) byContainer.Remove(container);
    }

    // ================= SETUP CHECKS =================
    void ResolveContainer()
    {
        RectTransform self = transform as RectTransform;
        if (container != null && container != self) return;

        GameObject go = new GameObject("HealthBars_Auto", typeof(RectTransform));
        go.transform.SetParent(transform, false);
        container = go.GetComponent<RectTransform>();
        container.sizeDelta = new Vector2(240f, 30f);

        Debug.LogWarning($"{Tag} Field Container đang trống -> tự tạo '{go.name}'. Hãy kéo HealthBars_P{playerIndex} vào field Container.");
    }

    bool RegisterOwners()
    {
        if (byIndex.TryGetValue(playerIndex, out var a) && a != null && a != this)
        {
            Debug.LogError($"{Tag} Player Index = {playerIndex} đã bị '{a.name}' dùng. Hai HUD không được trùng Player Index.");
            return false;
        }

        if (byContainer.TryGetValue(container, out var b) && b != null && b != this)
        {
            Debug.LogError($"{Tag} Container '{container.name}' đang dùng chung với '{b.name}'. Mỗi người chơi cần HealthBars riêng.");
            return false;
        }

        byIndex[playerIndex] = this;
        byContainer[container] = this;
        return true;
    }

    // ================= PLACEMENT =================
    void ApplyPlacement(int forceCount = -1)
    {
        //if (!autoPlace || container == null) return;

        //int n = forceCount >= 0 ? forceCount : groupSlots.Count + (eightSlot != null ? 1 : 0);
        //float w = n * slotWidth + Mathf.Max(0, n - 1) * spacing;
        //float h = iconSize + gap + barHeight;

        //container.localScale = Vector3.one;
        //container.localRotation = Quaternion.identity;
        //container.pivot = selfPoint;
        //container.sizeDelta = new Vector2(w, h);

        //if (alignTo != null)
        //{
        //    Canvas.ForceUpdateCanvases();
        //    container.anchorMin = container.anchorMax = new Vector2(0.5f, 0.5f);
        //    PlaceOnTarget();
        //}
        //else
        //{
        //    container.anchorMin = container.anchorMax = targetPoint;
        //    container.anchoredPosition = nudge;
        //}

        //LayoutElement le = container.GetComponent<LayoutElement>();
        //if (le == null) le = container.gameObject.AddComponent<LayoutElement>();
        //le.ignoreLayout = true;

        //if (container.parent != transform)
        //    Debug.LogWarning($"{Tag} Container '{container.name}' đang nằm dưới '{container.parent.name}', KHÔNG phải con trực tiếp của '{name}'.");

        //Debug.Log($"{Tag} Đặt vị trí: size={w:0}x{h:0} | mốc='{(alignTo != null ? alignTo.name : "cha: " + container.parent.name)}'");

        if (!autoPlace || container == null) return;

        bool single = UseClearedPlacement;
        int n = forceCount >= 0 ? forceCount
              : single ? 1
              : groupSlots.Count + (eightSlot != null ? 1 : 0);

        float w = n * slotWidth + Mathf.Max(0, n - 1) * spacing;
        float h = iconSize + gap + barHeight;

        container.localScale = Vector3.one;
        container.localRotation = Quaternion.identity;
        container.pivot = single ? clearedSelfPoint : selfPoint;
        container.sizeDelta = new Vector2(w, h);

        HorizontalLayoutGroup hl = container.GetComponent<HorizontalLayoutGroup>();
        if (hl != null) hl.childAlignment = single ? TextAnchor.MiddleCenter : alignment;

        if (CurrentTarget != null)
        {
            Canvas.ForceUpdateCanvases();
            container.anchorMin = container.anchorMax = new Vector2(0.5f, 0.5f);
            PlaceOnTarget();
        }
        else
        {
            container.anchorMin = container.anchorMax = targetPoint;
            container.anchoredPosition = nudge;
        }

        LayoutElement le = container.GetComponent<LayoutElement>();
        if (le == null) le = container.gameObject.AddComponent<LayoutElement>();
        le.ignoreLayout = true;

        if (container.parent != transform)
            Debug.LogWarning($"{Tag} Container '{container.name}' đang nằm dưới '{container.parent.name}', KHÔNG phải con trực tiếp của '{name}'.");

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);

        Debug.Log($"{Tag} Đặt vị trí: size={w:0}x{h:0} | mốc='{(CurrentTarget != null ? CurrentTarget.name : "cha")}' | chỉ còn bi 8={single}");
    }

    void PlaceOnTarget()
    {
        //alignTo.GetWorldCorners(corners); // 0 = dưới-trái, 2 = trên-phải
        //Vector3 world = new Vector3(
        //    Mathf.Lerp(corners[0].x, corners[2].x, targetPoint.x),
        //    Mathf.Lerp(corners[0].y, corners[2].y, targetPoint.y),
        //    corners[0].z);

        //container.position = world;
        //container.anchoredPosition += nudge;

        bool single = UseClearedPlacement;
        RectTransform target = single ? clearedAnchor : alignTo;
        Vector2 tp = single ? clearedTargetPoint : targetPoint;
        Vector2 nd = single ? clearedNudge : nudge;

        target.GetWorldCorners(corners); // 0 = dưới-trái, 2 = trên-phải
        Vector3 world = new Vector3(
            Mathf.Lerp(corners[0].x, corners[2].x, tp.x),
            Mathf.Lerp(corners[0].y, corners[2].y, tp.y),
            corners[0].z);

        container.position = world;
        container.anchoredPosition += nd;
    }

    void LateUpdate()
    {
        //if (!autoPlace || !liveFollow || alignTo == null || container == null) return;
        //if (groupSlots.Count == 0 && eightSlot == null) return;
        //PlaceOnTarget();

        if (!autoPlace || !liveFollow || container == null) return;
        if (CurrentTarget == null) return;
        if (groupSlots.Count == 0 && eightSlot == null) return;
        PlaceOnTarget();
    }

    [ContextMenu("Preview Placement (xem vị trí thật khi chưa Play)")]
    void PreviewPlacement()
    {
        if (container == null)
        {
            Debug.LogWarning($"{Tag} Chưa gán Container, không preview được.");
            return;
        }

        ApplyPlacement(8); // giả lập đủ 7 slot + bi 8

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(container);
        UnityEditor.EditorUtility.SetDirty(container.gameObject);
#endif
        Debug.Log($"{Tag} Đã áp dụng vị trí preview. Nhấn Ctrl+S để lưu scene.");
    }

    // ================= BUILD =================
    [ContextMenu("Force Rebuild")]
    void Rebuild()
    {
        if (!isActiveAndEnabled) return;

        ClearSlots();

        string group = pocket.GetPlayerGroup(playerIndex);
        if (string.IsNullOrEmpty(group))
        {
            Debug.Log($"{Tag} Chưa có nhóm, chưa build");
            return;
        }

        EnsureLayout();

        BallNo[] all = FindObjectsByType<BallNo>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        List<BallNo> list = new List<BallNo>();

        foreach (BallNo b in all)
        {
            if (b.isCueBall) continue;
            int n = b.ballNumber;
            bool inGroup = group == "SOLID" ? (n >= 1 && n <= 7) : (n >= 9 && n <= 15);
            if (inGroup || n == 8) list.Add(b);
        }

        list.Sort((x, y) =>
        {
            int kx = x.ballNumber == 8 ? 99 : x.ballNumber;
            int ky = y.ballNumber == 8 ? 99 : y.ballNumber;
            return kx.CompareTo(ky);
        });

        foreach (BallNo b in list)
        {
            BallHealth hp = b.GetComponent<BallHealth>();
            if (hp == null)
            {
                Debug.LogWarning($"{Tag} Bi {b.ballNumber} không có BallHealth -> bỏ qua");
                continue;
            }

            Slot s = CreateSlot(b, hp);
            if (b.ballNumber == 8) eightSlot = s;
            else groupSlots.Add(s);
        }

        string built = "";
        foreach (Slot sl in groupSlots) built += sl.ball.ballNumber + " ";
        if (eightSlot != null) built += "8";
        Debug.Log($"{Tag} Slot đã tạo: [{built.Trim()}]");

        if (groupSlots.Count < 7)
            Debug.LogWarning($"{Tag} Chỉ tạo được {groupSlots.Count}/7 slot nhóm {group}. Có bi bị thiếu BallNo/BallHealth hoặc đã bị Destroy trước khi gán nhóm.");
        if (eightSlot == null)
            Debug.LogWarning($"{Tag} KHÔNG có slot bi 8. Kiểm tra bi 8 có BallNo.ballNumber = 8 và có component BallHealth.");

        ApplyPlacement();
        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        RefreshStates();

        Debug.Log($"<color=cyan>{Tag} Build xong nhóm {group}: {groupSlots.Count} bi + bi 8 {(eightSlot != null ? "CÓ" : "KHÔNG CÓ")} | container='{container.name}'</color>");
    }

    void ClearSlots()
    {
        foreach (Slot s in groupSlots) if (s.root != null) Destroy(s.root);
        if (eightSlot != null && eightSlot.root != null) Destroy(eightSlot.root);
        groupSlots.Clear();
        eightSlot = null;
        isCleared = false;
    }

    void EnsureLayout()
    {
        HorizontalLayoutGroup h = container.GetComponent<HorizontalLayoutGroup>();
        if (h == null && container.GetComponent<LayoutGroup>() == null)
            h = container.gameObject.AddComponent<HorizontalLayoutGroup>();

        if (h != null)
        {
            h.spacing = spacing;
            h.childAlignment = alignment;
            h.childControlWidth = true;
            h.childControlHeight = true;
            h.childForceExpandWidth = false;
            h.childForceExpandHeight = false;
            h.reverseArrangement = reverseOrder;
        }

        if (showContainerBounds)
        {
            Image bg = container.GetComponent<Image>();
            if (bg == null) bg = container.gameObject.AddComponent<Image>();
            bg.color = new Color(0f, 1f, 0f, 0.25f);
            bg.raycastTarget = false;
        }
    }

    Sprite GetIcon(int n)
    {
        Sprite[] src = ballIcons;
        if (!HasAny(src) && shareIconsWith != null && shareIconsWith != this) src = shareIconsWith.ballIcons;
        return (src != null && n >= 0 && n < src.Length) ? src[n] : null;
    }

    static bool HasAny(Sprite[] arr)
    {
        if (arr == null) return false;
        foreach (Sprite s in arr) if (s != null) return true;
        return false;
    }

    Slot CreateSlot(BallNo ball, BallHealth hp)
    {
        float h = iconSize + gap + barHeight;

        GameObject root = new GameObject("HP_" + ball.ballNumber,
            typeof(RectTransform), typeof(LayoutElement), typeof(CanvasGroup));
        root.transform.SetParent(container, false);

        LayoutElement le = root.GetComponent<LayoutElement>();
        le.preferredWidth = slotWidth;
        le.preferredHeight = h;

        CanvasGroup cg = root.GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;

        // ---- ICON (sprite nếu có, không thì hiện số bi) ----
        GameObject iconGO = new GameObject("Icon", typeof(RectTransform));
        iconGO.transform.SetParent(root.transform, false);
        RectTransform ir = iconGO.GetComponent<RectTransform>();
        //ir.anchorMin = ir.anchorMax = new Vector2(0.5f, 1f);
        //ir.pivot = new Vector2(0.5f, 1f);

        float iy = numberBelowBar ? 0f : 1f;
        ir.anchorMin = ir.anchorMax = new Vector2(0.5f, iy);
        ir.pivot = new Vector2(0.5f, iy);

        ir.sizeDelta = new Vector2(iconSize, iconSize);
        ir.anchoredPosition = Vector2.zero;

        Sprite sp = GetIcon(ball.ballNumber);
        if (sp != null)
        {
            Image img = iconGO.AddComponent<Image>();
            img.sprite = sp;
            img.preserveAspect = true;
            img.raycastTarget = false;
        }
        else
        {
            TextMeshProUGUI tmp = iconGO.AddComponent<TextMeshProUGUI>();
            tmp.text = ball.ballNumber.ToString();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = iconSize * 0.75f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.raycastTarget = false;
        }

        // ---- NỀN THANH ----
        GameObject barGO = new GameObject("Bar", typeof(RectTransform), typeof(Image));
        barGO.transform.SetParent(root.transform, false);
        RectTransform br = barGO.GetComponent<RectTransform>();
        //br.anchorMin = new Vector2(0f, 0f);
        //br.anchorMax = new Vector2(1f, 0f);
        //br.pivot = new Vector2(0.5f, 0f);
        float by = numberBelowBar ? 1f : 0f;
        br.anchorMin = new Vector2(0f, by);
        br.anchorMax = new Vector2(1f, by);
        br.pivot = new Vector2(0.5f, by);
        br.sizeDelta = new Vector2(0f, barHeight);
        br.anchoredPosition = Vector2.zero;
        Image bg = barGO.GetComponent<Image>();
        bg.color = backColor;
        bg.raycastTarget = false;

        // ---- FILL (co giãn bằng anchorMax.x) ----
        GameObject fillGO = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillGO.transform.SetParent(barGO.transform, false);
        RectTransform fr = fillGO.GetComponent<RectTransform>();
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        fr.offsetMin = fr.offsetMax = Vector2.zero;
        Image fi = fillGO.GetComponent<Image>();
        fi.color = greenColor;
        fi.raycastTarget = false;

        return new Slot
        {
            ball = ball,
            health = hp,
            root = root,
            group = cg,
            fill = fr,
            fillImage = fi,
            shown = 1f
        };
    }

    // ================= UPDATE =================
    void Update()
    {
        if (groupSlots.Count == 0 && eightSlot == null) return;

        refreshTimer -= Time.unscaledDeltaTime;
        if (refreshTimer <= 0f)
        {
            refreshTimer = refreshInterval;
            RefreshStates();
        }

        float k = 1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime);
        foreach (Slot s in groupSlots) UpdateFill(s, k);
        if (eightSlot != null) UpdateFill(eightSlot, k);
    }

    void UpdateFill(Slot s, float k)
    {
        if (s.root == null || !s.root.activeSelf) return;

        float p = (s.alive && s.health.maxHealth > 0f)
            ? Mathf.Clamp01(s.health.GetCurrentHealth() / s.health.maxHealth)
            : 0f;

        s.shown = Mathf.Lerp(s.shown, p, k);
        //s.fill.anchorMax = new Vector2(s.shown, 1f);

        if (mirrorFill)
        {
            s.fill.anchorMin = new Vector2(1f - s.shown, 0f);
            s.fill.anchorMax = Vector2.one;
        }
        else
        {
            s.fill.anchorMin = Vector2.zero;
            s.fill.anchorMax = new Vector2(s.shown, 1f);
        }

        s.fillImage.color = GetColor(p);
    }

    Color GetColor(float p)
    {
        if (p < redBelow) return redColor;
        if (p < yellowBelow) return yellowColor;
        return greenColor;
    }

    void RefreshStates()
    {
        //bool groupAlive = false;
        //foreach (Slot s in groupSlots)
        //    if (IsAlive(s)) { groupAlive = true; break; }

        //// đã hết bi nhóm (chỉ tính khi đã build được slot nhóm)
        //bool cleared = groupSlots.Count > 0 && !groupAlive;

        //foreach (Slot s in groupSlots)
        //{
        //    bool alive = IsAlive(s);
        //    bool visible = alwaysShowAllSlots || alive || !hideDeadSlots;
        //    if (cleared && hideGroupWhenCleared) visible = false;   // hết bi -> ẩn hết, chỉ còn bi 8
        //    ApplyState(s, alive, visible, alive ? 1f : deadAlpha);
        //}

        //if (eightSlot != null)
        //{
        //    bool alive = IsAlive(eightSlot);

        //    // tick Show Locked Eight = luôn hiện (mờ tới khi mở khoá); bỏ tick = chỉ hiện khi đã hết bi nhóm
        //    bool visible = showLockedEight || cleared;
        //    if (!alive && hideDeadSlots) visible = false;

        //    float alpha = !alive ? deadAlpha : (cleared ? 1f : lockedAlpha);
        //    ApplyState(eightSlot, alive, visible, alpha);
        //}

        bool groupAlive = false;
        foreach (Slot s in groupSlots)
            if (IsAlive(s)) { groupAlive = true; break; }

        bool cleared = groupSlots.Count > 0 && !groupAlive;

        foreach (Slot s in groupSlots)
        {
            bool alive = IsAlive(s);
            bool visible = alwaysShowAllSlots || alive || !hideDeadSlots;
            if (cleared && hideGroupWhenCleared) visible = false;
            ApplyState(s, alive, visible, alive ? 1f : deadAlpha);
        }

        if (eightSlot != null)
        {
            bool alive = IsAlive(eightSlot);
            bool visible = showLockedEight || cleared;
            if (!alive && hideDeadSlots) visible = false;
            float alpha = !alive ? deadAlpha : (cleared ? 1f : lockedAlpha);
            ApplyState(eightSlot, alive, visible, alpha);
        }

        // trạng thái "hết bi nhóm" vừa đổi -> đặt lại vị trí container
        if (cleared != isCleared)
        {
            isCleared = cleared;
            Debug.Log($"{Tag} Hết bi nhóm = {cleared}");
            ApplyPlacement();
        }
    }

    static bool IsAlive(Slot s)
    {
        return s.ball != null && s.ball.gameObject.activeInHierarchy;
    }

    void ApplyState(Slot s, bool alive, bool visible, float alpha)
    {
        if (s.root == null) return;

        if (alive != s.alive)
        {
            s.alive = alive;
            if (alive) s.shown = 0f;
            Debug.Log($"{Tag} Bi {s.ball.ballNumber} -> {(alive ? "SỐNG" : "CHẾT/VÀO LỖ")}");
        }

        if (visible != s.visible)
        {
            s.visible = visible;
            s.root.SetActive(visible);
            Debug.Log($"{Tag} Bi {s.ball.ballNumber} slot -> {(visible ? "HIỆN" : "ẨN")}");
        }

        if (!Mathf.Approximately(alpha, s.alpha))
        {
            s.alpha = alpha;
            s.group.alpha = alpha;
        }
    }
}
