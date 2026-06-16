using UnityEngine;
using UnityEngine.Rendering;

public class SimpleCupBehavior : MonoBehaviour
{
    [Header("술 오브젝트 세팅")]
    [Tooltip("술잔 하위에 이미 들어있는 '술 모델링 오브젝트'를 드래그해서 넣으세요.")]
    public GameObject liquidObject;

    [Header("마시기 조건 설정")]
    [Tooltip("잔을 이 각도 이상 기울여야 마심 (90 = 수평, 110 = 확 뒤집음)")]
    public float drinkAngle = 90f;
    [Tooltip("잔이 얼굴(카메라)과 이 거리보다 가까워야 함 (미터 단위)")]
    public float drinkDistance = 0.25f;

    [Header("취기 효과 (URP Volume)")]
    [Tooltip("취기 효과가 세팅된 Global Volume을 연결하세요.")]
    public Volume drunkVolume;

    [Tooltip("술에 취해있는 시간 (초 단위, 예: 5를 넣으면 5초 뒤에 한 번에 깼습니다)")]
    public float drunkDuration = 5f; 

    private Transform playerCamera;
    private bool isFull = false;
    private float drunkTimer = 0f; 

    private void Start()
    {
        if (Camera.main != null) playerCamera = Camera.main.transform;

        if (drunkVolume != null) drunkVolume.weight = 0f;

        if (liquidObject != null)
        {
            isFull = liquidObject.activeSelf;
        }
    }

    private void Update()
    {
        CheckDrinking();
        HandleSoberUp(); 
    }

    // 술병 파티클이 잔의 콜라이더에 단 한 방울이라도 닿으면 작동!
    private void OnParticleCollision(GameObject other)
    {
        if (!isFull && liquidObject != null)
        {
            isFull = true;
            liquidObject.SetActive(true);
            Debug.Log("[술잔] 술이 가득 채워졌습니다!");
        }
    }

    private void CheckDrinking()
    {
        if (playerCamera == null || !isFull) return;

        float tiltAngle = Vector3.Angle(transform.up, Vector3.up);
        float distanceToFace = Vector3.Distance(transform.position, playerCamera.position);

        if (tiltAngle >= drinkAngle && distanceToFace <= drinkDistance)
        {
            isFull = false;

            if (liquidObject != null)
            {
                liquidObject.SetActive(false);
            }

            if (drunkVolume != null)
            {
                drunkVolume.weight = 1f; // 취기 효과 100% 발동
            }

            // 이미 취해있는 상태에서 또 마시면 타이머가 다시 5초로 초기화되어 연장됩니다.
            drunkTimer = drunkDuration;

            Debug.Log("[원샷!] 술을 마셔서 어질어질해집니다!");
        }
    }

    // 🌟 지속 시간을 체크해서 한 번에 꺼버리는 핵심 로직 함수
    private void HandleSoberUp()
    {
        // 타이머가 작동 중일 때만 흘러간 시간을 깎습니다.
        if (drunkTimer > 0f)
        {
            drunkTimer -= Time.deltaTime;

            // 시간이 마침내 0 이하로 떨어지는 바로 그 프레임에!
            if (drunkTimer <= 0f)
            {
                drunkTimer = 0f; // 안전하게 0으로 고정

                if (drunkVolume != null)
                {
                    drunkVolume.weight = 0f; // 🌟 서서히 줄이지 않고 즉시 0으로 꺼버림!
                }
                Debug.Log("[술 깨기] 지속 시간이 다 되어 술이 한 번에 깼습니다!");
            }
        }
    }
}