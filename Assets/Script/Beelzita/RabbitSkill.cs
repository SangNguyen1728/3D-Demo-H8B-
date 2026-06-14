using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Rabbit Skill", menuName = "Skills/Rabbit")]
public class RabbitSkill : BaseSkills
{
    //public override void Activate(GameObject player, GlaszekManager manager)
    //{
    //    // Rabbit luôn đặt trong vùng tam giác rack, không phải edge
    //    bool isEdge = false;
    //    GameObject hole = manager.ActivateHole(isEdge, false);
    //    HoleLogic logic = hole.GetComponent<HoleLogic>();

    //    // 2.0 — Thỏ: bi neutral, 3 lượt, chỉ trong tam giác
    //    if (skillID == 14)
    //    {
    //        logic.Init(true, false, 0f, 1, 3);
    //        logic.ownerSkillID = skillID;
    //        logic.SetPlacementZone(PlacementZone.RackTriangle);
    //        Debug.Log("[Thỏ 2.0] Bi neutral, 3 lượt, tam giác rack");
    //    }
    //    // 2.1 — Thỏ Tơ: 1000HP, vào lỗ nổi lại -300HP, miễn địch
    //    else if (skillID == 15)
    //    {
    //        logic.Init(false, true, 1000f, 1, 1);
    //        logic.ownerSkillID = skillID;
    //        logic.SetPlacementZone(PlacementZone.RackTriangle);
    //        logic.SetRespawnOnHit(true, 300f);
    //        logic.SetImmuneToEnemy(true);
    //        Debug.Log("[Thỏ Tơ 2.1] 1000HP, nổi lại -300HP, miễn địch");
    //    }
    //    // 2.2 — Thỏ Già: 1000HP, vào lỗ → địch đặt lại trong tam giác -300HP
    //    else if (skillID == 16)
    //    {
    //        logic.Init(false, true, 1000f, 1, 1);
    //        logic.ownerSkillID = skillID;
    //        logic.SetPlacementZone(PlacementZone.RackTriangle);
    //        logic.onBallEntered = () =>
    //        {
    //            Debug.Log("[Thỏ Già 2.2] Địch đặt lại bi trong tam giác, -300HP");
    //            // TODO: trigger cho phép địch đặt lại bi
    //            // EnemyReplaceController.Instance.TriggerReplace(300f);
    //        };
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




    public GameObject ballPrefab;

    public override void Activate(GameObject player, GlaszekManager manager)
    {
        Debug.Log($"[RabbitSkill] Activate skillID={skillID}");

        BeelzitaManager bm = BeelzitaManager.Instance;
        if (bm == null) { Debug.LogError("[RabbitSkill] Thiếu BeelzitaManager!"); return; }
        if (ballPrefab == null) { Debug.LogError("[RabbitSkill] Thiếu ballPrefab!"); return; }

        PocketTowPs pocket = Object.FindFirstObjectByType<PocketTowPs>();
        int currentPlayer = pocket != null ? pocket.currentPlayer : 1;
        Vector3 spawnPos = bm.GetRespawnPosition();

        // 2.0 — Thỏ: bi neutral, tồn tại 3 lượt, sau đó thành bi địch
        if (skillID == 14)
        {
            GameObject ball = bm.SpawnSkillBall(
                ballPrefab, spawnPos, 1000f,
                isEnemy: false, ownerSkillID: skillID);

            BeelzitaBallBehavior behavior =
                ball.AddComponent<BeelzitaBallBehavior>();
            behavior.Init(BeelzitaBallType.RabbitSkill20, bm, currentPlayer, turns: 3);

            Debug.Log("[Thỏ 2.0] Bi neutral spawn, 3 lượt");
        }
        // 2.1 — Thỏ Tơ: bi địch 1000HP
        //        rớt lỗ → trừ 300HP + móc lên + đặt tự do
        //        miễn sát thương từ người dùng skill
        else if (skillID == 15)
        {
            GameObject ball = bm.SpawnSkillBall(
                ballPrefab, spawnPos, 1000f,
                isEnemy: true, ownerSkillID: skillID);

            BeelzitaBallBehavior behavior =
                ball.AddComponent<BeelzitaBallBehavior>();
            behavior.Init(BeelzitaBallType.RabbitSkill21, bm, currentPlayer);

            Debug.Log("[Thỏ Tơ 2.1] Bi địch 1000HP spawn");
        }
        // 2.2 — Thỏ Già: bi địch 1000HP
        //        rớt lỗ → móc lên giữ HP (chỉ hạ bằng đánh hết máu)
        else if (skillID == 16)
        {
            GameObject ball = bm.SpawnSkillBall(
                ballPrefab, spawnPos, 1000f,
                isEnemy: true, ownerSkillID: skillID);

            BeelzitaBallBehavior behavior =
                ball.AddComponent<BeelzitaBallBehavior>();
            behavior.Init(BeelzitaBallType.RabbitSkill22, bm, currentPlayer);

            Debug.Log("[Thỏ Già 2.2] Bi địch 1000HP spawn");
        }
        else
        {
            Debug.LogWarning($"[RabbitSkill] skillID={skillID} không khớp!");
        }
    }

    public override void OnTurnEnd(GlaszekManager manager) { }
}
