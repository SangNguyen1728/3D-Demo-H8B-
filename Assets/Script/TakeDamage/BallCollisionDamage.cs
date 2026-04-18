using UnityEngine;

public class BallCollisionDamage : MonoBehaviour
{
    private BallHealth health;
    private BallNo ballNo;

    [Header("Damage Settings")]
    public Vector2 ballCollisionRange = new Vector2(30f, 50f);
    public Vector2 wrongBallRange = new Vector2(50f, 70f);
    public float correctHitDamage = 100f;

    [Header("Optimization")]
    public float hitCooldown = 0.1f;
    private float lastHitTime = 0f;

    private PocketTowPs pocketManager;

    void Start()
    {
        health = GetComponent<BallHealth>();
        ballNo = GetComponent<BallNo>();

        pocketManager = FindFirstObjectByType<PocketTowPs>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ballNo.ballNumber == 9 && !IsAllOtherBallsCleared())
        {
            return;
        }

        if (health == null) return;

        // 🚫 chống spam damage
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;

        BallNo otherBall = collision.gameObject.GetComponent<BallNo>();

        // ========================
        // 🎱 VA CHẠM BI - BI
        // ========================
        if (otherBall != null)
        {
            // ⚪ BI TRẮNG ĐẬP
            if (otherBall.isCueBall && !ballNo.isCueBall)
            {
                HandleCueBallHit();
                return;
            }

            // 🎱 BI MÀU VA NHAU
            if (!ballNo.isCueBall && !otherBall.isCueBall)
            {
                float damage = Random.Range(ballCollisionRange.x, ballCollisionRange.y);
                health.TakeDamage(damage);
                return;
            }
        }

        // ========================
        // 🟫 VA CHẠM BĂNG
        // ========================
        if (collision.gameObject.CompareTag("Table"))
        {
            if (!ballNo.isCueBall)
            {
                float damage = Random.Range(ballCollisionRange.x, ballCollisionRange.y);
                health.TakeDamage(damage);
            }
        }

       
    }

    private bool IsAllOtherBallsCleared()
    {
        BallNo[] allBalls = FindObjectsOfType<BallNo>();

        foreach (BallNo b in allBalls)
        {
            // bỏ qua bi trắng và bi 9
            if (b.isCueBall || b.ballNumber == 9) continue;

            // nếu còn bi đang active → chưa clear
            if (b.gameObject.activeInHierarchy)
                return false;
        }

        return true;
    }
    void HandleCueBallHit()
    {
        if (ballNo.ballNumber == 9 && !IsAllOtherBallsCleared())
        {
            Debug.Log("Chưa được đánh bi 9!");
            return;
        }

        if (pocketManager == null) return;

        // ❗ CHẶN DAMAGE BI 9
        if (ballNo.ballNumber == 9 && !IsAllOtherBallsCleared())
        {
            Debug.Log("Chưa được phép đánh bi 9!");
            return;
        }

        int target = pocketManager.targetBallNumber;

        if (ballNo.ballNumber == target)
        {
            health.TakeDamage(correctHitDamage);
        }
        else
        {
            float damage = Random.Range(wrongBallRange.x, wrongBallRange.y);
            health.TakeDamage(damage);
        }

        //if (pocketManager == null) return;

        //int target = pocketManager.targetBallNumber;

        //// 🎯 đúng bi mục tiêu
        //if (ballNo.ballNumber == target)
        //{
        //    Debug.Log("Hit đúng bi mục tiêu: -" + correctHitDamage);
        //    health.TakeDamage(correctHitDamage);
        //}
        //else
        //{
        //    float damage = Random.Range(wrongBallRange.x, wrongBallRange.y);
        //    Debug.Log("Hit sai bi: -" + damage);
        //    health.TakeDamage(damage);
        //}
    }

    //public float damageMultiplier = 0.05f;

    //private IDamageable damageable;

    //void Start()
    //{
    //    damageable = GetComponent<IDamageable>();
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    float impulse = collision.impulse.magnitude;

    //    float damage = impulse * damageMultiplier;

    //    // giảm damage khi va vào tường
    //    if (collision.gameObject.CompareTag("Wall"))
    //    {
    //        damage *= 0.5f;
    //    }

    //    // tính góc va chạm (realistic hơn)
    //    if (collision.contactCount > 0)
    //    {
    //        ContactPoint contact = collision.contacts[0];
    //        Vector3 normal = contact.normal;

    //        float angle = Vector3.Dot(
    //            -collision.relativeVelocity.normalized,
    //            normal
    //        );

    //        damage *= Mathf.Clamp01(angle);
    //    }

    //    damageable?.TakeDamage(damage);
    //}
}
