using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reward Skill", menuName = "Skills/Reward")]
public class RewardSkill : BaseSkills
{
    private bool isEdge = false;

    public override void Activate(GameObject player, SkillManager manager)
    {
        // 🟢 Skill 2.0
        if (skillID == 3)
        {
            manager.ActivateHoleWithLogic(isEdge, true, 5f);
        }
        // 🔵 Skill 2.1
        else if (skillID == 4)
        {
            manager.ActivateHoleWithLogic(isEdge, false, -1f);
        }
    }

    public override void OnTurnEnd(SkillManager manager)
    {
        if (skillID == 4)
        {
            Debug.Log("Skill 2.1 → bắt đầu đếm 5s");

            manager.DisableHolesAfterDelay(isEdge, 5f);
        }
    }

    //[Header("Setup Prefab")]
    //public GameObject rewardHolePrefab; // Kéo file Prefab cái lỗ từ cửa sổ Project vào đây

    //private GameObject currentInstance;

    //public override void Activate(GameObject player, SkillManager manager)
    //{
    //    // 1. ÉP BUỘC lấy vị trí ngẫu nhiên từ danh sách GIỮA BÀN (Middle)
    //    // Tham số 'false' truyền vào GetRandomSpawnPos để chọn list middleHolePoints
    //    //Vector3 spawnPos = manager.GetRandomSpawnPos(false);
    //    SkillManager.Instance.ActivateRandomHole(true);

    //    // 2. Kiểm tra nếu chưa gán Prefab để tránh lỗi Null
    //    if (rewardHolePrefab == null)
    //    {
    //        Debug.LogError($"<color=red>Lỗi:</color> Skill {skillName} chưa được gán Hole Prefab trong Inspector!");
    //        return;
    //    }

    //    // 3. Khởi tạo bản sao của lỗ
    //    //currentInstance = Instantiate(rewardHolePrefab, spawnPos, Quaternion.identity);

    //    // 4. QUAN TRỌNG: Ép lỗ hiển thị (Active = true) 
    //    // Đề phòng trường hợp file Prefab gốc đang bị ẩn
    //    //currentInstance.SetActive(true);

    //    // 5. Log ra Console để bạn kiểm tra vị trí trong lúc Test
    //    //Debug.Log($"<color=cyan>Test Skill:</color> Đã tạo lỗ '{rewardHolePrefab.name}' tại giữa bàn: {spawnPos}");
    //}

    //public override void OnTurnEnd(SkillManager manager)
    //{
    //    // Xóa lỗ khi kết thúc lượt (bi dừng) để chuẩn bị cho lần bấm sau
    //    if (currentInstance != null)
    //    {
    //        Destroy(currentInstance);
    //    }
    //}

    //public int baseRewardAmount = 1; // Số bi nhận được
    //public GameObject rewardHolePrefab;

    //public override void Activate(GameObject player, SkillManager manager)
    //{


    //    //int totalReward = baseRewardAmount + (level - 1);

    //    //// 1. Khởi tạo lỗ
    //    //GameObject hole = Instantiate(rewardHolePrefab, manager.GetSpawnPos(skillID), Quaternion.identity);

    //    //// 2. Lấy component và kiểm tra an toàn
    //    //HoleLogic script = hole.GetComponent<HoleLogic>();

    //    //if (script != null)
    //    //{
    //    //    script.rewardAmount = totalReward;
    //    //    // Nếu skillID là 1.2 hoặc 2.2 (hỏa mù/telelens) thì đánh dấu là nhận máu
    //    //    script.isHealthReward = (skillID == 1.2f || skillID == 2.2f);
    //    //}
    //    //else
    //    //{
    //    //    Debug.LogError("Lỗi: Prefab " + rewardHolePrefab.name + " chưa được gắn script HoleLogic!");
    //    //}

    //    //int totalReward = baseRewardAmount + (level - 1);
    //    //Vector3 spawnPos = manager.GetRandomSpawnPos(true); // Luôn ở cạnh bàn chẳng hạn

    //    //GameObject hole = Instantiate(rewardHolePrefab, spawnPos, Quaternion.identity);
    //    //HoleLogic script = hole.GetComponent<HoleLogic>();
    //    //if (script != null) script.rewardAmount = totalReward;
    //}

    //public override void OnTurnEnd(SkillManager manager) { /* Biến mất sau va chạm */ }
}
