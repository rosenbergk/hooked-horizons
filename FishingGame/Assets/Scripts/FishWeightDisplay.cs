// FishWeightDisplay.cs
using System.Collections;
using TMPro;
using UnityEngine;

public class FishWeightDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI fishWeightText;

    [SerializeField]
    private float popScale;

    [SerializeField]
    private float popDuration = 1;

    private Vector3 originalScale;
    private Coroutine popRoutine;

    public void Start()
    {
        fishWeightText.text = $"Fish caught: {FishWeightManager.Instance.TotalFishPounds:F2} lbs.";
        originalScale = fishWeightText.transform.localScale;
    }

    public void UpdateFishWeightText()
    {
        if (FishWeightManager.Instance != null && fishWeightText != null)
        {
            fishWeightText.text =
                $"Fish caught: {FishWeightManager.Instance.TotalFishPounds:F2} lbs.";
        }

        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
        }

        popRoutine = StartCoroutine(PopEffect());
    }

    private IEnumerator PopEffect()
    {
        fishWeightText.transform.localScale = originalScale * popScale;
        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            fishWeightText.transform.localScale = Vector3.Lerp(
                originalScale * popScale,
                originalScale,
                elapsed / popDuration
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        fishWeightText.transform.localScale = originalScale;
    }
}
