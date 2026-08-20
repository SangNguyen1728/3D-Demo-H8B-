using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    //private void Awake()
    //{
    //    SkillLoadout loadout = (SceneLoader.Instance != null)
    //        ? SceneLoader.Instance.SelectedSkillLoadout
    //        : null;

    //    PlayerSkillController controller = FindFirstObjectByType<PlayerSkillController>();
    //    if (controller == null)
    //    {
    //        Debug.LogError("[CharacterSpawner] Không tìm thấy PlayerSkillController có sẵn trong PlayScene!");
    //        return;
    //    }

    //    ApplySkillLoadout(controller, loadout);

    //    CharacterSkillDisplay display = controller.GetComponent<CharacterSkillDisplay>();
    //    if (display != null)
    //        display.ApplyLoadout(loadout);
    //}

    //private void ApplySkillLoadout(PlayerSkillController controller, SkillLoadout loadout)
    //{
    //    if (loadout == null || !loadout.IsComplete())
    //    {
    //        Debug.LogWarning("[CharacterSpawner] Loadout không đầy đủ, giữ nguyên loadout mặc định đang gán sẵn trên PlayerSkillController.");
    //        return;
    //    }

    //    PlayerSkillLoadout runtimeLoadout = ScriptableObject.CreateInstance<PlayerSkillLoadout>();
    //    runtimeLoadout.slot1 = loadout.skill1Variant;
    //    runtimeLoadout.slot2 = loadout.skill2Variant;
    //    runtimeLoadout.slot3 = loadout.skill3Variant;

    //    controller.loadout = runtimeLoadout;

    //    Debug.Log($"[CharacterSpawner] Đã áp dụng skill: " +
    //        $"{loadout.skill1Variant.skillName}, {loadout.skill2Variant.skillName}, {loadout.skill3Variant.skillName}");
    //}

    private void Awake()
    {
        SkillLoadout loadout = (SceneLoader.Instance != null)
            ? SceneLoader.Instance.SelectedSkillLoadout
            : null;

        PlayerSkillController[] allControllers = FindObjectsByType<PlayerSkillController>(FindObjectsSortMode.None);

        if (allControllers.Length == 0)
        {
            Debug.LogError("[CharacterSpawner] Không tìm thấy PlayerSkillController nào trong PlayScene!");
            return;
        }

        // Player01 và Player02 dùng chung 1 loadout (mirror) — theo đúng thiết kế hiện tại
        foreach (var controller in allControllers)
        {
            ApplySkillLoadout(controller, loadout);
        }
    }

    private void ApplySkillLoadout(PlayerSkillController controller, SkillLoadout loadout)
    {
        if (loadout == null || !loadout.IsComplete())
        {
            Debug.LogWarning("[CharacterSpawner] Loadout không đầy đủ, giữ nguyên loadout mặc định trên PlayerSkillController.");
            return;
        }

        PlayerSkillLoadout runtimeLoadout = ScriptableObject.CreateInstance<PlayerSkillLoadout>();
        runtimeLoadout.slot1 = loadout.skill1Variant;
        runtimeLoadout.slot2 = loadout.skill2Variant;
        runtimeLoadout.slot3 = loadout.skill3Variant;

        controller.loadout = runtimeLoadout;
    }
}
