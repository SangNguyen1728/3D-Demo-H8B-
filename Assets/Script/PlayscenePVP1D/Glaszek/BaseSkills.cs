using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum SkillTargetingType
{
    Toggle,   // bật tắt: áp dụng ngay, không cần chế độ chọn
    Area,     // chọn vùng
    Target    // chọn mục tiêu (bi)
}

/// <summary>Dữ liệu người chơi đã chọn ở bước chọn vùng / chọn mục tiêu.</summary>
public struct SkillTargetData
{
    public Vector3 position;   // Area: tâm vùng. Target: vị trí bi được chọn
    public BallNo ball;        // Target: bi được chọn
}

public abstract class BaseSkills : ScriptableObject
{
    public string skillName;
    public int skillID;
    public Sprite icon;
    public int level = 1;
    public string skill_ID;
    public Sprite skillIcon;
    public float cooldown;

    // phục vụ UI chọn skill ở HomeScene, không ảnh hưởng logic gameplay
    [TextArea] public string description;
    public int requiredLevel = 1;

    [Header("Stamina")]
    public int staminaCost = 0;

    [TextArea(2, 4)]
    public string skillDescription;

    public enum SkillType
    {
        Active,
        Passive
    }

    // ===== PHÂN LOẠI PHÉP =====
    [Header("Phân loại phép")]
    [Tooltip("Toggle = áp dụng ngay khi dùng. Target = người chơi chọn bi mục tiêu rồi Duyệt.")]
    public SkillTargetingType targetingType = SkillTargetingType.Toggle;

    [Tooltip("Chỉ cho Toggle: hiện logo skill dưới joystick cho tới khi bắt đầu cú đánh kế tiếp (dùng cho phép áp dụng vào gậy kế tiếp).")]
    public bool showLogoUntilNextShot = false;

    [Tooltip("Chỉ cho Target: dòng gợi ý hiện ở màn chọn (vd: 'Chọn bi của đối thủ'). Để trống sẽ dùng chữ mặc định.")]
    public string targetingHint = "";

    [Tooltip("Chỉ cho Target: bật = XEM TRƯỚC TRỰC TIẾP, kết quả của skill chạy theo bi đang chọn (Duyệt = chốt, Hủy = thu hồi). Tắt = skill chỉ chạy sau khi Duyệt. Chỉ có tác dụng khi skill đã viết BeginPreview / UpdatePreview.")]
    public bool livePreview = false;

    /// <summary>true nếu skill thuộc loại Target. Mọi giá trị khác (kể cả số 1 cũ của Area) coi là Toggle.</summary>
    public bool IsTargetSkill => targetingType == SkillTargetingType.Target;

    public abstract void Activate(GameObject player, GlaszekManager manager);
    public abstract void OnTurnEnd(GlaszekManager manager);

    /// <summary>
    /// Skill có dùng được ở thời điểm này không. Thanh giành sẽ làm tối skill trả về false.
    /// Mặc định: Toggle luôn dùng được, Target cần có ít nhất 1 bi hợp lệ.
    /// </summary>
    public virtual bool CanUse(GameObject player, GlaszekManager manager)
    {
        if (!IsTargetSkill) return true;

        BallNo[] all = Object.FindObjectsByType<BallNo>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            BallNo b = all[i];
            if (b == null || b.isCueBall || !b.gameObject.activeInHierarchy) continue;
            if (IsValidTarget(b, manager)) return true;
        }
        return false;
    }

    /// <summary>
    /// Bi này có được chọn làm mục tiêu không (chỉ cho Target).
    /// LƯU Ý khi viết skill: xét theo manager.playerNumber (người dùng skill), KHÔNG theo pocket.currentPlayer.
    /// </summary>
    public virtual bool IsValidTarget(BallNo ball, GlaszekManager manager) { return true; }

    /// <summary>Dùng phép với bi đã chọn. Mặc định gọi Activate như cũ.</summary>
    public virtual void ActivateWithTarget(GameObject player, GlaszekManager manager, SkillTargetData data)
    {
        if (IsTargetSkill)
            Debug.LogWarning($"[{name}] Loại Target nhưng chưa ghi đè ActivateWithTarget -> gọi Activate thường");
        Activate(player, manager);
    }

    // ===== XEM TRƯỚC TRỰC TIẾP (chỉ được gọi khi livePreview = true) =====

    /// <summary>Vào chế độ chọn: tạo bản xem trước tại bi ban đầu.</summary>
    public virtual void BeginPreview(GameObject player, GlaszekManager manager, SkillTargetData data) { }

    /// <summary>Bi được chọn vừa đổi.</summary>
    public virtual void UpdatePreview(GlaszekManager manager, SkillTargetData data) { }

    /// <summary>
    /// Duyệt = chốt. Mặc định: dọn bản xem trước rồi chạy skill như bình thường.
    /// Skill muốn GIỮ chính đối tượng đang xem trước làm kết quả thì ghi đè hàm này.
    /// </summary>
    public virtual void CommitPreview(GameObject player, GlaszekManager manager, SkillTargetData data)
    {
        CancelPreview(manager);
        ActivateWithTarget(player, manager, data);
    }

    /// <summary>Hủy / hết giờ / đổi lượt: thu hồi bản xem trước.</summary>
    public virtual void CancelPreview(GlaszekManager manager) { }
}
