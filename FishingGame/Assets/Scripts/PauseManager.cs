// PauseManager.cs
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuUI;

    private bool isPaused = false;

    private CastAndReel castAndReel;

    void Start()
    {
        castAndReel = FindAnyObjectByType<CastAndReel>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!GameOverManager.IsGameOver)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        castAndReel.enabled = false;
    }

    private void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        castAndReel.enabled = true;
    }

    public void GoToMainMenu()
    {
        GameOverManager.GoToMainMenu();
    }
}
