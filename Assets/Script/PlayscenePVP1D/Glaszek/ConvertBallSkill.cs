using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Convert Ball Skill", menuName = "Skills/Convert Ball")]
public class ConvertBallSkill : BaseSkills
{
    [Header("Flash Settings")]
    public int flashCount = 5;          // số lần nhấp nháy
    public float flashInterval = 0.12f; // tốc độ nhấp nháy

    public override void Activate(GameObject player, GlaszekManager manager)
    {
        PocketTowPs pocket = Object.FindFirstObjectByType<PocketTowPs>();
        if (pocket == null) return;

        // Lấy tất cả bi còn active, không phải bi trắng
        BallNo[] allBalls = Object.FindObjectsByType<BallNo>(FindObjectsSortMode.None);

        // Lọc ra các bi của ENEMY còn trên bàn
        List<BallNo> enemyBalls = new List<BallNo>();

        foreach (BallNo ball in allBalls)
        {
            if (ball.isCueBall) continue;
            if (!ball.gameObject.activeInHierarchy) continue;

            // Bi của đối thủ = bi KHÔNG phải của current player
            if (!pocket.IsPlayersBall(ball))
            {
                enemyBalls.Add(ball);
            }
        }

        if (enemyBalls.Count == 0)
        {
            Debug.Log("CONVERT SKILL: Không có bi đối thủ để convert");
            return;
        }

        // Chọn ngẫu nhiên 1 bi của đối thủ
        BallNo targetBall = enemyBalls[Random.Range(0, enemyBalls.Count)];

        Debug.Log($"CONVERT SKILL: Sẽ convert bi số {targetBall.ballNumber}");

        // Bắt đầu coroutine nhấp nháy + đổi material
        manager.StartCoroutine(FlashAndConvert(targetBall, pocket));
    }

    private IEnumerator FlashAndConvert(BallNo ball, PocketTowPs pocket)
    {
        BallOwnership ownership = ball.GetComponent<BallOwnership>();
        if (ownership == null || ownership.targetRenderer == null)
            yield break;

        Renderer rend = ownership.targetRenderer;

        // Lưu material gốc (mat của đối thủ)
        Material originalMat = rend.material;

        // Xác định material MỚI (mat của current player)
        // Current player đang là nhóm nào?
        Material newMat = GetConvertedMaterial(ownership, pocket);

        if (newMat == null)
        {
            Debug.LogWarning("CONVERT SKILL: Không xác định được material mới");
            yield break;
        }

        // === NHẤP NHÁY: xen kẽ mat cũ ↔ mat mới ===
        for (int i = 0; i < flashCount; i++)
        {
            rend.material = newMat;
            yield return new WaitForSeconds(flashInterval);

            rend.material = originalMat;
            yield return new WaitForSeconds(flashInterval);
        }

        // === DỪNG Ở MAT MỚI + CONVERT OWNERSHIP ===
        rend.material = newMat;

        // Đổi group trên BallOwnership
        ownership.currentGroup = GetCurrentPlayerGroup(pocket);

        // Đánh dấu convert về tay current player
        ownership.ConvertToPlayer(pocket.currentPlayer);

        Debug.Log($"CONVERT SKILL: Bi {ball.ballNumber} đã convert sang Player {pocket.currentPlayer}");
    }

    // Trả về material tương ứng với nhóm của current player
    private Material GetConvertedMaterial(BallOwnership ownership, PocketTowPs pocket)
    {
        BallOwnership.BallGroup targetGroup = GetCurrentPlayerGroup(pocket);

        if (targetGroup == BallOwnership.BallGroup.Solid)
            return ownership.solidMat;

        if (targetGroup == BallOwnership.BallGroup.Stripe)
            return ownership.stripeMat;

        return null;
    }

    // Trả về BallGroup của current player dựa vào player1Group/player2Group
    private BallOwnership.BallGroup GetCurrentPlayerGroup(PocketTowPs pocket)
    {
        // Dùng IsPlayersBall với bi giả để suy ngược group,
        // hoặc đơn giản: tìm 1 bi đang là của current player và lấy group của nó
        BallNo[] allBalls = Object.FindObjectsByType<BallNo>(FindObjectsSortMode.None);

        foreach (BallNo b in allBalls)
        {
            if (b.isCueBall) continue;
            if (!b.gameObject.activeInHierarchy) continue;

            if (pocket.IsPlayersBall(b))
            {
                BallOwnership ow = b.GetComponent<BallOwnership>();
                if (ow != null && ow.currentGroup != BallOwnership.BallGroup.None)
                    return ow.currentGroup;
            }
        }

        return BallOwnership.BallGroup.None;
    }

    public override void OnTurnEnd(GlaszekManager manager) { }
}