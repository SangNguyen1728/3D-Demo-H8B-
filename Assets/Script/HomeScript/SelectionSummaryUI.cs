using TMPro;
using UnityEngine;

public class SelectionSummaryUI : MonoBehaviour
{
    public TMP_Text summaryText;

    private void OnEnable()
    {
        Refresh();
    }

    // Gọi hàm này mỗi khi người chơi chọn 1 skill mới
    public void Refresh()
    {
        if (SceneLoader.Instance == null || summaryText == null) return;

        SkillLoadout loadout = SceneLoader.Instance.SelectedSkillLoadout;

        string skill1 = loadout.skill1Variant != null ? loadout.skill1Variant.skillName : "—";
        string skill2 = loadout.skill2Variant != null ? loadout.skill2Variant.skillName : "—";
        string skill3 = loadout.skill3Variant != null ? loadout.skill3Variant.skillName : "—";

        summaryText.text = $"Skill 1: {skill1}\nSkill 2: {skill2}\nSkill 3: {skill3}";
    }
}
