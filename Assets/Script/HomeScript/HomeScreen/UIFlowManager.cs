using UnityEngine;

public class UIFlowManager : MonoBehaviour
{
    public static UIFlowManager Instance { get; private set; }

    [Header("Toàn bộ panel trong HomeScene")]
    public GameObject homePanel;
    public GameObject modePanel;
    public GameObject characterPanel;
    public GameObject skillPanel; // MỚI

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SaveManager.Instance.LoadSavedSelection();
        ShowHomeOnly();
    }

    public void ShowHomeOnly()
    {
        SetAllInactive();
        homePanel.SetActive(true);
    }

    public void ShowMode()
    {
        SetAllInactive();
        modePanel.SetActive(true);
    }

    public void ShowCharacter()
    {
        SetAllInactive();
        characterPanel.SetActive(true);
    }

    public void ShowSkill() // MỚI
    {
        SetAllInactive();
        skillPanel.SetActive(true);
    }

    private void SetAllInactive()
    {
        homePanel.SetActive(false);
        modePanel.SetActive(false);
        characterPanel.SetActive(false);
        skillPanel.SetActive(false); // MỚI
    }
}
