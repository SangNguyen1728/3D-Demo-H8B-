using UnityEngine;

public class PocketDetector : MonoBehaviour
{
    private PocketTowPs masterLogic;

    void Start()
    {
        masterLogic = FindObjectOfType<PocketTowPs>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Logic gốc
        if (masterLogic != null)
            masterLogic.OnBallEnteredPocket(other);

        GameObject ball = other.gameObject;

        // Track cho BeelzitaManager
        if (BeelzitaManager.Instance != null)
            BeelzitaManager.Instance.RegisterPocketedBall(ball);

        // Notify behavior nếu có
        BeelzitaBallBehavior behavior =
            ball.GetComponent<BeelzitaBallBehavior>() ??
            ball.GetComponentInParent<BeelzitaBallBehavior>();

        if (behavior != null)
            behavior.OnEnteredPocket();
    }
}
