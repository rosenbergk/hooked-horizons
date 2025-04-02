using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public Transform rodTip;  
    public Transform hook;     
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        line.SetPosition(0, rodTip.position);
        line.SetPosition(1, hook.position);
    }
}
