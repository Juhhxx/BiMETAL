using UnityEngine;

public class PlayIdle : MonoBehaviour
{
    [SerializeField] private AudioClip idleClip;
    [Range(0f, 1f)]
    [SerializeField] private float idleVolume = 0.3f;

    public void PlayIdleSound()
    {
        if (idleClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(idleClip, idleVolume);
        }
    }
}
