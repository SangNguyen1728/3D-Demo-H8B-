using UnityEngine;

public class SkillController : MonoBehaviour
{
    private CueStickController cueStick;

    void Start()
    {
        cueStick = FindFirstObjectByType<CueStickController>();
    }

    public void UI_ActivateSkill1()
    {
        Debug.Log("CLICK SKILL 1");

        if (CanUseSkill())
        {
            GlaszekManager.Instance.UseSkill1();
        }
        else
        {
            Debug.LogWarning("Không thể dùng skill!");
        }
    }

    public void UI_ActivateSkill2()
    {
        Debug.Log("CLICK SKILL 2");

        if (CanUseSkill())
        {
            GlaszekManager.Instance.UseSkill2();
        }
    }

    // 🔥 TEST: tạm thời cho luôn true
    private bool CanUseSkill()
    {
        return true;
        // Sau này dùng lại:
        // return !cueStick.isMoving && !cueStick.hitPeriod;
    }
}
