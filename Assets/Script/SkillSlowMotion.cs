using UnityEngine;
using Unity.Cinemachine;

public class SkillSlowMotion : MonoBehaviour
{
    // CÁC THAM CHIẾU CẦN THIẾT
    public CueStickController cueStickController;
    public SkillButtonController slowMoButtonController; // Kéo thả nút Slow Motion
    public Rigidbody cueBall; // Bi trắng (Bắt buộc)

    // Lưu vị trí ban đầu của bi trắng
    private Vector3 cueBallOriginalPosition;

    // --- SLOW MOTION SKILL (CHỈ DÙNG 1 LẦN) ---
    [Header("Slow Motion Skill (One Time Use)")]
    public float slowMotionTimeScale = 0.1f;
    public int minBallsRequired = 2; // Số bi cần ăn để SlowMo sẵn sàng

    // Trạng thái nội bộ
    private int ballsSunk = 0;
    private float defaultFixedDeltaTime;

    // Trạng thái công khai
    public bool isSkillReadyToFire { get; private set; } = false; // Sẵn sàng để kích hoạt
    public bool skillActive { get; private set; } = false; // Đang chạy Slow Motion
    private bool skillUsedPermanently = false; // Đã dùng vĩnh viễn (sau khi tiêu thụ)
    public bool IsSkillUsedPermanently => skillUsedPermanently;

    // ---------------------------------------------------------------------------------

    void Start()
    {
        if (cueBall != null)
        {
            // Lưu lại vị trí ban đầu của bi trắng
            cueBallOriginalPosition = cueBall.transform.position;
        }

        // Khôi phục Fixed Delta Time (quan trọng cho vật lý)
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = 1f;

        // Cập nhật nút ban đầu (mờ)
        if (slowMoButtonController != null)
        {
            slowMoButtonController.UpdateButtonState(false);
        }
    }

    // --- CÁC HÀM CỦA SLOW MOTION ---

    /// <summary>
    /// Được gọi từ PocketTrigger khi bi lọt lỗ.
    /// </summary>
    public void BallPocketed(string ballName)
    {
        Debug.Log($"<color=white>SKILL MANAGER: Bi '{ballName}' đã lọt lỗ!</color>");

        // Nếu skill đã dùng vĩnh viễn, không đếm nữa
        if (skillUsedPermanently) return;

        ballsSunk++;
        Debug.Log($"<color=white>---> Bi lọt lỗ! Hiện tại: {ballsSunk}/{minBallsRequired} bi.</color>");

        if (ballsSunk >= minBallsRequired)
        {
            isSkillReadyToFire = true;
            Debug.Log($"<color=yellow>!!! SKILL SẴN SÀNG: isSkillReadyToFire = {isSkillReadyToFire} !!!</color>");

            // Cập nhật trạng thái highlight cho nút SlowMo
            if (slowMoButtonController != null)
            {
                slowMoButtonController.UpdateButtonState(true);
            }
        }
    }

    /// <summary>
    /// Được gọi từ nút nhấn. Kích hoạt Slow Motion ngay lập tức.
    /// </summary>
    public void TriggerSkill()
    {
        if (skillUsedPermanently || skillActive || !isSkillReadyToFire)
        {
            Debug.Log("<color=red>SKILL MANAGER: Không thể kích hoạt. Đã dùng hoặc chưa sẵn sàng.</color>");
            return;
        }

        Debug.Log("<color=red>--- SLOW MOTION ACTIVE! Tốc độ hiện tại: " + slowMotionTimeScale + "x ---</color>");

        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale; // Cần thiết cho vật lý
        skillActive = true;

        // Sau khi kích hoạt, nó không còn "sẵn sàng" để nhấn nút lần nữa trong cú đánh này
        // (nút đã bị vô hiệu hóa trong SkillButtonController ngay sau khi nhấn)
    }

    /// <summary>
    /// Được gọi từ CueStickController khi bi dừng. Kết thúc Slow Motion.
    /// </summary>
    public void EndSkill()
    {
        if (!skillActive) return;

        skillActive = false;
        Debug.Log("<color=green>--- SLOW MOTION KẾT THÚC: Trở lại tốc độ bình thường ---</color>");

        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime; // Khôi phục vật lý
    }

    /// <summary>
    /// Được gọi từ CueStickController để đánh dấu skill đã dùng VĨNH VIỄN sau cú đánh.
    /// </summary>
    public void ConsumeSkill()
    {
        if (!skillUsedPermanently)
        {
            skillUsedPermanently = true;
            isSkillReadyToFire = false;
            ballsSunk = 0; // Reset bộ đếm nếu cần

            Debug.Log("<color=red>SKILL MANAGER: Slow Motion đã bị TIÊU THỤ VĨNH VIỄN!</color>");

            // Cập nhật trạng thái nút SlowMo để vô hiệu hóa
            if (slowMoButtonController != null)
            {
                slowMoButtonController.UpdateButtonState(false);
            }
        }
    }

    // --- RESET POSITION SKILL (DÙNG NHIỀU LẦN) ---

    /// <summary>
    /// Được gọi từ nút Reset để đưa bi trắng về vị trí ban đầu.
    /// </summary>
    public void ResetCueBallPosition()
    {
        if (cueBall != null)
        {
            // Dừng bi ngay lập tức
            cueBall.linearVelocity = Vector3.zero;
            cueBall.angularVelocity = Vector3.zero;

            // Đặt lại vị trí bi
            cueBall.transform.position = cueBallOriginalPosition;

            Debug.Log("<color=cyan>SKILL MANAGER: Đã Reset vị trí bi trắng về: " + cueBallOriginalPosition + "</color>");

            // Reset gậy về vị trí bi
            if (cueStickController != null)
            {
                //cueStickController.AdjustStickPivotToCueBalls();
            }
        }
        else
        {
            Debug.LogError("SKILL MANAGER: Không tìm thấy Rigidbody của bi trắng để reset.");
        }
    }
    //[Header("Cài Đặt Slow Motion")]
    //[Tooltip("Tỉ lệ giảm tốc (ví dụ: 0.2f = 20% tốc độ gốc)")]
    //public float slowMoScale = 0.2f;

    //[Header("Logic Kích Hoạt")]
    //[Tooltip("Số lượng bi tối thiểu cần ăn để skill sẵn sàng")]
    //public int minBallsRequired = 2;

    //// Biến trạng thái công khai: Được CueStickController sử dụng
    //public bool isSkillReadyToFire { get; private set; } = false;
    //public bool skillActive { get; private set; } = false; // Trạng thái Slow Motion đang chạy

    //// Biến nội bộ
    //private int ballsSunk = 0;
    //private float defaultFixedDeltaTime;

    //void Start()
    //{
    //    defaultFixedDeltaTime = Time.fixedDeltaTime;
    //    Time.timeScale = 1f;
    //    Debug.Log($"<color=cyan>Skill Manager Khởi Động. Yêu cầu: {minBallsRequired} bi.</color>");
    //}

    //// 🚨 HÀM UPDATE ĐÃ ĐƯỢC DỌN DẸP
    //void Update()
    //{
    //    // Loại bỏ logic đếm thời gian
    //}

    ///// <summary>
    ///// Được gọi từ script Lỗ Bi (Pocket.cs) khi bi lọt lỗ.
    ///// </summary>
    //public void BallSunk()
    //{
    //    ballsSunk++;
    //    Debug.Log($"<color=white>---> Bi lọt lỗ! Hiện tại: {ballsSunk}/{minBallsRequired} bi.</color>");

    //    if (ballsSunk >= minBallsRequired)
    //    {
    //        isSkillReadyToFire = true;
    //        Debug.Log($"<color=yellow>!!! SKILL SẴN SÀNG: isSkillReadyToFire = {isSkillReadyToFire} !!!</color>");
    //    }
    //}

    ///// <summary>
    ///// Được gọi từ CueStickController ngay sau khi bi trắng được đánh.
    ///// </summary>
    //public void TriggerSkill()
    //{
    //    if (skillActive) return;

    //    // 🚨 Debug Kích hoạt và Tốc độ
    //    Debug.Log($"<color=red>--- SLOW MOTION ACTIVE! Tốc độ hiện tại: {slowMoScale:F2}x ---</color>");

    //    // 1. Slow Motion
    //    Time.timeScale = slowMoScale;
    //    Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

    //    // 2. Thiết lập trạng thái
    //    skillActive = true;
    //}

    ///// <summary>
    ///// Được gọi từ CueStickController khi tất cả bi dừng.
    ///// </summary>
    //public void EndSkill()
    //{
    //    if (!skillActive) return; // Chỉ kết thúc khi đang chạy

    //    skillActive = false;

    //    Debug.Log($"<color=green>--- SLOW MOTION KẾT THÚC: Bi đã dừng, trở lại tốc độ bình thường ---</color>");

    //    // 1. Khôi phục thời gian
    //    Time.timeScale = 1f;
    //    Time.fixedDeltaTime = defaultFixedDeltaTime;
    //}

    ///// <summary>
    ///// Được gọi từ CueStickController để reset bộ đếm bi sau khi sử dụng skill.
    ///// </summary>
    //public void ConsumeSkill()
    //{
    //    isSkillReadyToFire = false;
    //    ballsSunk = 0;
    //    Debug.Log("<color=lime>Bộ đếm bi và trạng thái sẵn sàng đã được RESET.</color>");
    //}
}
