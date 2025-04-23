// ReelSound.cs
using UnityEngine;

public class ReelSound : MonoBehaviour
{
    [SerializeField]
    private CastAndReel castAndReel;

    [SerializeField]
    private AudioClip reelClip;

    [SerializeField]
    private float reelVolume = 1f;

    private AudioSource audioSource;
    private bool wasReeling;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = reelClip;
        audioSource.loop = true;
        audioSource.volume = reelVolume;
    }

    void Update()
    {
        bool isReeling = castAndReel != null && castAndReel.IsReeling();
        if (reelSoundStart(isReeling))
            audioSource.Play();

        if (reelSoundStop(isReeling))
            audioSource.Stop();

        wasReeling = isReeling;
    }

    public bool reelSoundStart(bool isReeling)
    {
        return isReeling
            && !wasReeling
            && castAndReel.GetHookRodDistance() > castAndReel.GetHookRestDistance();
    }

    public bool reelSoundStop(bool isReeling)
    {
        return (!isReeling && wasReeling)
            || castAndReel.GetHookRodDistance() < castAndReel.GetHookRestDistance();
    }
}
