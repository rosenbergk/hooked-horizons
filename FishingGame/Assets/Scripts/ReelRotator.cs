// ReelRotator.cs
using UnityEngine;

public class ReelRotator : MonoBehaviour
{
    [SerializeField]
    private Transform pivot;

    [SerializeField]
    private float revolveSpeed = 500f;

    [SerializeField]
    private CastAndReel castAndReel;

    void Update()
    {
        if (castAndReel != null && castAndReel.IsReeling() && pivot != null)
        {
            float angle = -revolveSpeed * Time.deltaTime;
            transform.RotateAround(pivot.position, pivot.up, angle);
        }
    }
}
