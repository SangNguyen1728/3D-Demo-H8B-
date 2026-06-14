using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;


public class BallHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 1000f;
    private float currentHealth;

    [Header("UI")]
    public Image healthFill; // thanh chính

    [Header("Animation")]
    public float smoothSpeed = 10f;

    private float targetFill = 1f;
    private float currentFill = 1f;

    [Header("Low HP Blink")]
    public float lowHpThreshold = 0.25f;
    public float blinkSpeed = 5f;

    private bool isBlinking = false;
    private bool isDead = false;

    public bool isImmune = false;
    private int immuneTurns = 0;

    private BallNo ballNo;

    void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;
        currentFill = 1f;
        ballNo = GetComponent<BallNo>();
    }

    void Update()
    {
        

        // smooth fill
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
        healthFill.fillAmount = currentFill;

        // 🔥 BLINK khi low HP
        float percent = currentHealth / maxHealth;

        if (percent <= lowHpThreshold && currentHealth > 0)
        {
            if (!isBlinking)
                isBlinking = true;

            float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            Color c = healthFill.color;
            c.a = alpha;
            healthFill.color = c;
        }
        else
        {
            if (isBlinking)
            {
                isBlinking = false;

                Color c = healthFill.color;
                c.a = 1f;
                healthFill.color = c;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (isDead)
            return;

        // BLOCK BI 8
        if (ballNo != null && ballNo.isEightBall)
        {
            PocketTowPs pocket =
                FindFirstObjectByType<PocketTowPs>();

            if (pocket != null)
            {
                bool cleared =
                    pocket.HasClearedCurrentPlayerGroup();

                if (!cleared)
                {
                    Debug.Log(
                        "<color=red>BI 8 ĐANG KHÓA DAMAGE</color>");

                    return;
                }
            }
        }

        if (isImmune)
        {
            Debug.Log(gameObject.name + " miễn nhiễm damage!");
            return;
        }

        currentHealth -= damage;

        currentHealth =
            Mathf.Clamp(currentHealth, 0, maxHealth);

        float percent =
            currentHealth / maxHealth;

        targetFill = percent;

        UpdateColor(percent);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateColor(float percent)
    {
        Color color;

        if (percent > 0.75f)
            color = Color.green;
        else if (percent > 0.5f)
            color = Color.yellow;
        else if (percent > 0.25f)
            color = new Color(1f, 0.5f, 0f);
        else
            color = Color.red;

        // Glow nhẹ
        healthFill.color = color * 1.8f;
    }

    IEnumerator Flash()
    {
        Color original = healthFill.color;

        healthFill.color = Color.white;

        yield return new WaitForSeconds(0.08f);

        healthFill.color = original;
    }

    void Die()
    {
        

        if (isDead) return;

        isDead = true;

        Debug.Log(gameObject.name + " chết!");

        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Chờ damage chain settle
        yield return new WaitForSeconds(0.25f);

        PocketTowPs pocket =
            FindFirstObjectByType<PocketTowPs>();

        if (pocket != null)
        {
            pocket.OnBallDestroyed(gameObject);
        }

        yield return StartCoroutine(ExplodeRoutine());
    }
    public void Explode()
    {
        if (!isDead) return;

        Debug.Log(gameObject.name + " nổ!");

        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        float t = 0f;

        Vector3 startScale = transform.localScale;

        while (t < 1f)
        {
            transform.localScale =
                Vector3.Lerp(startScale, Vector3.zero, t);

            t += Time.deltaTime * 4f;

            yield return null;
        }

        transform.localScale = Vector3.zero;

        gameObject.SetActive(false);
    }

    // 🔥 KÍCH HOẠT IMMUNE
    public void ActivateImmunity(int turns)
    {
        isImmune = true;
        immuneTurns = turns;

        Debug.Log(gameObject.name + " được miễn damage trong " + turns + " lượt");

        // 👉 đổi màu để nhận biết
        if (healthFill != null)
        {
            healthFill.color = Color.cyan;
        }
    }

    // 🔥 GIẢM TURN
    public void ReduceTurn()
    {
        if (!isImmune) return;

        immuneTurns--;

        Debug.Log(gameObject.name + " còn " + immuneTurns + " lượt miễn damage");

        if (immuneTurns <= 0)
        {
            isImmune = false;

            Debug.Log(gameObject.name + " hết miễn nhiễm");

            // reset màu
            UpdateColor(currentHealth / maxHealth);
        }
    }

    public bool IsImmune()
    {
        return isImmune;
    }

    public void SetHealthDirect(float value)
    {
        currentHealth = Mathf.Clamp(value, 0, maxHealth);
        targetFill = currentHealth / maxHealth;
        UpdateColor(targetFill);
    }

    // Expose currentHealth để HoleLogic đọc
    public float GetCurrentHealth() => currentHealth;

    private Coroutine blinkRoutine;

    // Thêm method public
    public void BlinkOnRespawn(float duration = 3f)
    {
        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);
        blinkRoutine = StartCoroutine(BlinkRoutine(duration));
    }

    private IEnumerator BlinkRoutine(float duration)
    {
        float elapsed = 0f;
        float blinkInterval = 0.15f;
        bool visible = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        while (elapsed < duration)
        {
            visible = !visible;

            foreach (var r in renderers)
                r.enabled = visible;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // Đảm bảo hiện lại sau khi hết nhấp nháy
        foreach (var r in renderers)
            r.enabled = true;

        blinkRoutine = null;
    }

}
