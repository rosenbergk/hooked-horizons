using UnityEngine;

public class CastAndReel : MonoBehaviour
{
    public Rigidbody hookRb;
    public Transform rodPivot;
    public Transform rodTip;
    public float minCastForce = 5f;
    public float maxCastForce = 20f;
    public float chargeSpeed = 10f;
    public float reelSpeed = 5f;
    public float waterLevel = 0f;

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
        }

        // Release to cast
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            CastHook();
            isCharging = false;
        }

        // Reel
        if (Input.GetKey(KeyCode.R))
        {
            isReeling = true;
        }
        else
        {
            isReeling = false;
        }
    }

    void FixedUpdate()
    {
        if (isReeling && !hookRb.isKinematic)
        {
            Vector3 direction = (rodTip.position - hookRb.position).normalized;
            hookRb.linearVelocity = direction * reelSpeed;

            // Snap back and freeze when very close
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

            if (!fishCaught && Time.time >= nextRollTime)
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
    }

    void CastHook()
    {
        isCasting = true;
        hookRb.isKinematic = false;
        Vector3 castDirection = mainCam.transform.forward;
        castDirection.y = Mathf.Clamp(castDirection.y, -0.1f, 0.3f);
        castDirection.Normalize();
        hookRb.linearVelocity = Vector3.zero;
        hookRb.AddForce(castDirection * currentCastForce, ForceMode.VelocityChange);
    }
}
