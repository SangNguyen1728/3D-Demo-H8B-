using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public PlayerSkillLoadout loadout;

    private GlaszekManager manager;

    private void Awake()
    {
        manager = GetComponent<GlaszekManager>();
    }

    public void UseSkill1()
    {
        if (loadout.slot1 != null)
            loadout.slot1.Activate(gameObject, manager);
    }

    public void UseSkill2()
    {
        if (loadout.slot2 != null)
            loadout.slot2.Activate(gameObject, manager);
    }

    public void UseSkill3()
    {
        if (loadout.slot3 != null)
            loadout.slot3.Activate(gameObject, manager);
    }

    public void NotifyTurnEnd()
    {
        if (loadout.slot1 != null)
            loadout.slot1.OnTurnEnd(manager);

        if (loadout.slot2 != null)
            loadout.slot2.OnTurnEnd(manager);

        if (loadout.slot3 != null)
            loadout.slot3.OnTurnEnd(manager);
    }
}
