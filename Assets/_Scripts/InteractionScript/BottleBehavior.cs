using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BottleBehavior : MonoBehaviour
{
    [Header("따르기 위치 설정")]
    public Transform spawnPoint;

    [Header("파티클 설정")]
    public ParticleSystem liquidParticle;
    public float pourAngle = 90f;

    // [요청사항] pourAmount 관련 변수 주석 처리
    // public float pourAmount = 30f; 

    [Header("흔들림 방지 (버퍼)")]
    [Tooltip("90도 언저리에서 파티클이 깜빡이는 것을 막아주는 여유 각도")]
    public float angleBuffer = 5f;

    // private ParticleSystem.EmissionModule emissionModule; // 주석 처리
    private bool isPouring = false;

    private void Start()
    {
        if (liquidParticle != null)
        {
            // [요청사항] 코드에서 방출량을 0으로 강제하는 부분 주석 처리
            // emissionModule = liquidParticle.emission;
            // emissionModule.rateOverTime = 0f;

            // 대신, 게임이 시작되면 파티클을 완전히 멈춰둡니다.
            liquidParticle.Stop();
        }
    }

    private void Update()
    {
        if (liquidParticle == null || spawnPoint == null) return;

        // 개선점 1: 3D 모델의 축 대신, 우리가 직접 만든 spawnPoint의 축을 믿습니다!
        float tiltAngle = Vector3.Angle(spawnPoint.up, Vector3.up);

        // 개선점 2: 공식 스크립트의 '버퍼' 로직 적용
        // 따르고 있을 때는 판정 기준을 살짝 낮춰서 웬만하면 안 끊기게 해줍니다.
        float currentThreshold = isPouring ? (pourAngle - angleBuffer) : pourAngle;

        bool shouldPour = tiltAngle >= currentThreshold;

        if (isPouring != shouldPour)
        {
            isPouring = shouldPour;

            if (isPouring)
            {
                // [요청사항] 코드로 방출량을 덮어씌우는 부분 주석 처리
                // emissionModule.rateOverTime = pourAmount;

                // 🌟 파티클 재생 🌟
                liquidParticle.Play();
                Debug.Log("[술병] 술을 따릅니다!");
            }
            else
            {
                // [요청사항] 방출량 0으로 만드는 부분 주석 처리
                // emissionModule.rateOverTime = 0f;

                // 🌟 파티클 정지 🌟
                liquidParticle.Stop();
            }
        }
    }
}