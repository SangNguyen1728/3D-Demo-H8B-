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

    void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;
        currentFill = 1f;
    }

    void Update()
    {
        //// Làm mượt thanh máu
        //currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
        //healthFill.fillAmount = currentFill;

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TakeDamage(100);
        //}

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
        //currentHealth -= damage;
        //currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //float percent = currentHealth / maxHealth;
        //targetFill = percent;

        //UpdateColor(percent);
        //StartCoroutine(Flash()); // hiệu ứng hit

        //if (currentHealth <= 0)
        //{
        //    Die();
        //}

        if (isImmune)
        {
            Debug.Log(gameObject.name + " miễn nhiễm damage!");
            return;
        }

        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float percent = currentHealth / maxHealth;
        targetFill = percent;

        UpdateColor(percent);

        if (currentHealth <= 0)
        {
            //isDead = true; // ❗ KHÔNG DIE NGAY

            if (isDead) return;

            isDead = true;

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
        ////Debug.Log(gameObject.name + " đã hết máu!");

        ////// 1. Tắt collider (không va chạm nữa)
        ////Collider col = GetComponent<Collider>();
        ////if (col != null) col.enabled = false;

        ////// 2. Tắt rigidbody (dừng vật lý)
        ////Rigidbody rb = GetComponent<Rigidbody>();
        ////if (rb != null) rb.linearVelocity = Vector3.zero;

        ////// 3. Ẩn object
        ////gameObject.SetActive(false);

        //Debug.Log(gameObject.name + " chết!");

        //// 🔥 CHỈ disable, KHÔNG xử lý target ở đây
        //Collider col = GetComponent<Collider>();
        //if (col != null) col.enabled = false;

        //Rigidbody rb = GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.linearVelocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}

        //gameObject.SetActive(false);

        //Debug.Log(gameObject.name + " chết!");

        //// báo cho PocketManager
        //PocketTowPs pocket = FindFirstObjectByType<PocketTowPs>();

        //if (pocket != null)
        //{
        //    pocket.OnBallDestroyed(gameObject);
        //}

        //// disable collider
        //Collider col = GetComponent<Collider>();

        //if (col != null)
        //    col.enabled = false;

        //// stop rigidbody
        //Rigidbody rb = GetComponent<Rigidbody>();

        //if (rb != null)
        //{
        //    rb.linearVelocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;

        //    // 🔥 QUAN TRỌNG
        //    rb.isKinematic = true;
        //}

        //// 🔥 KHÔNG setActive false ở đây nữa
        //// gameObject.SetActive(false);

        //// 🔥 GỌI explode luôn
        //StartCoroutine(ExplodeRoutine());

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
        //// scale nhỏ dần
        //float t = 0;
        //Vector3 startScale = transform.localScale;

        //while (t < 1f)
        //{
        //    transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
        //    t += Time.deltaTime * 4f;
        //    yield return null;
        //}

        //gameObject.SetActive(false);

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

    //[Header("Health")]
    //public float maxHealth = 100f;
    //private float currentHealth;

    //[Header("UI")]
    //public Image healthFill;

    //[Header("Animation")]
    //public float smoothSpeed = 8f;

    //private float targetFill = 1f;
    //private float currentFill = 1f;

    //void Start()
    //{
    //    currentHealth = maxHealth;
    //    currentFill = 1f;
    //    targetFill = 1f;

    //    UpdateColor(1f);
    //}



    //void Update()
    //{
    //    // Smooth animation
    //    currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);

    //    if (healthFill != null)
    //    {
    //        healthFill.fillAmount = currentFill;
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        TakeDamage(100);
    //    }
    //}

    //public void TakeDamage(float damage)
    //{
    //    currentHealth -= damage;
    //    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    //    float percent = currentHealth / maxHealth;
    //    targetFill = percent;

    //    UpdateColor(percent);

    //    if (currentHealth <= 0)
    //    {
    //        Die();
    //    }
    //}

    //void UpdateColor(float percent)
    //{
    //    if (healthFill == null) return;

    //    Color color;

    //    if (percent > 0.75f)
    //        color = Color.green;
    //    else if (percent > 0.5f)
    //        color = Color.yellow;
    //    else if (percent > 0.25f)
    //        color = new Color(1f, 0.5f, 0f);
    //    else
    //        color = Color.red;

    //    // t?ng sáng (gi? glow)
    //    healthFill.color = color * 2f;
    //}

    //void Die()
    //{
    //    Destroy(gameObject);
    //}
}
