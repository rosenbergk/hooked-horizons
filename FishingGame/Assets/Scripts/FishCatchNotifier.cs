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
    private float popScale;

    [SerializeField]
    private float popDuration;

    [SerializeField]
    private float endScaleDuration;

    [SerializeField]
    private float displayDuration;

    private Vector3 originalScale;
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
        {
            notificationText.gameObject.SetActive(false);
            originalScale = notificationText.transform.localScale;
        }
    }

    public void ShowCatchNotification(float weight, string fishName)
    {
        if (notificationText == null)
            return;

        notificationText.text = $"You caught a {weight:F2} lb. {fishName}!";
        notificationText.gameObject.SetActive(true);

        if (displayRoutine != null)
            StopCoroutine(displayRoutine);
        displayRoutine = StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        float halfPop = popDuration / 2f;
        float elapsed = 0f;
        while (elapsed < halfPop)
        {
            float t = elapsed / halfPop;
            notificationText.transform.localScale = Vector3.Lerp(
                originalScale,
                originalScale * popScale,
                t
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < halfPop)
        {
            float t = elapsed / halfPop;
            notificationText.transform.localScale = Vector3.Lerp(
                originalScale * popScale,
                originalScale,
                t
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
        notificationText.transform.localScale = originalScale;

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < endScaleDuration)
        {
            float t = elapsed / endScaleDuration;
            notificationText.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        notificationText.gameObject.SetActive(false);
        notificationText.transform.localScale = originalScale;
    }
}
