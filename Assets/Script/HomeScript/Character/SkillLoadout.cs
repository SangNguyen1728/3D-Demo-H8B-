using System;
using UnityEngine;

[Serializable]
public class SkillLoadout
{
    public BaseSkills skill1Variant;
    public BaseSkills skill2Variant;
    public BaseSkills skill3Variant;

    public bool IsComplete()
    {
        return skill1Variant != null && skill2Variant != null && skill3Variant != null;
    }
}
