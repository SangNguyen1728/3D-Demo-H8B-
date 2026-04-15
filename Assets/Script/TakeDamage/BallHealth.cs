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

    void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;
        currentFill = 1f;
    }

    void Update()
    {
        // Làm mượt thanh máu
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
        healthFill.fillAmount = currentFill;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(100);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float percent = currentHealth / maxHealth;
        targetFill = percent;

        UpdateColor(percent);
        StartCoroutine(Flash()); // hiệu ứng hit

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
        Debug.Log(gameObject.name + " đã hết máu!");

        // 1. Tắt collider (không va chạm nữa)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 2. Tắt rigidbody (dừng vật lý)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        // 3. Ẩn object
        gameObject.SetActive(false);
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
