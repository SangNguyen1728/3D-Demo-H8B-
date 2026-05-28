using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reward Skill", menuName = "Skills/Reward")]
public class RewardSkill : BaseSkills
{
    private GameObject currentHole;

    public override void Activate(GameObject player, GlaszekManager manager)
    {
        bool isEdge = false;

        // =========================
        // 🔵 2.0 → ăn 2 bi
        // =========================
        if (skillID == 3)
        {
            currentHole = manager.ActivateHole(isEdge, false);

            HoleLogic logic = currentHole.GetComponent<HoleLogic>();

            logic.Init(false, false, 0, 2, 1);
        }

        // =========================
        // 🟢 2.1 → tồn tại 2 lượt
        // =========================
        else if (skillID == 4)
        {
            currentHole = manager.ActivateHole(isEdge, true);

            HoleLogic logic = currentHole.GetComponent<HoleLogic>();

            logic.Init(true, false, 0, 1, 2);
        }

        // =========================
        // 🔥 2.2 → HP 2500
        // =========================
        else if (skillID == 6)
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
