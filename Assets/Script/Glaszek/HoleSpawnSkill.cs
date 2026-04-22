using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hole Skill", menuName = "Skills/Hole Spawn")]
public class HoleSpawnSkill : BaseSkills
{
    private GameObject currentHole;

    public override void Activate(GameObject player, SkillManager manager)
    {
        //bool isEdge = (skillID == 1 || skillID == 2);

        //currentHole = manager.ActivateHole(isEdge, skillID == 2);

        //manager.StartCoroutine(WaitPlacementDone());

        bool isEdge = (skillID == 1 || skillID == 2 || skillID == 5);

        // 🔵 1.0
        if (skillID == 1)
            currentHole = manager.ActivateHole(isEdge, false);

        // 🟢 1.1
        else if (skillID == 2)
            currentHole = manager.ActivateHole(isEdge, true);

        // 🔥 1.2 (HP)
        else if (skillID == 5)
        {
            currentHole = manager.ActivateHole(isEdge, false);

            HoleLogic logic = currentHole.GetComponent<HoleLogic>();
            logic.Init(false, true, 2500f); // 🆕 HP MODE
        }

        manager.StartCoroutine(WaitPlacementDone());
    }

    System.Collections.IEnumerator WaitPlacementDone()
    {
        yield return new WaitUntil(() => !HolePlacementController.Instance.IsPlacing);
    }

    public override void OnTurnEnd(SkillManager manager)
    {
        if (currentHole == null) return;

        HoleLogic logic = currentHole.GetComponent<HoleLogic>();

        // 1.0
        if (skillID == 1)
        {
            manager.DisableHoleAfterDelay(currentHole, 5f);
        }

        // 1.1
        if (skillID == 2 && logic != null && logic.HasBallEntered())
        {
            Debug.Log("Skill 1.1 OK");
            manager.DisableHoleAfterDelay(currentHole, 5f);
        }

        //1.2
        if (skillID == 5 && logic != null && logic.HasBallEntered())
        {
            Debug.Log("Skill 1.2 → hết HP → 5s biến mất");
            manager.DisableHoleAfterDelay(currentHole, 5f);
        }
    }

    //private GameObject currentHole;

    //public override void Activate(GameObject player, SkillManager manager)
    //{
    //    Debug.Log("HoleSpawnSkill Activated");

    //    bool isEdge = (skillID == 1 || skillID == 2);

    //    if (skillID == 1) // 🔵 1.0
    //    {
    //        currentHole = manager.ActivateHole(isEdge, false);
    //    }
    //    else if (skillID == 2) // 🟢 1.1
    //    {
    //        currentHole = manager.ActivateHole(isEdge, true);
    //    }
    //}

    //public override void OnTurnEnd(SkillManager manager)
    //{
    //    if (currentHole == null) return;

    //    HoleLogic logic = currentHole.GetComponent<HoleLogic>();

    //    // 🔵 Skill 1.0
    //    if (skillID == 1)
    //    {
    //        manager.DisableHoleAfterDelay(currentHole, 5f);
    //    }

    //    // 🟢 Skill 1.1
    //    if (skillID == 2 && logic != null && logic.HasBallEntered())
    //    {
    //        Debug.Log("Skill 1.1 → đã ăn bi → 5s biến mất");
    //        manager.DisableHoleAfterDelay(currentHole, 5f);
    //    }

    //    //// 🔥 chỉ skill 1.0
    //    //if (skillID == 1 && currentHole != null)
    //    //{
    //    //    Debug.Log("Skill 1.0 → 5s biến mất");
    //    //    manager.DisableHoleAfterDelay(currentHole, 5f);
    //    //}
    //}

    //private bool isEdge;

    //public override void Activate(GameObject player, SkillManager manager)
    //{
    //    isEdge = (skillID < 2);

    //    // Skill 1.0 → ăn bi
    //    if (skillID == 1)
    //    {
    //        manager.ActivateHole(isEdge, true);
    //    }
    //    // Skill 1.1 → không ăn bi
    //    else if (skillID == 2)
    //    {
    //        manager.ActivateHole(isEdge, false);
    //    }
    //}

    //public override void OnTurnEnd(SkillManager manager)
    //{
    //    // 🔥 CẢ 2 SKILL đều biến mất sau 5s khi bi dừng
    //    if (skillID == 1 || skillID == 2)
    //    {
    //        manager.DisableHolesAfterDelay(isEdge, 5f);
    //    }
    //}

    //public GameObject holePrefab; // Kéo Prefab lỗ vào đây
    //public Vector3 spawnPosition; // Vị trí (Cạnh hoặc Giữa bàn)
    //private GameObject currentHole;

    //public override void Activate(GameObject player, SkillManager manager)
    //{
    //    // Xác định vị trí (ID 1.x là cạnh, 2.x là giữa)
    //    bool isEdge = (skillID < 2);
    //    //Vector3 spawnPos = manager.GetRandomSpawnPos(isEdge);
    //    SkillManager.Instance.ActivateRandomHole(true);

    //    // Tạo bản sao
    //    //currentHole = Instantiate(holePrefab, spawnPos, Quaternion.identity);

    //    // QUAN TRỌNG: Kích hoạt lỗ nếu nó đang bị ẩn trong Prefab
    //    if (currentHole != null)
    //    {
    //        currentHole.SetActive(true);
    //        //Debug.Log($"<color=green>Skill {skillName} đã tạo lỗ tại {spawnPos}</color>");
    //    }

    //    //spawnPos = manager.GetRandomSpawnPos(skillID < 2);
    //    //currentHole = Instantiate(holePrefab, spawnPos, Quaternion.identity);
    //}

    //public override void OnTurnEnd(SkillManager manager)
    //{
    //    if (currentHole != null) Destroy(currentHole);
    //}
}
