using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedModeManager : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Total time in seconds for this mode")]
    [SerializeField]
    private float timeLimit = 120f;

    [SerializeField]
    private float startDelay = 2f;

    [SerializeField]
    private TextMeshProUGUI timerText;

    private float remainingTime;
    private bool isRunning;

    void Start()
    {
        remainingTime = timeLimit;
        isRunning = false;
        UpdateTimerText();
        StartCoroutine(StartTimerWithDelay());
    }

    private IEnumerator StartTimerWithDelay()
    {
        yield return new WaitForSeconds(startDelay);
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning)
            return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame();
            return;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes}:{seconds:00}";
    }

    private void EndGame()
    {
        isRunning = false;

        // Disable player input
        var castAndReel = FindAnyObjectByType<CastAndReel>();
        if (castAndReel)
            castAndReel.enabled = false;
        var fishCatcher = FindAnyObjectByType<FishCatcher>();
        if (fishCatcher)
            fishCatcher.enabled = false;

        // Use GameOverManager to handle UI and restart/menu
        if (GameOverManager.Instance != null)
        {
            float finalScore =
                Mathf.Round(FishWeightManager.Instance.TotalFishPounds * 100f) / 100f;
            GameOverManager.Instance.ShowGameOver(finalScore);
        }
        else
        {
            Debug.LogWarning("GameOverManager instance not found in scene.");
        }
    }
}
