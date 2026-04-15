using UnityEngine;

public class BallCollisionDamage : MonoBehaviour
{
    public float damageMultiplier = 0.02f;
    public float minDamageThreshold = 0.2f;

    private BallHealth health;

    void Start()
    {
        health = GetComponent<BallHealth>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (health == null) return;

        // Lấy lực va chạm thực tế
        float impulse = collision.impulse.magnitude;

        // bỏ qua va chạm quá nhẹ
        if (impulse < minDamageThreshold) return;

        float damage = impulse * damageMultiplier;

        // giảm damage nếu va tường
        if (collision.gameObject.CompareTag("Wall"))
        {
            damage *= 0.5f;
        }

        // tính góc va chạm (realistic hơn)
        if (collision.contactCount > 0)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;

            float angle = Vector3.Dot(
                -collision.relativeVelocity.normalized,
                normal
            );

            damage *= Mathf.Clamp01(angle);
        }

        health.TakeDamage(damage);
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
