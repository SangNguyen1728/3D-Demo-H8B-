using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    //public PlayerSkillLoadout loadout; // ← giờ là ScriptableObject, reference ổn định

    //private GlaszekManager manager;
    //private bool hasUsedSkillThisShot = false; // MỚI

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



    //public void ResetShotSkillUsage()
    //{
    //    hasUsedSkillThisShot = false;
    //}
    //public void UseSkill1()
    //{

    //    if (hasUsedSkillThisShot)
    //    {
    //        Debug.LogWarning("[PSC] Đã dùng skill trong lượt đánh này, không thể dùng thêm!");
    //        return;
    //    }

    //    if (loadout == null || loadout.slot1 == null)
    //    {
    //        Debug.LogError("[PSC] slot1 NULL — kiểm tra Loadout!");
    //        return;
    //    }

    //    StaminaManager stamina = StaminaManagerRegistry.Get(manager.playerNumber);
    //    if (stamina != null && !stamina.TryConsume(loadout.slot1.staminaCost))
    //        return;

    //    hasUsedSkillThisShot = true; // MỚI — khóa lại cho tới cú đánh sau
    //    loadout.slot1.Activate(gameObject, manager);
    //}

    //public void UseSkill2()
    //{
    //    //if (loadout == null || loadout.slot2 == null)
    //    //{
    //    //    Debug.LogError("[PSC] slot2 NULL — kiểm tra BeelzitaLoadout!");
    //    //    return;
    //    //}
    //    //loadout.slot2.Activate(gameObject, manager);

    //    if (hasUsedSkillThisShot)
    //    {
    //        Debug.LogWarning("[PSC] Đã dùng skill trong lượt đánh này, không thể dùng thêm!");
    //        return;
    //    }

    //    if (loadout == null || loadout.slot2 == null)
    //    {
    //        Debug.LogError("[PSC] slot2 NULL — kiểm tra Loadout!");
    //        return;
    //    }

    //    StaminaManager stamina = StaminaManagerRegistry.Get(manager.playerNumber);
    //    if (stamina != null && !stamina.TryConsume(loadout.slot2.staminaCost))
    //        return;

    //    hasUsedSkillThisShot = true;
    //    loadout.slot2.Activate(gameObject, manager);
    //}

    //public void UseSkill3()
    //{
    //    //if (loadout == null || loadout.slot3 == null)
    //    //{
    //    //    Debug.LogError("[PSC] slot3 NULL — kiểm tra BeelzitaLoadout!");
    //    //    return;
    //    //}
    //    //loadout.slot3.Activate(gameObject, manager);

    //    if (hasUsedSkillThisShot)
    //    {
    //        Debug.LogWarning("[PSC] Đã dùng skill trong lượt đánh này, không thể dùng thêm!");
    //        return;
    //    }

    //    if (loadout == null || loadout.slot3 == null)
    //    {
    //        Debug.LogError("[PSC] slot3 NULL — kiểm tra Loadout!");
    //        return;
    //    }

    //    StaminaManager stamina = StaminaManagerRegistry.Get(manager.playerNumber);
    //    if (stamina != null && !stamina.TryConsume(loadout.slot3.staminaCost))
    //        return;

    //    hasUsedSkillThisShot = true;
    //    loadout.slot3.Activate(gameObject, manager);
    //}

    //public void NotifyTurnEnd()
    //{
    //    if (loadout == null) return;
    //    loadout.slot1?.OnTurnEnd(manager);
    //    loadout.slot2?.OnTurnEnd(manager);
    //    loadout.slot3?.OnTurnEnd(manager);
    //}

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

    /// <summary>Dùng skill ở slot (1..3). Trả về true nếu skill thực sự được kích hoạt.</summary>
    public bool TryUseSlot(int slot)
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
        Debug.Log($"[PSC P{PlayerNo()}] DÙNG slot{slot}: {skill.skillName}");
        skill.Activate(gameObject, manager);
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
