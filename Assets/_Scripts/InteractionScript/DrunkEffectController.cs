using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // URP 볼륨 효과들을 쓰기 위해 필수입니다.

public class DrunkEffectController : MonoBehaviour
{
    private Volume volume;
    private LensDistortion lensDistortion; 
    private ChromaticAberration chromaticAberration;

    [Header("울렁거림 속도 및 강도")]
    public float wobbleSpeed = 2f;      // 화면이 출렁거리는 속도
    public float wobbleIntensity = 0.5f; // 화면이 일그러지는 최대 강도

    private void Awake()
    {
        volume = GetComponent<Volume>();

        if (volume != null && volume.profile != null)
        {
            // 볼륨 프로필에서 효과들을 안전하게 가져옵니다.
            if (volume.profile.TryGet(out lensDistortion) == false) Debug.LogError("Global Volume에 Lens Distortion 효과가 추가되어 있는지 확인하세요!");
            if (volume.profile.TryGet(out chromaticAberration) == false) Debug.LogError("Global Volume에 Chromatic Aberration 효과가 추가되어 있는지 확인하세요!");
        }
    }

    private void Update()
    {
        // 볼륨 컴포넌트가 없거나 꺼져있을 때는 연산하지 않습니다.
        if (volume == null || volume.weight <= 0.01f) return;

        // Mathf.Sin으로 -1 ~ 1 사이를 부드럽게 왕복하는 파동 생성
        float wave = Mathf.Sin(Time.time * wobbleSpeed);

        // 1. 화면 왜곡 조절
        if (lensDistortion != null)
        {
            lensDistortion.intensity.value = wave * wobbleIntensity;
        }

        // 2. 색수차 조절
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = Mathf.Abs(wave) * 1.0f;
        }
    }
}