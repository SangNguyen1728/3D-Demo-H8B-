using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillSlot", menuName = "Billiards/Skill Slot")]
public class SkillSlotSO : ScriptableObject
{
    [Header("Định danh")]
    public string slotId;
    public string slotDisplayName;

    [Header("Các biến thể của skill này (VD: 1.0, 1.1, 1.2)")]
    public List<BaseSkills> variants;
}
