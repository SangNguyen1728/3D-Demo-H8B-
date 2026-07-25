using UnityEngine;

public class BallCollisionNotifier : MonoBehaviour
{
    private CueStickController cue;

    void Start()
    {
        cue = FindFirstObjectByType<CueStickController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        BallNo selfBall = GetComponent<BallNo>();

        // Chỉ xử lý nếu là bi trắng
        if (selfBall == null || !selfBall.isCueBall) return;

        if (cue != null)
        {
            cue.NotifyFirstCollision(collision.gameObject);
        }
    }
}
