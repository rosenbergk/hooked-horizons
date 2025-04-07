// Fish.cs
using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private Transform mouth;

    public void Catch(Transform hook)
    {
        transform.SetParent(hook);
        Vector3 mouthLocalPos = transform.InverseTransformPoint(mouth.position);
        transform.localPosition = -mouthLocalPos;
        transform.localRotation = Quaternion.identity;
    }

    public void ReleaseAndDisappear(float delay)
    {
        StartCoroutine(DisappearAfterDelay(delay));
    }

    private IEnumerator DisappearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
