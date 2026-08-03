using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [Header("Cần gán: registry chứa toàn bộ CharacterSO")]
    public CharacterRegistrySO characterRegistry;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save_selection.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveCurrentSelection()
    {
        if (SceneLoader.Instance == null || SceneLoader.Instance.SelectedCharacter == null)
        {
            Debug.LogWarning("[SaveManager] Chưa có lựa chọn nào để lưu.");
            return;
        }

        SaveData data = new SaveData
        {
            selectedCharacterId = int.Parse(SceneLoader.Instance.SelectedCharacter.characterId),
            selectedSkill1Id = SceneLoader.Instance.SelectedSkillLoadout.skill1Variant?.skillID ?? -1,
            selectedSkill2Id = SceneLoader.Instance.SelectedSkillLoadout.skill2Variant?.skillID ?? -1,
            selectedSkill3Id = SceneLoader.Instance.SelectedSkillLoadout.skill3Variant?.skillID ?? -1,
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveManager] Đã lưu: {json}");
    }

    public void LoadSavedSelection()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveManager] Chưa có save file, dùng mặc định.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data.selectedCharacterId == -1) return;

        CharacterSO character = characterRegistry.GetById(data.selectedCharacterId);
        if (character == null)
        {
            Debug.LogWarning("[SaveManager] Không tìm thấy character đã lưu.");
            return;
        }

        SceneLoader.Instance.SetSelectedCharacter(character);

        BaseSkills s1 = characterRegistry.FindSkillById(character, data.selectedSkill1Id);
        BaseSkills s2 = characterRegistry.FindSkillById(character, data.selectedSkill2Id);
        BaseSkills s3 = characterRegistry.FindSkillById(character, data.selectedSkill3Id);

        if (s1 != null) SceneLoader.Instance.SetSkillVariant(0, s1);
        if (s2 != null) SceneLoader.Instance.SetSkillVariant(1, s2);
        if (s3 != null) SceneLoader.Instance.SetSkillVariant(2, s3);

        Debug.Log($"[SaveManager] Đã khôi phục: {character.displayName}");
    }
}
