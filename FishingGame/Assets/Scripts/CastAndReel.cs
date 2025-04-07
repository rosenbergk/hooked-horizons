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
        // Start charging
        if (Input.GetMouseButtonDown(0) && !isCasting)
        {
            isCharging = true;
            currentCastForce = minCastForce;
        }

        // Charging
        if (Input.GetMouseButton(0) && isCharging)
        {
            currentCastForce += chargeSpeed * Time.deltaTime;
            currentCastForce = Mathf.Clamp(currentCastForce, minCastForce, maxCastForce);

            // Show cast arc while charging
            Vector3 castDirection = mainCam.transform.forward;
            castDirection.y = Mathf.Clamp(castDirection.y, -0.1f, 0.3f);
            arcPreview.ShowArc(castDirection, currentCastForce);
        }
        else
        {
            arcPreview.HideArc();
        }

        // Release to cast
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            CastHook();
            isCharging = false;
            arcPreview.HideArc();
        }

        // Reel
        isReeling = Input.GetKey(KeyCode.R);
    }

    void FixedUpdate()
    {
        if (isReeling && !hookRb.isKinematic)
        {
            Vector3 direction = (rodTip.position - hookRb.position).normalized;
            hookRb.linearVelocity = direction * reelSpeed;

            if (Vector3.Distance(hookRb.position, rodTip.position) < 0.5f)
            {
                hookRb.isKinematic = true;
                hookRb.linearVelocity = Vector3.zero;
                isCasting = false;
                fishCaught = false;
                nextRollTime = 0f;
            }
        }

        if (!hookRb.isKinematic && hookRb.position.y < waterLevel)
        {
            float depth = waterLevel - hookRb.position.y;
            Vector3 buoyancy = Vector3.up * depth * 2f;
            hookRb.AddForce(buoyancy, ForceMode.Acceleration);
        }

        if (isCasting && !fishCaught && Time.time >= nextRollTime)
        {
            int catchResult = fishCatcher.RollForCatch();

            nextRollTime = Time.time + 2f;

            if (catchResult == 0)
            {
                Debug.Log("No fish caught this time.");
            }
            else
            {
                Debug.Log("Fish caught: Fish" + catchResult);
                fishCaught = true;
            }
        }
    }

    void CastHook()
    {
        isCasting = true;
        hookRb.isKinematic = false;

        nextRollTime = Time.time + 2f;

        // Step 1: Base direction is forward from camera
        Vector3 forward = mainCam.transform.forward;
        forward.y = 0; // flatten it to horizontal
        forward.Normalize();

        // Step 2: Add upward arc
        Vector3 up = Vector3.up;
        Vector3 castDirection = (forward + up * 0.5f).normalized;

        // Step 3: Apply force
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
