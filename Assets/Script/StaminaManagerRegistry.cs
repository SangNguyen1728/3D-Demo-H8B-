using UnityEngine;
using System.Collections.Generic;

public static class StaminaManagerRegistry
{
    private static Dictionary<int, StaminaManager> managers = new Dictionary<int, StaminaManager>();

    public static void Register(int playerNumber, StaminaManager manager)
    {
        managers[playerNumber] = manager;
    }

    public static StaminaManager Get(int playerNumber)
    {
        managers.TryGetValue(playerNumber, out var m);
        return m;
    }
}
