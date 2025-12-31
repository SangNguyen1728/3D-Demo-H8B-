using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class SkillButtonController : MonoBehaviour
{
    public enum SkillType { SlowMotion, ResetPosition }

    [Header("Skill Settings")]
    public SkillSlowMotion skillManager;
    public SkillType skillType; // Chọn loại skill cho nút này

    // Các tham chiếu cho hiệu ứng Highlight (Chỉ dùng cho Slow Motion)
    [Header("UI Highlight Settings (For SlowMotion)")]
    public Image buttonBackground;
    public Color readyColor = Color.yellow;
    public Color usedColor = Color.gray;

    private Button skillButton;

    void Start()
    {
        skillButton = GetComponent<Button>();

        if (skillManager == null)
        {
            Debug.LogError("SKILL_BUTTON: Thiếu Skill Manager! Vô hiệu hóa nút " + gameObject.name);
            if (skillButton != null) skillButton.interactable = false;
            return;
        }

        if (skillButton != null)
        {
            skillButton.onClick.AddListener(OnSkillButtonClicked);
        }

        // Thiết lập tham chiếu ngược trong SkillManager (chỉ cần cho nút SlowMotion)
        if (skillType == SkillType.SlowMotion)
        {
            skillManager.slowMoButtonController = this;
            UpdateButtonState(false); // Đảm bảo nút mờ khi game bắt đầu
        }
    }

    // Hàm được gọi khi người chơi nhấn nút
    public void OnSkillButtonClicked()
    {
        if (skillManager == null) return;

        switch (skillType)
        {
            case SkillType.SlowMotion:
                HandleSlowMotionClick();
                break;
            case SkillType.ResetPosition:
                HandleResetPositionClick();
                break;
        }
    }

    private void HandleSlowMotionClick()
    {
        // 1. Kiểm tra trạng thái đã dùng vĩnh viễn (chỉ dùng 1 lần)
        if (skillManager.IsSkillUsedPermanently)
        {
            Debug.Log("<color=red>SKILL BUTTON (SlowMo): Đã sử dụng vĩnh viễn, không thể dùng lại.</color>");
            return;
        }

        // 2. Kích hoạt
        if (skillManager.isSkillReadyToFire)
        {
            skillManager.TriggerSkill();
            Debug.Log("<color=green>SKILL BUTTON (SlowMo): Kích hoạt Slow Motion!</color>");

            // Vô hiệu hóa và mất highlight ngay lập tức
            UpdateButtonState(false);
        }
        else
        {
            Debug.Log("<color=orange>SKILL BUTTON (SlowMo): Chưa đủ điều kiện (Chưa sẵn sàng).</color>");
        }
    }

    private void HandleResetPositionClick()
    {
        // Kiểm tra xem các bi có đang di chuyển không
        if (skillManager.cueStickController.AreAllBallsStopped())
        {
            skillManager.ResetCueBallPosition();
        }
        else
        {
            Debug.Log("<color=orange>SKILL BUTTON (Reset): Không thể Reset khi bi đang di chuyển.</color>");
        }
    }

    /// <summary>
    /// Cập nhật trạng thái nút và highlight (Chỉ dùng cho Slow Motion).
    /// </summary>
    public void UpdateButtonState(bool isReady)
    {
        if (skillType != SkillType.SlowMotion) return;

        if (skillButton == null || buttonBackground == null || skillManager == null) return;

        // Nếu đã được dùng vĩnh viễn, vô hiệu hóa hoàn toàn
        if (skillManager.IsSkillUsedPermanently)
        {
            skillButton.interactable = false;
            buttonBackground.color = usedColor;
            return;
        }

        // Cập nhật trạng thái và màu highlight
        skillButton.interactable = isReady;
        buttonBackground.color = isReady ? readyColor : usedColor;
    }
    //public SkillSlowMotion skillManager;

    //// Tham chiếu đến nút UI (tùy chọn, để quản lý trạng thái hiển thị/tương tác)
    //private Button skillButton;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    // 1. Lấy component Button trên GameObject này
    //    skillButton = GetComponent<Button>();

    //    // 2. Kiểm tra lỗi nếu chưa gán Skill Manager
    //    if (skillManager == null)
    //    {
    //        Debug.LogError("SKILL_BUTTON: Thiếu Skill Manager. Hãy gán SkillSlowMotion vào Script này.");
    //        // Vô hiệu hóa nút để tránh lỗi runtime
    //        if (skillButton != null) skillButton.interactable = false;
    //    }

    //    // 3. Đăng ký sự kiện Click của nút với hàm kích hoạt
    //    if (skillButton != null)
    //    {
    //        // Bấm nút sẽ gọi hàm OnSkillButtonClicked
    //        skillButton.onClick.AddListener(OnSkillButtonClicked);
    //    }
    //}

    //// Hàm được gọi khi người chơi nhấn nút
    //public void OnSkillButtonClicked()
    //{
    //    // Kiểm tra xem có Skill Manager và liệu skill có sẵn sàng không
    //    if (skillManager != null && skillManager.isSkillReadyToFire)
    //    {
    //        // ************ LOGIC QUAN TRỌNG ************
    //        // Bắt đầu chuỗi kích hoạt skill ngay TẠI ĐÂY (trước khi đánh bi)

    //        // 1. Kích hoạt Slow Motion (hoặc chuẩn bị trạng thái kích hoạt)
    //        skillManager.TriggerSkill();
    //        Debug.Log("SKILL_BUTTON: Nút được nhấn! Gọi TriggerSkill()!");

    //        // 2. Cần thông báo cho CueStickController rằng skill ĐÃ được kích hoạt trong cú đánh này.
    //        // (Bạn sẽ thêm biến/hàm public trong CueStickController ở bước 2)

    //        // Vô hiệu hóa nút tạm thời sau khi nhấn nếu cần
    //        if (skillButton != null) skillButton.interactable = false;

    //        // Sau khi skill được sử dụng, nó sẽ được reset khi các bi dừng (logic đã có trong CueStickController)
    //    }
    //    else
    //    {
    //        // Tùy chọn: Log lỗi hoặc hiển thị UI thông báo skill chưa sẵn sàng
    //        Debug.Log("<color=red>SKILL_BUTTON: Skill chưa sẵn sàng hoặc đã được kích hoạt.</color>");
    //    }
    //}

    //// Tùy chọn: Hàm để cập nhật trạng thái nút (gọi từ SkillSlowMotion khi trạng thái thay đổi)
    //public void UpdateButtonState(bool isReady)
    //{
    //    if (skillButton != null)
    //    {
    //        // Chỉ cho phép nhấn nếu bi đã dừng và skill sẵn sàng
    //        skillButton.interactable = isReady;
    //    }
    //}
}
