using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hole Skill", menuName = "Skills/Hole Spawn")]
public class HoleSpawnSkill : BaseSkills
{
    //private GameObject currentHole;

    //public override void Activate(GameObject player, GlaszekManager manager)
    //{

    //    bool isEdge = (skillID == 1 || skillID == 2 || skillID == 5);

    //    // 🔵 1.0
    //    if (skillID == 1)
    //        currentHole = manager.ActivateHole(isEdge, false);

    //    // 🟢 1.1
    //    else if (skillID == 2)
    //        currentHole = manager.ActivateHole(isEdge, true);

    //    // 🔥 1.2 (HP)
    //    else if (skillID == 5)
    //    {
    //        currentHole = manager.ActivateHole(isEdge, false);

    //        HoleLogic logic = currentHole.GetComponent<HoleLogic>();
    //        logic.Init(false, true, 2500f); // 🆕 HP MODE
    //    }

    //    manager.StartCoroutine(WaitPlacementDone());
    //}

    //System.Collections.IEnumerator WaitPlacementDone()
    //{
    //    yield return new WaitUntil(() => !HolePlacementController.Instance.IsPlacing);
    //}

    //public override void OnTurnEnd(GlaszekManager manager)
    //{
    //    if (currentHole == null) return;

    //    HoleLogic logic = currentHole.GetComponent<HoleLogic>();

    //    // 1.0
    //    if (skillID == 1)
    //    {
    //        manager.DisableHoleAfterDelay(currentHole, 5f);
    //    }

    //    // 1.1
    //    if (skillID == 2 && logic != null && logic.HasBallEntered())
    //    {
    //        Debug.Log("Skill 1.1 OK");
    //        manager.DisableHoleAfterDelay(currentHole, 5f);
    //    }

    //    //1.2
    //    if (skillID == 5 && logic != null && logic.HasBallEntered())
    //    {
    //        Debug.Log("Skill 1.2 → hết HP → 5s biến mất");
    //        manager.DisableHoleAfterDelay(currentHole, 5f);
    //    }
    //}

    private GameObject currentHole;

    public override void Activate(GameObject player, GlaszekManager manager)
    {
        bool isEdge = (skillID == 1 || skillID == 2 || skillID == 5);

        // =========================
        // 🔵 1.0 → ăn 2 bi
        // =========================
        if (skillID == 1)
        {
            currentHole = manager.ActivateHole(isEdge, false);

            HoleLogic logic = currentHole.GetComponent<HoleLogic>();

            logic.Init(false, false, 0, 2, 1);
        }

        // =========================
        // 🟢 1.1 → tồn tại 2 lượt
        // =========================
        else if (skillID == 2)
        {
            currentHole = manager.ActivateHole(isEdge, true);

            HoleLogic logic = currentHole.GetComponent<HoleLogic>();

            logic.Init(true, false, 0, 1, 2);
        }

        // =========================
        // 🔥 1.2 → HP 2500
        // =========================
        else if (skillID == 5)
        {
            currentHole = manager.ActivateHole(isEdge, false);

            HoleLogic logic = currentHole.GetComponent<HoleLogic>();

            logic.Init(false, true, 2500f, 1, 1);
        }

        manager.StartCoroutine(WaitPlacementDone());
    }

    System.Collections.IEnumerator WaitPlacementDone()
    {
        yield return new WaitUntil(() => !HolePlacementController.Instance.IsPlacing);
    }

    public override void OnTurnEnd(GlaszekManager manager)
    {
        if (currentHole == null) return;

        HoleLogic logic = currentHole.GetComponent<HoleLogic>();

        if (logic == null) return;

        logic.ReduceTurn();

        // =========================
        // 🔥 TURN EXPIRE
        // =========================
        if (logic.IsExpired())
        {
            manager.DisableHoleAfterDelay(currentHole, 5f);
            return;
        }

        // =========================
        // 🔥 COMPLETE
        // =========================
        if (logic.IsCompleted())
        {
            manager.DisableHoleAfterDelay(currentHole, 5f);
        }
    }
}
