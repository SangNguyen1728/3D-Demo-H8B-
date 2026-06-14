using UnityEngine;



[CreateAssetMenu(fileName = "New Loadout", menuName = "Skills/PlayerSkillLoadout")]
public class PlayerSkillLoadout : ScriptableObject
{
    public BaseSkills slot1;
    public BaseSkills slot2;
    public BaseSkills slot3;
}
