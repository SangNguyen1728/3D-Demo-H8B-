using UnityEngine;
using System.Collections.Generic;

public static class BuffManagerRegistry
{
    private static Dictionary<int, BuffManager> managers = new Dictionary<int, BuffManager>();

    public static void Register(int playerNumber, BuffManager manager)
    {
        managers[playerNumber] = manager;
    }

    public static BuffManager Get(int playerNumber)
    {
        managers.TryGetValue(playerNumber, out var m);
        return m;
    }
}
