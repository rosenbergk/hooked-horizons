// BoatRock.cs
using UnityEngine;

public class BoatRock : MonoBehaviour
{
    [SerializeField]
    private float rockingSpeed = 1f;

    [SerializeField]
    private float basePitchAmount = 1f;

    [SerializeField]
    private float baseRollAmount = 1f;

    [SerializeField]
    private float variationAmount = 0.3f;

    [SerializeField]
    private float transitionSpeed = 0.5f;

    private Vector3 initialRotation;

    private float currentPitchAmount;
    private float targetPitchAmount;
    private float nextPitchUpdateTime;

    private float currentRollAmount;
    private float targetRollAmount;
    private float nextRollUpdateTime;

    void Start()
    {
        initialRotation = transform.eulerAngles;

        currentPitchAmount = basePitchAmount;
        targetPitchAmount = basePitchAmount;
        SetNewTargetPitchAmount();

        currentRollAmount = baseRollAmount;
        targetRollAmount = baseRollAmount;
        SetNewTargetRollAmount();
    }

    void Update()
    {
        currentPitchAmount = Mathf.Lerp(
            currentPitchAmount,
            targetPitchAmount,
            Time.deltaTime * transitionSpeed
        );
        if (Time.time >= nextPitchUpdateTime)
        {
            SetNewTargetPitchAmount();
        }

        currentRollAmount = Mathf.Lerp(
            currentRollAmount,
            targetRollAmount,
            Time.deltaTime * transitionSpeed
        );
        if (Time.time >= nextRollUpdateTime)
        {
            SetNewTargetRollAmount();
        }

        float pitchRock = Mathf.Sin(Time.time * rockingSpeed) * currentPitchAmount;
        float rollRock = Mathf.Sin(Time.time * rockingSpeed * 1.1f) * currentRollAmount;

        transform.eulerAngles = initialRotation + new Vector3(pitchRock, 0f, rollRock);
    }

    void SetNewTargetPitchAmount()
    {
        targetPitchAmount = basePitchAmount + Random.Range(-variationAmount, variationAmount);
        nextPitchUpdateTime = Time.time + Random.Range(2f, 4f);
    }

    void SetNewTargetRollAmount()
    {
        targetRollAmount = baseRollAmount + Random.Range(-variationAmount, variationAmount);
        nextRollUpdateTime = Time.time + Random.Range(2f, 4f);
    }
}
