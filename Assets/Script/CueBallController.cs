using UnityEngine;

public class CueBallController : MonoBehaviour
{
    public float ballRadius = 0.0285f;
    public float sensitivity = 0.5f;
    public Vector2 spinValues = Vector2.zero;
    private Rigidbody rb;

    private PocketTowPs pocketManager;
    private CueStickController stick;

    void Start()
    {
        pocketManager = Object.FindFirstObjectByType<PocketTowPs>();
        stick = Object.FindFirstObjectByType<CueStickController>();
    }

    public void UpdateEnglish(float x, float y)
    {
        spinValues += new Vector2(x, y) * sensitivity;
        if (spinValues.magnitude > 1f) spinValues = spinValues.normalized;
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
    //public Vector3 GetHitOffset(Transform pivot) => (pivot.right * spinValues.x * ballRadius) + (pivot.up * spinValues.y * ballRadius);

    public void ResetEnglish() => spinValues = Vector2.zero;

    public void StopBall()
    {
        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Gửi thông tin va chạm về cho CueStickController xử lý
        if (stick != null) stick.NotifyFirstCollision(collision.gameObject);
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
