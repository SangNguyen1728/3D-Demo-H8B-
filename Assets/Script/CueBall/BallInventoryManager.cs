using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class BallInventoryManager : MonoBehaviour
{
    //public static BallInventoryManager Instance { get; private set; }

    //[Header("Refs")]
    //public CueBallController cueBallController;
    //public CueStickController cueStickController;

    //[Header("Configs")]
    //public BallScaleConfig normalConfig;

    //public BallScaleConfig CurrentConfig { get; private set; }

    //// UI đăng ký lắng nghe sự kiện này để tự cập nhật highlight
    //public event Action<BallScaleConfig> OnBallSelected;

    //void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Instance = this;
    //}

    //void Start()
    //{
    //    SelectBall(normalConfig, forced: true);
    //}

    //// Chỉ cho chọn bi khi chưa vào pha đánh (tránh đổi bi giữa chừng cú đánh)
    //public bool CanSelect()
    //{
    //    if (cueStickController == null) return true;
    //    return !cueStickController.isMoving && !cueStickController.hitPeriod;
    //}

    //public void SelectBall(BallScaleConfig config)
    //{
    //    SelectBall(config, forced: false);
    //}

    //private void SelectBall(BallScaleConfig config, bool forced)
    //{
    //    if (config == null) return;

    //    if (!forced && !CanSelect())
    //    {
    //        Debug.Log("[BallInventoryManager] Không thể đổi bi lúc này (đang trong cú đánh)");
    //        return;
    //    }

    //    CurrentConfig = config;
    //    cueBallController.ApplyScaleConfig(config);

    //    OnBallSelected?.Invoke(config);

    //    Debug.Log($"[BallInventoryManager] Đã chọn bi: {config.mode}");
    //}

    //// Gọi từ CueStickController ngay sau khi 1 cú đánh xử lý xong hoàn toàn
    //public void ResetToNormalForNextShot()
    //{
    //    SelectBall(normalConfig, forced: true);

    //    Debug.Log("[BallInventoryManager] Reset về bi bình thường cho lượt kế tiếp");
    //}

    //public static BallInventoryManager Instance { get; private set; }

    //[Header("Refs")]
    //public CueBallController cueBallController;
    //public CueStickController cueStickController;

    //[Header("Configs")]
    //public BallScaleConfig normalConfig;

    //[Header("Cooldown")]
    //[Tooltip("Số lượt cooldown áp dụng cho 1 bi đặc biệt sau khi được dùng")]
    //public int cooldownTurns = 2;

    //public BallScaleConfig CurrentConfig { get; private set; }

    //// Cooldown còn lại của từng config — chỉ chứa config đang bị khoá
    //private Dictionary<BallScaleConfig, int> cooldowns = new Dictionary<BallScaleConfig, int>();

    //// UI đăng ký lắng nghe để tự cập nhật highlight khi đổi bi đang chọn
    //public event Action<BallScaleConfig> OnBallSelected;

    //// UI đăng ký lắng nghe để tự cập nhật trạng thái khoá/mở do cooldown
    //public event Action OnCooldownChanged;

    //[Header("Dynamic Panel — tự tạo nút theo loadout từ HomeScene")]
    //public Transform buttonsContainer;      // 🆕 kéo BallSelectionPanel vào đây
    //public GameObject ballButtonPrefab;

    //public event Action OnUserSelectedBall;
    //void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Instance = this;
    //}

    //void Start()
    //{
    //    // BallScaleConfig startingConfig = SceneLoader.Instance != null && SceneLoader.Instance.SelectedBallConfig != null
    //    //? SceneLoader.Instance.SelectedBallConfig
    //    //: normalConfig;

    //    // SelectBall(startingConfig, forced: true);

    //    // SelectBall(normalConfig, forced: true);

    //    List<BallScaleConfig> loadout = SceneLoader.Instance != null && SceneLoader.Instance.SelectedBallLoadout != null && SceneLoader.Instance.SelectedBallLoadout.Count > 0
    //   ? SceneLoader.Instance.SelectedBallLoadout
    //   : new List<BallScaleConfig> { normalConfig };

    //    foreach (var config in loadout)
    //    {
    //        GameObject btnObj = Instantiate(ballButtonPrefab, buttonsContainer);
    //        BallSelectionButtonUI btnUI = btnObj.GetComponent<BallSelectionButtonUI>();
    //        btnUI.config = config;
    //    }

    //    SelectBall(normalConfig, forced: true);
    //}

    //public bool CanSelect()
    //{
    //    if (cueStickController == null) return true;
    //    return !cueStickController.isMoving && !cueStickController.hitPeriod;
    //}

    //public bool IsOnCooldown(BallScaleConfig config)
    //{
    //    return config != null && cooldowns.ContainsKey(config) && cooldowns[config] > 0;
    //}

    //public int GetCooldownRemaining(BallScaleConfig config)
    //{
    //    if (config == null) return 0;
    //    return cooldowns.TryGetValue(config, out int value) ? value : 0;
    //}

    //public void SelectBall(BallScaleConfig config)
    //{
    //    SelectBall(config, forced: false);
    //}

    //private void SelectBall(BallScaleConfig config, bool forced)
    //{
    //    if (config == null) return;

    //    if (!forced)
    //    {
    //        if (!CanSelect())
    //        {
    //            Debug.Log("[BallInventoryManager] Không thể đổi bi lúc này (đang trong cú đánh)");
    //            return;
    //        }

    //        if (IsOnCooldown(config))
    //        {
    //            Debug.Log($"[BallInventoryManager] Bi {config.mode} đang hồi, còn {GetCooldownRemaining(config)} lượt");
    //            return;
    //        }
    //    }

    //    CurrentConfig = config;
    //    cueBallController.ApplyScaleConfig(config);

    //    OnBallSelected?.Invoke(config);

    //    Debug.Log($"[BallInventoryManager] Đã chọn bi: {config.mode}");
    //}



    public static BallInventoryManager Instance { get; private set; }

    [Header("Refs")]
    public CueBallController cueBallController;
    public CueStickController cueStickController;

    [Header("Configs")]
    public BallScaleConfig normalConfig;

    [Header("Cooldown")]
    [Tooltip("Số lượt cooldown áp dụng cho 1 bi đặc biệt sau khi được dùng")]
    public int cooldownTurns = 2;

    [Header("Dynamic Panel — tự tạo nút theo loadout từ HomeScene")]
    [Tooltip("Kéo BallSelectionPanel vào đây")]
    public Transform buttonsContainer;
    [Tooltip("Kéo prefab BallSelectionButtonPrefab (asset trong Project) vào đây")]
    public GameObject ballButtonPrefab;

    [Header("Kích hoạt kỹ năng bằng chạm màn hình")]
    [Tooltip("Bật: chạm màn hình khi bi trắng đang lăn sẽ kích hoạt kỹ năng của bi đã chọn")]
    public bool tapToActivate = true;

    [Tooltip("Bật: chạm trúng nút / thanh trượt / joystick (UI tương tác được) thì KHÔNG tính là chạm kích hoạt")]
    public bool ignoreTapOverInteractiveUI = true;

    public BallScaleConfig CurrentConfig { get; private set; }

    // Cooldown còn lại của từng config — chỉ chứa config đang bị khoá
    private Dictionary<BallScaleConfig, int> cooldowns = new Dictionary<BallScaleConfig, int>();
    private List<RaycastResult> uiRaycastResults = new List<RaycastResult>();

    // UI đăng ký để tự cập nhật highlight khi đổi bi đang chọn
    public event Action<BallScaleConfig> OnBallSelected;

    // UI đăng ký để tự cập nhật trạng thái khoá/mở do cooldown
    public event Action OnCooldownChanged;

    // Chỉ bắn khi NGƯỜI CHƠI chủ động chọn bi thành công (không bắn khi hệ thống tự reset)
    public event Action OnUserSelectedBall;

    // Bắn khi kỹ năng vừa được kích hoạt bằng chạm màn hình (dùng cho gợi ý TAP sau này)
    public event Action OnAbilityActivated;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (ballButtonPrefab == null || buttonsContainer == null)
        {
            Debug.LogError("[BallInventoryManager] THIẾU Ball Button Prefab hoặc Buttons Container trong Inspector!");
        }
        else
        {
            // Lấy loadout 3 bi từ HomeScene, nếu chưa có thì chỉ dùng bi Bình Thường
            List<BallScaleConfig> loadout =
                SceneLoader.Instance != null &&
                SceneLoader.Instance.SelectedBallLoadout != null &&
                SceneLoader.Instance.SelectedBallLoadout.Count > 0
                    ? SceneLoader.Instance.SelectedBallLoadout
                    : new List<BallScaleConfig> { normalConfig };

            foreach (BallScaleConfig config in loadout)
            {
                if (config == null) continue;

                GameObject btnObj = Instantiate(ballButtonPrefab, buttonsContainer);
                BallSelectionButtonUI btnUI = btnObj.GetComponent<BallSelectionButtonUI>();

                if (btnUI == null)
                {
                    Debug.LogError("[BallInventoryManager] Prefab nút KHÔNG có component BallSelectionButtonUI!");
                    Destroy(btnObj);
                    continue;
                }

                btnUI.config = config;
            }
        }

        // Bi khởi đầu trận luôn là Bình Thường
        SelectBall(normalConfig, forced: true);
    }

    public bool CanSelect()
    {
        if (cueStickController == null) return true;
        return !cueStickController.isMoving && !cueStickController.hitPeriod;
    }

    public bool IsOnCooldown(BallScaleConfig config)
    {
        return config != null && cooldowns.ContainsKey(config) && cooldowns[config] > 0;
    }

    public int GetCooldownRemaining(BallScaleConfig config)
    {
        if (config == null) return 0;
        return cooldowns.TryGetValue(config, out int value) ? value : 0;
    }

    public bool SelectBall(BallScaleConfig config)
    {
        return SelectBall(config, forced: false);
    }

    private bool SelectBall(BallScaleConfig config, bool forced)
    {
        if (config == null) return false;

        if (!forced)
        {
            if (!CanSelect())
            {
                Debug.Log("[BallInventoryManager] Không thể đổi bi lúc này (đang trong cú đánh)");
                return false;
            }

            if (IsOnCooldown(config))
            {
                Debug.Log($"[BallInventoryManager] Bi {config.displayName} đang hồi, còn {GetCooldownRemaining(config)} lượt");
                return false;
            }
        }

        CurrentConfig = config;
        cueBallController.ApplyScaleConfig(config);

        OnBallSelected?.Invoke(config);

        Debug.Log($"[BallInventoryManager] Đã chọn bi: {config.displayName}");

        if (!forced)
            OnUserSelectedBall?.Invoke();

        return true;
    }

    // Gọi từ CueStickController ngay sau khi 1 cú đánh xử lý xong hoàn toàn
    public void ResetToNormalForNextShot()
    {
        // 1. Giảm cooldown của các bi đang bị khoá đi 1 lượt
        List<BallScaleConfig> keys = new List<BallScaleConfig>(cooldowns.Keys);
        foreach (BallScaleConfig key in keys)
        {
            cooldowns[key] = Mathf.Max(0, cooldowns[key] - 1);

            if (cooldowns[key] <= 0)
                cooldowns.Remove(key);
        }

        // 2. Cú đánh vừa rồi dùng bi đặc biệt (dù có chạm kích hoạt hay không) -> khoá cooldown
        if (CurrentConfig != null && CurrentConfig != normalConfig)
        {
            cooldowns[CurrentConfig] = cooldownTurns;

            Debug.Log($"[BallInventoryManager] Bi {CurrentConfig.displayName} vào cooldown {cooldownTurns} lượt");
        }

        // 3. Reset lựa chọn về bi bình thường cho lượt kế tiếp
        SelectBall(normalConfig, forced: true);

        // 4. Báo cho UI cập nhật khung cooldown
        OnCooldownChanged?.Invoke();
    }

    // ================================
    // ✋ CHẠM MÀN HÌNH ĐỂ KÍCH HOẠT KỸ NĂNG
    // ================================
    void Update()
    {
        if (!tapToActivate) return;

        if (!TryGetTapPosition(out Vector2 tapPos)) return;

        HandleTap(tapPos);
    }

    private bool TryGetTapPosition(out Vector2 position)
    {
        position = Vector2.zero;

        // Cảm ứng: lấy ngón vừa chạm xuống
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (t.phase == TouchPhase.Began)
                {
                    position = t.position;
                    return true;
                }
            }
            return false;
        }

        // Chuột (Editor / PC)
        if (Input.GetMouseButtonDown(0))
        {
            position = Input.mousePosition;
            return true;
        }

        return false;
    }

    private void HandleTap(Vector2 tapPos)
    {
        if (cueStickController == null || cueBallController == null) return;

        // 1) Chỉ nhận chạm khi các bi đang lăn sau cú đánh
        CueStickController.ShotState state = cueStickController.shotState;
        if (state != CueStickController.ShotState.BallsMoving)
        {
            // Idle = đang ngắm/thao tác bình thường nên không log để tránh spam
            if (state != CueStickController.ShotState.Idle)
                Debug.Log($"[BallInventoryManager] Chạm bị bỏ qua: trạng thái cú đánh = {state}");
            return;
        }

        // 2) Bỏ qua chạm trúng UI tương tác được
        if (ignoreTapOverInteractiveUI && IsPointerOverInteractiveUI(tapPos, out string blocker))
        {
            Debug.Log($"[BallInventoryManager] Chạm bị bỏ qua: đang chạm vào UI '{blocker}'");
            return;
        }

        // 3) Kích hoạt kỹ năng
        string ballName = CurrentConfig != null ? CurrentConfig.displayName : "?";

        if (cueBallController.ActivateAbility(out string reason))
        {
            Debug.Log($"[BallInventoryManager] Chạm màn hình -> đã kích hoạt kỹ năng '{ballName}'");
            OnAbilityActivated?.Invoke();
        }
        else
        {
            Debug.Log($"[BallInventoryManager] Chạm màn hình nhưng chưa kích hoạt ('{ballName}'): {reason}");
        }
    }

    // Chỉ coi là "chạm UI" khi object trên cùng là thứ tương tác được (nút, slider, joystick...)
    // — nền UI trong suốt không chặn chạm
    private bool IsPointerOverInteractiveUI(Vector2 screenPos, out string blockerName)
    {
        blockerName = "";

        if (EventSystem.current == null) return false;

        PointerEventData data = new PointerEventData(EventSystem.current) { position = screenPos };

        uiRaycastResults.Clear();
        EventSystem.current.RaycastAll(data, uiRaycastResults);

        if (uiRaycastResults.Count == 0) return false;

        GameObject top = uiRaycastResults[0].gameObject;

        bool interactive =
            ExecuteEvents.GetEventHandler<IPointerClickHandler>(top) != null ||
            ExecuteEvents.GetEventHandler<IPointerDownHandler>(top) != null ||
            ExecuteEvents.GetEventHandler<IDragHandler>(top) != null;

        if (interactive) blockerName = top.name;

        return interactive;
    }
}
