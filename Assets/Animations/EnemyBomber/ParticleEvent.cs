using UnityEngine;

public class ParticleEvent : MonoBehaviour
{
    public ParticleSystem particleSystem;

    public void PlayParticle()
    {
        if (particleSystem != null)
            particleSystem.Play();
    }
}
