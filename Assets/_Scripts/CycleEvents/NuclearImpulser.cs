using Unity.Cinemachine;
using UnityEngine;

public class NuclearImpulser : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (particle != null)
            particle.Play();
    }
}
