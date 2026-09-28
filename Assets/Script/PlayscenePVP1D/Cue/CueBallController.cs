using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private Collider myCollider;
    private PocketTowPs pocketManager;
    private CueStickController stick;

    // Lưu lại spin lúc đánh để áp dụng sau va chạm
    private Vector2 storedSpinAtHit = Vector2.zero;
    private bool hasStoredSpin = false;

    [Header("Ball Scale Effect — Phóng to / Thu nhỏ khi lăn")]
    [Tooltip("Object con chứa MeshRenderer (KHÔNG kéo object gốc có Collider vào đây!)")]
    public Transform visualTransform;

    [Tooltip("Config quyết định loại bi này là bình thường / to / nhỏ / xuyên bi / hồi vị")]
    public BallScaleConfig scaleConfig;

    private float currentScaleMultiplier = 1f;
    private float targetScaleMultiplier = 1f;
    private bool wasRolling = false;

    // 🆕 Vị trí bi cái lúc bắt đầu cú đánh — dùng cho tính năng Hồi Vị
    private Vector3 shotStartPosition;

    // 🆕 Danh sách Collider đang bị tắt va chạm tạm thời — dùng cho tính năng Xuyên Bi
    private List<Collider> ignoredCollidersThisShot = new List<Collider>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();

        pocketManager = Object.FindFirstObjectByType<PocketTowPs>();
        stick = Object.FindFirstObjectByType<CueStickController>();
    }

    void FixedUpdate()
    {
        ApplySpinPhysics();
        HandleBallScaleEffect();
    }

    // ================================
    // 📏 SCALE EFFECT KHI LĂN / DỪNG
    // ================================
    private void HandleBallScaleEffect()
    {
        if (rb == null || visualTransform == null || scaleConfig == null) return;

        if (scaleConfig.mode == BallScaleMode.None) return;

        float speed = rb.linearVelocity.magnitude;
        bool isRolling = speed > scaleConfig.rollingSpeedThreshold;

        if (isRolling && !wasRolling)
        {
            targetScaleMultiplier = scaleConfig.mode == BallScaleMode.AlwaysGrow
                ? (1f + scaleConfig.scaleChangeAmount)
                : (1f - scaleConfig.scaleChangeAmount);

            Debug.Log($"[CueBallController] Bắt đầu lăn -> Mode: {scaleConfig.mode} x{targetScaleMultiplier}");
        }
        else if (!isRolling && wasRolling)
        {
            targetScaleMultiplier = 1f;

            Debug.Log("[CueBallController] Bi dừng -> Trả về scale gốc");
        }

        wasRolling = isRolling;

        currentScaleMultiplier = Mathf.Lerp(
            currentScaleMultiplier,
            targetScaleMultiplier,
            Time.fixedDeltaTime * scaleConfig.scaleLerpSpeed
        );

        visualTransform.localScale = Vector3.one * currentScaleMultiplier;
        visualTransform.rotation = Quaternion.identity;
        visualTransform.position = transform.position + Vector3.up * (ballRadius * (currentScaleMultiplier - 1f));
    }

    // Gọi hàm này để đổi loại bi đang dùng (từ BallInventoryManager)
    public void ApplyScaleConfig(BallScaleConfig newConfig)
    {
        scaleConfig = newConfig;

        currentScaleMultiplier = 1f;
        targetScaleMultiplier = 1f;
        wasRolling = false;

        if (visualTransform != null)
        {
            visualTransform.localScale = Vector3.one;
            visualTransform.position = transform.position;
            visualTransform.rotation = Quaternion.identity;
        }

        Debug.Log($"[CueBallController] Áp dụng config mới: {(newConfig != null ? newConfig.mode.ToString() : "null")}");
    }

    // ================================
    // 👻 XUYÊN BI
    // ================================
    private void TryStartPhaseThrough()
    {
        if (scaleConfig == null || !scaleConfig.enablePhaseThrough) return;
        if (myCollider == null || stick == null) return;

        StopAllCoroutines(); // tránh chồng 2 coroutine xuyên bi nếu lỡ đánh liên tiếp quá nhanh
        StartCoroutine(PhaseThroughRoutine(scaleConfig.phaseThroughDuration));
    }

    private IEnumerator PhaseThroughRoutine(float duration)
    {
        ignoredCollidersThisShot.Clear();

        foreach (Rigidbody ballRb in stick.balls)
        {
            if (ballRb == null) continue;

            Collider col = ballRb.GetComponent<Collider>();
            if (col == null) continue;

            Physics.IgnoreCollision(myCollider, col, true);
            ignoredCollidersThisShot.Add(col);
        }

        Debug.Log($"[CueBallController] Bắt đầu XUYÊN BI trong {duration}s");

        yield return new WaitForSeconds(duration);

        foreach (Collider col in ignoredCollidersThisShot)
        {
            if (col != null)
                Physics.IgnoreCollision(myCollider, col, false);
        }

        ignoredCollidersThisShot.Clear();

        Debug.Log("[CueBallController] Hết XUYÊN BI -> va chạm bình thường trở lại");
    }

    // ================================
    // 🔄 HỒI VỊ SAU CÚ ĐÁNH
    // ================================
    // Gọi từ CueStickController ngay sau khi 1 cú đánh xử lý xong hoàn toàn
    public void TryReturnToShotPosition()
    {
        if (scaleConfig == null || !scaleConfig.enableReturnToShotPosition) return;

        Collider[] nearbyColliders = Physics.OverlapSphere(shotStartPosition, scaleConfig.minSafeDistanceFromOtherBalls);

        foreach (Collider col in nearbyColliders)
        {
            BallNo ball = col.GetComponent<BallNo>();

            if (ball != null && !ball.isCueBall)
            {
                Debug.Log("[CueBallController] Có bi mục tiêu quá gần vị trí cũ -> HỦY hồi vị");
                return;
            }
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = shotStartPosition;

        Debug.Log("[CueBallController] Đã HỒI VỊ bi trắng về vị trí đánh trước đó");
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
    public void StoreSpinForShot()
    {
        storedSpinAtHit = spinValues;
        hasStoredSpin = storedSpinAtHit.magnitude > 0.01f;

        if (hasStoredSpin)
            Debug.Log($"[CueBallController] Lưu spin cho cú đánh này: {storedSpinAtHit}");

        // 🆕 Lưu vị trí xuất phát cú đánh (dùng cho Hồi Vị)
        shotStartPosition = transform.position;

        // 🆕 Kích hoạt Xuyên Bi nếu config đang bật
        TryStartPhaseThrough();
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
        if (stick != null) stick.NotifyFirstCollision(collision.gameObject);

        BallNo otherBall = collision.gameObject.GetComponent<BallNo>();
        if (otherBall != null && !otherBall.isCueBall && hasStoredSpin)
        {
            ApplySpinEffectOnCollision(collision);
        }
    }

    // ================================
    // 🌀 SPIN PHYSICS
    // ================================
    private void ApplySpinPhysics()
    {
        if (rb == null || !hasStoredSpin) return;

        float speed = rb.linearVelocity.magnitude;

        if (speed < minSpeedForSpinEffect)
        {
            hasStoredSpin = false;
            return;
        }

        if (Mathf.Abs(storedSpinAtHit.x) > 0.05f)
        {
            Vector3 moveDir = rb.linearVelocity.normalized;
            Vector3 curveDir = Vector3.Cross(Vector3.up, moveDir);
            rb.AddForce(curveDir * storedSpinAtHit.x * curveStrength, ForceMode.Acceleration);
        }

        storedSpinAtHit *= (1f - spinDecayRate * Time.fixedDeltaTime);
    }

    // ================================
    // 🎱 FOLLOW / DRAW SAU VA CHẠM
    // ================================
    private void ApplySpinEffectOnCollision(Collision collision)
    {
        if (rb == null) return;

        float topSpin = storedSpinAtHit.y;

        if (Mathf.Abs(topSpin) < 0.05f) return;

        Vector3 contactNormal = collision.GetContact(0).normal;
        Vector3 forwardDir = -contactNormal;

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
    //public Vector2 spinValues = Vector2.zero;

    //[Header("Spin Physics — Tinh chỉnh độ chân thực")]
    //[Tooltip("Lực ma sát chuyển spin thành chuyển động sau khi bi chạm bi khác")]
    //public float spinToVelocityFactor = 2.5f;

    //[Tooltip("Tốc độ giảm spin theo thời gian (giả lập ma sát bi-bàn)")]
    //public float spinDecayRate = 0.8f;

    //[Tooltip("Ngưỡng tốc độ để coi bi đã dừng — dưới ngưỡng này spin ngừng tác động")]
    //public float minSpeedForSpinEffect = 0.05f;

    //[Tooltip("Độ mạnh bẻ cong đường đi do side-spin")]
    //public float curveStrength = 0.3f;

    //private Rigidbody rb;
    //private PocketTowPs pocketManager;
    //private CueStickController stick;

    //// Lưu lại spin lúc đánh để áp dụng sau va chạm
    //private Vector2 storedSpinAtHit = Vector2.zero;
    //private bool hasStoredSpin = false;

    //[Header("Ball Scale Effect — Phóng to / Thu nhỏ khi lăn")]
    //[Tooltip("Object con chứa MeshRenderer (KHÔNG kéo object gốc có Collider vào đây!)")]
    //public Transform visualTransform;

    //[Tooltip("Config quyết định loại bi này là bình thường / luôn to / luôn nhỏ")]
    //public BallScaleConfig scaleConfig;

    //private float currentScaleMultiplier = 1f;
    //private float targetScaleMultiplier = 1f;
    //private bool wasRolling = false;

    //void Start()
    //{
    //    rb = GetComponent<Rigidbody>();

    //    pocketManager = Object.FindFirstObjectByType<PocketTowPs>();
    //    stick = Object.FindFirstObjectByType<CueStickController>();
    //}

    //void FixedUpdate()
    //{
    //    ApplySpinPhysics();
    //    HandleBallScaleEffect();
    //}

    //// ================================
    //// 📏 SCALE EFFECT KHI LĂN / DỪNG
    //// ================================
    //private void HandleBallScaleEffect()
    //{
    //    if (rb == null || visualTransform == null || scaleConfig == null) return;

    //    // Bi loại "None" -> không cần làm gì, giữ nguyên scale gốc luôn
    //    if (scaleConfig.mode == BallScaleMode.None) return;

    //    float speed = rb.linearVelocity.magnitude;
    //    bool isRolling = speed > scaleConfig.rollingSpeedThreshold;

    //    // Vừa bắt đầu lăn -> chọn phóng to hay thu nhỏ theo config
    //    if (isRolling && !wasRolling)
    //    {
    //        targetScaleMultiplier = scaleConfig.mode == BallScaleMode.AlwaysGrow
    //            ? (1f + scaleConfig.scaleChangeAmount)
    //            : (1f - scaleConfig.scaleChangeAmount);

    //        Debug.Log($"[CueBallController] Bắt đầu lăn -> Mode: {scaleConfig.mode} x{targetScaleMultiplier}");
    //    }
    //    // Vừa dừng lại -> trở về scale gốc
    //    else if (!isRolling && wasRolling)
    //    {
    //        targetScaleMultiplier = 1f;

    //        Debug.Log("[CueBallController] Bi dừng -> Trả về scale gốc");
    //    }

    //    wasRolling = isRolling;

    //    // Lerp mượt hệ số scale
    //    currentScaleMultiplier = Mathf.Lerp(
    //        currentScaleMultiplier,
    //        targetScaleMultiplier,
    //        Time.fixedDeltaTime * scaleConfig.scaleLerpSpeed
    //    );

    //    // Scale visual (không đụng Collider)
    //    visualTransform.localScale = Vector3.one * currentScaleMultiplier;

    //    // Khoá rotation của Visual về mặc định, tránh xoay lung tung theo vật lý lăn của bi
    //    visualTransform.rotation = Quaternion.identity;

    //    // Bù trừ vị trí theo WORLD SPACE — đáy bi luôn dính đúng mặt bàn dù to hay nhỏ
    //    visualTransform.position = transform.position + Vector3.up * (ballRadius * (currentScaleMultiplier - 1f));
    //}

    //// Gọi hàm này để đổi loại bi đang dùng (từ BallInventoryManager)
    //public void ApplyScaleConfig(BallScaleConfig newConfig)
    //{
    //    scaleConfig = newConfig;

    //    // Reset trạng thái scale hiện tại về gốc khi đổi bi, tránh giữ scale cũ
    //    currentScaleMultiplier = 1f;
    //    targetScaleMultiplier = 1f;
    //    wasRolling = false;

    //    if (visualTransform != null)
    //    {
    //        visualTransform.localScale = Vector3.one;
    //        visualTransform.position = transform.position;
    //        visualTransform.rotation = Quaternion.identity;
    //    }

    //    Debug.Log($"[CueBallController] Áp dụng config mới: {(newConfig != null ? newConfig.mode.ToString() : "null")}");
    //}

    //// ================================
    //// 🎯 ENGLISH INPUT
    //// ================================
    //public void UpdateEnglish(float x, float y)
    //{
    //    spinValues += new Vector2(x, y) * sensitivity;
    //    LimitSpin();
    //}

    //public void SetEnglishExplicit(Vector2 input)
    //{
    //    spinValues = input;
    //    LimitSpin();
    //}

    //private void LimitSpin()
    //{
    //    if (spinValues.magnitude > 1f) spinValues = spinValues.normalized;
    //}

    //public Vector3 GetHitOffset(Transform pivot)
    //{
    //    return (pivot.right * spinValues.x * ballRadius) + (pivot.up * spinValues.y * ballRadius);
    //}

    //public void ResetEnglish() => spinValues = Vector2.zero;

    //// ================================
    //// 🔥 LƯU SPIN TRƯỚC CÚ ĐÁNH
    //// ================================
    //public void StoreSpinForShot()
    //{
    //    storedSpinAtHit = spinValues;
    //    hasStoredSpin = storedSpinAtHit.magnitude > 0.01f;

    //    if (hasStoredSpin)
    //        Debug.Log($"[CueBallController] Lưu spin cho cú đánh này: {storedSpinAtHit}");
    //}

    //public void StopBall()
    //{
    //    if (rb != null)
    //    {
    //        rb.linearVelocity = Vector3.zero;
    //        rb.angularVelocity = Vector3.zero;
    //    }
    //    hasStoredSpin = false;
    //    storedSpinAtHit = Vector2.zero;
    //}

    //// ================================
    //// 💥 VA CHẠM
    //// ================================
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (stick != null) stick.NotifyFirstCollision(collision.gameObject);

    //    BallNo otherBall = collision.gameObject.GetComponent<BallNo>();
    //    if (otherBall != null && !otherBall.isCueBall && hasStoredSpin)
    //    {
    //        ApplySpinEffectOnCollision(collision);
    //    }
    //}

    //// ================================
    //// 🌀 SPIN PHYSICS
    //// ================================
    //private void ApplySpinPhysics()
    //{
    //    if (rb == null || !hasStoredSpin) return;

    //    float speed = rb.linearVelocity.magnitude;

    //    if (speed < minSpeedForSpinEffect)
    //    {
    //        hasStoredSpin = false;
    //        return;
    //    }

    //    if (Mathf.Abs(storedSpinAtHit.x) > 0.05f)
    //    {
    //        Vector3 moveDir = rb.linearVelocity.normalized;
    //        Vector3 curveDir = Vector3.Cross(Vector3.up, moveDir);
    //        rb.AddForce(curveDir * storedSpinAtHit.x * curveStrength, ForceMode.Acceleration);
    //    }

    //    storedSpinAtHit *= (1f - spinDecayRate * Time.fixedDeltaTime);
    //}

    //// ================================
    //// 🎱 FOLLOW / DRAW SAU VA CHẠM
    //// ================================
    //private void ApplySpinEffectOnCollision(Collision collision)
    //{
    //    if (rb == null) return;

    //    float topSpin = storedSpinAtHit.y;

    //    if (Mathf.Abs(topSpin) < 0.05f) return;

    //    Vector3 contactNormal = collision.GetContact(0).normal;
    //    Vector3 forwardDir = -contactNormal;

    //    Vector3 spinForce = forwardDir * topSpin * spinToVelocityFactor;

    //    StartCoroutine(ApplyDelayedSpinForce(spinForce));

    //    Debug.Log($"[CueBallController] Áp dụng spin effect: topSpin={topSpin}, force={spinForce}");
    //}

    //private IEnumerator ApplyDelayedSpinForce(Vector3 force)
    //{
    //    yield return new WaitForFixedUpdate();

    //    if (rb != null)
    //        rb.AddForce(force, ForceMode.Impulse);
    //}
}
