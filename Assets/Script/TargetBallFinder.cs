using UnityEngine;

public class TargetBallFinder : MonoBehaviour
{
    public PocketTowPs pocketManager;

    public Transform GetTargetBallTransform()
    {
        if (pocketManager == null) return null;

        string targetTag = "BallNo." + pocketManager.targetBallNumber;
        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);

        return targetObj != null ? targetObj.transform : null;
    }
}
