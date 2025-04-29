// Fish.cs
using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public string FishName { get; private set; }

    [SerializeField]
    private Transform mouth;

    private float weight;

    public void SetFishName(string name)
    {
        FishName = name;
    }

    public void SetFishWeight(float weight)
    {
        this.weight = weight;
    }

    public float GetFishWeight()
    {
        return weight;
    }

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
