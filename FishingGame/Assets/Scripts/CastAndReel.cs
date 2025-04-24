using UnityEngine;

public class CastAndReel : MonoBehaviour
{
    public Rigidbody hookRb;
    public Transform rodPivot;
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
    private float waterLevel = 0f;

    [SerializeField]
    private float buoyancyForce;

    [SerializeField]
    private float rollInterval = 5f;

    [SerializeField]
    private float hookRestDistance;

    private GameObject currentFish;
    private float currentCastForce;
    private bool isCharging = false;
    private bool isCasting = false;
    private bool isReeling = false;
    private Camera mainCam;
    private FishCatcher fishCatcher;
    private bool fishCaught = false;
    private float nextRollTime = 0f;

    void Start()
    {
        mainCam = Camera.main;
        fishCatcher = FindAnyObjectByType<FishCatcher>();
        Debug.Log("CastAndReel started.");
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
        ProcessReeling();
        ProcessBuoyancy();
        ProcessFishCatching();
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
            Vector3 castDirection = mainCam.transform.forward;
            castDirection.y = Mathf.Clamp(castDirection.y, -0.1f, 0.3f);
            arcPreview.ShowArc(castDirection, currentCastForce);
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

    private void UpdateReelingInput()
    {
        isReeling = Input.GetKey(KeyCode.R);
    }

    private void ProcessReeling()
    {
        if (isReeling && !hookRb.isKinematic)
        {
            hookRb.linearVelocity = Vector3.zero;
            Vector3 direction = (rodTip.position - hookRb.position).normalized;
            hookRb.linearVelocity = direction * reelSpeed;

            if (Vector3.Distance(hookRb.position, rodTip.position) < hookRestDistance)
            {
                hookRb.isKinematic = true;
                hookRb.linearVelocity = Vector3.zero;
                isCasting = false;
                fishCaught = false;
                nextRollTime = 0f;
                if (currentFish != null)
                {
                    Fish fishScript = currentFish.GetComponent<Fish>();
                    if (fishScript != null)
                        fishScript.ReleaseAndDisappear(2f);
                    else
                        Destroy(currentFish, 2f);
                    currentFish = null;
                }

                Debug.Log("Hook distance: " + Vector3.Distance(hookRb.position, rodTip.position));
                Debug.Log(
                    "After activation, slider active: "
                        + TugOfWarManager.Instance.tugSlider.gameObject.activeSelf
                );

                TugOfWarManager.Instance.DeactivateSlider();
                Debug.Log("Reeling finished, slider deactivated.");
            }
        }
    }

    private void ProcessBuoyancy()
    {
        if (!hookRb.isKinematic && !isReeling && hookRb.position.y < waterLevel)
        {
            float depth = waterLevel - hookRb.position.y;
            hookRb.AddForce(Vector3.up * depth * buoyancyForce, ForceMode.Acceleration);
        }
    }

    private void ProcessFishCatching()
    {
        if (isCasting && !fishCaught && Time.time >= nextRollTime)
        {
            int catchResult = fishCatcher.RollForCatch();
            nextRollTime = Time.time + rollInterval;
            if (catchResult == 0)
            {
                Debug.Log("RollForCatch: No fish.");
            }
            else
            {
                Debug.Log("RollForCatch: Fish caught: " + catchResult);
                float fishWeight = FishWeightManager.Instance.RegisterFishCatch(catchResult);
                fishCaught = true;

                int fishIndex = catchResult - 1;
                if (
                    fishPrefabs != null
                    && fishIndex >= 0
                    && fishIndex < fishPrefabs.Length
                    && currentFish == null
                )
                {
                    GameObject fishInstance = Instantiate(
                        fishPrefabs[fishIndex],
                        hookRb.transform.position,
                        Quaternion.identity
                    );

                    string assignedName = FishNaming.GetFishName(catchResult);

                    Fish fish = fishInstance.GetComponent<Fish>();

                    if (fish != null)
                    {
                        fish.SetFishName(assignedName);
                        Debug.Log("Fish caught and named: " + assignedName);

                        fish.Catch(hookRb.transform);
                        FishCatchNotifier.Instance.ShowCatchNotification(fishWeight, assignedName);
                    }
                    else
                    {
                        fishInstance.transform.SetParent(hookRb.transform);
                    }

                    currentFish = fishInstance;
                    float fishDecayRate = 2f + (catchResult - 1) * 1.2f;
                    TugOfWarManager.Instance.ActivateSlider(fishDecayRate);
                }
            }
        }
    }

    void CastHook()
    {
        isCasting = true;
        hookRb.isKinematic = false;
        nextRollTime = Time.time + rollInterval;
        Vector3 forward = mainCam.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 castDirection = (forward + Vector3.up * 0.5f).normalized;
        hookRb.linearVelocity = Vector3.zero;
        hookRb.AddForce(castDirection * currentCastForce, ForceMode.VelocityChange);
        Debug.Log("Hook cast with force: " + currentCastForce);
    }

    public bool IsCasting()
    {
        return isCasting;
    }

    public bool IsReeling()
    {
        return isReeling;
    }

    public float GetHookRestDistance()
    {
        return hookRestDistance;
    }

    public float GetHookRodDistance()
    {
        return Vector3.Distance(hookRb.position, rodTip.position);
    }

    // ONLY FOR DEBUGGING
    private void SetToDebug()
    {
        rollInterval = 0.3f;
    }
}
