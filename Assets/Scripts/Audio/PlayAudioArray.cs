using UnityEngine;

public class PlayAudioArray: MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;
    [Range(0f, 1f)]
    [SerializeField] private float audioVolume = 0.3f;

    public void PlayAudio()
    {
        if (audioClips.Length == 0 || AudioManager.Instance == null) return;

        AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];
        AudioManager.Instance.PlaySound(randomClip, audioVolume);
    }
}
