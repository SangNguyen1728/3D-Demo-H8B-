using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hole Skill", menuName = "Skills/Hole Spawn")]
public class HoleSpawnSkill : BaseSkills
{
    //public override void Activate(GameObject player, GlaszekManager manager)
    //{
    //    bool isEdge = (skillID == 1 || skillID == 2 || skillID == 5);

    //    GameObject hole = manager.ActivateHole(isEdge, false);
    //    HoleLogic logic = hole.GetComponent<HoleLogic>();

    //    // 1.0 → ăn 2 bi, 1 lượt
    //    if (skillID == 1)
    //        logic.Init(false, false, 0f, 2, 1);

    //    // 1.1 → ăn 1 bi, tồn tại 2 lượt
    //    else if (skillID == 2)
    //        logic.Init(true, false, 0f, 1, 2);

    //    // 1.2 → HP 2500
    //    else if (skillID == 5)
    //        logic.Init(false, true, 2500f, 1, 1);

    //    // Gắn skillID vào HoleLogic để OnTurnEnd tìm đúng hole
    //    logic.ownerSkillID = skillID;

    //    manager.StartCoroutine(WaitPlacementDone());
    //}

    //System.Collections.IEnumerator WaitPlacementDone()
    //{
    //    yield return new WaitUntil(() => !HolePlacementController.Instance.IsPlacing);
    //}

    //public override void OnTurnEnd(GlaszekManager manager)
    //{
    //    // Tìm hole đang active thuộc skill này
    //    HoleLogic logic = FindActiveHoleBySkillID(manager, skillID);
    //    if (logic == null) return;

    //    logic.ReduceTurn();

    //    if (logic.IsExpired() || logic.IsCompleted())
    //        manager.DisableHoleAfterDelay(logic.gameObject, 5f);
    //}

    //private HoleLogic FindActiveHoleBySkillID(GlaszekManager manager, int id)
    //{
    //    HoleLogic[] all = UnityEngine.Object.FindObjectsOfType<HoleLogic>();
    //    foreach (var h in all)
    //        if (h.gameObject.activeSelf && h.ownerSkillID == id)
    //            return h;
    //    return null;
    //}

    public override void Activate(GameObject player, GlaszekManager manager)
    {
        bool isEdge = (skillID == 1 || skillID == 2 || skillID == 5);

        GameObject hole = manager.ActivateHole(isEdge, false);
        HoleLogic logic = hole.GetComponent<HoleLogic>();

        if (skillID == 1)
            logic.Init(false, false, 0f, 2, 1);
        else if (skillID == 2)
            logic.Init(true, false, 0f, 1, 2);
        else if (skillID == 5)
            logic.Init(false, true, 2500f, 1, 1);

        logic.ownerSkillID = skillID;
        logic.ownerPlayer = manager.playerNumber; // MỚI

        manager.StartCoroutine(WaitPlacementDone());
    }

    System.Collections.IEnumerator WaitPlacementDone()
    {
        yield return new WaitUntil(() => !HolePlacementController.Instance.IsPlacing);
    }

    public override void OnTurnEnd(GlaszekManager manager)
    {
        HoleLogic logic = FindActiveHoleBySkillID(manager, skillID);
        if (logic == null) return;

        logic.ReduceTurn();

        if (logic.IsExpired() || logic.IsCompleted())
            manager.DisableHoleAfterDelay(logic.gameObject, 5f);
    }

    private HoleLogic FindActiveHoleBySkillID(GlaszekManager manager, int id)
    {
        HoleLogic[] all = UnityEngine.Object.FindObjectsByType<HoleLogic>(FindObjectsSortMode.None);
        foreach (var h in all)
            if (h.gameObject.activeSelf && h.ownerSkillID == id && h.ownerPlayer == manager.playerNumber)
                return h;
        return null;
    }
}
