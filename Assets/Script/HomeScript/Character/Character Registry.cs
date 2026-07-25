using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterRegistry", menuName = "Billiards/Character Registry")]
public class CharacterRegistrySO : ScriptableObject
{
    public List<CharacterSO> allCharacters;

    public CharacterSO GetById(string characterId)
    {
        foreach (var c in allCharacters)
        {
            if (c.characterId == characterId) return c;
        }
        Debug.LogError($"[CharacterRegistry] Không tìm thấy character với id: {characterId}");
        return null;
    }
}
