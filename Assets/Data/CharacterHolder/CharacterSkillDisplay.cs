using UnityEngine;
using TMPro;
public class CharacterSkillDisplay : MonoBehaviour
{
    [Header("Text hiển thị (kéo vào nếu muốn xem trực quan trong PlayScene)")]
    public TMP_Text debugText;

    public void ApplyLoadout(SkillLoadout loadout)
    {
        //if (loadout == null || !loadout.IsComplete())
        //{
        //    Debug.LogWarning("[CharacterSkillDisplay] Loadout không đầy đủ!");
        //    if (debugText != null) debugText.text = "Chưa có skill";
        //    return;
        //}

        //string info = $"Skill1: {loadout.skill1Variant.skillName}\n" +
        //              $"Skill2: {loadout.skill2Variant.skillName}\n" +
        //              $"Skill3: {loadout.skill3Variant.skillName}";

        //Debug.Log("[CharacterSkillDisplay]\n" + info);

        //if (debugText != null)
        //    debugText.text = info;

        if (loadout == null || !loadout.IsComplete())
        {
            Debug.LogWarning("[CharacterSkillDisplay] Loadout không đầy đủ!");
            return;
        }

        string info = $"Skill1: {loadout.skill1Variant.skillName}\n" +
                      $"Skill2: {loadout.skill2Variant.skillName}\n" +
                      $"Skill3: {loadout.skill3Variant.skillName}";

        Debug.Log("[CharacterSkillDisplay]\n" + info);
    }
}
