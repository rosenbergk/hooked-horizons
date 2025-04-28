// LevelSelectorManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorManager : MonoBehaviour
{
    public void StartOpenWater()
    {
        GameManager.Instance.StartGame();
        SceneManager.LoadScene("OpenWater");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
