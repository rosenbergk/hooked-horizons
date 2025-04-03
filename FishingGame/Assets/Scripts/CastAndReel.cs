// CastAndReel.cs
using UnityEngine;

public class CastAndReel : MonoBehaviour
{
    public Rigidbody hookRb;
    public Transform rodTip;

    [SerializeField]
    private float castForce = 10f;

    [SerializeField]
    private float reelSpeed = 5f;

    [SerializeField]
    private float waterLevel = 0f;

    private bool isCasting = false;
    private bool isReeling = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isCasting)
        {
            CastHook();
        }

        isReeling = Input.GetKey(KeyCode.R);

        if (isReeling)
        {
            Vector3 direction = (rodTip.position - hookRb.position).normalized;
            hookRb.linearVelocity = direction * reelSpeed;
        }

        if (hookRb.position.y < waterLevel)
        {
            Vector3 pos = hookRb.position;
            pos.y = waterLevel;
            hookRb.position = pos;
            hookRb.linearVelocity = Vector3.zero;
        }
    }

    void CastHook()
    {
        isCasting = true;
        hookRb.isKinematic = false;
        Vector3 castDirection = rodTip.forward + rodTip.up * 0.2f;
        hookRb.AddForce(castDirection * castForce, ForceMode.VelocityChange);
    }
}
