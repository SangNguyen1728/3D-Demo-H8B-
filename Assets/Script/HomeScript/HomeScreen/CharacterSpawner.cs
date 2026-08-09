using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    //[Header("Vị trí spawn nhân vật trong PlayScene")]
    //public Transform spawnPoint;

    //[Header("Fallback nếu vào PlayScene trực tiếp trong Editor (không qua Home)")]
    //public CharacterSO fallbackCharacter;

    //private void Start()
    //{
    //    CharacterSO character = (SceneLoader.Instance != null && SceneLoader.Instance.SelectedCharacter != null)
    //        ? SceneLoader.Instance.SelectedCharacter
    //        : fallbackCharacter;

    //    SkillLoadout loadout = (SceneLoader.Instance != null)
    //        ? SceneLoader.Instance.SelectedSkillLoadout
    //        : null;

    //    if (character == null)
    //    {
    //        Debug.LogError("[CharacterSpawner] Không có character nào để spawn!");
    //        return;
    //    }

    //    SpawnCharacter(character, loadout);
    //}

    //private void SpawnCharacter(CharacterSO character, SkillLoadout loadout)
    //{
    //    GameObject instance = Instantiate(character.characterPrefab, spawnPoint.position, spawnPoint.rotation);
    //    Debug.Log($"[CharacterSpawner] Đã spawn: {character.displayName}");

    //    // MỚI — gọi display tạm thời để xác nhận data test
    //    CharacterSkillDisplay display = instance.GetComponent<CharacterSkillDisplay>();
    //    if (display != null)
    //        display.ApplyLoadout(loadout);
    //}

    private void Awake()
    {
        SkillLoadout loadout = (SceneLoader.Instance != null)
            ? SceneLoader.Instance.SelectedSkillLoadout
            : null;

        PlayerSkillController controller = FindFirstObjectByType<PlayerSkillController>();
        if (controller == null)
        {
            Debug.LogError("[CharacterSpawner] Không tìm thấy PlayerSkillController có sẵn trong PlayScene!");
            return;
        }

        ApplySkillLoadout(controller, loadout);

        CharacterSkillDisplay display = controller.GetComponent<CharacterSkillDisplay>();
        if (display != null)
            display.ApplyLoadout(loadout);
    }

    private void ApplySkillLoadout(PlayerSkillController controller, SkillLoadout loadout)
    {
        if (loadout == null || !loadout.IsComplete())
        {
            Debug.LogWarning("[CharacterSpawner] Loadout không đầy đủ, giữ nguyên loadout mặc định đang gán sẵn trên PlayerSkillController.");
            return;
        }

        PlayerSkillLoadout runtimeLoadout = ScriptableObject.CreateInstance<PlayerSkillLoadout>();
        runtimeLoadout.slot1 = loadout.skill1Variant;
        runtimeLoadout.slot2 = loadout.skill2Variant;
        runtimeLoadout.slot3 = loadout.skill3Variant;

        controller.loadout = runtimeLoadout;

        Debug.Log($"[CharacterSpawner] Đã áp dụng skill: " +
            $"{loadout.skill1Variant.skillName}, {loadout.skill2Variant.skillName}, {loadout.skill3Variant.skillName}");
    }
}
