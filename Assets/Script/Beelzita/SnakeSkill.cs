using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Snake Skill", menuName = "Skills/Snake")]
public class SnakeSkill : BaseSkills
{
    //// Bi số cần check (không phải bi 8)
    //private readonly int[] targetBallNumbers = { 1, 2, 3 };
    //// Ngưỡng HP % để execute
    //private readonly float[] thresholds = { 0.30f, 0.20f, 0.10f };

    //public override void Activate(GameObject player, GlaszekManager manager)
    //{
    //    if (skillID != 17) return; // ← sửa từ 7 thành 17

    //    Debug.Log($"[Rắn] Activate, skillID = {skillID}");

    //    bool anyExecuted = false;
    //    BallHealth[] allBalls = Object.FindObjectsOfType<BallHealth>();

    //    for (int i = 0; i < targetBallNumbers.Length; i++)
    //    {
    //        int targetNum = targetBallNumbers[i];
    //        float threshold = thresholds[i];

    //        foreach (BallHealth bh in allBalls)
    //        {
    //            BallNo bn = bh.GetComponent<BallNo>();
    //            if (bn == null) continue;
    //            if (bn.ballNumber != targetNum) continue;
    //            if (!bh.gameObject.activeInHierarchy) continue;

    //            float percent = bh.GetCurrentHealth() / bh.maxHealth;
    //            Debug.Log($"[Rắn] Bi {targetNum}: {percent * 100:F1}% | Ngưỡng {threshold * 100}%");

    //            if (percent < threshold)
    //            {
    //                bh.TakeDamage(bh.maxHealth);
    //                anyExecuted = true;
    //                Debug.Log($"[Rắn] Execute bi số {targetNum}!");
    //            }
    //        }
    //    }

    //    if (!anyExecuted)
    //        Debug.Log("[Rắn] Không có bi nào đủ điều kiện");
    //}

    //public override void OnTurnEnd(GlaszekManager manager)
    //{
    //    // Rắn không có hiệu ứng theo turn
    //}




    public override void Activate(GameObject player, GlaszekManager manager)
    {
        Debug.Log($"[SnakeSkill] Activate skillID={skillID}");

        CueStickController cue =
            Object.FindFirstObjectByType<CueStickController>();

        if (cue == null || cue.cueBall == null)
        {
            Debug.LogError("[SnakeSkill] Không tìm thấy bi trắng!");
            return;
        }

        // Gắn tracker vào bi trắng nếu chưa có
        SnakeSkillTracker tracker =
            cue.cueBall.GetComponent<SnakeSkillTracker>() ??
            cue.cueBall.gameObject.AddComponent<SnakeSkillTracker>();

        tracker.Activate();
        Debug.Log("[SnakeSkill] Đã gắn tracker vào bi trắng");
    }

    public override void OnTurnEnd(GlaszekManager manager)
    {
        CueStickController cue =
            Object.FindFirstObjectByType<CueStickController>();

        if (cue?.cueBall == null) return;

        SnakeSkillTracker tracker =
            cue.cueBall.GetComponent<SnakeSkillTracker>();

        if (tracker != null)
            tracker.Deactivate();
    }
}

// ================================
// Gắn vào bi trắng lúc runtime
// ================================
public class SnakeSkillTracker : MonoBehaviour
{
    private bool isActive = false;
    private List<int> hitOrder = new List<int>();

    // Thứ tự chạm: bi 1→30%, bi 2→20%, bi 3→10%
    private readonly float[] damagePercents = { 0.30f, 0.20f, 0.10f };

    public void Activate()
    {
        isActive = true;
        hitOrder.Clear();
        Debug.Log("[SnakeTracker] Kích hoạt");
    }

    public void Deactivate()
    {
        isActive = false;
        hitOrder.Clear();
        Debug.Log("[SnakeTracker] Tắt");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;
        if (hitOrder.Count >= 3) return;

        BallNo otherBall =
            collision.gameObject.GetComponent<BallNo>();

        if (otherBall == null || otherBall.isCueBall) return;

        // Tránh đếm trùng cùng 1 bi
        if (hitOrder.Contains(otherBall.ballNumber)) return;

        int hitIndex = hitOrder.Count;
        hitOrder.Add(otherBall.ballNumber);

        float percent = damagePercents[hitIndex];

        BallHealth bh = otherBall.GetComponent<BallHealth>();
        if (bh == null) return;

        float damage = bh.maxHealth * percent;
        bh.TakeDamage(damage);

        Debug.Log($"[SnakeTracker] Bi {otherBall.ballNumber} " +
                  $"chạm thứ {hitIndex + 1} → -{percent * 100}% HP ({damage})");

        if (hitOrder.Count >= 3)
        {
            Deactivate();
            Debug.Log("[SnakeTracker] Đủ 3 bi → tắt skill");
        }
    }
}

