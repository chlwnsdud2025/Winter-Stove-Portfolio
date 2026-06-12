using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class TrashItem : MonoBehaviour
{
    [Header("거리 제한 세팅")]
    public Transform trashCan; // 휴지통의 중심 Transform (인스펙터에서 연결)
    public float requiredDistance = 2.0f; // 성공 인정 최소 거리 (예: 2m)

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private Vector3 throwStartPosition; // 던진 시작 위치
    private bool isThrown = false; // 던져진 상태인지 체크
    private bool isInsideTrigger = false; // 현재 휴지통 트리거 안에 있는지 여부
    private bool evaluationComplete = false; // 판정이 끝났는지 체크


    public ConditionData TrashGoalin;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // XR Grab 이벤트 연결
        grabInteractable.selectExited.AddListener(OnThrow);
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // 다시 주웠을 때는 상태를 초기화합니다.
        isThrown = false;
        evaluationComplete = false;
    }

    private void OnThrow(SelectExitEventArgs args)
    {
        // 손에서 놓는 순간(던진 순간)의 내 위치를 기록합니다.
        throwStartPosition = transform.position;
        isThrown = true;
        evaluationComplete = false;
    }

    void FixedUpdate()
    {
        // 던져진 상태이고, 아직 판정이 완료되지 않았을 때만 실시간 정지 여부를 검사합니다.
        if (isThrown && !evaluationComplete)
        {
            // 속도의 크기가 거의 0에 가깝다면 (완전히 정지했다면)
            if (rb.linearVelocity.sqrMagnitude < 0.005f)
            {
                EvaluateResult();
            }
        }
    }

    private void EvaluateResult()
    {
        evaluationComplete = true; // 판정 중복 실행 방지

        // 1. 조건: 정지했을 때 트리거 안에 남아있는가?
        if (isInsideTrigger)
        {
            if (trashCan == null)
            {
                Debug.LogWarning("[경고] 휴지통(TrashCan) Transform이 지정되지 않아 거리 계산을 할 수 없습니다.");
                return;
            }

            // 2. 조건: 던진 위치와 휴지통 사이의 실제 거리 계산
            // (만약 높낮이 변화를 무시하고 오직 평면 거리만 재고 싶다면 Vector2.Distance로 X, Z축만 계산해도 좋습니다.)
            float throwDistance = Vector3.Distance(throwStartPosition, trashCan.position);

            if (throwDistance >= requiredDistance)
            {
                Debug.Log($"[미션 달성] 골인 성공! 던진 거리: {throwDistance:F2}m (목표: {requiredDistance}m)");
                TrashGoalin.achived = true;
                // 성공 이벤트 알림 (예: 도전과제 채널 혹은 매니저 연동)
            }
            else
            {
                Debug.Log($"골인은 했으나 너무 가까운 거리였습니다. 던진 거리: {throwDistance:F2}m (최소 요구: {requiredDistance}m)");
            }
        }
        else
        {
            Debug.Log("골인 실패! (휴지통 밖에 정지함)");
        }
    }

    // 휴지통 내부 CapsuleCollider(Is Trigger 체크됨)와의 충돌 영역 감지
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrashCanZone"))
        {
            isInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 들어갔다가 밖으로 튕겨 나가면 다시 false가 되므로 림아웃을 완벽히 방지합니다.
        if (other.CompareTag("TrashCanZone"))
        {
            isInsideTrigger = false;
        }
    }
}