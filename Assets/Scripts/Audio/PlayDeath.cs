using UnityEngine;

public class PlayDeath : MonoBehaviour
{
    [SerializeField] private AudioClip[] deathClips;
    [Range(0f, 1f)]
    [SerializeField] private float deathAudioVolume = 0.3f;

    public void PlayDeathSound()
    {
        if (deathClips.Length == 0 || AudioManager.Instance == null) return;

        AudioClip randomClip = deathClips[Random.Range(0, deathClips.Length)];
        AudioManager.Instance.PlaySound(randomClip, deathAudioVolume);
    }
}
