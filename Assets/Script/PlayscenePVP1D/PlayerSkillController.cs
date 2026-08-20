using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public PlayerSkillLoadout loadout; // ← giờ là ScriptableObject, reference ổn định

    private GlaszekManager manager;
    private bool hasUsedSkillThisShot = false; // MỚI

    private void Awake()
    {
        //manager = GetComponent<GlaszekManager>();

        //if (manager == null)
        //    manager = FindFirstObjectByType<GlaszekManager>();

        //if (manager == null)
        //    Debug.LogError("Không tìm thấy GlaszekManager!");

        //if (loadout == null)
        //{
        //    Debug.LogError("Loadout chưa được gán!");
        //    return; // ← thoát sớm, không crash
        //}

        //Debug.Log($"Loadout OK | slot1={loadout.slot1?.skillName ?? "NULL"} | slot2={loadout.slot2?.skillName ?? "NULL"} | slot3={loadout.slot3?.skillName ?? "NULL"}");

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

    //public void UseSkill1()
    //{
    //    Debug.Log($"[PSC] UseSkill1 | slot1 = {(loadout?.slot1 != null ? loadout.slot1.skillName : "NULL")}");
    //    if (loadout?.slot1 != null)
    //        loadout.slot1.Activate(gameObject, manager);
    //    else
    //        Debug.LogError("[PSC] slot1 là NULL!");
    //}

    //public void UseSkill2()
    //{
    //    Debug.Log($"[PSC] UseSkill2 | slot2 = {(loadout?.slot2 != null ? loadout.slot2.skillName : "NULL")}");
    //    if (loadout?.slot2 != null)
    //        loadout.slot2.Activate(gameObject, manager);
    //    else
    //        Debug.LogError("[PSC] slot2 là NULL!");
    //}

    //public void UseSkill3()
    //{
    //    Debug.Log($"[PSC] UseSkill3 | slot3 = {(loadout?.slot3 != null ? loadout.slot3.skillName : "NULL")}");
    //    if (loadout?.slot3 != null)
    //        loadout.slot3.Activate(gameObject, manager);
    //    else
    //        Debug.LogError("[PSC] slot3 là NULL!");
    //}

    public void ResetShotSkillUsage()
    {
        hasUsedSkillThisShot = false;
    }
    public void UseSkill1()
    {
        //if (loadout == null || loadout.slot1 == null)
        //{
        //    Debug.LogError("[PSC] slot1 NULL — kiểm tra BeelzitaLoadout!");
        //    return;
        //}
        //loadout.slot1.Activate(gameObject, manager);

        if (hasUsedSkillThisShot)
        {
            Debug.LogWarning("[PSC] Đã dùng skill trong lượt đánh này, không thể dùng thêm!");
            return;
        }

        if (loadout == null || loadout.slot1 == null)
        {
            Debug.LogError("[PSC] slot1 NULL — kiểm tra Loadout!");
            return;
        }

        StaminaManager stamina = StaminaManagerRegistry.Get(manager.playerNumber);
        if (stamina != null && !stamina.TryConsume(loadout.slot1.staminaCost))
            return;

        hasUsedSkillThisShot = true; // MỚI — khóa lại cho tới cú đánh sau
        loadout.slot1.Activate(gameObject, manager);
    }

    public void UseSkill2()
    {
        //if (loadout == null || loadout.slot2 == null)
        //{
        //    Debug.LogError("[PSC] slot2 NULL — kiểm tra BeelzitaLoadout!");
        //    return;
        //}
        //loadout.slot2.Activate(gameObject, manager);

        if (hasUsedSkillThisShot)
        {
            Debug.LogWarning("[PSC] Đã dùng skill trong lượt đánh này, không thể dùng thêm!");
            return;
        }

        if (loadout == null || loadout.slot2 == null)
        {
            Debug.LogError("[PSC] slot2 NULL — kiểm tra Loadout!");
            return;
        }

        StaminaManager stamina = StaminaManagerRegistry.Get(manager.playerNumber);
        if (stamina != null && !stamina.TryConsume(loadout.slot2.staminaCost))
            return;

        hasUsedSkillThisShot = true;
        loadout.slot2.Activate(gameObject, manager);
    }

    public void UseSkill3()
    {
        //if (loadout == null || loadout.slot3 == null)
        //{
        //    Debug.LogError("[PSC] slot3 NULL — kiểm tra BeelzitaLoadout!");
        //    return;
        //}
        //loadout.slot3.Activate(gameObject, manager);

        if (hasUsedSkillThisShot)
        {
            Debug.LogWarning("[PSC] Đã dùng skill trong lượt đánh này, không thể dùng thêm!");
            return;
        }

        if (loadout == null || loadout.slot3 == null)
        {
            Debug.LogError("[PSC] slot3 NULL — kiểm tra Loadout!");
            return;
        }

        StaminaManager stamina = StaminaManagerRegistry.Get(manager.playerNumber);
        if (stamina != null && !stamina.TryConsume(loadout.slot3.staminaCost))
            return;

        hasUsedSkillThisShot = true;
        loadout.slot3.Activate(gameObject, manager);
    }

    public void NotifyTurnEnd()
    {
        if (loadout == null) return;
        loadout.slot1?.OnTurnEnd(manager);
        loadout.slot2?.OnTurnEnd(manager);
        loadout.slot3?.OnTurnEnd(manager);
    }
}
