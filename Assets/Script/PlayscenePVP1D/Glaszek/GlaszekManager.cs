using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlaszekManager : MonoBehaviour
{
    //public static GlaszekManager Instance;

    //private PlayerSkillController skillController;

    //void Awake()
    //{
    //    Instance = this;
    //    skillController = GetComponent<PlayerSkillController>();
    //}

    //// =========================
    //// 🎯 SPAWN HOLE — giờ gọi qua TableHoleManager
    //// =========================
    //public GameObject ActivateHole(bool isEdge, bool destroyOnBallEnter)
    //{
    //    return TableHoleManager.Instance.ActivateHole(isEdge, destroyOnBallEnter);
    //}

    //// =========================
    //// 🎯 DELAY DISABLE — giờ gọi qua TableHoleManager
    //// =========================
    //public void DisableHoleAfterDelay(GameObject hole, float delay)
    //{
    //    TableHoleManager.Instance.DisableHoleAfterDelay(hole, delay);
    //}

    //// =========================
    //public void UseSkill1()
    //{
    //    skillController.UseSkill1();
    //}

    //public void UseSkill2()
    //{
    //    skillController.UseSkill2();
    //}

    //public void UseSkill3()
    //{
    //    skillController.UseSkill3();
    //}

    //public void NotifyBallStopped()
    //{
    //    Debug.Log("=== BI ĐÃ DỪNG ===");
    //    skillController.NotifyTurnEnd();

    //    if (BuffManager.Instance != null)
    //        BuffManager.Instance.TickTurn();

    //    // Tick Beelzita behaviors
    //    BeelzitaBallBehavior[] behaviors =
    //        FindObjectsByType<BeelzitaBallBehavior>(FindObjectsSortMode.None);
    //    foreach (var b in behaviors)
    //        b.OnTurnEnd();
    //}

    //public void NotifyShotStarted()
    //{
    //    Debug.Log("=== SHOT START ===");

    //    HoleLogic[] holes = FindObjectsByType<HoleLogic>(FindObjectsSortMode.None);

    //    foreach (var h in holes)
    //    {
    //        if (h.gameObject.activeSelf)
    //        {
    //            h.ActivateHole();
    //        }
    //    }
    //}

    [Header("Player Identity")]
    [Tooltip("1 hoặc 2 — xác định đây là manager của Player nào")]
    public int playerNumber = 1;

    private PlayerSkillController skillController;

    void Awake()
    {
        PlayerManagerRegistry.Register(playerNumber, this);
        skillController = GetComponent<PlayerSkillController>();
    }

    public GameObject ActivateHole(bool isEdge, bool destroyOnBallEnter)
    {
        return TableHoleManager.Instance.ActivateHole(isEdge, destroyOnBallEnter);
    }

    public void DisableHoleAfterDelay(GameObject hole, float delay)
    {
        TableHoleManager.Instance.DisableHoleAfterDelay(hole, delay);
    }

    public void ResetSkillUsage()
    {
        skillController?.ResetShotSkillUsage();
    }
    public void UseSkill1()
    {
        skillController.UseSkill1();
    }

    public void UseSkill2()
    {
        skillController.UseSkill2();
    }

    public void UseSkill3()
    {
        skillController.UseSkill3();
    }

    public void NotifyBallStopped()
    {
        Debug.Log($"=== BI ĐÃ DỪNG (Player {playerNumber}) ===");
        skillController.NotifyTurnEnd();

        //if (BuffManager.Instance != null)
        //    BuffManager.Instance.TickTurn();
        BuffManager buff = BuffManagerRegistry.Get(playerNumber);
        if (buff != null)
            buff.TickTurn();

        BeelzitaBallBehavior[] behaviors =
            FindObjectsByType<BeelzitaBallBehavior>(FindObjectsSortMode.None);
        foreach (var b in behaviors)
            b.OnTurnEnd();
    }

    public void NotifyShotStarted()
    {
        Debug.Log($"=== SHOT START (Player {playerNumber}) ===");

        HoleLogic[] holes = FindObjectsByType<HoleLogic>(FindObjectsSortMode.None);
        foreach (var h in holes)
        {
            if (h.gameObject.activeSelf)
                h.ActivateHole();
        }
    }



}
