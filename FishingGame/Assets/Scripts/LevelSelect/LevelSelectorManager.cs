// LevelSelectorManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorManager : MonoBehaviour
{
    [SerializeField]
    private string openWaterSceneName = "OpenWater";

    [SerializeField]
    private string timedModeSceneName = "TimedMode";

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartTimedMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartOpenWater();
        }
    }

    public void StartOpenWater()
    {
        GameManager.Instance.StartGame();
        SceneManager.LoadScene(openWaterSceneName);
    }

    public void StartTimedMode()
    {
        GameManager.Instance.StartGame();
        SceneManager.LoadScene(timedModeSceneName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
