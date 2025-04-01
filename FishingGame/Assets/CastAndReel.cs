using UnityEngine;

public class CastAndReel : MonoBehaviour
{
    public Rigidbody hookRb;
    public Transform rodTip;
    public float castForce = 10f;
    public float reelSpeed = 5f;

    private bool isCasting = false;
    private bool isReeling = false;

    void Update()
    {
        // CAST
        if (Input.GetKeyDown(KeyCode.Space) && !isCasting)
        {
            CastHook();
        }

        // REEL
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
        if (isReeling)
        {
            Vector3 direction = (rodTip.position - hookRb.position).normalized;
            hookRb.linearVelocity = direction * reelSpeed;
        }
    }

    void CastHook()
    {
        isCasting = true;
        hookRb.isKinematic = false; // In case you want to freeze it initially
        Vector3 castDirection = rodTip.forward + rodTip.up * 0.2f;
        hookRb.AddForce(castDirection * castForce, ForceMode.VelocityChange);
    }
}
