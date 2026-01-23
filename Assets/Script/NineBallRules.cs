using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NineBallRules : MonoBehaviour
{
    // Tìm bi có số nhỏ nhất hiện còn trên bàn
    public int GetCurrentTargetBall(List<int> pottedBalls)
    {
        //for (int i = 1; i <= 9; i++)
        //{
        //    if (!pottedBalls.Contains(i)) return i;
        //}
        //return -1; // Đã vào hết bi

        for (int i = 1; i <= 9; i++)
        {
            if (!pottedBalls.Contains(i)) return i;
        }
        return 9;
    }

    // Kiểm tra xem bi chạm đầu tiên có đúng là bi mục tiêu không
    public bool IsValidHit(int firstBallHit, int currentTarget)
    {
        return firstBallHit == currentTarget;
    }

    // Kiểm tra điều kiện thắng (Vào bi số 9 mà không phạm lỗi)
    public bool CheckWinCondition(int ballPotted, bool isFoul)
    {
        return (ballPotted == 9 && !isFoul);
    }

    public bool IsLegalHit(int firstBallTouched, int targetBall)
    {
        // Cú đánh hợp lệ khi bi đầu tiên chạm phải là bi mục tiêu nhỏ nhất
        return firstBallTouched == targetBall;
    }


}
