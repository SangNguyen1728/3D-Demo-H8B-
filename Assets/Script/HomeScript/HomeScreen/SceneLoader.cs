using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //public static SceneLoader Instance { get; private set; }

    //private string sceneToLoad;

    //// Ch? ?? ch?i ?ang ???c ch?n — PlayScene s? ??c cái này ?? t? config
    //public GameModeSO CurrentMode { get; private set; }

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //// Dùng khi chuy?n scene KHÔNG liên quan ch? ?? ch?i (Home, ModeSelect...)
    //public void LoadScene(string sceneName)
    //{
    //    sceneToLoad = sceneName;
    //    CurrentMode = null;
    //    Debug.Log($"[SceneLoader] Chu?n b? load scene: {sceneName}");
    //    SceneManager.LoadScene("LoadingScene");
    //}

    //// Dùng khi ch?n 1 ch? ?? ch?i c? th? t? ModeSelectScene
    //public void LoadGameMode(GameModeSO mode)
    //{
    //    if (mode == null || string.IsNullOrEmpty(mode.sceneName))
    //    {
    //        Debug.LogError("[SceneLoader] GameModeSO không h?p l? ho?c thi?u sceneName!");
    //        return;
    //    }

    //    sceneToLoad = mode.sceneName;
    //    CurrentMode = mode;
    //    Debug.Log($"[SceneLoader] Chu?n b? load ch? ??: {mode.displayName} -> scene {mode.sceneName}");
    //    SceneManager.LoadScene("LoadingScene");
    //}

    //public string GetSceneToLoad()
    //{
    //    return sceneToLoad;
    //}


    //public static SceneLoader Instance { get; private set; }

    //private string sceneToLoad;

    //// Mode đang được áp dụng khi vào PlayScene (đọc bởi GameManager)
    //public GameModeSO CurrentMode { get; private set; }

    //// MỚI: Mode người dùng vừa chọn trong ModePanel, ghi nhớ tạm trước khi bấm Play
    //public GameModeSO SelectedMode { get; private set; }

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    //// MỚI: gọi khi người dùng bấm chọn 1 mode trong ModePanel
    //public void SetSelectedMode(GameModeSO mode)
    //{
    //    SelectedMode = mode;
    //    Debug.Log($"[SceneLoader] Đã ghi nhớ mode: {mode.displayName}");
    //}

    //// Dùng cho các trường hợp chuyển scene không liên quan chế độ chơi
    //public void LoadScene(string sceneName)
    //{
    //    sceneToLoad = sceneName;
    //    CurrentMode = null;
    //    Debug.Log($"[SceneLoader] Chuẩn bị load scene: {sceneName}");
    //    SceneManager.LoadScene("LoadingScene");
    //}

    //// Gọi khi bấm nút Play ở HomePanel, dùng mode đã ghi nhớ
    //public void LoadGameMode(GameModeSO mode)
    //{
    //    if (mode == null || string.IsNullOrEmpty(mode.sceneName))
    //    {
    //        Debug.LogError("[SceneLoader] GameModeSO không hợp lệ hoặc thiếu sceneName!");
    //        return;
    //    }

    //    sceneToLoad = mode.sceneName;
    //    CurrentMode = mode;
    //    Debug.Log($"[SceneLoader] Chuẩn bị load chế độ: {mode.displayName} -> scene {mode.sceneName}");
    //    SceneManager.LoadScene("LoadingScene");
    //}

    //public string GetSceneToLoad()
    //{
    //    return sceneToLoad;
    //}


    public static SceneLoader Instance { get; private set; }

    private string sceneToLoad;

    public GameModeSO CurrentMode { get; private set; }
    public GameModeSO SelectedMode { get; private set; }

    public CharacterSO SelectedCharacter { get; private set; }
    public SkillLoadout SelectedSkillLoadout { get; private set; } = new SkillLoadout();

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

    public void SetSelectedMode(GameModeSO mode)
    {
        SelectedMode = mode;
        Debug.Log($"[SceneLoader] Đã ghi nhớ mode: {mode.displayName}");
    }

    public void SetSelectedCharacter(CharacterSO character)
    {
        SelectedCharacter = character;
        SelectedSkillLoadout = new SkillLoadout(); // đổi nhân vật -> reset skill đã chọn
        Debug.Log($"[SceneLoader] Đã chọn nhân vật: {character.displayName}");
    }

    public void SetSkillVariant(int slotIndex, BaseSkills variant)
    {
        switch (slotIndex)
        {
            case 0: SelectedSkillLoadout.skill1Variant = variant; break;
            case 1: SelectedSkillLoadout.skill2Variant = variant; break;
            case 2: SelectedSkillLoadout.skill3Variant = variant; break;
            default:
                Debug.LogError($"[SceneLoader] slotIndex không hợp lệ: {slotIndex}");
                return;
        }
        Debug.Log($"[SceneLoader] Slot {slotIndex} -> {variant.skillName}");
    }

    public void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        CurrentMode = null;
        Debug.Log($"[SceneLoader] Chuẩn bị load scene: {sceneName}");
        SceneManager.LoadScene("LoadingScene");
    }

    public void LoadGameMode(GameModeSO mode)
    {
        if (mode == null || string.IsNullOrEmpty(mode.sceneName))
        {
            Debug.LogError("[SceneLoader] GameModeSO không hợp lệ hoặc thiếu sceneName!");
            return;
        }

        sceneToLoad = mode.sceneName;
        CurrentMode = mode;
        Debug.Log($"[SceneLoader] Chuẩn bị load chế độ: {mode.displayName} -> scene {mode.sceneName}");
        SceneManager.LoadScene("LoadingScene");
    }

    public string GetSceneToLoad()
    {
        return sceneToLoad;
    }
}
