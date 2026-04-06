using UnityEngine;

public class SkillController : MonoBehaviour
{
    private CueStickController cueStick;

    [Header("TEST MODE")]
    public bool ignoreCondition = true; // bật lên để test nhanh

    void Start()
    {
        // Tự tìm trong scene
        cueStick = Object.FindFirstObjectByType<CueStickController>();

        if (cueStick == null)
        {
            Debug.LogWarning("Không tìm thấy CueStickController → sẽ dùng TEST MODE");
        }
    }

    // =========================
    // 🎮 BUTTON UI
    // =========================

    public void UI_ActivateSkill1()
    {
        Debug.Log("Nhấn Skill 1");

        if (CanUseSkill())
        {
            if (SkillManager.Instance != null)
            {
                SkillManager.Instance.UseSkillSlot1();
            }
            else
            {
                Debug.LogError("SkillManager Instance NULL!");
            }
        }
    }

    public void UI_ActivateSkill2()
    {
        Debug.Log("Nhấn Skill 2");

        if (CanUseSkill())
        {
            if (SkillManager.Instance != null)
            {
                SkillManager.Instance.UseSkillSlot2();
            }
            else
            {
                Debug.LogError("SkillManager Instance NULL!");
            }
        }
    }

    // =========================
    // 🔒 CHECK ĐIỀU KIỆN
    // =========================

    private bool CanUseSkill()
    {
        // 👉 TEST NHANH (bỏ qua điều kiện)
        if (ignoreCondition)
            return true;

        if (cueStick == null)
        {
            Debug.LogWarning("CueStick NULL!");
            return false;
        }

        if (cueStick.isMoving)
        {
            Debug.Log("Không dùng skill: bi đang chạy");
            return false;
        }

        if (cueStick.hitPeriod)
        {
            Debug.Log("Không dùng skill: đang đánh");
            return false;
        }

        return true;
    }

    //private CueStickController cueStick;

    //void Start()
    //{
    //    cueStick = GameObject.FindFirstObjectByType<CueStickController>();

    //    // GIẢ SỬ: Khi vào game, bạn Load skill từ nhân vật đã chọn
    //    // SkillManager.Instance.LoadCharacterSkills(skillA, skillB);
    //}

    //// Gán hàm này vào Event OnClick của Button 1
    //public void UI_Click_Skill1()
    //{
    //    if (CanUse()) SkillManager.Instance.UseSkillSlot1();
    //}

    //// Gán hàm này vào Event OnClick của Button 2
    //public void UI_Click_Skill2()
    //{
    //    if (CanUse()) SkillManager.Instance.UseSkillSlot2();
    //}

    //private bool CanUse()
    //{
    //    // Chỉ cho dùng khi bi dừng và không trong lúc đâm gậy
    //    return cueStick != null && !cueStick.isMoving && !cueStick.hitPeriod;
    //}
}
