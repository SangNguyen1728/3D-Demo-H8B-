using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlaszekManager : MonoBehaviour
{
    public static GlaszekManager Instance;

    //[Header("Skills")]
    //public BaseSkills slot1;
    //public BaseSkills slot2;

    [Header("Hole Groups")]
    public List<GameObject> edgeHoles;
    public List<GameObject> middleHoles;

    private PlayerSkillController skillController;



    void Awake()
    {
        Instance = this;

        skillController = GetComponent<PlayerSkillController>();
    }

    // =========================
    // 🎯 SPAWN HOLE
    // =========================
    public GameObject ActivateHole(bool isEdge, bool destroyOnBallEnter)
    {
        TurnOffAllHoles();

        List<GameObject> targetList = isEdge ? edgeHoles : middleHoles;

        int index = Random.Range(0, targetList.Count);
        GameObject hole = targetList[index];

        hole.SetActive(true);

        HoleLogic logic = hole.GetComponent<HoleLogic>();

        if (logic != null)
        {
            // 🔥 luôn reset về default
            //logic.Init(destroyOnBallEnter, false, 0);
            logic.Init(destroyOnBallEnter, false, 0, 1,1);
        }

        HolePlacementController.Instance.StartPlacing(hole, isEdge);

        return hole;

        
    }

    private IEnumerator StartPlacingDelay(GameObject hole, bool isEdge)
    {
        yield return new WaitForSeconds(1f);

        if (HolePlacementController.Instance != null)
        {
            Debug.Log("Bắt đầu cho phép kéo lỗ");

            HolePlacementController.Instance.StartPlacing(hole, isEdge);
        }
        else
        {
            Debug.LogError("Thiếu HolePlacementController!");
        }
    }

    private Vector3 GetDefaultSpawnPosition(bool isEdge)
    {
        float tableY = 0.75f; // 🔥 chỉnh theo bàn bạn

        if (isEdge)
        {
            return new Vector3(0f, tableY, 1.2f); // gần băng
        }
        else
        {
            return new Vector3(0f, tableY, 0f); // giữa bàn
        }
    }

    // =========================
    // 🎯 DELAY DISABLE (FIX CHUẨN)
    // =========================
    public void DisableHoleAfterDelay(GameObject hole, float delay)
    {
        StartCoroutine(DisableCoroutine(hole, delay));
    }

    IEnumerator DisableCoroutine(GameObject hole, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (hole != null && hole.activeSelf)
        {
            Debug.Log("Disable hole: " + hole.name);
            hole.SetActive(false);
        }
    }

    // =========================
    void TurnOffAllHoles()
    {
        foreach (var h in edgeHoles)
            if (h != null) h.SetActive(false);

        foreach (var h in middleHoles)
            if (h != null) h.SetActive(false);
    }

    // =========================
    //public void UseSkillSlot1()
    //{
    //    Debug.Log("Use Skill 1");

    //    if (slot1 != null)
    //        slot1.Activate(gameObject, this);
    //}

    //public void UseSkillSlot2()
    //{
    //    Debug.Log("Use Skill 2");

    //    if (slot2 != null)
    //        slot2.Activate(gameObject, this);
    //}

    //// =========================
    //public void NotifyBallStopped()
    //{
    //    Debug.Log("=== BI ĐÃ DỪNG ===");

    //    if (slot1 != null) slot1.OnTurnEnd(this);
    //    if (slot2 != null) slot2.OnTurnEnd(this);
    //}
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
        Debug.Log("=== BI ĐÃ DỪNG ===");
        skillController.NotifyTurnEnd();
    }

    public void NotifyShotStarted()
    {
        Debug.Log("=== SHOT START ===");

        HoleLogic[] holes = FindObjectsOfType<HoleLogic>();

        foreach (var h in holes)
        {
            if (h.gameObject.activeSelf)
            {
                h.ActivateHole(); // 🔥 CHỈ Ở ĐÂY
            }
        }
    }

    
}
