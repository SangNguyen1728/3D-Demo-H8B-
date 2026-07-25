using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    private float currentDEF = 0f;
    private int defBuffTurnsLeft = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddDefBuff(float amount, int turns)
    {
        currentDEF += amount;
        defBuffTurnsLeft = Mathf.Max(defBuffTurnsLeft, turns);
        Debug.Log($"[Buff] DEF +{amount}, còn {defBuffTurnsLeft} gậy. Tổng DEF: {currentDEF}");
        // TODO: cập nhật UI
    }

    public float GetCurrentDEF() => currentDEF;
    public int GetDefTurnsLeft() => defBuffTurnsLeft;

    // Gọi mỗi khi lượt kết thúc (trong GlaszekManager.NotifyBallStopped)
    public void TickTurn()
    {
        if (defBuffTurnsLeft <= 0) return;

        defBuffTurnsLeft--;
        Debug.Log($"[Buff] DEF còn {defBuffTurnsLeft} gậy");

        if (defBuffTurnsLeft <= 0)
        {
            currentDEF = 0f;
            Debug.Log("[Buff] DEF buff hết hạn");
        }
    }
}
