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
    //public Vector3 fixedSpawnPosition = new Vector3(0f, 0.85f, 0.3f);

    [Header("Spawn Position")]
    public Vector3 fixedSpawnPosition = new Vector3(3.224f, 1.185f, -0.172f);

    [Header("Spawn Offset Range — tránh đè bi")]
    public float spawnOffsetRange = 0.15f;

    void Awake()
    {
        Instance = this;
        Debug.Log($"[BeelzitaManager] Awake | fixedSpawnPosition hiện tại trên Inspector = {fixedSpawnPosition}");
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
        //Debug.Log($"[BeelzitaManager] Tìm bi địch trong lỗ | currentPlayer={currentPlayer} | Tổng bi đã track={pocketedBalls.Count}");

        //if (pocketedBalls.Count == 0)
        //{
        //    Debug.LogWarning("[BeelzitaManager] Danh sách bi trong lỗ đang trống!");
        //    return null;
        //}

        //// In danh sách toàn bộ bi đang track
        //for (int i = 0; i < pocketedBalls.Count; i++)
        //{
        //    GameObject b = pocketedBalls[i];
        //    if (b == null) { Debug.Log($"  [{i}] NULL"); continue; }

        //    BallNo bn = b.GetComponent<BallNo>();
        //    BallOwnership own = b.GetComponent<BallOwnership>();

        //    string ownerInfo = own != null
        //        ? $"ownerPlayer={own.ownerPlayer} isConverted={own.isConverted} group={own.currentGroup}"
        //        : "không có BallOwnership";

        //    string ballInfo = bn != null
        //        ? $"Bi số {bn.ballNumber} isCueBall={bn.isCueBall}"
        //        : "không có BallNo";

        //    Debug.Log($"  [{i}] {b.name} | {ballInfo} | {ownerInfo}");
        //}

        //PocketTowPs pocket = FindFirstObjectByType<PocketTowPs>();

        //for (int i = pocketedBalls.Count - 1; i >= 0; i--)
        //{
        //    GameObject ball = pocketedBalls[i];
        //    if (ball == null) { pocketedBalls.RemoveAt(i); continue; }

        //    BallNo bn = ball.GetComponent<BallNo>();
        //    if (bn == null || bn.isCueBall)
        //    {
        //        Debug.Log($"[BeelzitaManager] Bỏ qua {ball.name} — isCueBall hoặc thiếu BallNo");
        //        continue;
        //    }

        //    // Bỏ qua bi do skill spawn
        //    SkillBallMarker marker = ball.GetComponent<SkillBallMarker>();
        //    BeelzitaBallBehavior behavior = ball.GetComponent<BeelzitaBallBehavior>();
        //    if (marker != null || behavior != null)
        //    {
        //        Debug.Log($"[BeelzitaManager] Bỏ qua {ball.name} — bi do skill spawn");
        //        continue;
        //    }

        //    bool isEnemy = false;

        //    // Ưu tiên check BallOwnership
        //    BallOwnership ownership = ball.GetComponent<BallOwnership>();
        //    if (ownership != null && ownership.isConverted)
        //    {
        //        isEnemy = ownership.ownerPlayer != currentPlayer;
        //        Debug.Log($"[BeelzitaManager] {ball.name} converted → ownerPlayer={ownership.ownerPlayer} isEnemy={isEnemy}");
        //    }
        //    else if (pocket != null)
        //    {
        //        // Tạm đổi currentPlayer để check
        //        int saved = pocket.currentPlayer;
        //        pocket.currentPlayer = currentPlayer;
        //        bool isOwn = pocket.IsPlayersBall(bn);
        //        pocket.currentPlayer = saved;

        //        isEnemy = !isOwn;

        //        string group = ownership != null ? ownership.currentGroup.ToString() : "unknown";
        //        Debug.Log($"[BeelzitaManager] {ball.name} Bi số {bn.ballNumber} group={group} isOwn={isOwn} isEnemy={isEnemy}");
        //    }

        //    if (isEnemy)
        //    {
        //        Debug.Log($"[BeelzitaManager] ✅ Chọn bi địch: {ball.name} Bi số {bn.ballNumber}");
        //        pocketedBalls.RemoveAt(i);
        //        return ball;
        //    }
        //}

        //Debug.LogWarning($"[BeelzitaManager] ❌ Không tìm thấy bi địch nào trong lỗ cho Player {currentPlayer}!");
        //return null;
        Debug.Log($"[BeelzitaManager] Tìm bi địch trong lỗ | currentPlayer={currentPlayer} | Tổng bi đã track={pocketedBalls.Count}");

        if (pocketedBalls.Count == 0)
        {
            Debug.LogWarning("[BeelzitaManager] Danh sách bi trong lỗ đang trống!");
            return null;
        }

        // In danh sách toàn bộ bi đang track
        for (int i = 0; i < pocketedBalls.Count; i++)
        {
            GameObject b = pocketedBalls[i];
            if (b == null) { Debug.Log($"  [{i}] NULL"); continue; }

            BallNo bn = b.GetComponent<BallNo>();
            BallOwnership own = b.GetComponent<BallOwnership>();

            string ownerInfo = own != null
                ? $"ownerPlayer={own.ownerPlayer} isConverted={own.isConverted} group={own.currentGroup}"
                : "không có BallOwnership";

            string ballInfo = bn != null
                ? $"Bi số {bn.ballNumber} isCueBall={bn.isCueBall}"
                : "không có BallNo";

            Debug.Log($"  [{i}] {b.name} | {ballInfo} | {ownerInfo}");
        }

        PocketTowPs pocket = FindFirstObjectByType<PocketTowPs>();

        for (int i = pocketedBalls.Count - 1; i >= 0; i--)
        {
            GameObject ball = pocketedBalls[i];
            if (ball == null) { pocketedBalls.RemoveAt(i); continue; }

            BallNo bn = ball.GetComponent<BallNo>();
            if (bn == null || bn.isCueBall)
            {
                Debug.Log($"[BeelzitaManager] Bỏ qua {ball.name} — isCueBall hoặc thiếu BallNo");
                continue;
            }

            // Bỏ qua bi do skill spawn
            SkillBallMarker marker = ball.GetComponent<SkillBallMarker>();
            BeelzitaBallBehavior behavior = ball.GetComponent<BeelzitaBallBehavior>();
            if (marker != null || behavior != null)
            {
                Debug.Log($"[BeelzitaManager] Bỏ qua {ball.name} — bi do skill spawn");
                continue;
            }

            bool isEnemy = false;

            // Ưu tiên check BallOwnership
            BallOwnership ownership = ball.GetComponent<BallOwnership>();
            if (ownership != null && ownership.isConverted)
            {
                isEnemy = ownership.ownerPlayer != currentPlayer;
                Debug.Log($"[BeelzitaManager] {ball.name} converted → ownerPlayer={ownership.ownerPlayer} isEnemy={isEnemy}");
            }
            else if (pocket != null)
            {
                // Tạm đổi currentPlayer để check
                int saved = pocket.currentPlayer;
                pocket.currentPlayer = currentPlayer;
                bool isOwn = pocket.IsPlayersBall(bn);
                pocket.currentPlayer = saved;

                isEnemy = !isOwn;

                string group = ownership != null ? ownership.currentGroup.ToString() : "unknown";
                Debug.Log($"[BeelzitaManager] {ball.name} Bi số {bn.ballNumber} group={group} isOwn={isOwn} isEnemy={isEnemy}");
            }

            if (isEnemy)
            {
                Debug.Log($"[BeelzitaManager] ✅ Chọn bi địch: {ball.name} Bi số {bn.ballNumber}");
                pocketedBalls.RemoveAt(i);
                return ball;
            }
        }

        Debug.LogWarning($"[BeelzitaManager] ❌ Không tìm thấy bi địch nào trong lỗ cho Player {currentPlayer}!");
        return null;
    }

    //public void OnTurnChanged(int newCurrentPlayer)
    //{
    //    //// Tìm tất cả bi do skill spawn đang active
    //    //foreach (GameObject ball in spawnedSkillBalls)
    //    //{
    //    //    if (ball == null) continue;

    //    //    BallHealth bh = ball.GetComponent<BallHealth>();
    //    //    SkillBallMarker marker = ball.GetComponent<SkillBallMarker>();
    //    //    BeelzitaBallBehavior behavior = ball.GetComponent<BeelzitaBallBehavior>();

    //    //    if (bh == null) continue;

    //    //    // Xác định bi này thuộc ai
    //    //    int ballOwner = -1;
    //    //    if (marker != null) ballOwner = marker.isEnemyBall ? GetEnemyPlayer(newCurrentPlayer) : newCurrentPlayer;
    //    //    BallOwnership own = ball.GetComponent<BallOwnership>();
    //    //    if (own != null && own.isConverted) ballOwner = own.ownerPlayer;

    //    //    // Nếu là bi địch trong lượt hiện tại → blink cảnh báo
    //    //    bool isEnemyBall = (ballOwner != -1 && ballOwner != newCurrentPlayer);

    //    //    if (isEnemyBall)
    //    //    {
    //    //        Debug.Log($"[BeelzitaManager] Lượt Player {newCurrentPlayer} | {ball.name} là bi địch → blink");
    //    //        bh.BlinkEnemyBall(true);
    //    //    }
    //    //    else
    //    //    {
    //    //        bh.BlinkEnemyBall(false);
    //    //    }
    //    //}

    //    // Tìm tất cả bi do skill spawn đang active
    //    foreach (GameObject ball in spawnedSkillBalls)
    //    {
    //        if (ball == null) continue;

    //        BallHealth bh = ball.GetComponent<BallHealth>();
    //        SkillBallMarker marker = ball.GetComponent<SkillBallMarker>();
    //        BeelzitaBallBehavior behavior = ball.GetComponent<BeelzitaBallBehavior>();

    //        if (bh == null) continue;

    //        // Xác định bi này thuộc ai
    //        int ballOwner = -1;
    //        if (marker != null) ballOwner = marker.isEnemyBall ? GetEnemyPlayer(newCurrentPlayer) : newCurrentPlayer;
    //        BallOwnership own = ball.GetComponent<BallOwnership>();
    //        if (own != null && own.isConverted) ballOwner = own.ownerPlayer;

    //        // Nếu là bi địch trong lượt hiện tại → blink cảnh báo
    //        bool isEnemyBall = (ballOwner != -1 && ballOwner != newCurrentPlayer);

    //        if (isEnemyBall)
    //        {
    //            Debug.Log($"[BeelzitaManager] Lượt Player {newCurrentPlayer} | {ball.name} là bi địch → blink");
    //            bh.BlinkEnemyBall(true);
    //        }
    //        else
    //        {
    //            bh.BlinkEnemyBall(false);
    //        }
    //    }
    //}

    public void MarkAsSkillBall(GameObject ball)
    {
        BallHealth bh = ball.GetComponent<BallHealth>();
        if (bh == null) return;

        bh.BlinkEnemyBall(true);
        Debug.Log($"[BeelzitaManager] {ball.name} bắt đầu blink liên tục (bi skill)");
    }

    // Gọi khi bi bị destroy/disable/hết hiệu lực
    public void UnmarkSkillBall(GameObject ball)
    {
        if (ball == null) return;
        BallHealth bh = ball.GetComponent<BallHealth>();
        if (bh != null) bh.BlinkEnemyBall(false);
    }

    private int GetEnemyPlayer(int current) => current == 1 ? 2 : 1;

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

        //yield return new WaitForSeconds(0.5f);

        //// 🔥 Disable physics trước khi đặt vị trí
        //Rigidbody rb = ball.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.isKinematic = true;
        //    rb.linearVelocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}

        //ball.transform.position = position;

        //Collider col = ball.GetComponent<Collider>();
        //if (col != null) col.enabled = true;

        //ball.SetActive(true);

        //// Đợi 1 frame để Unity xác nhận vị trí
        //yield return null;

        //// 🔥 Bật lại physics sau khi đã đặt đúng vị trí
        //if (rb != null)
        //{
        //    rb.isKinematic = false;
        //    rb.linearVelocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}

        //// Set HP
        //BallHealth bh = ball.GetComponent<BallHealth>();
        //if (bh != null)
        //{
        //    bh.SetHealthDirect(hp);
        //    bh.BlinkOnRespawn(3f); // 🔥 Nhấp nháy báo hiệu
        //}

        //// Thêm lại vào danh sách CueStickController
        //CueStickController cue = FindFirstObjectByType<CueStickController>();
        //if (cue != null && rb != null && !cue.balls.Contains(rb))
        //    cue.balls.Add(rb);

        //Debug.Log($"[BeelzitaManager] ✅ Bi {ball.name} lên bàn tại {position}, HP={hp}");

        //onRespawned?.Invoke(ball);

        //yield return new WaitForSeconds(0.5f);

        //Rigidbody rb = ball.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.isKinematic = true;
        //    rb.linearVelocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}

        //ball.transform.position = position;

        //Collider col = ball.GetComponent<Collider>();
        //if (col != null) col.enabled = true;

        //ball.SetActive(true);

        //yield return null;

        //if (rb != null)
        //{
        //    rb.isKinematic = false;
        //    rb.linearVelocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}

        //BallHealth bh = ball.GetComponent<BallHealth>();
        //if (bh != null)
        //{
        //    bh.SetHealthDirect(hp);
        //    bh.BlinkOnRespawn(1.5f); // 🔥 Chỉ blink ngắn 1.5s khi respawn
        //}

        //CueStickController cue = FindFirstObjectByType<CueStickController>();
        //if (cue != null && rb != null && !cue.balls.Contains(rb))
        //    cue.balls.Add(rb);

        //Debug.Log($"[BeelzitaManager] ✅ Bi {ball.name} lên bàn tại {position}, HP={hp}");

        //onRespawned?.Invoke(ball);

        Debug.Log($"[BeelzitaManager] RespawnRoutine bắt đầu | target position = {position}");

        yield return new WaitForSeconds(0.5f);

        // 🔥 BƯỚC 1: SetActive TRƯỚC để physics engine nhận diện object
        ball.SetActive(true);

        // 🔥 BƯỚC 2: Bỏ parent nếu có (tránh local position lệch)
        ball.transform.SetParent(null, false);

        // 🔥 BƯỚC 3: Freeze physics
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 🔥 BƯỚC 4: Set position SAU khi đã active + freeze
        ball.transform.position = position;

        Debug.Log($"[BeelzitaManager] Sau khi set position | thực tế = {ball.transform.position}");

        Collider col = ball.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // Đợi 2 frame để physics engine đồng bộ transform
        yield return null;
        yield return null;

        Debug.Log($"[BeelzitaManager] Sau 2 frame | thực tế = {ball.transform.position}");

        // 🔥 BƯỚC 5: Set lại position 1 lần nữa để chắc chắn (phòng physics đẩy lệch)
        ball.transform.position = position;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        BallHealth bh = ball.GetComponent<BallHealth>();
        if (bh != null)
        {
            bh.SetHealthDirect(hp);
            bh.BlinkOnRespawn(1.5f);
        }

        CueStickController cue = FindFirstObjectByType<CueStickController>();
        if (cue != null && rb != null && !cue.balls.Contains(rb))
            cue.balls.Add(rb);

        Debug.Log($"[BeelzitaManager] ✅ HOÀN TẤT | vị trí cuối = {ball.transform.position}, HP={hp}");

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

        // Vector3 spawnPos = new Vector3(
        //position.x,
        //GetActiveBallY(),
        //position.z);

        // GameObject ball = Instantiate(prefab, spawnPos, Quaternion.identity);
        // ball.transform.SetParent(null, true); // worldPositionStays = true

        // Debug.Log($"[BeelzitaManager] Spawn tại {position} | thực tế sau spawn = {ball.transform.position}");

        // // 🔥 Freeze ngay sau spawn để tránh nảy
        // Rigidbody rb = ball.GetComponent<Rigidbody>();
        // if (rb != null)
        // {
        //     rb.isKinematic = true;
        //     rb.linearVelocity = Vector3.zero;
        //     rb.angularVelocity = Vector3.zero;
        // }

        // SkillBallMarker marker =
        //     ball.GetComponent<SkillBallMarker>() ??
        //     ball.AddComponent<SkillBallMarker>();

        // marker.ownerSkillID = ownerSkillID;
        // marker.isEnemyBall = isEnemy;
        // marker.isNeutralBall = !isEnemy;

        // BallHealth bh = ball.GetComponent<BallHealth>();
        // if (bh != null)
        // {
        //     bh.SetHealthDirect(hp);
        //     bh.BlinkOnRespawn(3f); // 🔥 Nhấp nháy báo hiệu
        // }

        // // Bật lại physics sau 1 frame
        // StartCoroutine(EnablePhysicsNextFrame(rb));

        // CueStickController cue = FindFirstObjectByType<CueStickController>();
        // if (cue != null && rb != null)
        //     cue.balls.Add(rb);

        // spawnedSkillBalls.Add(ball);

        // Debug.Log($"[BeelzitaManager] ✅ Spawn bi skill={ownerSkillID} tại {spawnPos}");

        // return ball;

        GameObject ball = Instantiate(prefab);
        ball.transform.SetParent(null, false);

        // Freeze trước khi set position
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        ball.transform.position = position;
        ball.SetActive(true);

        Debug.Log($"[BeelzitaManager] Spawn tại {position} | thực tế sau spawn = {ball.transform.position}");

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
            bh.BlinkOnRespawn(1.5f);
        }

        StartCoroutine(EnablePhysicsAndVerify(ball, rb, position));

        CueStickController cue = FindFirstObjectByType<CueStickController>();
        if (cue != null && rb != null)
            cue.balls.Add(rb);

        spawnedSkillBalls.Add(ball);

        return ball;
    }

    private IEnumerator EnablePhysicsAndVerify(GameObject ball, Rigidbody rb, Vector3 expectedPos)
    {
        yield return null;
        yield return null;

        if (ball == null) yield break;

        // Set lại position 1 lần nữa để chắc chắn
        ball.transform.position = expectedPos;

        Debug.Log($"[BeelzitaManager] Verify sau 2 frame | thực tế = {ball.transform.position}");

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
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

        //
        //float ballY = GetActiveBallY();

        //if (useFixedPosition)
        //    return new Vector3(
        //        fixedSpawnPosition.x,
        //        ballY,
        //        fixedSpawnPosition.z);

        //if (defaultRespawnPoint != null)
        //    return new Vector3(
        //        defaultRespawnPoint.position.x,
        //        ballY,
        //        defaultRespawnPoint.position.z);

        //return new Vector3(0f, ballY, 0f);
        Vector3 basePos = useFixedPosition
               ? fixedSpawnPosition
               : (defaultRespawnPoint != null ? defaultRespawnPoint.position : new Vector3(0f, 0.0032f, 0f));

        // Random offset nhỏ trên mặt bàn (giữ nguyên Y)
        float offsetX = Random.Range(-spawnOffsetRange, spawnOffsetRange);
        float offsetZ = Random.Range(-spawnOffsetRange, spawnOffsetRange);

        Vector3 result = new Vector3(
            basePos.x + offsetX,
            basePos.y,
            basePos.z + offsetZ);

        Debug.Log($"[BeelzitaManager] GetRespawnPosition = {result}");
        return result;
    }

    //private float GetActiveBallY()
    //{
    //    // Tìm bi thật đang active để lấy Y chính xác
    //    BallNo[] allBalls = FindObjectsOfType<BallNo>();
    //    foreach (BallNo b in allBalls)
    //    {
    //        if (b.isCueBall) continue;
    //        if (!b.gameObject.activeInHierarchy) continue;

    //        // Bỏ qua bi do skill spawn
    //        if (b.GetComponent<SkillBallMarker>() != null) continue;
    //        if (b.GetComponent<BeelzitaBallBehavior>() != null) continue;

    //        return b.transform.position.y;
    //    }

    //    // Fallback nếu không tìm được
    //    return fixedSpawnPosition.y;
    //}


}
