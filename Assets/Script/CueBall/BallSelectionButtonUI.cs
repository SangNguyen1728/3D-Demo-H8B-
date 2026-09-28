using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallSelectionButtonUI : MonoBehaviour
{
    //[Tooltip("Config bi mà nút này đại diện (Normal / Grow / Shrink)")]
    //public BallScaleConfig config;

    //[Tooltip("Object viền/khung sáng hiển thị khi bi này đang được chọn")]
    //public GameObject highlightFrame;

    //private Button button;

    //void Start()
    //{
    //    button = GetComponent<Button>();

    //    button.onClick.AddListener(() =>
    //    {
    //        BallInventoryManager.Instance.SelectBall(config);
    //    });

    //    BallInventoryManager.Instance.OnBallSelected += HandleSelectionChanged;

    //    HandleSelectionChanged(BallInventoryManager.Instance.CurrentConfig);
    //}

    //void OnDestroy()
    //{
    //    if (BallInventoryManager.Instance != null)
    //        BallInventoryManager.Instance.OnBallSelected -= HandleSelectionChanged;
    //}

    //private void HandleSelectionChanged(BallScaleConfig selected)
    //{
    //    if (highlightFrame != null)
    //        highlightFrame.SetActive(selected == config);
    //}

    [Tooltip("Config bi mà nút này đại diện")]
    public BallScaleConfig config;

    [Tooltip("Khung sáng hiển thị khi bi này đang được CHỌN")]
    public GameObject highlightFrame;

    [Tooltip("Khung màu khác hiển thị khi bi này đang bị COOLDOWN (khoá)")]
    public GameObject cooldownFrame;

    [Tooltip("Text hiển thị số lượt cooldown còn lại")]
    public TMP_Text cooldownText;

    [Tooltip("Text tên nút (Bình Thường/Phóng To...) — tự ẩn khi đang cooldown để không đè lên số đếm")]
    public TMP_Text buttonLabel;

    private Button button;

    void Start()
    {
        //button = GetComponent<Button>();

        //if (config == null)
        //{
        //    Debug.LogError($"[BallSelectionButtonUI] Object '{gameObject.name}' CHƯA gán Config! Kiểm tra lại Inspector.");
        //}

        //button.onClick.AddListener(() =>
        //{
        //    BallInventoryManager.Instance.SelectBall(config);
        //});

        //BallInventoryManager.Instance.OnBallSelected += HandleSelectionChanged;
        //BallInventoryManager.Instance.OnCooldownChanged += HandleCooldownChanged;

        //HandleSelectionChanged(BallInventoryManager.Instance.CurrentConfig);
        //HandleCooldownChanged();

        button = GetComponent<Button>();

        if (buttonLabel != null && config != null)
        {
            buttonLabel.text = config.displayName;
        }

        // 🆕 Kiểm tra đầy đủ tất cả field, báo lỗi rõ ràng ngay khi Play nếu thiếu field nào
        if (config == null)
            Debug.LogError($"[BallSelectionButtonUI] '{gameObject.name}': THIẾU field Config!");

        if (highlightFrame == null)
            Debug.LogError($"[BallSelectionButtonUI] '{gameObject.name}': THIẾU field Highlight Frame!");

        if (cooldownFrame == null)
            Debug.LogError($"[BallSelectionButtonUI] '{gameObject.name}': THIẾU field Cooldown Frame!");

        if (cooldownText == null)
            Debug.LogError($"[BallSelectionButtonUI] '{gameObject.name}': THIẾU field Cooldown Text!");

        if (buttonLabel == null)
            Debug.LogError($"[BallSelectionButtonUI] '{gameObject.name}': THIẾU field Button Label!");

        button.onClick.AddListener(() =>
        {
            BallInventoryManager.Instance.SelectBall(config);
        });

        BallInventoryManager.Instance.OnBallSelected += HandleSelectionChanged;
        BallInventoryManager.Instance.OnCooldownChanged += HandleCooldownChanged;

        HandleSelectionChanged(BallInventoryManager.Instance.CurrentConfig);
        HandleCooldownChanged();
    }

    void OnDestroy()
    {
        if (BallInventoryManager.Instance != null)
        {
            BallInventoryManager.Instance.OnBallSelected -= HandleSelectionChanged;
            BallInventoryManager.Instance.OnCooldownChanged -= HandleCooldownChanged;
        }
    }

    private void HandleSelectionChanged(BallScaleConfig selected)
    {
        if (highlightFrame != null)
            highlightFrame.SetActive(selected == config);
    }

    private void HandleCooldownChanged()
    {
        bool onCooldown = BallInventoryManager.Instance.IsOnCooldown(config);

        if (cooldownFrame != null)
            cooldownFrame.SetActive(onCooldown);

        if (button != null)
            button.interactable = !onCooldown;

        // 🆕 Tắt hẳn (không chỉ để rỗng) khi hết cooldown
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(onCooldown);

            if (onCooldown)
            {
                int remaining = BallInventoryManager.Instance.GetCooldownRemaining(config);
                cooldownText.text = remaining.ToString();
            }
        }

        // 🆕 Ẩn tên nút trong lúc cooldown để không đè lên số đếm ngược
        if (buttonLabel != null)
        {
            buttonLabel.gameObject.SetActive(!onCooldown);
        }
    }
}
