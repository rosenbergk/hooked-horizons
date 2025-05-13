// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private float timedRollInterval = 0.5f;

    [SerializeField]
    private float freeplayRollInterval = 4f;

    private float rollInterval;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FishWeightManager.Instance?.ResetTotalFishPounds();

        if (scene.name == "TimedMode")
        {
            rollInterval = timedRollInterval;
        }
        else
        {
            rollInterval = freeplayRollInterval;
        }
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public float GetRollInterval() => rollInterval;
}
