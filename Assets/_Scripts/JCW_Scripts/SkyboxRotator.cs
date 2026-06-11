using UnityEngine;

/// <summary>
/// 스카이박스 천천히 회전 — Skybox/Panoramic 전용
/// </summary>
public class SkyboxRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.5f; // 낮을수록 천천히

    private void Update()
    {
        float current = RenderSettings.skybox.GetFloat("_Rotation");
        RenderSettings.skybox.SetFloat("_Rotation", current + rotationSpeed * Time.deltaTime);
    }
    private void OnDestroy()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }
}