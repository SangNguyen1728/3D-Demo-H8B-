using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    //public PlayerSkillLoadout loadout;

    //[Header("Skill Selection Gate")]
    //[Tooltip("true = chỉ dùng được skill đã được SkillSelectionManager cấp quyền (giành được). Tắt để test kiểu cũ.")]
    //public bool enforceSelectionGate = true;

    //private GlaszekManager manager;
    //private bool hasUsedSkillThisShot = false;
    //private int grantedSlot = 0; // 0 = chưa được cấp quyền slot nào

    //private void Awake()
    //{
    //    manager = GetComponent<GlaszekManager>();
    //    if (manager == null)
    //        manager = FindFirstObjectByType<GlaszekManager>();
    //    if (manager == null)
    //        Debug.LogError("Không tìm thấy GlaszekManager!");
    //    if (loadout == null)
    //    {
    //        Debug.LogError("Loadout chưa được gán!");
    //        return;
    //    }
    //    Debug.Log($"Loadout OK | slot1={loadout.slot1?.skillName ?? "NULL"} | slot2={loadout.slot2?.skillName ?? "NULL"} | slot3={loadout.slot3?.skillName ?? "NULL"}");
    //}

    //// ===== API cho SkillSelectionManager =====
    //public BaseSkills GetSkill(int slot)
    //{
    //    if (loadout == null) return null;
    //    switch (slot)
    //    {
    //        case 1: return loadout.slot1;
    //        case 2: return loadout.slot2;
    //        case 3: return loadout.slot3;
    //        default: return null;
    //    }
    //}

    //public void GrantSlot(int slot)
    //{
    //    grantedSlot = slot;
    //    Debug.Log($"[PSC P{PlayerNo()}] GRANT slot{slot}");
    //}

    //public void RevokeGrant()
    //{
    //    if (grantedSlot != 0) Debug.Log($"[PSC P{PlayerNo()}] REVOKE slot{grantedSlot}");
    //    grantedSlot = 0;
    //}

    //public string DebugState() => $"grantedSlot={grantedSlot} | hasUsedSkillThisShot={hasUsedSkillThisShot}";

    //public void ResetShotSkillUsage()
    //{
    //    hasUsedSkillThisShot = false;
    //    grantedSlot = 0;
    //}

    ///// <summary>Dùng skill ở slot (1..3). Trả về true nếu skill thực sự được kích hoạt.</summary>
    //public bool TryUseSlot(int slot)
    //{
    //    BaseSkills skill = GetSkill(slot);
    //    if (skill == null)
    //    {
    //        Debug.LogError($"[PSC P{PlayerNo()}] slot{slot} NULL — kiểm tra Loadout!");
    //        return false;
    //    }

    //    if (enforceSelectionGate && grantedSlot != slot)
    //    {
    //        Debug.LogWarning($"[PSC P{PlayerNo()}] BỊ CHẶN: slot{slot} chưa được giành (granted={grantedSlot})");
    //        return false;
    //    }

    //    if (hasUsedSkillThisShot)
    //    {
    //        Debug.LogWarning("[PSC] Đã dùng skill trong lượt đánh này, không thể dùng thêm!");
    //        return false;
    //    }

    //    StaminaManager stamina = StaminaManagerRegistry.Get(manager.playerNumber);
    //    if (stamina != null && !stamina.TryConsume(skill.staminaCost))
    //        return false;

    //    hasUsedSkillThisShot = true;
    //    grantedSlot = 0;
    //    Debug.Log($"[PSC P{PlayerNo()}] DÙNG slot{slot}: {skill.skillName}");
    //    skill.Activate(gameObject, manager);
    //    return true;
    //}

    //// ===== Giữ API cũ để GlaszekManager không phải sửa =====
    //public void UseSkill1() { TryUseSlot(1); }
    //public void UseSkill2() { TryUseSlot(2); }
    //public void UseSkill3() { TryUseSlot(3); }

    //public void NotifyTurnEnd()
    //{
    //    if (loadout == null) return;
    //    loadout.slot1?.OnTurnEnd(manager);
    //    loadout.slot2?.OnTurnEnd(manager);
    //    loadout.slot3?.OnTurnEnd(manager);
    //}

    //private int PlayerNo() => manager != null ? manager.playerNumber : -1;

    public PlayerSkillLoadout loadout;

    [Header("Skill Selection Gate")]
    [Tooltip("true = chỉ dùng được skill đã được SkillSelectionManager cấp quyền (giành được). Tắt để test kiểu cũ.")]
    public bool enforceSelectionGate = true;

    private GlaszekManager manager;
    private bool hasUsedSkillThisShot = false;
    private int grantedSlot = 0; // 0 = chưa được cấp quyền slot nào

    private void Awake()
    {
        manager = GetComponent<GlaszekManager>();
        if (manager == null)
            manager = FindFirstObjectByType<GlaszekManager>();
        if (manager == null)
            Debug.LogError("Không tìm thấy GlaszekManager!");
        if (loadout == null)
        {
            Debug.LogError("Loadout chưa được gán!");
            return;
        }
        Debug.Log($"Loadout OK | slot1={loadout.slot1?.skillName ?? "NULL"} | slot2={loadout.slot2?.skillName ?? "NULL"} | slot3={loadout.slot3?.skillName ?? "NULL"}");
    }

    // ===== API cho SkillSelectionManager =====
    public BaseSkills GetSkill(int slot)
    {
        if (loadout == null) return null;
        switch (slot)
        {
            case 1: return loadout.slot1;
            case 2: return loadout.slot2;
            case 3: return loadout.slot3;
            default: return null;
        }
    }

    public void GrantSlot(int slot)
    {
        grantedSlot = slot;
        Debug.Log($"[PSC P{PlayerNo()}] GRANT slot{slot}");
    }

    public void RevokeGrant()
    {
        if (grantedSlot != 0) Debug.Log($"[PSC P{PlayerNo()}] REVOKE slot{grantedSlot}");
        grantedSlot = 0;
    }

    public void ResetShotSkillUsage()
    {
        hasUsedSkillThisShot = false;
        grantedSlot = 0;
    }

    public string DebugState() => $"grantedSlot={grantedSlot} | hasUsedSkillThisShot={hasUsedSkillThisShot}";

    /// <summary>Dùng skill ở slot (1..3), loại Toggle. Trả về true nếu skill thực sự được kích hoạt.</summary>
    public bool TryUseSlot(int slot)
    {
        return TryUseInternal(slot, false, default(SkillTargetData));
    }

    /// <summary>Dùng skill ở slot (1..3) với dữ liệu đã chọn (Area/Target). Trừ stamina ở đây.</summary>
    public bool TryUseSlotWithTarget(int slot, SkillTargetData data)
    {
        return TryUseInternal(slot, true, data);
    }

    private bool TryUseInternal(int slot, bool withTarget, SkillTargetData data)
    {
        BaseSkills skill = GetSkill(slot);
        if (skill == null)
        {
            Debug.LogError($"[PSC P{PlayerNo()}] slot{slot} NULL — kiểm tra Loadout!");
            return false;
        }

        if (enforceSelectionGate && grantedSlot != slot)
        {
            Debug.LogWarning($"[PSC P{PlayerNo()}] BỊ CHẶN: slot{slot} chưa được giành (granted={grantedSlot})");
            return false;
        }

        if (hasUsedSkillThisShot)
        {
            Debug.LogWarning("[PSC] Đã dùng skill trong lượt đánh này, không thể dùng thêm!");
            return false;
        }

        StaminaManager stamina = StaminaManagerRegistry.Get(manager.playerNumber);
        if (stamina != null && !stamina.TryConsume(skill.staminaCost))
            return false;

        hasUsedSkillThisShot = true;
        grantedSlot = 0;
        Debug.Log($"[PSC P{PlayerNo()}] DÙNG slot{slot}: {skill.skillName} ({skill.targetingType})");

        //if (withTarget)
        //    skill.ActivateWithTarget(gameObject, manager, data);
        //else
        //    skill.Activate(gameObject, manager);

        if (withTarget)
        {
            if (skill.livePreview) skill.CommitPreview(gameObject, manager, data);
            else skill.ActivateWithTarget(gameObject, manager, data);
        }
        else
        {
            skill.Activate(gameObject, manager);
        }

        return true;
    }

    // ===== Giữ API cũ để GlaszekManager không phải sửa =====
    public void UseSkill1() { TryUseSlot(1); }
    public void UseSkill2() { TryUseSlot(2); }
    public void UseSkill3() { TryUseSlot(3); }

    public void NotifyTurnEnd()
    {
        if (loadout == null) return;
        loadout.slot1?.OnTurnEnd(manager);
        loadout.slot2?.OnTurnEnd(manager);
        loadout.slot3?.OnTurnEnd(manager);
    }

    private int PlayerNo() => manager != null ? manager.playerNumber : -1;
}

