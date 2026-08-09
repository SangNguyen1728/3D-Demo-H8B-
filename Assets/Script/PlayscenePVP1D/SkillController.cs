using TMPro;
using UnityEngine;

public class SkillController : MonoBehaviour
{

    //private CueStickController cueStick;

    //void Start()
    //{
    //    cueStick = FindFirstObjectByType<CueStickController>();
    //}

    //public void UI_ActivateSkill1()
    //{
    //    Debug.Log("CLICK SKILL 1");
    //    if (CanUseSkill())
    //        GlaszekManager.Instance.UseSkill1();
    //    else
    //        Debug.LogWarning("Không thể dùng skill!");
    //}

    //public void UI_ActivateSkill2()
    //{
    //    Debug.Log("CLICK SKILL 2");
    //    if (CanUseSkill())
    //        GlaszekManager.Instance.UseSkill2();
    //    else
    //        Debug.LogWarning("Không thể dùng skill!");
    //}

    //public void UI_ActivateSkill3()
    //{
    //    Debug.Log("CLICK SKILL 3");
    //    if (CanUseSkill())
    //        GlaszekManager.Instance.UseSkill3();
    //    else
    //        Debug.LogWarning("Không thể dùng skill!");
    //}

    [Header("Text hiển thị tên skill — kéo Text (TMP) con của từng nút vào đây")]
    public TMP_Text skill1Label;
    public TMP_Text skill2Label;
    public TMP_Text skill3Label;

    private CueStickController cueStick;

    void Start()
    {
        cueStick = FindFirstObjectByType<CueStickController>();
        UpdateSkillLabels();
    }

    private void UpdateSkillLabels()
    {
        PlayerSkillController controller = FindFirstObjectByType<PlayerSkillController>();
        if (controller == null || controller.loadout == null)
        {
            Debug.LogWarning("[SkillController] Không tìm thấy loadout để cập nhật tên skill!");
            return;
        }

        if (skill1Label != null)
            skill1Label.text = controller.loadout.slot1 != null ? controller.loadout.slot1.skillName : "—";

        if (skill2Label != null)
            skill2Label.text = controller.loadout.slot2 != null ? controller.loadout.slot2.skillName : "—";

        if (skill3Label != null)
            skill3Label.text = controller.loadout.slot3 != null ? controller.loadout.slot3.skillName : "—";

        Debug.Log($"[SkillController] Đã cập nhật tên nút: " +
            $"{controller.loadout.slot1?.skillName}, {controller.loadout.slot2?.skillName}, {controller.loadout.slot3?.skillName}");
    }

    public void UI_ActivateSkill1()
    {
        Debug.Log("CLICK SKILL 1");
        if (CanUseSkill())
            GlaszekManager.Instance.UseSkill1();
        else
            Debug.LogWarning("Không thể dùng skill!");
    }

    public void UI_ActivateSkill2()
    {
        Debug.Log("CLICK SKILL 2");
        if (CanUseSkill())
            GlaszekManager.Instance.UseSkill2();
        else
            Debug.LogWarning("Không thể dùng skill!");
    }

    public void UI_ActivateSkill3()
    {
        Debug.Log("CLICK SKILL 3");
        if (CanUseSkill())
            GlaszekManager.Instance.UseSkill3();
        else
            Debug.LogWarning("Không thể dùng skill!");
    }

    private bool CanUseSkill()
    {
        return true;
    }
}
