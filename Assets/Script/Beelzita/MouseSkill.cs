using UnityEngine;
using System.Collections;
using static BeelzitaBallBehavior;
[CreateAssetMenu(fileName = "New Mouse Skill", menuName = "Skills/Mouse")]
public class MouseSkill : BaseSkills
{
    //public override void Activate(GameObject player, GlaszekManager manager)
    //{
    //    // Mouse skill luôn spawn bi địch, đặt trong khung hoặc tự do
    //    bool isEdge = true;
    //    GameObject hole = manager.ActivateHole(isEdge, false);
    //    HoleLogic logic = hole.GetComponent<HoleLogic>();

    //    // 1.0 — Chuột: hồi 300HP khi bi vào lỗ
    //    if (skillID == 11)
    //    {
    //        logic.Init(true, false, 0f, 1, 1);
    //        logic.ownerSkillID = skillID;
    //        logic.onBallEntered = () =>
    //        {
    //            // Hồi 300 HP cho player
    //            // TODO: thay bằng PlayerHealth.Instance.Heal(300f) nếu có
    //            Debug.Log("[Chuột 1.0] Hồi 300 HP cho player");
    //        };
    //    }
    //    // 1.1 — Chuột Tơ: hồi 300HP + 700DEF/3 gậy, tồn tại 3 turn
    //    else if (skillID == 12)
    //    {
    //        logic.Init(true, false, 0f, 1, 3);
    //        logic.ownerSkillID = skillID;
    //        logic.onBallEntered = () =>
    //        {
    //            Debug.Log("[Chuột Tơ 1.1] Hồi 300HP + buff 700DEF/3 gậy");
    //            if (BuffManager.Instance != null)
    //                BuffManager.Instance.AddDefBuff(700f, 3);
    //            // TODO: PlayerHealth.Instance.Heal(300f)
    //        };
    //    }
    //    // 1.2 — Chuột Già: móc lên 1 lần, giữ HP
    //    else if (skillID == 13)
    //    {
    //        logic.Init(false, true, 999999f, 1, 1);
    //        logic.ownerSkillID = skillID;
    //        logic.SetRespawnOnce(true);
    //        Debug.Log("[Chuột Già 1.2] Bi móc lên 1 lần, giữ HP");
    //    }

    //    manager.StartCoroutine(WaitPlacementDone());
    //}

    //private IEnumerator WaitPlacementDone()
    //{
    //    yield return new WaitUntil(() => !HolePlacementController.Instance.IsPlacing);
    //}

    //public override void OnTurnEnd(GlaszekManager manager)
    //{
    //    HoleLogic logic = FindActiveHoleBySkillID(skillID);
    //    if (logic == null) return;

    //    logic.ReduceTurn();

    //    if (logic.IsExpired() || logic.IsCompleted())
    //        manager.DisableHoleAfterDelay(logic.gameObject, 5f);
    //}

    //private HoleLogic FindActiveHoleBySkillID(int id)
    //{
    //    foreach (var h in Object.FindObjectsOfType<HoleLogic>())
    //        if (h.gameObject.activeSelf && h.ownerSkillID == id)
    //            return h;
    //    return null;
    //}





    public override void Activate(GameObject player, GlaszekManager manager)
    {
        Debug.Log($"[MouseSkill] Activate skillID={skillID}");

        BeelzitaManager bm = BeelzitaManager.Instance;
        if (bm == null) { Debug.LogError("[MouseSkill] Thiếu BeelzitaManager!"); return; }

        PocketTowPs pocket = Object.FindFirstObjectByType<PocketTowPs>();
        int currentPlayer = pocket != null ? pocket.currentPlayer : 1;

        // 1.0 — Chuột: móc bi địch từ lỗ lên, HP=300
        if (skillID == 11)
        {
            GameObject ball = bm.GetLastEnemyPocketedBall(currentPlayer);
            if (ball == null) { Debug.LogWarning("[Chuột 1.0] Không có bi địch trong lỗ!"); return; }

            bm.RespawnBall(ball, 300f, bm.GetRespawnPosition());
            Debug.Log("[Chuột 1.0] Móc bi địch lên HP=300");
        }
        // 1.1 — Chuột Tơ: móc bi địch từ lỗ lên, HP=300 + 700DEF/3 lượt
        else if (skillID == 12)
        {
            GameObject ball = bm.GetLastEnemyPocketedBall(currentPlayer);
            if (ball == null) { Debug.LogWarning("[Chuột Tơ 1.1] Không có bi địch trong lỗ!"); return; }

            bm.RespawnBall(ball, 300f, bm.GetRespawnPosition(), (b) =>
            {
                if (BuffManager.Instance != null)
                    BuffManager.Instance.AddDefBuff(700f, 3);
                Debug.Log("[Chuột Tơ 1.1] +700DEF/3 lượt");
            });
        }
        // 1.2 — Chuột Già: móc bi địch từ lỗ lên, HP=1000
        //        rớt lỗ → móc lên lại trừ 300HP, lặp đến khi hết máu
        else if (skillID == 13)
        {
            GameObject ball = bm.GetLastEnemyPocketedBall(currentPlayer);
            if (ball == null) { Debug.LogWarning("[Chuột Già 1.2] Không có bi địch trong lỗ!"); return; }

            bm.RespawnBall(ball, 1000f, bm.GetRespawnPosition(), (b) =>
            {
                BeelzitaBallBehavior behavior =
                    b.GetComponent<BeelzitaBallBehavior>() ??
                    b.AddComponent<BeelzitaBallBehavior>();

                behavior.Init(BeelzitaBallType.MouseSkill12, bm, currentPlayer);
                Debug.Log("[Chuột Già 1.2] HP=1000, sẽ móc lên khi rớt lỗ");
            });
        }
        else
        {
            Debug.LogWarning($"[MouseSkill] skillID={skillID} không khớp!");
        }
    }

    public override void OnTurnEnd(GlaszekManager manager)
    {
        if (BuffManager.Instance != null)
            BuffManager.Instance.TickTurn();
    }
}
