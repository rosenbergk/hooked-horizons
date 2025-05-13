// FishCatcher.cs
using System.Collections.Generic;
using UnityEngine;

public class FishCatcher : MonoBehaviour
{
    private static float noCatchChance = 0.8f;
    private static float fish1CatchChance = 0.08f;
    private static float fish2CatchChance = 0.05f;
    private static float fish3CatchChance = 0.035f;
    private static float fish4CatchChance = 0.025f;
    private static float fish5CatchChance = 0.01f;

    private Dictionary<int, float> catchChances = new Dictionary<int, float>()
    {
        { 0, noCatchChance },
        { 1, fish1CatchChance },
        { 2, fish2CatchChance },
        { 3, fish3CatchChance },
        { 4, fish4CatchChance },
        { 5, fish5CatchChance },
    };

    public int RollForCatch()
    {
        float roll = Random.value;
        float cumulative = 0;

        foreach (var chance in catchChances)
        {
            cumulative += chance.Value;

            if (roll < cumulative)
            {
                return chance.Key;
            }
        }

        return 0;
    }
}
