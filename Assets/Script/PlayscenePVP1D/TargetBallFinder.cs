using UnityEngine;

public class TargetBallFinder : MonoBehaviour
{
    //public PocketTowPs pocketManager;
    //public Transform currentTarget;
    //public Transform GetTargetBallTransform()
    //{
    //    if (pocketManager == null) return null;

    //    string targetTag = "BallNo." + pocketManager.targetBallNumber;
    //    GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);

    //    return targetObj != null ? targetObj.transform : null;
    //}

    //public void UpdateTargetBall()
    //{
    //    BallNo[] balls = FindObjectsOfType<BallNo>();

    //    int minBall = int.MaxValue;
    //    Transform nextTarget = null;

    //    foreach (BallNo b in balls)
    //    {
    //        if (b.isCueBall) continue;
    //        if (!b.gameObject.activeInHierarchy) continue;

    //        if (b.ballNumber < minBall)
    //        {
    //            minBall = b.ballNumber;
    //            nextTarget = b.transform;
    //        }
    //    }

    //    currentTarget = nextTarget;

    //    Debug.Log("Target mới: " + minBall);
    //}

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
        if (pocketManager == null)
        {
            Debug.LogWarning("[TargetBallFinder] Thiếu PocketTowPs!");
            return;
        }

        // ĐỔI — không tự quét bi nhỏ nhất nữa, dùng đúng targetBallNumber
        // mà PocketTowPs đã tính (bao gồm cả trường hợp target = bi 8 khi đã clear nhóm)
        currentTarget = GetTargetBallTransform();

        Debug.Log("Target mới: " + pocketManager.targetBallNumber);
    }
}
