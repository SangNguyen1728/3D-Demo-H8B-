using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[DefaultExecutionOrder(10000)]
public class SkillTargetingController : MonoBehaviour
{
    [Header("Tham chiếu (để trống sẽ tự tìm)")]
    public SkillSelectionManager manager;
    public CueStickController cueStick;

    [Header("UI (kéo từ Hierarchy)")]
    public CanvasGroup rootGroup;
    public SkillJoystick joystick;
    public Button confirmButton;
    public Button cancelButton;
    public Image skillLogo;
    public TMP_Text hintText;
    public TMP_Text timerText;
    [Tooltip("Vòng bám bi đang chọn (anchor + pivot ở giữa, Raycast Target TẮT)")]
    public RectTransform reticle;
    public Image reticleImage;
    [Tooltip("Tùy chọn: kéo VÒNG NGOÀI của joystick English vào đây. Joystick chọn sẽ nhảy tới đúng tâm vòng đó khi hiện")]
    public RectTransform joystickAnchor;

    [Header("Đánh dấu các bi có thể chọn (tùy chọn)")]
    [Tooltip("Mẫu (Image, nên kèm Button, TẮT sẵn) được nhân bản cho mỗi bi hợp lệ. Để trống = không đánh dấu.")]
    public RectTransform candidateTemplate;
    [Tooltip("Kích thước vòng đánh dấu (đơn vị UI)")]
    public float candidateRingSize = 60f;
    public Color candidateColor = new Color(1f, 1f, 1f, 0.4f);
    [Tooltip("true = chạm vào vòng của bi để chọn luôn (cần mẫu có component Button)")]
    public bool tapToSelect = true;

    [Header("Chọn bằng joystick")]
    [Tooltip("Kích thước vòng bám bi đang chọn (đơn vị UI)")]
    public float targetRingSize = 90f;
    [Tooltip("Gạt quá ngưỡng này mới tính là 1 lần chuyển bi")]
    public float navThreshold = 0.6f;
    [Tooltip("Phải thả về dưới ngưỡng này thì lần gạt sau mới tính")]
    public float navReleaseThreshold = 0.3f;
    [Range(20f, 89f)]
    [Tooltip("Chỉ xét bi nằm trong góc này quanh hướng gạt")]
    public float navAngleLimit = 70f;

    [Header("Màu vòng bi đang chọn")]
    public Color validColor = new Color(0.2f, 1f, 0.4f, 0.6f);
    public Color invalidColor = new Color(1f, 0.25f, 0.25f, 0.6f);

    [Header("Chữ mặc định (khi skill không có Targeting Hint)")]
    public string targetHint = "Chọn mục tiêu";

    [Header("Debug")]
    public bool verboseLog = true;

    private bool wasActive;
    private BaseSkills skill;
    private BallNo targetBall;
    private readonly List<BallNo> candidates = new List<BallNo>();
    private readonly List<RectTransform> markers = new List<RectTransform>();
    private bool navLatched;
    private BallNo lastPreviewBall;

    // ================================================================
    private void Awake()
    {
        if (manager == null) manager = FindFirstObjectByType<SkillSelectionManager>();
        if (manager == null) Debug.LogError("[SkillTargeting] Không tìm thấy SkillSelectionManager!");
        if (cueStick == null) cueStick = FindFirstObjectByType<CueStickController>();

        if (confirmButton != null) confirmButton.onClick.AddListener(OnConfirmClicked);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancelClicked);

        if (candidateTemplate != null) candidateTemplate.gameObject.SetActive(false);

        SetRootVisible(false);
    }

    private void OnConfirmClicked()
    {
        if (manager == null || skill == null || manager.CurrentState != SkillSelectionManager.State.Targeting) return;
        manager.RequestConfirm(manager.ReadyPlayer, BuildData());
    }

    private void OnCancelClicked()
    {
        if (manager == null || manager.CurrentState != SkillSelectionManager.State.Targeting) return;
        manager.RequestCancel(manager.ReadyPlayer);
    }

    private SkillTargetData BuildData()
    {
        SkillTargetData d = new SkillTargetData();
        d.ball = targetBall;
        return d;
    }

    // ================================================================
    private void LateUpdate()
    {
        if (manager == null) return;

        bool active = manager.CurrentState == SkillSelectionManager.State.Targeting;

        if (active && !wasActive) OnEnter();
        else if (!active && wasActive) OnExit();
        wasActive = active;

        SetRootVisible(active);
        if (!active || skill == null) return;

        Camera cam = GetCamera();
        if (cam == null) return;

        UpdateTarget(cam);
        UpdateCandidateMarkers(cam);

        if (timerText != null)
            timerText.text = manager.targetingDuration > 0f ? Mathf.CeilToInt(manager.TargetingTimeLeft).ToString() : "";
    }

    private void OnEnter()
    {
        skill = manager.TargetingSkill;
        navLatched = false;

        if (skill == null)
        {
            Debug.LogWarning("[SkillTargeting] Vào chế độ chọn nhưng manager không có TargetingSkill");
            return;
        }

        SnapToAnchor(joystick != null ? joystick.transform as RectTransform : null);
        if (joystick != null) joystick.ResetValue();

        if (skillLogo != null)
        {
            Sprite sp = skill.icon != null ? skill.icon : skill.skillIcon;
            skillLogo.sprite = sp;
            skillLogo.enabled = sp != null;
        }

        if (hintText != null)
            hintText.text = string.IsNullOrEmpty(skill.targetingHint) ? targetHint : skill.targetingHint;

        GlaszekManager g = PlayerManagerRegistry.Get(manager.ReadyPlayer);

        BuildCandidates(g);
        EnsureMarkers(candidates.Count);

        // Chọn sẵn bi gần bi trắng nhất
        Vector3 from = cueStick != null && cueStick.cueBall != null ? cueStick.cueBall.position : Vector3.zero;
        targetBall = FindNearestTo(from);

        Log($"VÀO chọn MỤC TIÊU | {skill.skillName} | {candidates.Count} bi hợp lệ | chọn sẵn: {(targetBall != null ? targetBall.name : "none")} | livePreview={skill.livePreview}");

        // Xem trước trực tiếp: tạo bản xem trước tại bi ban đầu
        lastPreviewBall = targetBall;
        if (skill.livePreview)
        {
            skill.BeginPreview(g != null ? g.gameObject : null, g, BuildData());
            Log("BeginPreview");
        }
    }

    private void OnExit()
    {
        if (joystick != null) joystick.ResetValue();
        HideAllMarkers();
        skill = null;
        targetBall = null;
        candidates.Clear();
        Log("THOÁT chế độ chọn");
    }

    // ================================================================
    // CHỌN MỤC TIÊU
    // ================================================================
    private void UpdateTarget(Camera cam)
    {
        Vector2 v = joystick != null ? joystick.Value : Vector2.zero;
        float mag = v.magnitude;

        if (mag < navReleaseThreshold)
        {
            navLatched = false;
        }
        else if (mag >= navThreshold && !navLatched)
        {
            navLatched = true;
            NavigateTarget(cam, v / mag);
        }

        bool valid = targetBall != null && targetBall.gameObject.activeInHierarchy;
        if (confirmButton != null) confirmButton.interactable = valid;
        SetReticleColor(valid);

        NotifyPreview();

        if (reticle == null) return;

        if (!valid) { SetReticleVisible(false); return; }

        Vector3 sp = cam.WorldToScreenPoint(targetBall.transform.position);
        if (sp.z <= 0f) { SetReticleVisible(false); return; }

        SetReticleVisible(true);
        reticle.position = new Vector3(sp.x, sp.y, 0f);
        reticle.sizeDelta = Vector2.one * targetRingSize;
    }

    private void BuildCandidates(GlaszekManager g)
    {
        candidates.Clear();

        BallNo[] all = FindObjectsByType<BallNo>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            BallNo b = all[i];
            if (b == null || b.isCueBall || !b.gameObject.activeInHierarchy) continue;
            if (!skill.IsValidTarget(b, g)) continue;
            candidates.Add(b);
        }
    }

    private BallNo FindNearestTo(Vector3 p)
    {
        BallNo best = null;
        float bestSq = float.MaxValue;

        for (int i = 0; i < candidates.Count; i++)
        {
            BallNo c = candidates[i];
            if (c == null || !c.gameObject.activeInHierarchy) continue;

            Vector3 d = c.transform.position - p;
            d.y = 0f;
            float sq = d.sqrMagnitude;
            if (sq < bestSq) { bestSq = sq; best = c; }
        }

        return best;
    }

    // Chọn bi gần nhất theo HƯỚNG GẠT trên màn hình (như điều hướng bằng tay cầm)
    private void NavigateTarget(Camera cam, Vector2 dir)
    {
        if (candidates.Count == 0) return;

        if (targetBall == null || !targetBall.gameObject.activeInHierarchy)
        {
            targetBall = FindNearestTo(Vector3.zero);
            return;
        }

        Vector3 cur = cam.WorldToScreenPoint(targetBall.transform.position);
        float cosLimit = Mathf.Cos(navAngleLimit * Mathf.Deg2Rad);

        BallNo best = null;
        float bestScore = float.MaxValue;

        for (int i = 0; i < candidates.Count; i++)
        {
            BallNo c = candidates[i];
            if (c == null || c == targetBall || !c.gameObject.activeInHierarchy) continue;

            Vector3 sp = cam.WorldToScreenPoint(c.transform.position);
            if (sp.z <= 0f) continue;

            Vector2 delta = new Vector2(sp.x - cur.x, sp.y - cur.y);
            float dist = delta.magnitude;
            if (dist < 1f) continue;

            float cos = Vector2.Dot(delta / dist, dir);
            if (cos < cosLimit) continue;

            float score = dist * (1.5f - cos); // gần + đúng hướng thì điểm thấp
            if (score < bestScore) { bestScore = score; best = c; }
        }

        if (best != null)
        {
            targetBall = best;
            Log($"Chuyển mục tiêu -> {best.name} (gạt {dir})");
        }
        else
        {
            Log($"Không có bi nào theo hướng gạt {dir}");
        }
    }

    // ================================================================
    // ĐÁNH DẤU CÁC BI CÓ THỂ CHỌN + CHẠM ĐỂ CHỌN
    // ================================================================
    private void EnsureMarkers(int count)
    {
        if (candidateTemplate == null) return;

        while (markers.Count < count)
        {
            RectTransform m = Instantiate(candidateTemplate, candidateTemplate.parent);
            m.name = $"CandidateMarker_{markers.Count}";
            m.SetSiblingIndex(candidateTemplate.GetSiblingIndex() + 1); // nằm DƯỚI vòng Reticle và các nút

            Image img = m.GetComponent<Image>();
            if (img != null)
            {
                img.color = candidateColor;
                img.raycastTarget = tapToSelect;
            }

            Button b = m.GetComponent<Button>();
            if (b != null)
            {
                int idx = markers.Count;
                b.onClick.RemoveAllListeners();
                if (tapToSelect) b.onClick.AddListener(() => OnMarkerClicked(idx));
                else b.interactable = false;
            }

            m.gameObject.SetActive(false);
            markers.Add(m);
        }
    }

    private void UpdateCandidateMarkers(Camera cam)
    {
        for (int i = 0; i < markers.Count; i++)
        {
            RectTransform m = markers[i];
            if (m == null) continue;

            bool show = false;

            if (i < candidates.Count)
            {
                BallNo b = candidates[i];

                // Bi đang chọn đã có vòng lớn (Reticle) nên không vẽ thêm vòng mờ
                if (b != null && b.gameObject.activeInHierarchy && b != targetBall)
                {
                    Vector3 sp = cam.WorldToScreenPoint(b.transform.position);
                    if (sp.z > 0f)
                    {
                        m.position = new Vector3(sp.x, sp.y, 0f);
                        m.sizeDelta = Vector2.one * candidateRingSize;
                        show = true;
                    }
                }
            }

            if (m.gameObject.activeSelf != show) m.gameObject.SetActive(show);
        }
    }

    private void HideAllMarkers()
    {
        for (int i = 0; i < markers.Count; i++)
            if (markers[i] != null && markers[i].gameObject.activeSelf) markers[i].gameObject.SetActive(false);
    }

    private void OnMarkerClicked(int idx)
    {
        if (manager == null || skill == null || manager.CurrentState != SkillSelectionManager.State.Targeting) return;
        if (idx < 0 || idx >= candidates.Count) return;

        BallNo b = candidates[idx];
        if (b == null || !b.gameObject.activeInHierarchy) return;

        targetBall = b;
        Log($"Chạm chọn mục tiêu -> {b.name}");
    }

    // ================================================================
    // XEM TRƯỚC TRỰC TIẾP
    // ================================================================
    private void NotifyPreview()
    {
        if (skill == null || !skill.livePreview) return;
        if (targetBall == lastPreviewBall) return;

        lastPreviewBall = targetBall;
        GlaszekManager g = PlayerManagerRegistry.Get(manager.ReadyPlayer);
        skill.UpdatePreview(g, BuildData());
    }

    // ================================================================
    // HELPERS
    // ================================================================
    private Camera GetCamera()
    {
        if (cueStick != null && cueStick.mainCamera != null) return cueStick.mainCamera;
        return Camera.main;
    }

    private void SetRootVisible(bool visible)
    {
        if (rootGroup == null) return;
        rootGroup.alpha = visible ? 1f : 0f;
        rootGroup.blocksRaycasts = visible;
        rootGroup.interactable = visible;
    }

    private void SetReticleVisible(bool visible)
    {
        if (reticleImage != null && reticleImage.enabled != visible) reticleImage.enabled = visible;
    }

    private void SetReticleColor(bool valid)
    {
        if (reticleImage != null) reticleImage.color = valid ? validColor : invalidColor;
    }

    private void SnapToAnchor(RectTransform rt)
    {
        if (rt == null || joystickAnchor == null) return;

        Vector3 anchorCenter = joystickAnchor.TransformPoint(joystickAnchor.rect.center);
        Vector3 center = rt.TransformPoint(rt.rect.center);
        rt.position += anchorCenter - center;
    }

    private void Log(string msg)
    {
        if (verboseLog) Debug.Log($"[SkillTargeting] {msg}");
    }
}
