using UnityEngine;

public class CastAndReel : MonoBehaviour
{
    public Rigidbody hookRb;
    public Transform rodTip;
    public CastArcPreview arcPreview;
    public GameObject[] fishPrefabs;

    [SerializeField]
    private float minCastForce = 5f;

    [SerializeField]
    private float maxCastForce = 20f;

    [SerializeField]
    private float chargeSpeed = 10f;

    [SerializeField]
    private float reelSpeed = 5f;

    [SerializeField]
    private float hookRestDistance;

    private Camera mainCam;
    private FishCatcher fishCatcher;
    private bool isCharging = false;
    private bool isCasting = false;
    private bool isReeling = false;

    private float currentCastForce;
    private GameObject currentFish;
    private bool fishCaught = false;
    private float nextRollTime = 0f;

    void Start()
    {
        mainCam = Camera.main;
        fishCatcher = FindAnyObjectByType<FishCatcher>();

        if (TugOfWarManager.Instance != null)
        {
            TugOfWarManager.Instance.OnSuccess += HandleFishCaughtSuccess;
            TugOfWarManager.Instance.OnFailure += HandleFishLost;
        }
    }

    void Update()
    {
        HandleChargeInput();
        ProcessCharging();
        HandleCastRelease();
        UpdateReelingInput();
    }

    void FixedUpdate()
    {
        UpdateReelingMotion();
        TryCatchFish();
    }

    public bool IsReeling() => isReeling;

    public bool IsCasting() => isCasting;

    public float GetHookRodDistance() => Vector3.Distance(hookRb.position, rodTip.position);

    public float GetHookRestDistance() => hookRestDistance;

    private void HandleChargeInput()
    {
        if (Input.GetMouseButtonDown(0) && !isCasting)
        {
            isCharging = true;
            currentCastForce = minCastForce;
        }
    }

    private void ProcessCharging()
    {
        if (Input.GetMouseButton(0) && isCharging)
        {
            currentCastForce += chargeSpeed * Time.deltaTime;
            currentCastForce = Mathf.Clamp(currentCastForce, minCastForce, maxCastForce);
            Vector3 dir = mainCam.transform.forward;
            dir.y = Mathf.Clamp(dir.y, -0.1f, 0.3f);
            arcPreview.ShowArc(dir, currentCastForce);
        }
        else
        {
            arcPreview.HideArc();
        }
    }

    private void HandleCastRelease()
    {
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            CastHook();
            isCharging = false;
            arcPreview.HideArc();
        }
    }

    private void CastHook()
    {
        isCasting = true;
        hookRb.isKinematic = false;
        nextRollTime = Time.time + GameManager.Instance.GetRollInterval();
        Vector3 forward = mainCam.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 castDirection = (forward + Vector3.up * 0.5f).normalized;
        hookRb.linearVelocity = Vector3.zero;
        hookRb.AddForce(castDirection * currentCastForce, ForceMode.VelocityChange);
        Debug.Log("Hook cast with force: " + currentCastForce);
    }

    private void UpdateReelingInput()
    {
        isReeling =
            Input.GetKey(KeyCode.R)
            && (
                TugOfWarManager.Instance.IsSliderInGreen()
                || !TugOfWarManager.Instance.IsSliderActive()
            );
    }

    private void UpdateReelingMotion()
    {
        if (!isReeling || hookRb.isKinematic)
            return;
        hookRb.linearVelocity = Vector3.zero;
        Vector3 dir = (rodTip.position - hookRb.position).normalized;
        hookRb.linearVelocity = dir * reelSpeed;
        if (Vector3.Distance(hookRb.position, rodTip.position) < hookRestDistance)
            CompleteReelSuccess();
    }

    private void CompleteReelSuccess()
    {
        hookRb.isKinematic = true;
        hookRb.linearVelocity = Vector3.zero;
        TugOfWarManager.Instance.Success();
        ReleaseCurrentFish();
        ResetHookState();
    }

    private void ReleaseCurrentFish()
    {
        if (currentFish == null)
            return;
        Fish fishScript = currentFish.GetComponent<Fish>();
        if (fishScript != null)
            fishScript.ReleaseAndDisappear(2f);
        else
            Destroy(currentFish, 2f);
        currentFish = null;
    }

    private void TryCatchFish()
    {
        if (
            !isCasting
            || fishCaught
            || Time.time < nextRollTime
            || hookRb.transform.position.y > 0.5
        )
            return;

        int fishType = fishCatcher.RollForCatch();
        nextRollTime = Time.time + GameManager.Instance.GetRollInterval();

        if (fishType == 0)
        {
            Debug.Log("RollForCatch: No fish.");
        }
        else
        {
            float weight = FishWeightManager.Instance.DetermineFishWeight(fishType);
            string fishName = FishNaming.GetFishName(fishType);
            HandleFishHooked(fishType, weight, fishName);
        }
    }

    private void HandleFishHooked(int fishType, float fishWeight, string fishName)
    {
        Debug.Log($"Fish hooked (type {fishType}): weight={fishWeight:F2}, name={fishName}");
        fishCaught = true;

        currentFish = Instantiate(
            fishPrefabs[fishType - 1],
            hookRb.transform.position,
            Quaternion.identity
        );
        Fish fishComponent = currentFish.GetComponent<Fish>();
        if (fishComponent != null)
        {
            fishComponent.Catch(hookRb.transform);
            fishComponent.SetFishWeight(fishWeight);
            fishComponent.SetFishName(fishName);
        }
        else
        {
            currentFish.transform.SetParent(hookRb.transform);
        }

        float decay = TugOfWarManager.Instance.CalculateDecayRate(fishWeight);
        TugOfWarManager.Instance.ActivateSlider(decay);
    }

    private void HandleFishCaughtSuccess()
    {
        if (currentFish != null)
        {
            Fish fishComponent = currentFish.GetComponent<Fish>();
            if (fishComponent != null)
            {
                float weight = fishComponent.GetFishWeight();
                FishWeightManager.Instance.AddFishWeight(weight);

                FishCatchNotifier.Instance.ShowCatchNotification(weight, fishComponent.FishName);

                TopFishTracker.Instance.RecordFish(name, weight);
            }
        }
        FindAnyObjectByType<Tutorial>()?.OnFirstFishCaught();

        ResetHookState();
    }

    private void HandleFishLost()
    {
        ReleaseCurrentFish();
        ResetHookState();
    }

    private void ResetHookState()
    {
        isCasting = false;
        fishCaught = false;
        nextRollTime = 0f;
    }
}
