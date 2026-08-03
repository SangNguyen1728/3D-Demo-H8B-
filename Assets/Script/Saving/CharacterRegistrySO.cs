using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterRegistry", menuName = "Billiards/Character Registry")]
public class CharacterRegistrySO : ScriptableObject
{
    public List<CharacterSO> allCharactersSO;

    public CharacterSO GetById(int characterId)
    {
        foreach (var c in allCharactersSO)
            if (c.characterId == characterId.ToString()) return c;
        return null;
    }

    public BaseSkills FindSkillById(CharacterSO character, int skillId)
    {
        if (character == null) return null;
        foreach (var slot in character.skillSlots)
            foreach (var variant in slot.variants)
                if (variant.skillID == skillId) return variant;
        return null;
    }
}