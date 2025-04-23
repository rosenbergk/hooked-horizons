// HookSplash.cs
using System.Collections;
using UnityEngine;

public class HookSplash : MonoBehaviour
{
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

    private bool wasBelowWater;
    private bool wasBelowExitThreshold;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        wasBelowWater = transform.position.y < 0f;
    }

    void Update()
    {
        bool isBelowWater = transform.position.y < waterLevel;

        if (!wasBelowWater && isBelowWater)
        {
            audioSource.PlayOneShot(splashClip, enterVolume);
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
