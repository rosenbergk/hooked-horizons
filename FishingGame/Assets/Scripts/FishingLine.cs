// FishingLine.cs
using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public Transform rodTip;
    public Transform hook;
    public int segmentCount = 25;
    public float maxSagAmount = 1.5f;

    private LineRenderer line;
    private CastAndReel castAndReel;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segmentCount;
        castAndReel = FindAnyObjectByType<CastAndReel>();
    }

    void Update()
    {
        Vector3 start = rodTip.position;
        Vector3 end = hook.position;
        float distance = Vector3.Distance(start, end);

        float sagFactor = 0f;

        bool isCasting = castAndReel != null && castAndReel.IsCasting();
        bool isReeling = castAndReel != null && castAndReel.IsReeling();

        if (!isCasting && distance > 1f)
        {
            sagFactor = Mathf.InverseLerp(2f, 10f, distance);

            if (isReeling)
            {
                sagFactor = Mathf.Lerp(sagFactor, 1f, 0.6f);
            }
        }

        Vector3[] points = new Vector3[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            if (sagFactor > 0f)
            {
                float sag = Mathf.Sin(t * Mathf.PI) * sagFactor * maxSagAmount;
                point.y -= sag;
            }

            points[i] = point;
        }

        line.SetPositions(points);
    }
}
