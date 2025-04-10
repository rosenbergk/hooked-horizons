// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Total pounds of fish caught in the current level/game session.
    public float TotalFishPounds { get; private set; } = 0f;

    // Weight ranges for each fish type (index 0 = fish type 1, index 1 = fish type 2, ...).
    // Each Vector2 stores (minWeight, maxWeight).
    // Set these in the Inspector or assign default values here.
    public Vector2[] fishWeightRanges = new Vector2[5];

    // Ensure that only one GameManager exists
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

    // Call this method when a fish is caught.
    // The parameter fishType is expected to be between 1 and 5.
    public void RegisterFishCatch(int fishType)
    {
        int index = fishType - 1;
        if (fishWeightRanges != null && index < fishWeightRanges.Length)
        {
            // Get the weight range for the fish type.
            Vector2 range = fishWeightRanges[index];
            // Pick a random weight between min and max.
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

    // Resets the total fish weight.
    public void ResetTotalFishPounds()
    {
        TotalFishPounds = 0f;
    }

    // Example level load method.
    // When a new level is loaded, we reset the fish total (or you can manage totals per session as needed).
    public void LoadLevel(string levelName)
    {
        // Reset accumulated fish weight on level change.
        ResetTotalFishPounds();
        SceneManager.LoadScene(levelName);
    }
}
