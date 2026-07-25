using UnityEngine;

public class TargetBallFinder : MonoBehaviour
{
    public PocketTowPs pocketManager;
    public Transform currentTarget;
    public Transform GetTargetBallTransform()
    {
        if (pocketManager == null) return null;

        string targetTag = "BallNo." + pocketManager.targetBallNumber;
        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);

        return targetObj != null ? targetObj.transform : null;
    }

    public void UpdateTargetBall()
    {
        BallNo[] balls = FindObjectsOfType<BallNo>();

        int minBall = int.MaxValue;
        Transform nextTarget = null;

        foreach (BallNo b in balls)
        {
            if (b.isCueBall) continue;
            if (!b.gameObject.activeInHierarchy) continue;

            if (b.ballNumber < minBall)
            {
                minBall = b.ballNumber;
                nextTarget = b.transform;
            }
        }

        currentTarget = nextTarget;

        Debug.Log("Target mới: " + minBall);
    }
}
