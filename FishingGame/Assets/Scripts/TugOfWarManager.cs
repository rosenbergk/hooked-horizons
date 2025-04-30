using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TugOfWarManager : MonoBehaviour
{
    public static TugOfWarManager Instance;

    public Slider tugSlider;
    public TugOfWarUI ui;

    [SerializeField]
    private float maxValue = 100f;

    [SerializeField]
    private float rightRedStart = 85f;

    [SerializeField]
    private float greenWidth = 10f;

    [SerializeField]
    private float baseFishDecay = 5f;

    [SerializeField]
    private float timedModeDecayMultiplier = 1.8f;

    [SerializeField]
    private float fishWeightMultiplier = 2.3f;

    [SerializeField]
    private float encroachRate = 1f;

    [SerializeField]
    private float lossBuffer = 4f;

    [SerializeField]
    private float minBoostAmount = 2f;

    [SerializeField]
    private float maxBoostAmount = 10f;

    [SerializeField]
    private float minGreenStart = 30f;

    [SerializeField]
    private float maxGreenStart = 60f;

    private float currentBaseFishDecay;
    private float decayRate;
    private float greenStartOffset;
    private float currentLeftRedEnd;
    private float currentRightRedEnd;
    private bool sliderActive;

    public event Action OnFailure;
    public event Action OnSuccess;

    void Awake()
    {
        Instance = this;
        currentBaseFishDecay = baseFishDecay;

        if (SceneManager.GetActiveScene().name == "TimedMode")
        {
            currentBaseFishDecay = baseFishDecay * timedModeDecayMultiplier;
        }
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
            DeactivateUI();
        }
    }

    void Update()
    {
        if (!sliderActive)
            return;

        tugSlider.value = Mathf.Max(tugSlider.value - decayRate * Time.deltaTime, 0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Boost();
        }

        float greenStart = greenStartOffset;
        float greenEnd = greenStart + greenWidth;

        ui.UpdateZones(currentLeftRedEnd, currentRightRedEnd);

        CheckSliderValue(greenStart, greenEnd);
    }

    public void ActivateSlider(float fishDecay)
    {
        decayRate = fishDecay;
        greenStartOffset = UnityEngine.Random.Range(minGreenStart, maxGreenStart);

        Debug.Log("Decay rate is " + decayRate);
        sliderActive = true;
        tugSlider.gameObject.SetActive(true);

        ui.Initialize(maxValue, greenWidth, greenStartOffset);
        ActivateUI();

        currentLeftRedEnd = maxValue - rightRedStart;
        currentRightRedEnd = rightRedStart;
        tugSlider.value = maxValue * 0.5f;
    }

    public void DeactivateSlider()
    {
        sliderActive = false;
        tugSlider.gameObject.SetActive(false);
    }

    public bool IsSliderInGreen()
    {
        float greenStart = greenStartOffset;
        float greenEnd = greenStart + greenWidth;
        float val = tugSlider.value;

        return val >= greenStart && val <= greenEnd;
    }

    public void Success()
    {
        DeactivateSlider();
        DeactivateUI();
        OnSuccess?.Invoke();
        Debug.Log("TugOfWar: hook fully reeled — success!");
    }

    public bool IsSliderActive()
    {
        return sliderActive;
    }

    public float CalculateDecayRate(float weight)
    {
        return currentBaseFishDecay + fishWeightMultiplier * Mathf.Pow(weight, 1f / 3f); // Magic numbers for cube root
    }

    private void Fail()
    {
        DeactivateSlider();
        DeactivateUI();
        OnFailure?.Invoke();
        Debug.Log("TugOfWar: you drifted into the red zone — failure!");
    }

    private void ActivateUI()
    {
        ui.SetActive(true);
    }

    private void DeactivateUI()
    {
        ui.SetActive(false);
    }

    private void CheckSliderValue(float greenStart, float greenEnd)
    {
        if (
            tugSlider.value + lossBuffer <= currentLeftRedEnd
            || tugSlider.value - lossBuffer >= currentRightRedEnd
        )
        {
            Fail();
            return;
        }

        if (tugSlider.value < greenStart)
        {
            currentLeftRedEnd = Mathf.Min(
                currentLeftRedEnd + encroachRate * Time.deltaTime,
                greenStart
            );
        }
        else if (tugSlider.value > greenEnd)
        {
            currentRightRedEnd = Mathf.Max(
                currentRightRedEnd - encroachRate * Time.deltaTime,
                greenEnd
            );
        }
    }

    private float DetermineBoostAmount()
    {
        return UnityEngine.Random.Range(minBoostAmount, maxBoostAmount);
    }

    private void Boost()
    {
        float boostAmount = DetermineBoostAmount();
        tugSlider.value = Mathf.Min(tugSlider.value + boostAmount, maxValue);
    }
}
