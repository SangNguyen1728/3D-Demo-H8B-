using UnityEngine;
using System.Collections;

public class CueBallController : MonoBehaviour
{
    [Header("English (Spin) Settings")]
    public float ballRadius = 0.0285f;
    public float sensitivity = 0.5f;
    public Vector2 spinValues = Vector2.zero;

    [Header("Spin Physics — Tinh chỉnh độ chân thực")]
    [Tooltip("Lực ma sát chuyển spin thành chuyển động sau khi bi chạm bi khác")]
    public float spinToVelocityFactor = 2.5f;

    [Tooltip("Tốc độ giảm spin theo thời gian (giả lập ma sát bi-bàn)")]
    public float spinDecayRate = 0.8f;

    [Tooltip("Ngưỡng tốc độ để coi bi đã dừng — dưới ngưỡng này spin ngừng tác động")]
    public float minSpeedForSpinEffect = 0.05f;

    [Tooltip("Độ mạnh bẻ cong đường đi do side-spin")]
    public float curveStrength = 0.3f;

    private Rigidbody rb;
    private PocketTowPs pocketManager;
    private CueStickController stick;

    // Lưu lại spin lúc đánh để áp dụng sau va chạm
    private Vector2 storedSpinAtHit = Vector2.zero;
    private bool hasStoredSpin = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        pocketManager = Object.FindFirstObjectByType<PocketTowPs>();
        stick = Object.FindFirstObjectByType<CueStickController>();
    }

    void FixedUpdate()
    {
        ApplySpinPhysics();
    }

    // ================================
    // 🎯 ENGLISH INPUT
    // ================================
    public void UpdateEnglish(float x, float y)
    {
        spinValues += new Vector2(x, y) * sensitivity;
        LimitSpin();
    }

    public void SetEnglishExplicit(Vector2 input)
    {
        spinValues = input;
        LimitSpin();
    }

    private void LimitSpin()
    {
        if (spinValues.magnitude > 1f) spinValues = spinValues.normalized;
    }

    public Vector3 GetHitOffset(Transform pivot)
    {
        return (pivot.right * spinValues.x * ballRadius) + (pivot.up * spinValues.y * ballRadius);
    }

    public void ResetEnglish() => spinValues = Vector2.zero;

    // ================================
    // 🔥 LƯU SPIN TRƯỚC CÚ ĐÁNH
    // ================================
    // Gọi từ CueStickController.HitCueBall() trước khi AddForceAtPosition
    public void StoreSpinForShot()
    {
        storedSpinAtHit = spinValues;
        hasStoredSpin = storedSpinAtHit.magnitude > 0.01f;

        if (hasStoredSpin)
            Debug.Log($"[CueBallController] Lưu spin cho cú đánh này: {storedSpinAtHit}");
    }

    public void StopBall()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        hasStoredSpin = false;
        storedSpinAtHit = Vector2.zero;
    }

    // ================================
    // 💥 VA CHẠM
    // ================================
    private void OnCollisionEnter(Collision collision)
    {
        // Logic gốc — gửi thông tin va chạm cho CueStickController
        if (stick != null) stick.NotifyFirstCollision(collision.gameObject);

        // 🔥 Khi bi trắng chạm bi khác → áp dụng hiệu ứng spin (follow/draw)
        BallNo otherBall = collision.gameObject.GetComponent<BallNo>();
        if (otherBall != null && !otherBall.isCueBall && hasStoredSpin)
        {
            ApplySpinEffectOnCollision(collision);
        }
    }

    // ================================
    // 🌀 SPIN PHYSICS — TẠO HIỆU ỨNG CONG ĐƯỜNG ĐI
    // ================================
    private void ApplySpinPhysics()
    {
        if (rb == null || !hasStoredSpin) return;

        float speed = rb.linearVelocity.magnitude;

        // Bi đã dừng hoặc quá chậm → ngừng áp dụng spin
        if (speed < minSpeedForSpinEffect)
        {
            hasStoredSpin = false;
            return;
        }

        // Side spin (x) → tạo lực bẻ cong đường đi (giống Magnus effect đơn giản hóa)
        if (Mathf.Abs(storedSpinAtHit.x) > 0.05f)
        {
            Vector3 moveDir = rb.linearVelocity.normalized;
            Vector3 curveDir = Vector3.Cross(Vector3.up, moveDir);
            rb.AddForce(curveDir * storedSpinAtHit.x * curveStrength, ForceMode.Acceleration);
        }

        // Giảm dần spin theo thời gian (ma sát bi-bàn làm mất spin)
        storedSpinAtHit *= (1f - spinDecayRate * Time.fixedDeltaTime);
    }

    // ================================
    // 🎱 FOLLOW / DRAW SAU VA CHẠM
    // ================================
    private void ApplySpinEffectOnCollision(Collision collision)
    {
        if (rb == null) return;

        float topSpin = storedSpinAtHit.y; // y > 0 = top spin (follow), y < 0 = back spin (draw)

        if (Mathf.Abs(topSpin) < 0.05f) return;

        Vector3 contactNormal = collision.GetContact(0).normal;
        Vector3 forwardDir = -contactNormal; // hướng bi đang di chuyển tới khi chạm

        // Top spin: đẩy bi tiếp tục theo hướng cũ sau va chạm (follow)
        // Back spin: đẩy bi theo hướng ngược lại (draw)
        Vector3 spinForce = forwardDir * topSpin * spinToVelocityFactor;

        StartCoroutine(ApplyDelayedSpinForce(spinForce));

        Debug.Log($"[CueBallController] Áp dụng spin effect: topSpin={topSpin}, force={spinForce}");
    }

    private IEnumerator ApplyDelayedSpinForce(Vector3 force)
    {
        yield return new WaitForFixedUpdate();

        if (rb != null)
            rb.AddForce(force, ForceMode.Impulse);
    }

    //[Header("English (Spin) Settings")]
    //public float ballRadius = 0.0285f;
    //public float sensitivity = 0.5f;
    //public Vector2 spinValues = Vector2.zero; // x: Side, y: Top/Bottom

    //private PocketTowPs gameManager;
    //private Rigidbody rb;

    //void Start()
    //{
    //    gameManager = Object.FindFirstObjectByType<PocketTowPs>();
    //    rb = GetComponent<Rigidbody>();
    //}

    //// --- LOGIC ENGLISH (Gom từ CueBallEnglish) ---
    //public void UpdateEnglish(float mouseX, float mouseY)
    //{
    //    spinValues.x += mouseX * sensitivity;
    //    spinValues.y += mouseY * sensitivity;
    //    LimitSpin();
    //}

    //public void SetEnglishExplicit(Vector2 input)
    //{
    //    spinValues = input;
    //    LimitSpin();
    //}

    //private void LimitSpin() { if (spinValues.magnitude > 1f) spinValues = spinValues.normalized; }
    //public void ResetEnglish() { spinValues = Vector2.zero; }

    //public Vector3 GetHitOffset(Transform cuePivot)
    //{
    //    return (cuePivot.right * spinValues.x * ballRadius) + (cuePivot.up * spinValues.y * ballRadius);
    //}

    //// --- LOGIC VA CHẠM (Thay thế CueBallSensor) ---
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (gameManager == null) return;

    //    // Chỉ ghi nhận va chạm với bi mục tiêu (có Tag BallNo.X)
    //    string otherTag = collision.gameObject.tag;
    //    if (otherTag.StartsWith("BallNo."))
    //    {
    //        // Gửi thông tin bi chạm đầu tiên về GameManager
    //        int ballNum = GetBallNumber(otherTag);
    //        gameManager.NotifyFirstCollision(ballNum);
    //    }
    //}

    //private int GetBallNumber(string tag)
    //{
    //    if (tag == "BallNo.9") return 9;
    //    if (int.TryParse(tag.Replace("BallNo.", ""), out int n)) return n;
    //    return 0;
    //}

    //public void StopBall()
    //{
    //    rb.linearVelocity = Vector3.zero;
    //    rb.angularVelocity = Vector3.zero;
    //}
}
