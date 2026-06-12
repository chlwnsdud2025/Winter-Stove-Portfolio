using Unity.Cinemachine;
using UnityEngine;

public class NuclearImpulser : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    [SerializeField] CinemachineImpulseSource impulse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(impulse != null)
            impulse.GenerateImpulse();
        if (particle != null)
            particle.Play();
    }
}
