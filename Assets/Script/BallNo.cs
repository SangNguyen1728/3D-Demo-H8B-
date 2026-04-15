using UnityEngine;

public class BallNo : MonoBehaviour
{
    public int ballNumber;     // 1 → 9
    public bool isCueBall;     // bi trắng

    public bool IsTargetBall(int target)
    {
        return ballNumber == target;
    }
    public bool IsCueBall()
    {
        return isCueBall;
    }
}
