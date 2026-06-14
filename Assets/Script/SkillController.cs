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

    private CueStickController cueStick;

    void Start()
    {
        cueStick = FindFirstObjectByType<CueStickController>();
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
