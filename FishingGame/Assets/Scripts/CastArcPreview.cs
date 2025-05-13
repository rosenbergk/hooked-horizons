// CastArcPreview.cs
using UnityEngine;

public class CastArcPreview : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int resolution = 30;
    public float timeStep = 0.1f;
    public float gravity = -9.81f;

    public Transform origin;
    public Vector3 castDirection;
    public float castPower;

    public void ShowArc(Vector3 direction, float power)
    {
        castDirection = direction;
        castPower = power;

        Vector3[] points = new Vector3[resolution];
        Vector3 startPos = origin.position;
        Vector3 velocity = castDirection.normalized * castPower;

        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPos + velocity * t;
            point.y += 0.5f * gravity * t * t;
            points[i] = point;
        }

        lineRenderer.positionCount = resolution;
        lineRenderer.SetPositions(points);
    }

    public void HideArc()
    {
        lineRenderer.positionCount = 0;
    }
}
