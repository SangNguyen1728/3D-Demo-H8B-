using UnityEngine;

public class FixedShieldSkill : MonoBehaviour
{
    public int immuneTurns = 2;

    private bool isActive = false;

    // 🎯 GỌI TỪ BUTTON
    public void ActivateSkill()
    {
        if (isActive)
        {
            Debug.Log("Skill đang active!");
            return;
        }

        Debug.Log("🛡 KÍCH HOẠT SHIELD");

        BallNo[] balls = FindObjectsOfType<BallNo>();

        foreach (var b in balls)
        {
            if (IsOpponentBall(b))
            {
                BallHealth hp = b.GetComponent<BallHealth>();
                if (hp != null)
                {
                    hp.ActivateImmunity(immuneTurns);
                }
            }
        }

        isActive = true;
    }

    // 🎯 GỌI SAU MỖI LƯỢT
    public void OnTurnEnd()
    {
        if (!isActive) return;

        BallNo[] balls = FindObjectsOfType<BallNo>();

        foreach (var b in balls)
        {
            if (IsOpponentBall(b))
            {
                BallHealth hp = b.GetComponent<BallHealth>();
                if (hp != null)
                {
                    hp.ReduceTurn();
                }
            }
        }

        // kiểm tra hết effect
        if (CheckAllExpired())
        {
            Debug.Log("🛡 Shield hết hiệu lực");
            isActive = false;
        }
    }

    bool CheckAllExpired()
    {
        BallNo[] balls = FindObjectsOfType<BallNo>();

        foreach (var b in balls)
        {
            if (IsOpponentBall(b))
            {
                BallHealth hp = b.GetComponent<BallHealth>();
                if (hp != null && hp.IsImmune())
                    return false;
            }
        }

        return true;
    }

    bool IsOpponentBall(BallNo ball)
    {
        // ⚠️ tạm thời
        return !ball.isCueBall;
    }
}
