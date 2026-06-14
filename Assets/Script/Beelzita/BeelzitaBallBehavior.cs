using UnityEngine;
using System.Collections;


public enum BeelzitaBallType
{
    None,
    MouseSkill12,    // Chuột Già — rớt lỗ móc lên, trừ 300HP
    RabbitSkill20,   // Thỏ — neutral, sau 3 lượt thành bi địch
    RabbitSkill21,   // Thỏ Tơ — rớt lỗ móc lên trừ 300HP, miễn damage từ skill owner
    RabbitSkill22,   // Thỏ Già — rớt lỗ móc lên giữ HP, chỉ hạ bằng đánh hết máu
}

public class BeelzitaBallBehavior : MonoBehaviour
{
    [HideInInspector] public BeelzitaBallType ballType = BeelzitaBallType.None;
    [HideInInspector] public BeelzitaManager bm;
    [HideInInspector] public int skillOwnerPlayer = -1;
    [HideInInspector] public int turnsLeft = 0;

    private BallHealth bh;

    void Awake()
    {
        bh = GetComponent<BallHealth>();
    }

    // ================================
    // INIT
    // ================================
    public void Init(
        BeelzitaBallType type,
        BeelzitaManager manager,
        int ownerPlayer = -1,
        int turns = 0)
    {
        ballType = type;
        bm = manager;
        skillOwnerPlayer = ownerPlayer;
        turnsLeft = turns;

        Debug.Log($"[BeelzitaBallBehavior] Init type={type} owner={ownerPlayer} turns={turns}");
    }

    // ================================
    // GỌI TỪ PocketDetector
    // ================================
    public void OnEnteredPocket()
    {
        switch (ballType)
        {
            case BeelzitaBallType.MouseSkill12:
                HandleMouseSkill12Pocket();
                break;

            case BeelzitaBallType.RabbitSkill21:
                HandleRabbitSkill21Pocket();
                break;

            case BeelzitaBallType.RabbitSkill22:
                HandleRabbitSkill22Pocket();
                break;

            case BeelzitaBallType.RabbitSkill20:
                // Thỏ 2.0 không có logic đặc biệt khi vào lỗ
                Debug.Log("[Thỏ 2.0] Vào lỗ bình thường");
                break;
        }
    }

    // ================================
    // GỌI TỪ GlaszekManager.NotifyBallStopped
    // ================================
    public void OnTurnEnd()
    {
        if (ballType != BeelzitaBallType.RabbitSkill20) return;

        turnsLeft--;
        Debug.Log($"[Thỏ 2.0] Còn {turnsLeft} lượt");

        if (turnsLeft <= 0)
            ConvertToEnemyBall();
    }

    // ================================
    // KIỂM TRA BLOCK DAMAGE (Thỏ Tơ 2.1)
    // ================================
    public bool ShouldBlockDamage(GameObject attacker)
    {
        if (ballType != BeelzitaBallType.RabbitSkill21) return false;
        if (skillOwnerPlayer == -1) return false;

        // Block damage từ bi của người dùng skill
        PocketTowPs pocket = FindFirstObjectByType<PocketTowPs>();
        if (pocket == null) return false;

        int savedPlayer = pocket.currentPlayer;
        pocket.currentPlayer = skillOwnerPlayer;
        bool isOwnerBall = pocket.IsPlayersBall(attacker.GetComponent<BallNo>());
        pocket.currentPlayer = savedPlayer;

        return isOwnerBall;
    }

    // ================================
    // PRIVATE HANDLERS
    // ================================

    // Chuột Già 1.2: rớt lỗ → trừ 300HP + móc lên
    private void HandleMouseSkill12Pocket()
    {
        if (bh == null) return;

        float currentHP = bh.GetCurrentHealth();
        float newHP = Mathf.Max(0, currentHP - 300f);

        Debug.Log($"[Chuột Già 1.2] Rớt lỗ HP {currentHP}→{newHP}");

        if (newHP <= 0)
        {
            Debug.Log("[Chuột Già 1.2] Hết máu → không móc lên nữa");
            Destroy(this);
            return;
        }

        if (bm != null)
            bm.RespawnBall(gameObject, newHP, bm.GetRespawnPosition());
    }

    // Thỏ Tơ 2.1: rớt lỗ → trừ 300HP + móc lên
    private void HandleRabbitSkill21Pocket()
    {
        if (bh == null) return;

        float currentHP = bh.GetCurrentHealth();
        float newHP = Mathf.Max(0, currentHP - 300f);

        Debug.Log($"[Thỏ Tơ 2.1] Rớt lỗ HP {currentHP}→{newHP}");

        if (bm != null)
            bm.RespawnBall(gameObject, newHP, bm.GetRespawnPosition());
    }

    // Thỏ Già 2.2: rớt lỗ → móc lên giữ nguyên HP
    private void HandleRabbitSkill22Pocket()
    {
        if (bh == null) return;

        float currentHP = bh.GetCurrentHealth();

        Debug.Log($"[Thỏ Già 2.2] Rớt lỗ → móc lên giữ HP={currentHP}");

        if (bm != null)
            bm.RespawnBall(gameObject, currentHP, bm.GetRespawnPosition());
    }

    // Thỏ 2.0: hết lượt → biến thành bi địch
    private void ConvertToEnemyBall()
    {
        BallOwnership ownership =
            GetComponent<BallOwnership>() ??
            gameObject.AddComponent<BallOwnership>();

        PocketTowPs pocket = FindFirstObjectByType<PocketTowPs>();
        if (pocket != null)
        {
            int enemy = skillOwnerPlayer == 1 ? 2 : 1;
            ownership.ownerPlayer = enemy;
            ownership.isConverted = true;
            Debug.Log($"[Thỏ 2.0] Biến thành bi của Player {enemy}");
        }

        Destroy(this);
    }
}