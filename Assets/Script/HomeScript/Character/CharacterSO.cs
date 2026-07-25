using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Billiards/Character")]
public class CharacterSO : ScriptableObject
{
    [Header("Định danh (dùng để lưu/tra cứu, network sau này)")]
    public string characterId;

    [Header("Hiển thị")]
    public string displayName;
    [TextArea] public string description;
    public Sprite portrait;

    [Header("Gameplay")]
    public GameObject characterPrefab;

    [Header("Skill Slots (đúng thứ tự: Skill1, Skill2, Skill3)")]
    public List<SkillSlotSO> skillSlots;
}
