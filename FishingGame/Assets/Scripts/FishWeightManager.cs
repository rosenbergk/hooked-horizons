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
            Vector2 range = fishWeightRanges[index];
            float weight = Random.Range(range.x, range.y);
            TotalFishPounds += weight;
            FindAnyObjectByType<FishWeightDisplay>()?.UpdateFishWeightText();
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
