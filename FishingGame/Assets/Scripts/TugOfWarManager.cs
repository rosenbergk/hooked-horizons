using UnityEngine;
using UnityEngine.UI;

public class TugOfWarManager : MonoBehaviour
{
    public Slider tugSlider;
    public float reelMultiplier = 1f;

    [SerializeField]
    private float tensionIncreaseRate = 0.2f;

    [SerializeField]
    private float tensionDecayRate = 0.1f;

    [SerializeField]
    private float fishPullForce = 0.05f;

    [SerializeField]
    private float successBoost = 0.15f;

    [SerializeField]
    private float penaltyDrain = 0.1f;

    [SerializeField]
    private float sweetSpotCenter = 0.5f;

    [SerializeField]
    private float sweetSpotWidth = 0.1f;
    private float currentTension = 0.5f;

    void Start()
    {
        if (tugSlider == null)
            Debug.LogError("tugSlider not assigned!");
        else
        {
            tugSlider.value = currentTension;
            Debug.Log("TugOfWarManager started.");
        }
    }

    void Update()
    {
        currentTension +=
            (currentTension > sweetSpotCenter ? -fishPullForce : fishPullForce) * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsWithinSweetSpot())
            {
                currentTension += successBoost;
                Debug.Log("Space pressed: within sweet spot.");
            }
            else
            {
                currentTension -= penaltyDrain;
                Debug.Log("Space pressed: outside sweet spot.");
            }
        }
        currentTension = Mathf.Lerp(
            currentTension,
            sweetSpotCenter,
            tensionDecayRate * Time.deltaTime
        );
        currentTension = Mathf.Clamp01(currentTension);
        if (tugSlider != null)
            tugSlider.value = currentTension;
        reelMultiplier = currentTension;
    }

    private bool IsWithinSweetSpot()
    {
        return currentTension >= (sweetSpotCenter - sweetSpotWidth)
            && currentTension <= (sweetSpotCenter + sweetSpotWidth);
    }

    public void SetDifficulty(float newSweetSpotWidth, float newFishPullForce)
    {
        sweetSpotWidth = newSweetSpotWidth;
        fishPullForce = newFishPullForce;
        Debug.Log("Difficulty updated.");
    }

    public void ActivateSlider()
    {
        tugSlider.gameObject.SetActive(true);
    }

    public void DeactivateSlider()
    {
        tugSlider.gameObject.SetActive(false);
    }
}
