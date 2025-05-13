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

    public void RecordFish(string fishName, float weight)
    {
        topFish.Add(new FishRecord(fishName, weight));
        topFish = topFish.OrderByDescending(f => f.Weight).Take(10).ToList();
    }

    public List<(string name, float weight)> GetTopFish()
    {
        return topFish.Select(f => (f.Name, f.Weight)).ToList();
    }
}
