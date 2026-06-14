using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeelzitaManager : MonoBehaviour
{
    public static BeelzitaManager Instance;

    // Track các bi đã bị disable (vào lỗ)
    private List<GameObject> pocketedBalls = new List<GameObject>();

    // Track bi do skill 2.x spawn ra
    private List<GameObject> spawnedSkillBalls = new List<GameObject>();

    [Header("Spawn Settings")]
    public Transform defaultRespawnPoint;

    [Header("Spawn Position")]
    public bool useFixedPosition = true;

    // Tọa độ trung tâm bàn — chỉnh X/Y/Z theo scene của bạn
    public Vector3 fixedSpawnPosition = new Vector3(0f, 0.85f, 0.3f);

    void Awake()
    {
        Instance = this;
    }

    // ================================
    // 🎱 TRACK BI VÀO LỖ
    // ================================
    // Gọi từ PocketDetector khi bi vào lỗ
    public void RegisterPocketedBall(GameObject ball)
    {
        if (!pocketedBalls.Contains(ball))
        {
            pocketedBalls.Add(ball);
            Debug.Log($"[BeelzitaManager] Đã track bi vào lỗ: {ball.name}");
        }
    }

    // Lấy bi địch đã vào lỗ (không phải bi trắng, không phải bi do skill spawn)
    public GameObject GetLastEnemyPocketedBall(int currentPlayer)
    {
        // Duyệt từ cuối để lấy bi mới nhất
        for (int i = pocketedBalls.Count - 1; i >= 0; i--)
        {
            GameObject ball = pocketedBalls[i];
            if (ball == null) continue;

            BallNo bn = ball.GetComponent<BallNo>();
            if (bn == null || bn.isCueBall) continue;

            // Bỏ qua bi do skill spawn
            SkillBallMarker marker = ball.GetComponent<SkillBallMarker>();
            if (marker != null) continue;

            // Kiểm tra là bi địch
            BallOwnership ownership = ball.GetComponent<BallOwnership>();
            if (ownership != null && ownership.ownerPlayer != currentPlayer)
            {
                pocketedBalls.RemoveAt(i);
                return ball;
            }

            // Nếu không có ownership thì dùng group logic
            PocketTowPs pocket = FindFirstObjectByType<PocketTowPs>();
            if (pocket != null)
            {
                // Bi không thuộc người dùng skill = bi địch
                if (!pocket.IsPlayersBall(bn))
                {
                    pocketedBalls.RemoveAt(i);
                    return ball;
                }
            }
        }

        Debug.LogWarning("[BeelzitaManager] Không tìm thấy bi địch trong lỗ!");
        return null;
    }

    // ================================
    // 🔄 RESPAWN BI TỪ LỖ LÊN BÀN
    // ================================
    public void RespawnBall(
        GameObject ball,
        float hp,
        Vector3 position,
        System.Action<GameObject> onRespawned = null)
    {
        StartCoroutine(RespawnRoutine(ball, hp, position, onRespawned));
    }

    private IEnumerator RespawnRoutine(
        GameObject ball,
        float hp,
        Vector3 position,
        System.Action<GameObject> onRespawned)
    {
        //yield return new WaitForSeconds(0.5f);

        //ball.transform.position = position;

        //Rigidbody rb = ball.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.isKinematic = false;
        //    rb.linearVelocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}

        //Collider col = ball.GetComponent<Collider>();
        //if (col != null) col.enabled = true;

        //ball.SetActive(true);

        //// Set HP
        //BallHealth bh = ball.GetComponent<BallHealth>();
        //if (bh != null)
        //{
        //    bh.SetHealthDirect(hp);
        //    bh.BlinkOnRespawn(3f);
        //}

        //// Thêm lại vào danh sách bi của CueStickController
        //Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        //CueStickController cue = FindFirstObjectByType<CueStickController>();
        //if (cue != null && ballRb != null && !cue.balls.Contains(ballRb))
        //    cue.balls.Add(ballRb);

        //Debug.Log($"[BeelzitaManager] Bi {ball.name} đã lên bàn tại {position}, HP={hp}");

        //onRespawned?.Invoke(ball);

        yield return new WaitForSeconds(0.5f);

        // 🔥 Disable physics trước khi đặt vị trí
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        ball.transform.position = position;

        Collider col = ball.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        ball.SetActive(true);

        // Đợi 1 frame để Unity xác nhận vị trí
        yield return null;

        // 🔥 Bật lại physics sau khi đã đặt đúng vị trí
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Set HP
        BallHealth bh = ball.GetComponent<BallHealth>();
        if (bh != null)
        {
            bh.SetHealthDirect(hp);
            bh.BlinkOnRespawn(3f); // 🔥 Nhấp nháy báo hiệu
        }

        // Thêm lại vào danh sách CueStickController
        CueStickController cue = FindFirstObjectByType<CueStickController>();
        if (cue != null && rb != null && !cue.balls.Contains(rb))
            cue.balls.Add(rb);

        Debug.Log($"[BeelzitaManager] ✅ Bi {ball.name} lên bàn tại {position}, HP={hp}");

        onRespawned?.Invoke(ball);
    }

    // ================================
    // 🆕 SPAWN BI MỚI (skill 2.x)
    // ================================
    public GameObject SpawnSkillBall(
        GameObject prefab,
        Vector3 position,
        float hp,
        bool isEnemy,
        int ownerSkillID)
    {
        //GameObject ball = Instantiate(prefab, position, Quaternion.identity);

        //SkillBallMarker marker = ball.GetComponent<SkillBallMarker>();
        //if (marker == null) marker = ball.AddComponent<SkillBallMarker>();
        //marker.ownerSkillID = ownerSkillID;
        //marker.isEnemyBall = isEnemy;
        //marker.isNeutralBall = !isEnemy;

        //BallHealth bh = ball.GetComponent<BallHealth>();
        //if (bh != null)
        //{
        //    bh.SetHealthDirect(hp);
        //    bh.BlinkOnRespawn(3f);
        //}

        //CueStickController cue = FindFirstObjectByType<CueStickController>();
        //Rigidbody rb = ball.GetComponent<Rigidbody>();
        //if (cue != null && rb != null)
        //    cue.balls.Add(rb);

        //spawnedSkillBalls.Add(ball);
        //Debug.Log($"[BeelzitaManager] Spawn bi skill {ownerSkillID} tại {position}");

        //return ball;

        Vector3 spawnPos = new Vector3(
       position.x,
       GetActiveBallY(),
       position.z);

        GameObject ball = Instantiate(prefab, spawnPos, Quaternion.identity);

        // 🔥 Freeze ngay sau spawn để tránh nảy
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        SkillBallMarker marker =
            ball.GetComponent<SkillBallMarker>() ??
            ball.AddComponent<SkillBallMarker>();

        marker.ownerSkillID = ownerSkillID;
        marker.isEnemyBall = isEnemy;
        marker.isNeutralBall = !isEnemy;

        BallHealth bh = ball.GetComponent<BallHealth>();
        if (bh != null)
        {
            bh.SetHealthDirect(hp);
            bh.BlinkOnRespawn(3f); // 🔥 Nhấp nháy báo hiệu
        }

        // Bật lại physics sau 1 frame
        StartCoroutine(EnablePhysicsNextFrame(rb));

        CueStickController cue = FindFirstObjectByType<CueStickController>();
        if (cue != null && rb != null)
            cue.balls.Add(rb);

        spawnedSkillBalls.Add(ball);

        Debug.Log($"[BeelzitaManager] ✅ Spawn bi skill={ownerSkillID} tại {spawnPos}");

        return ball;
    }

    private IEnumerator EnablePhysicsNextFrame(Rigidbody rb)
    {
        yield return null; // Đợi 1 frame

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // ================================
    // 🧹 CLEANUP
    // ================================
    public void RemoveSpawnedBall(GameObject ball)
    {
        spawnedSkillBalls.Remove(ball);

        CueStickController cue = FindFirstObjectByType<CueStickController>();
        Rigidbody rb = ball?.GetComponent<Rigidbody>();
        if (cue != null && rb != null)
            cue.balls.Remove(rb);
    }

    public Vector3 GetRespawnPosition()
    {
        //if (defaultRespawnPoint != null)
        //    return defaultRespawnPoint.position;

        //return new Vector3(0f, 0.85f, 0f);

        float ballY = GetActiveBallY();

        if (useFixedPosition)
            return new Vector3(
                fixedSpawnPosition.x,
                ballY,
                fixedSpawnPosition.z);

        if (defaultRespawnPoint != null)
            return new Vector3(
                defaultRespawnPoint.position.x,
                ballY,
                defaultRespawnPoint.position.z);

        return new Vector3(0f, ballY, 0f);
    }

    private float GetActiveBallY()
    {
        // Tìm bi thật đang active để lấy Y chính xác
        BallNo[] allBalls = FindObjectsOfType<BallNo>();
        foreach (BallNo b in allBalls)
        {
            if (b.isCueBall) continue;
            if (!b.gameObject.activeInHierarchy) continue;

            // Bỏ qua bi do skill spawn
            if (b.GetComponent<SkillBallMarker>() != null) continue;
            if (b.GetComponent<BeelzitaBallBehavior>() != null) continue;

            return b.transform.position.y;
        }

        // Fallback nếu không tìm được
        return fixedSpawnPosition.y;
    }


}
