using UnityEngine;

//[CreateAssetMenu(fileName = "BaseSkills", menuName = "Scriptable Objects/BaseSkills")]
public abstract class BaseSkills : ScriptableObject
{
    public string skillName;
    public int skillID;
    public Sprite icon;
    public int level = 1;

    // Hàm thực thi kỹ năng (Sẽ được ghi đè ở lớp con)
    public abstract void Activate(GameObject player, SkillManager manager);

    // Hàm dọn dẹp sau khi lượt cơ kết thúc
    public abstract void OnTurnEnd(SkillManager manager);
}
