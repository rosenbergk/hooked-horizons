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
    private float rollInterval = 5f;

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
    private int pendingFishType = 0;
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetToDebug();
            Debug.Log("Set to debug");
        }

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

    public bool IsReeling()
    {
        return isReeling;
    }

    public bool IsCasting()
    {
        return isCasting;
    }

    public float GetHookRodDistance()
    {
        return Vector3.Distance(hookRb.position, rodTip.position);
    }

    public float GetHookRestDistance()
    {
        return hookRestDistance;
    }

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
        nextRollTime = Time.time + rollInterval;
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

        if (HasHookReachedRest())
            CompleteReelSuccess();
    }

    private bool HasHookReachedRest()
    {
        return Vector3.Distance(hookRb.position, rodTip.position) < hookRestDistance;
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
        if (!isCasting || fishCaught || Time.time < nextRollTime)
            return;

        int result = fishCatcher.RollForCatch();
        nextRollTime = Time.time + rollInterval;

        if (result == 0)
        {
            Debug.Log("RollForCatch: No fish.");
        }
        else
        {
            HandleFishHooked(result);
        }
    }

    private void HandleFishHooked(int fishType)
    {
        Debug.Log($"RollForCatch: Fish hooked: {fishType}");
        fishCaught = true;
        pendingFishType = fishType;
        currentFish = SpawnAndAttachFish(fishType);
        ActivateTugOfWar(fishType);
    }

    private GameObject SpawnAndAttachFish(int fishType)
    {
        int index = fishType - 1;
        if (fishPrefabs == null || index < 0 || index >= fishPrefabs.Length)
            return null;

        GameObject instance = Instantiate(
            fishPrefabs[index],
            hookRb.transform.position,
            Quaternion.identity
        );
        Fish fish = instance.GetComponent<Fish>();
        if (fish != null)
            fish.Catch(hookRb.transform);
        else
            instance.transform.SetParent(hookRb.transform);
        return instance;
    }

    private void ActivateTugOfWar(int fishType)
    {
        float decay = SetFishDecayRate(fishType);
        TugOfWarManager.Instance.ActivateSlider(decay);
    }

    private void HandleFishCaughtSuccess()
    {
        FinalizeCatch();
        ResetHookState();
    }

    private void HandleFishLost()
    {
        ReleaseOnFailure();
        ResetHookState();
    }

    private void FinalizeCatch()
    {
        if (pendingFishType <= 0 || currentFish == null)
            return;
        float weight = FishWeightManager.Instance.RegisterFishCatch(pendingFishType);
        string name = FishNaming.GetFishName(pendingFishType);
        Fish fish = currentFish.GetComponent<Fish>();
        if (fish != null)
            fish.SetFishName(name);
        FishCatchNotifier.Instance.ShowCatchNotification(weight, name);
    }

    private void ReleaseOnFailure()
    {
        if (currentFish != null)
        {
            Destroy(currentFish);
            currentFish = null;
        }
    }

    private void ResetHookState()
    {
        isCasting = false;
        fishCaught = false;
        pendingFishType = 0;
        nextRollTime = 0f;
    }

    private float SetFishDecayRate(int catchResult)
    {
        return 3f + (catchResult - 1) * 1.2f;
    }

    // ONLY FOR DEBUGGING
    private void SetToDebug()
    {
        rollInterval = 0.3f;
    }
}
