// GameOverManager.cs
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }
    public static bool IsGameOver { get; private set; } = false;

    [SerializeField]
    private GameObject endPanel;

    [SerializeField]
    private TextMeshProUGUI finalScoreText;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            IsGameOver = false;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShowGameOver(float weight)
    {
        IsGameOver = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        endPanel.SetActive(true);

        finalScoreText.text = "Total weight: " + weight + " lbs.";

        TugOfWarManager.Instance.DeactivateSlider();
        TugOfWarManager.Instance.ui.SetActive(false);
    }

    public void RestartGame()
    {
        IsGameOver = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void GoToMainMenu()
    {
        Time.timeScale = 1f;
        FishWeightManager.Instance.ResetTotalFishPounds();
        SceneManager.LoadScene("MainMenu");
    }
}
