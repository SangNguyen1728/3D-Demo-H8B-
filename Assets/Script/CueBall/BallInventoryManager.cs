using System;
using System.Collections.Generic;
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

    public static BallInventoryManager Instance { get; private set; }

    [Header("Refs")]
    public CueBallController cueBallController;
    public CueStickController cueStickController;

    [Header("Configs")]
    public BallScaleConfig normalConfig;

    [Header("Cooldown")]
    [Tooltip("Số lượt cooldown áp dụng cho 1 bi đặc biệt sau khi được dùng")]
    public int cooldownTurns = 2;

    public BallScaleConfig CurrentConfig { get; private set; }

    // Cooldown còn lại của từng config — chỉ chứa config đang bị khoá
    private Dictionary<BallScaleConfig, int> cooldowns = new Dictionary<BallScaleConfig, int>();

    // UI đăng ký lắng nghe để tự cập nhật highlight khi đổi bi đang chọn
    public event Action<BallScaleConfig> OnBallSelected;

    // UI đăng ký lắng nghe để tự cập nhật trạng thái khoá/mở do cooldown
    public event Action OnCooldownChanged;

    [Header("Dynamic Panel — tự tạo nút theo loadout từ HomeScene")]
    public Transform buttonsContainer;      // 🆕 kéo BallSelectionPanel vào đây
    public GameObject ballButtonPrefab;

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
        // BallScaleConfig startingConfig = SceneLoader.Instance != null && SceneLoader.Instance.SelectedBallConfig != null
        //? SceneLoader.Instance.SelectedBallConfig
        //: normalConfig;

        // SelectBall(startingConfig, forced: true);

        // SelectBall(normalConfig, forced: true);

        List<BallScaleConfig> loadout = SceneLoader.Instance != null && SceneLoader.Instance.SelectedBallLoadout != null && SceneLoader.Instance.SelectedBallLoadout.Count > 0
       ? SceneLoader.Instance.SelectedBallLoadout
       : new List<BallScaleConfig> { normalConfig };

        foreach (var config in loadout)
        {
            GameObject btnObj = Instantiate(ballButtonPrefab, buttonsContainer);
            BallSelectionButtonUI btnUI = btnObj.GetComponent<BallSelectionButtonUI>();
            btnUI.config = config;
        }

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

    public void SelectBall(BallScaleConfig config)
    {
        SelectBall(config, forced: false);
    }

    private void SelectBall(BallScaleConfig config, bool forced)
    {
        if (config == null) return;

        if (!forced)
        {
            if (!CanSelect())
            {
                Debug.Log("[BallInventoryManager] Không thể đổi bi lúc này (đang trong cú đánh)");
                return;
            }

            if (IsOnCooldown(config))
            {
                Debug.Log($"[BallInventoryManager] Bi {config.mode} đang hồi, còn {GetCooldownRemaining(config)} lượt");
                return;
            }
        }

        CurrentConfig = config;
        cueBallController.ApplyScaleConfig(config);

        OnBallSelected?.Invoke(config);

        Debug.Log($"[BallInventoryManager] Đã chọn bi: {config.mode}");
    }

    // Gọi từ CueStickController ngay sau khi 1 cú đánh xử lý xong hoàn toàn
    public void ResetToNormalForNextShot()
    {
        // 1. Giảm cooldown của các bi đang bị khoá đi 1 lượt
        List<BallScaleConfig> keys = new List<BallScaleConfig>(cooldowns.Keys);
        foreach (var key in keys)
        {
            cooldowns[key] = Mathf.Max(0, cooldowns[key] - 1);

            if (cooldowns[key] <= 0)
                cooldowns.Remove(key);
        }

        // 2. Nếu cú đánh vừa rồi dùng bi đặc biệt (không phải Normal) -> khoá nó lại
        if (CurrentConfig != null && CurrentConfig != normalConfig)
        {
            cooldowns[CurrentConfig] = cooldownTurns;

            Debug.Log($"[BallInventoryManager] Bi {CurrentConfig.mode} vào cooldown {cooldownTurns} lượt");
        }

        // 3. Reset lựa chọn về bi bình thường cho lượt kế tiếp
        SelectBall(normalConfig, forced: true);

        // 4. Báo cho UI biết cooldown vừa thay đổi để tự cập nhật khung khoá
        OnCooldownChanged?.Invoke();
    }
}
