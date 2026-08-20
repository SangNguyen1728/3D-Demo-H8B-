using System.Collections.Generic;

public static class PlayerManagerRegistry
{
    private static Dictionary<int, GlaszekManager> managers = new Dictionary<int, GlaszekManager>();

    public static void Register(int playerNumber, GlaszekManager manager)
    {
        managers[playerNumber] = manager;
    }

    public static GlaszekManager Get(int playerNumber)
    {
        managers.TryGetValue(playerNumber, out var m);
        return m;
    }
}
