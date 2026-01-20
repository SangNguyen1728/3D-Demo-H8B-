using UnityEngine;

public class CueBallEnglish : MonoBehaviour
{
    [Header("Settings")]
    public float ballRadius = 0.0285f;
    public float sensitivity = 0.5f;

    [Header("Current State")]
    public Vector2 spinValues = Vector2.zero; // x: Side spin, y: Top/Bottom spin

    // Dùng cho Chuột (Cộng dồn Delta)
    public void UpdateEnglish(float mouseX, float mouseY)
    {
        spinValues.x += mouseX * sensitivity;
        spinValues.y += mouseY * sensitivity;
        LimitSpin();
    }

    // Dùng cho Joystick (Gán giá trị tuyệt đối)
    public void SetEnglishExplicit(Vector2 input)
    {
        spinValues = input;
        LimitSpin();
    }

    private void LimitSpin()
    {
        if (spinValues.magnitude > 1f) spinValues = spinValues.normalized;
    }

    public void ResetEnglish() { spinValues = Vector2.zero; }

    public Vector3 GetHitOffset(Transform pivot)
    {
        // Trả về vị trí lệch dựa trên hệ trục LOCAL của cây cơ (Phải và Lên)
        return (pivot.right * spinValues.x * ballRadius) + (pivot.up * spinValues.y * ballRadius);
    }

    //[Header("Settings")]
    //public float ballRadius = 0.0285f;
    //public float sensitivity = 0.5f;

    //[Header("Current State")]
    //public Vector2 spinValues = Vector2.zero;

    //public void UpdateEnglish(float mouseX, float mouseY)
    //{
    //    spinValues.x += mouseX * sensitivity;
    //    spinValues.y += mouseY * sensitivity;
    //    if (spinValues.magnitude > 1f) spinValues = spinValues.normalized;
    //}

    //public void ResetEnglish() { spinValues = Vector2.zero; }

    //public Vector3 GetHitOffset(Transform pivot)
    //{
    //    // Trả về vị trí lệch dựa trên hệ trục của cây cơ
    //    return (pivot.right * spinValues.x * ballRadius) + (pivot.up * spinValues.y * ballRadius);
    //}

    //[Header("Settings")]
    //public float ballRadius = 0.0285f;
    //public float sensitivity = 0.5f;
    //public GameObject cueBall;

    //[Header("Current State (Read Only)")]
    //public Vector2 spinValues = Vector2.zero;
    //// X: Trái/Phải, Y: Trên/Dưới

    //public void UpdateEnglish(float mouseX, float mouseY)
    //{
    //    spinValues.x += mouseX * sensitivity;
    //    spinValues.y += mouseY * sensitivity;

    //    // Giới hạn trong vòng tròn bi trắng
    //    if (spinValues.magnitude > 1f)
    //        spinValues = spinValues.normalized;
    //}

    //public Vector3 GetHitOffset(Transform pivot)
    //{
    //    // Trả về vị trí lệch trong không gian 3D dựa trên hướng của cây cơ
    //    return (pivot.right * spinValues.x * ballRadius)
    //         + (pivot.up * spinValues.y * ballRadius);
    //}

    //public void ResetEnglish()
    //{
    //    spinValues = Vector2.zero;
    //}

    //private void OnDrawGizmos()
    //{
    //    if (cueBall == null) return;

    //    Transform ballTransform = cueBall.transform;

    //    // Vẽ vòng tròn đại diện cho bi trắng
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(ballTransform.position, ballRadius);

    //    // Vẽ hướng cây cơ
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(ballTransform.position, transform.forward * 0.2f);

    //    // Vẽ điểm chạm dự kiến
    //    if (Application.isPlaying)
    //    {
    //        Gizmos.color = Color.cyan;

    //        Vector3 hitPoint =
    //            ballTransform.position
    //            + (transform.right * spinValues.x * ballRadius)
    //            + (transform.up * spinValues.y * ballRadius);

    //        Gizmos.DrawSphere(hitPoint, 0.005f);
    //    }
    //}

    //    [Header("Settings")]
    //    public float ballRadius = 0.0285f;
    //    public float sensitivity = 0.5f;


    //    [Header("Current State (Read Only)")]
    //    public Vector2 spinValues = Vector2.zero; // X: Trái/Phải, Y: Trên/Dưới

    //    public void UpdateEnglish(float mouseX, float mouseY)
    //    {
    //        spinValues.x += mouseX * sensitivity;
    //        spinValues.y += mouseY * sensitivity;

    //        // Giới hạn trong vòng tròn bi trắng
    //        if (spinValues.magnitude > 1f)
    //            spinValues = spinValues.normalized;
    //    }

    //    public Vector3 GetHitOffset(Transform pivot)
    //    {
    //        // Trả về vị trí lệch trong không gian 3D dựa trên hướng của cây cơ
    //        return (pivot.right * spinValues.x * ballRadius) + (pivot.up * spinValues.y * ballRadius);
    //    }

    //    private void OnDrawGizmos()
    //    {
    //        if (cueBall == null) return;

    //        // Lấy bán kính thực tế từ Collider của bi (tính cả Scale của Object)
    //        SphereCollider col = cueBall.GetComponent<SphereCollider>();
    //        float radius = (col != null) ? col.radius * cueBall.transform.lossyScale.x : 0.0285f;

    //        // 1. Vẽ vòng tròn dây quanh bi trắng
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(cueBall.position, radius);

    //        // 2. Vẽ điểm chạm (English) khi đang chạy Game
    //        if (Application.isPlaying && spinValues != Vector2.zero)
    //        {
    //            // Tính toán vị trí điểm chạm dựa trên Pivot của cây cơ
    //            // Lưu ý: Phải dùng transform của CueStickPivot để xác định hướng Right/Up
    //            Transform pivot = transform; // Nếu code này nằm trong CueStickController
    //            Vector3 hitOffset = (pivot.right * spinValues.x * radius) + (pivot.up * spinValues.y * radius);
    //            Vector3 hitPoint = cueBall.position + hitOffset;

    //            Gizmos.color = Color.red;
    //            Gizmos.DrawSphere(hitPoint, radius * 0.2f); // Vẽ chấm đỏ nhỏ tại điểm chạm

    //            // Vẽ đường nối từ tâm bi đến điểm chạm
    //            Gizmos.DrawLine(cueBall.position, hitPoint);
    //        }
    //    }
}
