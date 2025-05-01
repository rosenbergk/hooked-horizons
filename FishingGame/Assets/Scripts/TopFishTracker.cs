// TopFishTracker.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TopFishTracker : MonoBehaviour
{
    public static TopFishTracker Instance { get; private set; }

    private class FishRecord
    {
        public string Name;
        public float Weight;

        public FishRecord(string n, float w)
        {
            Name = n;
            Weight = w;
        }
    }

    private List<FishRecord> topFish = new List<FishRecord>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Call this whenever a fish is successfully reeled in.
    /// </summary>
    public void RecordFish(string fishName, float weight)
    {
        topFish.Add(new FishRecord(fishName, weight));
        // sort descending, keep only top 10
        topFish = topFish.OrderByDescending(f => f.Weight).Take(10).ToList();
    }

    /// <summary>
    /// Returns the current top list (may be <10 entries).
    /// </summary>
    public List<(string name, float weight)> GetTopFish()
    {
        return topFish.Select(f => (f.Name, f.Weight)).ToList();
    }
}
