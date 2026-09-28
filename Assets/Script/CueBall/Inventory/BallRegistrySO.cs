using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallRegistry", menuName = "Billiards/Ball Registry")]
public class BallRegistrySO : ScriptableObject
{
    public List<BallScaleConfig> allBalls = new List<BallScaleConfig>();

    public BallScaleConfig GetById(int id)
    {
        foreach (var ball in allBalls)
        {
            if (ball != null && ball.equipmentId == id)
                return ball;
        }

        Debug.LogWarning($"[BallRegistrySO] Không tìm thấy bi có ID = {id}");
        return null;
    }
}
