// FishCatchNotifier.cs
using System.Collections;
using TMPro;
using UnityEngine;

public class FishCatchNotifier : MonoBehaviour
{
    public static FishCatchNotifier Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI notificationText;

    [SerializeField]
    private float displayDuration = 2f;

    private Coroutine displayRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }

    public void ShowCatchNotification(float weight, string fishName)
    {
        if (notificationText == null)
            return;

        notificationText.text = $"You caught a {weight:F2} lb. {fishName}!";
        notificationText.gameObject.SetActive(true);

        if (displayRoutine != null)
            StopCoroutine(displayRoutine);
        displayRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (notificationText != null)
            notificationText.gameObject.SetActive(false);
    }
}
