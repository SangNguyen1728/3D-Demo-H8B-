using UnityEngine;

public class BallNo : MonoBehaviour
{
    [Header("Ball Info")]
    public int ballNumber;

    public bool isCueBall;

    [HideInInspector]
    public bool isStripe;

    [HideInInspector]
    public bool isSolid;

    [HideInInspector]
    public bool isEightBall;

    public BallOwnership ownership;

    private void Awake()
    {
        isSolid =
            ballNumber >= 1 &&
            ballNumber <= 7;

        isStripe =
            ballNumber >= 9 &&
            ballNumber <= 15;

        isEightBall =
            ballNumber == 8;
    }

    public bool IsTargetBall(int target)
    {
        return ballNumber == target;
    }

    public bool IsCueBall()
    {
        return isCueBall;
    }
}
