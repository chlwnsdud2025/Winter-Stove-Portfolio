using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class TrashItem : MonoBehaviour
{
    [Header("거리 제한 세팅")]
    public Transform trashCan;
    public float requiredDistance = 2.0f;

    [Header("판정 세팅")]
    public float requiredStopTime = 1.0f; // 이 시간(초) 동안 완전히 정지해야 판정 시작

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private Vector3 throwStartPosition;
    private bool isThrown = false;
    private bool isInsideTrigger = false;
    private bool evaluationComplete = false;

    private float stopTimer = 0f; // 정지 시간을 측정할 타이머

    public ConditionData TrashGoalin;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectExited.AddListener(OnThrow);
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isThrown = false;
        evaluationComplete = false;
        stopTimer = 0f; // 주우면 타이머 초기화
    }

    private void OnThrow(SelectExitEventArgs args)
    {
        throwStartPosition = transform.position;
        isThrown = true;
        evaluationComplete = false;
        stopTimer = 0f; // 던지는 순간 타이머 초기화
    }

    void FixedUpdate()
    {
        if (isThrown && !evaluationComplete)
        {
            // 이동 속도와 회전 속도가 모두 0에 가까운지 체크 (수치를 0.05f 정도로 넉넉하게 잡는 것이 VR에서 안정적입니다)
            if (rb.linearVelocity.sqrMagnitude < 0.05f && rb.angularVelocity.sqrMagnitude < 0.05f)
            {
                stopTimer += Time.fixedDeltaTime; // 멈춰있는 시간 누적

                // 지정된 시간(예: 1초) 이상 멈춰있었다면 최종 판정
                if (stopTimer >= requiredStopTime)
                {
                    EvaluateResult();
                }
            }
            else
            {
                // 조금이라도 움직이면 멈춤 타이머 리셋
                stopTimer = 0f;
            }
        }
    }

    private void EvaluateResult()
    {
        evaluationComplete = true;

        if (isInsideTrigger)
        {
            if (trashCan == null)
            {
                Debug.LogWarning("[경고] 휴지통 Transform이 지정되지 않았습니다.");
                return;
            }

            float throwDistance = Vector3.Distance(throwStartPosition, trashCan.position);

            if (throwDistance >= requiredDistance)
            {
                Debug.Log($"[미션 달성] 골인 성공! 던진 거리: {throwDistance:F2}m (목표: {requiredDistance}m)");
                TrashGoalin.achived = true;
            }
            else
            {
                Debug.Log($"골인은 했으나 너무 가까운 거리였습니다. 던진 거리: {throwDistance:F2}m");
            }
        }
        else
        {
            Debug.Log("골인 실패! (휴지통 밖에 정지함)");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrashCanZone"))
        {
            isInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TrashCanZone"))
        {
            isInsideTrigger = false;
        }
    }
}