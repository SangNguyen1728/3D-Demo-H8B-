using UnityEngine;

public enum BallScaleMode
{
    None,        // Bi bình thường — không đổi scale
    AlwaysGrow,  // Luôn phóng to khi lăn
    AlwaysShrink // Luôn thu nhỏ khi lăn
}

[CreateAssetMenu(fileName = "BallScaleConfig", menuName = "Billiards/Ball Scale Config")]
public class BallScaleConfig : EquipmentConfig
{
    [Header("Chế độ")]
    public BallScaleMode mode = BallScaleMode.None;

    [Header("Thông số")]
    [Tooltip("Mức thay đổi scale so với gốc, vd 0.5 = ±50%")]
    public float scaleChangeAmount = 0.5f;

    [Tooltip("Tốc độ chuyển scale mượt (càng cao càng nhanh)")]
    public float scaleLerpSpeed = 6f;

    [Tooltip("Ngưỡng vận tốc để coi là 'đang lăn'")]
    public float rollingSpeedThreshold = 0.05f;

    // 🆕 XUYÊN BI
    [Header("Xuyên Bi")]
    [Tooltip("Bật để bi cái xuyên qua các bi khác trong 1 khoảng thời gian sau khi đánh")]
    public bool enablePhaseThrough = false;

    [Tooltip("Thời gian xuyên bi tính bằng giây")]
    public float phaseThroughDuration = 3f;

    // 🆕 HỒI VỊ
    [Header("Hồi Vị Sau Cú Đánh")]
    [Tooltip("Bật để bi cái tự quay về vị trí đánh trước đó sau khi cú đánh kết thúc")]
    public bool enableReturnToShotPosition = false;

    [Tooltip("Khoảng cách an toàn tối thiểu — nếu có bi mục tiêu nằm gần hơn khoảng này thì HỦY hồi vị")]
    public float minSafeDistanceFromOtherBalls = 0.15f;
}
