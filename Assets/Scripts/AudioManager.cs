using UnityEngine;

/// <summary>
/// Central singleton for playing one-shot sound effects and controlling
/// overall SFX volume (hooked up to the Settings panel's Volume Slider).
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip slideClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip hitBoxClip;
    [SerializeField] private AudioClip barrierClip;
    [SerializeField] private AudioClip fireballClip;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Called by the Settings panel's Volume Slider (On Value Changed).
    /// </summary>
    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public float GetVolume()
    {
        return sfxVolume;
    }

    public void PlayJump() => Play(jumpClip);
    public void PlaySlide() => Play(slideClip);
    public void PlayHurt() => Play(hurtClip);
    public void PlayHitBox() => Play(hitBoxClip);
    public void PlayBarrier() => Play(barrierClip);
    public void PlayFireball() => Play(fireballClip);

    private void Play(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
