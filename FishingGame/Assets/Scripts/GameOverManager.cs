// GameOverManager.cs
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [SerializeField]
    private GameObject endPanel;

    [SerializeField]
    private TextMeshProUGUI finalScoreText;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShowGameOver(float weight)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        endPanel.SetActive(true);

        finalScoreText.text = "Total weight: " + weight + " lbs.";
    }

    public void RestartGame()
    {
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
