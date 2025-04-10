using UnityEngine;

public class CastAndReel : MonoBehaviour
{
    public Rigidbody hookRb;
    public Transform rodPivot;
    public Transform rodTip;
    public CastArcPreview arcPreview;

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

    // Array of fish prefabs where index 0 corresponds to fish type 1, index 1 to fish type 2, etc.
    public GameObject[] fishPrefabs;

    // Holds the current fish instance (if one is caught)
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
    }

    void Update()
    {
        // Start charging the cast
        if (Input.GetMouseButtonDown(0) && !isCasting)
        {
            isCharging = true;
            currentCastForce = minCastForce;
        }

        // Charging: increase cast force and show the cast arc
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

        // Release to cast the hook
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            CastHook();
            isCharging = false;
            arcPreview.HideArc();
        }

        // Check for reeling input
        isReeling = Input.GetKey(KeyCode.R);
    }

    void FixedUpdate()
    {
        // Reel the hook in when reeling is active
        if (isReeling && !hookRb.isKinematic)
        {
            hookRb.linearVelocity = Vector3.zero;
            Vector3 direction = (rodTip.position - hookRb.position).normalized;
            hookRb.linearVelocity = direction * reelSpeed;

            // When the hook is close enough to the rod tip, finish reeling
            if (Vector3.Distance(hookRb.position, rodTip.position) < 0.5f)
            {
                hookRb.isKinematic = true;
                hookRb.linearVelocity = Vector3.zero;
                isCasting = false;
                fishCaught = false;
                nextRollTime = 0f;

                // If a fish is attached, trigger its disappearance after a 2-second delay
                if (currentFish != null)
                {
                    Fish fishScript = currentFish.GetComponent<Fish>();
                    if (fishScript != null)
                    {
                        fishScript.ReleaseAndDisappear(2f);
                    }
                    else
                    {
                        Destroy(currentFish, 2f);
                    }
                    currentFish = null;
                }
            }
        }

        // Apply buoyancy if the hook is underwater and not being reeled in
        if (!hookRb.isKinematic && !isReeling && hookRb.position.y < waterLevel)
        {
            float depth = waterLevel - hookRb.position.y;
            Vector3 buoyancy = Vector3.up * depth * buoyancyForce;
            hookRb.AddForce(buoyancy, ForceMode.Acceleration);
        }

        // While casting, roll for a catch at set intervals
        if (isCasting && !fishCaught && Time.time >= nextRollTime)
        {
            int catchResult = fishCatcher.RollForCatch();
            nextRollTime = Time.time + rollInterval;

            if (catchResult == 0)
            {
                Debug.Log("No fish caught this time.");
            }
            else
            {
                Debug.Log("Fish caught: Fish" + catchResult);
                fishCaught = true;

                // Map catch result to prefab index (result 1 => index 0, etc.)
                int fishIndex = catchResult - 1;
                if (
                    fishPrefabs != null
                    && fishIndex >= 0
                    && fishIndex < fishPrefabs.Length
                    && currentFish == null
                )
                {
                    // Instantiate the fish prefab at the hook's position
                    GameObject fishInstance = Instantiate(
                        fishPrefabs[fishIndex],
                        hookRb.transform.position,
                        Quaternion.identity
                    );
                    Fish fishScript = fishInstance.GetComponent<Fish>();
                    if (fishScript != null)
                    {
                        // Parent the fish to the hook and offset it so that its mouth lines up with the hook
                        fishScript.Catch(hookRb.transform);
                    }
                    else
                    {
                        // Fallback: simply parent it if the Fish script is not present
                        fishInstance.transform.SetParent(hookRb.transform);
                    }
                    currentFish = fishInstance;
                }
            }
        }
    }

    void CastHook()
    {
        isCasting = true;
        hookRb.isKinematic = false;
        nextRollTime = Time.time + rollInterval;

        // Base direction: forward from the camera, flattened to horizontal
        Vector3 forward = mainCam.transform.forward;
        forward.y = 0;
        forward.Normalize();

        // Add upward arc to the cast direction
        Vector3 up = Vector3.up;
        Vector3 castDirection = (forward + up * 0.5f).normalized;

        hookRb.linearVelocity = Vector3.zero;
        hookRb.AddForce(castDirection * currentCastForce, ForceMode.VelocityChange);
    }

    public bool IsCasting()
    {
        return isCasting;
    }

    public bool IsReeling()
    {
        return isReeling;
    }
}
