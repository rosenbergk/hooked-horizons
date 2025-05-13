// HookSplash.cs
using UnityEngine;

public class HookSplash : MonoBehaviour
{
    private CastAndReel castAndReel;

    [SerializeField]
    private AudioClip splashClip;

    [SerializeField]
    private float enterVolume = 1f;

    [SerializeField]
    private float exitVolume = 0.2f;

    [SerializeField]
    private float waterLevel = 0f;

    [SerializeField]
    private float exitThreshold = -0.1f;

    [SerializeField]
    private GameObject splashEffectPrefab;

    private bool wasBelowWater;
    private bool wasBelowExitThreshold;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        castAndReel = GetComponentInParent<CastAndReel>();
        wasBelowWater = transform.position.y < 0f;
    }

    void Update()
    {
        bool isBelowWater = transform.position.y < waterLevel;

        if (!wasBelowWater && isBelowWater && castAndReel != null && castAndReel.IsCasting())
        {
            audioSource.PlayOneShot(splashClip, enterVolume);

            if (splashEffectPrefab != null)
            {
                Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
            }
        }

        bool isBelowExit = transform.position.y < exitThreshold;

        if (wasBelowExitThreshold && !isBelowExit)
        {
            audioSource.PlayOneShot(splashClip, exitVolume);
        }

        wasBelowWater = isBelowWater;
        wasBelowExitThreshold = isBelowExit;
    }
}
