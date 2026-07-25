using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "NewSkillVariant", menuName = "Billiards/Skill Variant")]
public class SkillVariantSO : ScriptableObject
{
    [Header("Định danh")]
    [Tooltip("VD: '1.0', '1.1', '1.2'")]
    public string variantId;

    [Header("Hiển thị")]
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Unlock (chưa dùng thật, chừa sẵn cho hệ thống level sau)")]
    public int requiredLevel = 1;

    [Header("Liên kết tới skill thật")]
    [Tooltip("Kéo vào đây ScriptableObject skill thật đã làm (VD: MouseSkill, RabbitSkill, SnakeSkill, ConvertBallSkill...)")]
    public ScriptableObject skillData;
}
