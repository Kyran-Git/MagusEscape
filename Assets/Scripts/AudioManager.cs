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
    [SerializeField] private AudioSource musicSource;

    [Header("Background Music (plays in order, looping through the list)")]
    [SerializeField] private AudioClip[] musicTracks;

    private int currentTrackIndex = 0;

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

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        musicSource.playOnAwake = false;
        musicSource.loop = false; // we manually advance to the next track instead of looping one clip
        musicSource.volume = sfxVolume;
    }

    private void Start()
    {
        if (musicTracks != null && musicTracks.Length > 0)
        {
            PlayTrack(0);
        }
    }

    private void Update()
    {
        // Once the current track finishes, move on to the next one (wrapping around)
        if (musicTracks != null && musicTracks.Length > 0 && musicSource != null && !musicSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    private void PlayTrack(int index)
    {
        currentTrackIndex = index;
        musicSource.clip = musicTracks[currentTrackIndex];
        musicSource.Play();
    }

    private void PlayNextTrack()
    {
        int nextIndex = (currentTrackIndex + 1) % musicTracks.Length;
        PlayTrack(nextIndex);
    }

    /// <summary>
    /// Called by the Settings panel's Volume Slider (On Value Changed).
    /// </summary>
    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (musicSource != null) musicSource.volume = sfxVolume;
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
