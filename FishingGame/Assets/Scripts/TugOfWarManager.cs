using System;
using UnityEngine;
using UnityEngine.UI;

public class TugOfWarManager : MonoBehaviour
{
    public static TugOfWarManager Instance;

    public Slider tugSlider;
    public float maxValue = 100f;
    public float leftRedStart = 0f;
    public float rightRedStart = 80f;
    public float greenWidth = 20f;
    public float decayRate = 0.1f;
    public float boostAmount = 5f;
    public float encroachRate = 1f;
    public float secondsRequiredInGreen;

    public float requiredGreenTime = 5f;

    private float currentLeftRedEnd;
    private float currentRightRedEnd;
    private float timeInGreen;
    private bool sliderActive;

    public event Action OnFailure;
    public event Action OnSuccess;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (tugSlider == null)
            Debug.LogError("TugOfWarManager: assign a Slider!");
        else
        {
            tugSlider.minValue = 0f;
            tugSlider.maxValue = maxValue;
            DeactivateSlider();
        }
    }

    void Update()
    {
        if (!sliderActive)
            return;

        // 1) continuous decay
        tugSlider.value = Mathf.Max(tugSlider.value - decayRate * Time.deltaTime, 0f);

        // 2) space boost
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tugSlider.value = Mathf.Min(tugSlider.value + boostAmount, maxValue);
        }

        // 3) compute zone boundaries
        float greenStart = (maxValue - greenWidth) * 0.5f;
        float greenEnd = greenStart + greenWidth;

        // 4) check fail immediately if in a red zone
        if (tugSlider.value <= currentLeftRedEnd || tugSlider.value >= currentRightRedEnd)
        {
            Fail();
            return;
        }

        // 5) if in left-yellow, encroach left red inward
        if (tugSlider.value < greenStart)
        {
            currentLeftRedEnd = Mathf.Min(
                currentLeftRedEnd + encroachRate * Time.deltaTime,
                greenStart
            );
            timeInGreen = 0f;
        }
        // 6) if in right-yellow, encroach right red inward
        else if (tugSlider.value > greenEnd)
        {
            currentRightRedEnd = Mathf.Max(
                currentRightRedEnd - encroachRate * Time.deltaTime,
                greenEnd
            );
            timeInGreen = 0f;
        }
        // 7) if in green, accumulate time
        else
        {
            timeInGreen += Time.deltaTime;
            if (timeInGreen >= requiredGreenTime)
            {
                Success();
                return;
            }
        }

        // 8) clamp timeInGreen so it never drifts
        timeInGreen = Mathf.Clamp(timeInGreen, 0f, requiredGreenTime);
    }

    /// <summary>
    /// Call this to kick off a tug-of-war for a newly hooked fish.
    /// </summary>
    /// <param name="fishDecay">How fast the slider should drift down (e.g. 0.1f for fish1, 0.2f for fish2, etc)</param>
    public void ActivateSlider(float fishDecay)
    {
        decayRate = fishDecay;
        sliderActive = true;
        tugSlider.gameObject.SetActive(true);

        // reset everything
        currentLeftRedEnd = leftRedStart;
        currentRightRedEnd = rightRedStart;
        timeInGreen = 0f;
        tugSlider.value = maxValue * 0.5f; // start in middle
    }

    public void DeactivateSlider()
    {
        sliderActive = false;
        tugSlider.gameObject.SetActive(false);
    }

    private void Fail()
    {
        DeactivateSlider();
        OnFailure?.Invoke();
        Debug.Log("TugOfWar: you drifted into the red zone — failure!");
    }

    private void Success()
    {
        DeactivateSlider();
        OnSuccess?.Invoke();
        Debug.Log("TugOfWar: 5 seconds perched in green — success!");
    }
}
