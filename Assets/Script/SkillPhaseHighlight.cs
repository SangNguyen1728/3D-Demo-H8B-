using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(20000)]
public class SkillPhaseHighlight : MonoBehaviour
{
    [Header("Tham chiếu (để trống sẽ tự tìm)")]
    public SkillSelectionManager manager;
    public PocketTowPs pocket;

    [Header("Thanh ĐỎ: slider chuẩn, đặt DƯỚI 'SkillSelection' (không đặt trong ID1)")]
    public Slider redSlider1;
    public Slider redSlider2;

    [Header("Vị trí thanh ĐỎ: tự nhảy tới đúng chỗ thanh xanh gốc")]
    [Tooltip("RectTransform của thanh xanh gốc đang vẽ dưới avatar P1 (vd HighlightSlider1). Để trống = tự lấy từ slider của PocketTowPs")]
    public RectTransform snapTarget1;
    [Tooltip("Như trên, cho P2")]
    public RectTransform snapTarget2;
    [Tooltip("0 = cao bằng thanh gốc. >0 = ép chiều cao thanh đỏ (đơn vị UI) cho dễ nhìn, vd 8")]
    public float forceHeight = 0f;

    [Header("Khung avatar đỏ (tùy chọn): object hiện lên đè avatar trong giai đoạn dùng skill")]
    public GameObject avatarHighlight1;
    public GameObject avatarHighlight2;

    [Header("Tùy chọn")]
    [Tooltip("true = trong lúc đồng hồ lượt đóng băng, thanh xanh + số giữa HUD của người có lượt hiện đúng mức đồng hồ (đầy) thay vì giá trị cũ")]
    public bool syncNormalSliderWhileFrozen = true;
    public bool verboseLog = true;

    [Header("DEBUG (Play Mode): ép hiện thanh đỏ để kiểm tra giao diện, không cần giành skill")]
    [Tooltip("0 = tắt. 1 = ép thanh đỏ P1. 2 = ép thanh đỏ P2")]
    [Range(0, 2)] public int debugForcePlayer = 0;
    [Tooltip("Mức của thanh đỏ khi ép (kéo thanh trượt này lúc đang Play để xem thanh đỏ chạy)")]
    [Range(0f, 1f)] public float debugFraction = 0.6f;

    private bool phaseOn1, phaseOn2;
    private readonly Vector3[] corners = new Vector3[4];

    // ================================================================
    private void Awake()
    {
        if (manager == null) manager = FindFirstObjectByType<SkillSelectionManager>();
        if (manager == null) Debug.LogError("[SkillPhase] Không tìm thấy SkillSelectionManager!");

        if (pocket == null && manager != null) pocket = manager.pocket;
        if (pocket == null) pocket = FindFirstObjectByType<PocketTowPs>();
        if (pocket == null) Debug.LogError("[SkillPhase] Không tìm thấy PocketTowPs!");

        if (pocket != null)
        {
            Log($"Thanh highlight GỐC (PocketTowPs đang dùng): P1='{PathOf(pocket.highlightSlider_01)}' | P2='{PathOf(pocket.highlightSlider_02)}'");
            CheckPlacement(redSlider1, pocket.highlightSlider_01, "Red Slider 1");
            CheckPlacement(redSlider2, pocket.highlightSlider_02, "Red Slider 2");
        }

        PrepareRed(redSlider1, "Red Slider 1");
        PrepareRed(redSlider2, "Red Slider 2");
        Log($"Thanh ĐỎ: P1='{PathOf(redSlider1)}' | P2='{PathOf(redSlider2)}'");

        ResetVisuals();
    }

    private void OnDisable()
    {
        ResetVisuals();
    }

    private static void PrepareRed(Slider s, string label)
    {
        if (s == null)
        {
            Debug.LogWarning($"[SkillPhase] {label} chưa gán: bên này sẽ không có thanh đỏ");
            return;
        }

        if (s.fillRect == null)
            Debug.LogWarning($"[SkillPhase] {label} ('{s.name}') CHƯA có Fill Rect: đổi giá trị cũng KHÔNG thấy thanh chạy. Cần slider chuẩn (Slider > Fill Area > Fill) và gán ô Fill Rect của component Slider.");

        if (s.GetComponentInParent<Canvas>() == null)
            Debug.LogWarning($"[SkillPhase] {label} ('{PathOf(s)}') không nằm trong Canvas nào: sẽ không hiển thị.");

        // Thanh đỏ chạy theo phần trăm 0..1, không cho người chơi kéo
        s.minValue = 0f;
        s.maxValue = 1f;
        s.wholeNumbers = false;
        s.interactable = false;
    }

    // Cảnh báo nếu thanh đỏ nằm TRONG object của thanh highlight gốc (sẽ bị ẩn theo khi PocketTowPs bật/tắt nó)
    private static void CheckPlacement(Slider red, Slider normal, string label)
    {
        if (red == null || normal == null) return;

        if (red.transform.IsChildOf(normal.transform))
            Debug.LogWarning($"[SkillPhase] {label} ('{PathOf(red)}') đang nằm TRONG '{PathOf(normal)}' (thanh highlight gốc mà PocketTowPs bật/tắt theo lượt) nên sẽ bị ẩn theo. Hãy kéo thanh đỏ ra DƯỚI 'SkillSelection'.");
    }

    private void ResetVisuals()
    {
        phaseOn1 = false;
        phaseOn2 = false;

        if (redSlider1 != null) redSlider1.gameObject.SetActive(false);
        if (redSlider2 != null) redSlider2.gameObject.SetActive(false);
        if (avatarHighlight1 != null) avatarHighlight1.SetActive(false);
        if (avatarHighlight2 != null) avatarHighlight2.SetActive(false);
    }

    // ================================================================
    private void LateUpdate()
    {
        if (manager == null) return;

        SkillSelectionManager.State st = manager.CurrentState;
        //bool phase = st == SkillSelectionManager.State.Ready || st == SkillSelectionManager.State.Targeting;
        //int owner = phase ? manager.ReadyPlayer : 0;

        // Phần thời gian còn lại của giai đoạn dùng skill (1 -> 0)
        //float frac = 1f;
        //if (phase)
        //{
        //    if (st == SkillSelectionManager.State.Ready)
        //        frac = manager.readyDuration > 0f ? Mathf.Clamp01(manager.ReadyTimeLeft / manager.readyDuration) : 1f;
        //    else
        //        frac = manager.targetingDuration > 0f ? Mathf.Clamp01(manager.TargetingTimeLeft / manager.targetingDuration) : 1f;
        //}

        bool phase = st == SkillSelectionManager.State.Ready
                 || st == SkillSelectionManager.State.Targeting
                 || st == SkillSelectionManager.State.Performing;
        int owner = phase ? manager.ReadyPlayer : 0;

        // Phần thời gian còn lại của giai đoạn dùng skill (1 -> 0)
        float frac = 1f;
        if (phase)
        {
            float total = manager.SkillTimeTotal;
            frac = total > 0f ? Mathf.Clamp01(manager.SkillTimeLeft / total) : 1f;
        }

        // DEBUG: ép hiện thanh đỏ để kiểm tra giao diện
        if (debugForcePlayer > 0)
        {
            owner = debugForcePlayer;
            frac = debugFraction;
        }

        ApplySide(1, owner == 1, frac);
        ApplySide(2, owner == 2, frac);

        SyncNormalDisplay(st);
    }

    private void ApplySide(int player, bool on, float frac)
    {
        Slider red = player == 1 ? redSlider1 : redSlider2;
        RectTransform manual = player == 1 ? snapTarget1 : snapTarget2;
        Slider normal = pocket != null ? (player == 1 ? pocket.highlightSlider_01 : pocket.highlightSlider_02) : null;
        GameObject avatar = player == 1 ? avatarHighlight1 : avatarHighlight2;
        bool wasOn = player == 1 ? phaseOn1 : phaseOn2;

        // Đang bật: cập nhật giá trị và bám lại vị trí mỗi frame (phòng HUD đổi kích thước / vị trí)
        RectTransform bar = null;
        if (on && red != null)
        {
            red.value = frac;
            bar = GetBarRect(normal, manual);
            SnapTo(red.transform as RectTransform, bar);
        }

        if (on == wasOn) return;

        // Chỉ đổi trạng thái hiển thị khi có thay đổi
        if (player == 1) phaseOn1 = on; else phaseOn2 = on;

        if (red != null) red.gameObject.SetActive(on);
        if (avatar != null) avatar.SetActive(on);

        if (on && red != null && !red.gameObject.activeInHierarchy)
            Debug.LogWarning($"[SkillPhase] Thanh đỏ P{player} đã bật nhưng VẪN KHÔNG hiện vì object cha '{FirstInactiveAncestor(red.transform)}' đang tắt. Kéo thanh đỏ ra dưới 'SkillSelection'.");

        Log(on
            ? $"ĐỎ BẬT cho P{player} | bám theo '{PathOf(bar)}'"
            : $"ĐỎ TẮT cho P{player} (thanh xanh hiện lại, đồng hồ đánh sẽ bắt đầu đếm)");
    }

    // Thanh xanh gốc dưới avatar: ưu tiên object bạn gán tay, không thì lấy Fill Area của slider gốc
    private static RectTransform GetBarRect(Slider normal, RectTransform manual)
    {
        if (manual != null) return manual;
        if (normal == null) return null;

        if (normal.fillRect != null && normal.fillRect.parent != null)
        {
            RectTransform area = normal.fillRect.parent as RectTransform;
            if (area != null) return area;
        }

        return normal.transform as RectTransform;
    }

    // Đưa thanh đỏ trùng tâm + kích thước (theo màn hình) với target, bất kể 2 bên khác cha / anchor / scale
    private void SnapTo(RectTransform rt, RectTransform target)
    {
        if (rt == null || target == null) return;

        target.GetWorldCorners(corners);
        Vector3 center = (corners[0] + corners[2]) * 0.5f;
        float worldW = Vector3.Distance(corners[0], corners[3]);
        float worldH = Vector3.Distance(corners[0], corners[1]);

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.position = center;

        float sx = Mathf.Abs(rt.lossyScale.x) > 0.0001f ? rt.lossyScale.x : 1f;
        float sy = Mathf.Abs(rt.lossyScale.y) > 0.0001f ? rt.lossyScale.y : 1f;

        Vector2 size = new Vector2(worldW / sx, worldH / sy);
        if (forceHeight > 0f) size.y = forceHeight;
        rt.sizeDelta = size;
    }

    // Đồng hồ lượt đang đóng băng thì thanh xanh + số giữa HUD của người có lượt hiện đúng mức đồng hồ (đầy)
    private void SyncNormalDisplay(SkillSelectionManager.State st)
    {
        if (!syncNormalSliderWhileFrozen || pocket == null || pocket.totalTimeInput <= 0f) return;

        //bool frozen = st == SkillSelectionManager.State.Delay
        //           || st == SkillSelectionManager.State.Selecting
        //           || st == SkillSelectionManager.State.Ready
        //           || st == SkillSelectionManager.State.Targeting;

        bool frozen = st == SkillSelectionManager.State.Delay
                   || st == SkillSelectionManager.State.Selecting
                   || st == SkillSelectionManager.State.Ready
                   || st == SkillSelectionManager.State.Targeting
                   || st == SkillSelectionManager.State.Performing;

        if (!frozen) return;

        Slider s = pocket.currentPlayer == 1 ? pocket.highlightSlider_01 : pocket.highlightSlider_02;
        if (s != null) s.value = Mathf.Clamp01(pocket.timeRemaining / pocket.totalTimeInput);

        // Cùng định dạng với PocketTowPs.UpdateTimer
        if (pocket.totalTimetext != null)
            pocket.totalTimetext.text = string.Format("{0:00}", Mathf.Clamp(Mathf.FloorToInt(pocket.timeRemaining), 0, 99));
    }

    // ================================================================
    private static string FirstInactiveAncestor(Transform t)
    {
        while (t != null)
        {
            if (!t.gameObject.activeSelf) return PathOf(t);
            t = t.parent;
        }
        return "(không có)";
    }

    // Đường dẫn trong Hierarchy, để đọc trong Console xem game đang dùng object nào
    private static string PathOf(Component c)
    {
        if (c == null) return "(chưa gán)";

        string path = c.name;
        Transform t = c.transform.parent;
        while (t != null)
        {
            path = t.name + "/" + path;
            t = t.parent;
        }
        return path;
    }

    private void Log(string msg)
    {
        if (verboseLog) Debug.Log($"[SkillPhase] {msg}");
    }
}
