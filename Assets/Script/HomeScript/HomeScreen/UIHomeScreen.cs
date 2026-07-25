using UnityEngine;
using UnityEngine.UI;

public class UIHomeScreen : MonoBehaviour
{
    //public Button playButton;
    //public string modeSelectSceneName = "ModeSelectScene"; // đổi từ playSceneName cũ

    //private void Start()
    //{
    //    if (playButton != null)
    //        playButton.onClick.AddListener(OnPlayClicked);
    //}

    //private void OnPlayClicked()
    //{
    //    Debug.Log("[UIHomeScreen] Nút Play được nhấn -> vào màn chọn chế độ");
    //    SceneLoader.Instance.LoadScene(modeSelectSceneName);
    //}


    [Header("Nút bấm ở Home")]
    public Button playButton;
    public Button modeButton;
    public Button characterButton;

    [Header("Mode mặc định nếu người dùng chưa từng chọn")]
    public GameModeSO defaultMode;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        modeButton.onClick.AddListener(OnModeClicked);
        characterButton.onClick.AddListener(OnCharacterClicked);
    }

    private void OnModeClicked()
    {
        Debug.Log("[UIHomeScreen] Mở ModePanel");
        UIFlowManager.Instance.ShowMode();
    }

    private void OnCharacterClicked()
    {
        Debug.Log("[UIHomeScreen] Mở CharacterPanel");
        UIFlowManager.Instance.ShowCharacter();
    }

    private void OnPlayClicked()
    {
        GameModeSO modeToPlay = SceneLoader.Instance.SelectedMode != null
            ? SceneLoader.Instance.SelectedMode
            : defaultMode;

        if (modeToPlay == null)
        {
            Debug.LogError("[UIHomeScreen] Chưa có mode nào được chọn và cũng chưa gán defaultMode!");
            return;
        }

        Debug.Log($"[UIHomeScreen] Bắt đầu chơi với mode: {modeToPlay.displayName}");
        SceneLoader.Instance.LoadGameMode(modeToPlay);
    }
}
