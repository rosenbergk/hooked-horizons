using System;
using UnityEngine;
using UnityEngine.UI;

public class TugOfWarManager : MonoBehaviour
{
    public static TugOfWarManager Instance;

    public Slider tugSlider;
    public TugOfWarUI ui;
    public float maxValue = 100f;
    public float rightRedStart = 85f;
    public float greenWidth = 10f;
    public float decayRate = 0.8f;
    public float boostAmount = 5f;
    public float encroachRate = 1f;
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
            tugSlider.value = Mathf.Min(tugSlider.value + boostAmount, maxValue);
        }

        float greenStart = (maxValue - greenWidth) * 0.5f;
        float greenEnd = greenStart + greenWidth;

        if (tugSlider.value <= currentLeftRedEnd || tugSlider.value >= currentRightRedEnd)
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
            timeInGreen = 0f;
        }
        else if (tugSlider.value > greenEnd)
        {
            currentRightRedEnd = Mathf.Max(
                currentRightRedEnd - encroachRate * Time.deltaTime,
                greenEnd
            );
            timeInGreen = 0f;
        }
        else
        {
            timeInGreen += Time.deltaTime;
            if (timeInGreen >= requiredGreenTime)
            {
                Success();
                return;
            }
        }

        timeInGreen = Mathf.Clamp(timeInGreen, 0f, requiredGreenTime);

        ui.UpdateZones(currentLeftRedEnd, currentRightRedEnd);
    }

    public void ActivateSlider(float fishDecay)
    {
        decayRate = fishDecay;
        sliderActive = true;
        tugSlider.gameObject.SetActive(true);

        ui.Initialize(maxValue, greenWidth);
        ActivateUI();

        currentLeftRedEnd = maxValue - rightRedStart;
        currentRightRedEnd = rightRedStart;
        timeInGreen = 0f;
        tugSlider.value = maxValue * 0.5f;
    }

    public void DeactivateSlider()
    {
        sliderActive = false;
        tugSlider.gameObject.SetActive(false);
    }

    private void Fail()
    {
        DeactivateSlider();
        DeactivateUI();
        OnFailure?.Invoke();
        Debug.Log("TugOfWar: you drifted into the red zone — failure!");
    }

    private void Success()
    {
        DeactivateSlider();
        DeactivateUI();
        OnSuccess?.Invoke();
        Debug.Log("TugOfWar: 5 seconds perched in green — success!");
    }

    private void ActivateUI()
    {
        ui.SetActive(true);
    }

    private void DeactivateUI()
    {
        ui.SetActive(false);
    }
}
