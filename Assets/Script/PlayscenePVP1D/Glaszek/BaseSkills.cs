using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "BaseSkills", menuName = "Scriptable Objects/BaseSkills")]
public abstract class BaseSkills : ScriptableObject
{
    //public string skillName;
    //public int skillID;
    //public Sprite icon;
    //public int level = 1;

    //public string skill_ID;
    //public Sprite skillIcon;
    //public float cooldown;

    //public enum SkillType
    //{
    //    Active,
    //    Passive
    //}

    //// Hàm thực thi kỹ năng (Sẽ được ghi đè ở lớp con)
    //public abstract void Activate(GameObject player, GlaszekManager manager);

    //// Hàm dọn dẹp sau khi lượt cơ kết thúc
    //public abstract void OnTurnEnd(GlaszekManager manager);

    public string skillName;
    public int skillID;
    public Sprite icon;
    public int level = 1;
    public string skill_ID;
    public Sprite skillIcon;
    public float cooldown;

    // MỚI — phục vụ UI chọn skill ở HomeScene, không ảnh hưởng logic gameplay
    [TextArea] public string description;
    public int requiredLevel = 1;

    public enum SkillType
    {
        Active,
        Passive
    }

    public abstract void Activate(GameObject player, GlaszekManager manager);
    public abstract void OnTurnEnd(GlaszekManager manager);
}
