using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hole Skill", menuName = "Skills/Hole Spawn")]
public class HoleSpawnSkill : BaseSkills
{
    private bool isEdge;

    public override void Activate(GameObject player, SkillManager manager)
    {
        isEdge = (skillID < 2);

        // 🟢 Skill 1.0 → ăn bi + auto 5s
        if (skillID == 1)
        {
            manager.ActivateHoleWithLogic(isEdge, true, 5f);
        }
        // 🔵 Skill 1.1 → chỉ tồn tại đến hết lượt
        else if (skillID == 2)
        {
            manager.ActivateHoleWithLogic(isEdge, false, -1f);
        }
    }

    public override void OnTurnEnd(SkillManager manager)
    {
        // 🔵 Skill 1.1 → sau khi bi dừng → 5s
        if (skillID == 2)
        {
            Debug.Log("Skill 1.1 → bắt đầu đếm 5s");

            manager.DisableHolesAfterDelay(isEdge, 5f);
        }
    }

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
