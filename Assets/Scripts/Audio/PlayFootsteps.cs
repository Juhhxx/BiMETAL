using UnityEngine;

public class PlayFootsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepClips;
    [Range(0f, 1f)]
    [SerializeField] private float footstepVolume = 0.3f;

    public void PlayFootstep()
    {
        if (footstepClips.Length == 0 || AudioManager.Instance == null) return;

        AudioClip randomClip = footstepClips[Random.Range(0, footstepClips.Length)];
        AudioManager.Instance.PlaySound(randomClip, footstepVolume);
    }
}
