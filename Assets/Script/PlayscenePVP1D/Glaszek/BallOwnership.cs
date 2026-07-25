using UnityEngine;

public class BallOwnership : MonoBehaviour
{

    public enum BallGroup
    {
        None,
        Solid,
        Stripe
    }

    public BallGroup currentGroup;

    [Header("Converted Ball")]
    public bool isConverted = false;

    [Header("Current Owner")]
    public int ownerPlayer = 0;

    [Header("Visual")]
    public Renderer targetRenderer;

    public Material solidMat;
    public Material stripeMat;

    private BallNo ballNo;

    private void Awake()
    {
        ballNo = GetComponent<BallNo>();

        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        InitializeFromBallNumber();
    }
    public void ConvertToPlayer(int player)
    {
        ownerPlayer = player;

        isConverted = true;

        Debug.Log(
            $"BALL {ballNo.ballNumber} CONVERTED TO PLAYER {player}"
        );
    }
    public void ResetOwnership()
    {
        ownerPlayer = 0;

        isConverted = false;
    }

    void InitializeFromBallNumber()
    {
        if (ballNo == null) return;

        int nr = ballNo.ballNumber;

        if (nr >= 1 && nr <= 7)
            currentGroup = BallGroup.Solid;
        else if (nr >= 9 && nr <= 15)
            currentGroup = BallGroup.Stripe;
        else
            currentGroup = BallGroup.None;

        UpdateVisual();
    }

    public void ConvertGroup()
    {
        if (currentGroup == BallGroup.Solid)
        {
            currentGroup = BallGroup.Stripe;
        }
        else if (currentGroup == BallGroup.Stripe)
        {
            currentGroup = BallGroup.Solid;
        }

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (targetRenderer == null)
            return;

        if (currentGroup == BallGroup.Solid)
        {
            targetRenderer.material = solidMat;
        }
        else if (currentGroup == BallGroup.Stripe)
        {
            targetRenderer.material = stripeMat;
        }
    }
}
