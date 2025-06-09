using UnityEngine;

public class PlayAttack: MonoBehaviour
{
    [SerializeField] private AudioClip[] attackClips;
    [Range(0f, 1f)]
    [SerializeField] private float attackVolume = 0.3f;

    public void PlayAttackSound()
    {
        if (attackClips.Length == 0 || AudioManager.Instance == null) return;

        AudioClip randomClip = attackClips[Random.Range(0, attackClips.Length)];
        AudioManager.Instance.PlaySound(randomClip, attackVolume);
    }
}
