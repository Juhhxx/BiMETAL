using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int _maxAudioSources = 10;
    [SerializeField] private bool _allowOneShotInterruption = true;

    private List<AudioSource> _audioSources = new List<AudioSource>();
    private Dictionary<AudioClip, AudioSource> _loopingSounds = new Dictionary<AudioClip, AudioSource>();
    private Dictionary<AudioClip, List<AudioSource>> _activeOneShots = new Dictionary<AudioClip, List<AudioSource>>();

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Pre-create audio sources
        for (int i = 0; i < _maxAudioSources; i++)
        {
            _audioSources.Add(gameObject.AddComponent<AudioSource>());
        }
    }

    #region One-Shot Sounds
    /// <summary>
    /// Play a sound once (non-looping)
    /// </summary>
    public void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f, bool allowMultiple = false, float blend = 0f)
    {
        if (clip == null) return;

        if (!allowMultiple && IsOneShotPlaying(clip))
        {
            return;
        }

        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource == null) return;

        ConfigureAudioSource(availableSource, clip, volume, pitch, false, blend);
        availableSource.Play();

        TrackOneShot(clip, availableSource);
        StartCoroutine(RemoveOneShotWhenComplete(clip, availableSource));
    }

    /// <summary>
    /// Stop all instances of a one-shot sound
    /// </summary>
    public void StopOneShot(AudioClip clip)
    {
        if (clip == null || !_activeOneShots.ContainsKey(clip)) return;

        foreach (var source in _activeOneShots[clip])
        {
            if (source != null && source.isPlaying)
            {
                source.Stop();
            }
        }
        _activeOneShots[clip].Clear();
    }

    /// <summary>
    /// Stop all one-shot sounds
    /// </summary>
    public void StopAllOneShots()
    {
        foreach (var clipEntry in _activeOneShots)
        {
            foreach (var source in clipEntry.Value)
            {
                if (source != null && source.isPlaying)
                {
                    source.Stop();
                }
            }
            clipEntry.Value.Clear();
        }
    }

    /// <summary>
    /// Check if a specific one-shot is playing
    /// </summary>
    public bool IsOneShotPlaying(AudioClip clip)
    {
        if (clip == null || !_activeOneShots.ContainsKey(clip)) return false;

        _activeOneShots[clip].RemoveAll(source => source == null);

        foreach (var source in _activeOneShots[clip])
        {
            if (source.isPlaying)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Looping Sounds
    /// <summary>
    /// Play a looping sound
    /// </summary>
    public void PlayLoopingSound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        // If already playing as looping sound, just update settings
        if (_loopingSounds.ContainsKey(clip))
        {
            var source = _loopingSounds[clip];
            source.volume = volume;
            source.pitch = pitch;
            if (!source.isPlaying) source.Play();
            return;
        }

        AudioSource availableSource = GetAvailableAudioSource();
        if (availableSource == null) return;

        ConfigureAudioSource(availableSource, clip, volume, pitch, true);
        availableSource.Play();

        _loopingSounds[clip] = availableSource;
    }

    /// <summary>
    /// Stop a specific looping sound
    /// </summary>
    public void StopLoopingSound(AudioClip clip)
    {
        if (clip == null || !_loopingSounds.ContainsKey(clip)) return;

        _loopingSounds[clip].Stop();
        _loopingSounds.Remove(clip);
    }

    /// <summary>
    /// Stop all looping sounds
    /// </summary>
    public void StopAllLoopingSounds()
    {
        foreach (var source in _loopingSounds.Values)
        {
            if (source != null)
            {
                source.Stop();
            }
        }
        _loopingSounds.Clear();
    }

    /// <summary>
    /// Check if a specific looping sound is playing
    /// </summary>
    public bool IsLoopingSoundPlaying(AudioClip clip)
    {
        return _loopingSounds.ContainsKey(clip) && _loopingSounds[clip].isPlaying;
    }
    #endregion

    #region Helper Methods
    private void ConfigureAudioSource(AudioSource source, AudioClip clip, float volume, float pitch, bool loop, float blend = 0f)
    {
        source.clip = clip;
        source.volume = Mathf.Clamp01(volume);
        source.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
        source.loop = loop;
        source.spatialBlend = blend; // Force 2D sound
    }

    private void TrackOneShot(AudioClip clip, AudioSource source)
    {
        if (!_activeOneShots.ContainsKey(clip))
        {
            _activeOneShots[clip] = new List<AudioSource>();
        }
        _activeOneShots[clip].Add(source);
    }

    private IEnumerator RemoveOneShotWhenComplete(AudioClip clip, AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);

        if (_activeOneShots.ContainsKey(clip))
        {
            _activeOneShots[clip].Remove(source);
            if (_activeOneShots[clip].Count == 0)
            {
                _activeOneShots.Remove(clip);
            }
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        // First try to find an available source
        foreach (var source in _audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // If allowed, interrupt the oldest one-shot
        if (_allowOneShotInterruption)
        {
            AudioSource oldestSource = null;
            float longestTime = 0;

            foreach (var clipEntry in _activeOneShots)
            {
                foreach (var source in clipEntry.Value)
                {
                    if (source.isPlaying && source.time > longestTime)
                    {
                        oldestSource = source;
                        longestTime = source.time;
                    }
                }
            }

            if (oldestSource != null)
            {
                oldestSource.Stop();
                return oldestSource;
            }
        }

        // If no available sources, create a new one (with limit)
        if (_audioSources.Count < _maxAudioSources * 2)
        {
            var newSource = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(newSource);
            return newSource;
        }

        Debug.LogWarning("No available audio sources!");
        return null;
    }
    #endregion
}