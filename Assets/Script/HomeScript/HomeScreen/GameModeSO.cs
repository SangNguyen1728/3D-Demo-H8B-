using UnityEngine;

[CreateAssetMenu(fileName = "NewGameMode", menuName = "Billiards/Game Mode")]
public class GameModeSO : ScriptableObject
{
    [Header("Thông tin hiển thị")]
    public string displayName = "Classic";
    [TextArea] public string description = "Chế độ chơi bi-a truyền thống.";
    public Sprite icon;

    [Header("Scene liên kết")]
    public string sceneName = "PlayScene_Classic";

    [Header("Config gameplay (tuỳ chọn, mở rộng sau)")]
    public float turnTimeLimit = 30f;
    public bool allowSpinShot = true;
}
