// FishWeightManager.cs
using UnityEngine;

public class FishWeightManager : MonoBehaviour
{
    public static FishWeightManager Instance { get; private set; }

    public float TotalFishPounds { get; private set; } = 0f;

    public Vector2[] fishWeightRanges = new Vector2[5];

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterFishCatch(int fishType)
    {
        int index = fishType - 1;
        if (fishWeightRanges != null && index < fishWeightRanges.Length)
        {
            // Get the weight range for the fish type.
            Vector2 range = fishWeightRanges[index];
            // Pick a random weight between the defined min and max.
            float weight = Random.Range(range.x, range.y);
            TotalFishPounds += weight;
            Debug.Log(
                $"Caught fish type {fishType} weighing {weight:F2} lbs. Total: {TotalFishPounds:F2} lbs."
            );
        }
        else
        {
            Debug.LogWarning("No defined weight range for fish type: " + fishType);
        }
    }

    public void ResetTotalFishPounds()
    {
        TotalFishPounds = 0f;
    }
}
