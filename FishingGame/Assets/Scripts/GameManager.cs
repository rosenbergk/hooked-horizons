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

    private bool onDebug = false;
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

        if (onDebug)
        {
            rollInterval = 0.3f;
        }
        else if (scene.name == "TimedMode")
        {
            rollInterval = timedRollInterval;
        }
        else
        {
            rollInterval = freeplayRollInterval;
        }

        Debug.Log($"[GameManager] Loaded {scene.name}, rollInterval = {rollInterval:F2}s");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            onDebug = !onDebug;
            Debug.Log(
                onDebug ? "DEBUG MODE: fast roll enabled" : "DEBUG MODE: normal roll restored"
            );
            if (onDebug)
                SetToDebug();
            else
                RemoveDebug();
        }
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public float GetRollInterval() => rollInterval;

    public void SetToDebug()
    {
        rollInterval = 0.3f;
    }

    private void RemoveDebug()
    {
        var scene = SceneManager.GetActiveScene();
        rollInterval = (scene.name == "TimedMode") ? timedRollInterval : freeplayRollInterval;
    }
}
