using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class DartBehavior : MonoBehaviour
{
    [SerializeField] AudioClip dartAudio;
    [Header("다트 판정 세팅")]
    public Transform dartTip; // 다트 끝부분(뾰족한 곳)에 넣을 빈 오브젝트
    public float hitCheckRadius = 0.01f; // 판정 반경 (1cm 정도로 아주 작게)
    public float raycastrength = 0.01f;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private bool isFlying = false;
    private bool isStuck = false;

    public VoidEventChannel dartHitChannel;
    public ConditionData darthit_Middle;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectExited.AddListener(OnThrow);
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnThrow(SelectExitEventArgs args)
    {
        isFlying = true;
        isStuck = false;

        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (isStuck)
        {
            isStuck = false;
            isFlying = false;
        }
    }

    void FixedUpdate()
    {
        if (isFlying && !isStuck)
        {
            // [수정 1] 레이저 시작점을 다트 끝(dartTip)으로 설정합니다.
            Vector3 rayOrigin = dartTip != null ? dartTip.position : transform.position;

            // 다음 프레임에 이동할 거리(속도 기반) + 여유 거리 계산
            float rayDistance = rb.linearVelocity.magnitude * Time.fixedDeltaTime + raycastrength;
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, transform.forward, out hit, rayDistance))
            {
                if (hit.collider.CompareTag("DartBoard") || hit.collider.CompareTag("DartMiddle"))
                {
                    // [핵심 로직: 순간이동] 멈추기 전에 다트 끝부분을 다트판 표면(hit.point)에 정확히 맞춥니다.
                    if (dartTip != null)
                    {
                        // hit.point(목표점)와 현재 dartTip의 거리 차이만큼 다트 전체를 이동시킵니다.
                        transform.position += (hit.point - dartTip.position);
                    }
                    else
                    {
                        transform.position = hit.point;
                    }


                    SoundManager.Instance?.PlaySFX(dartAudio);
                    // 위치를 맞춘 후 멈춤 및 점수 판독을 실행합니다.
                    StickToBoard();
                    return;
                }
            }

            // 비행 자세 보정
            if (rb.linearVelocity.sqrMagnitude > 0.1f)
            {
                transform.forward = rb.linearVelocity.normalized;
            }
        }
    }

    private void StickToBoard()
    {
        isFlying = false;
        isStuck = true;

        // 물리 연산 강제 정지
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 완벽하게 정지 및 표면에 부착된 직후, 점수를 판독합니다.
        EvaluateScore();
    }

    // 다트 끝부분(Tip)을 기준으로 점수를 판독하는 함수
    private void EvaluateScore()
    {
        if (dartTip == null) return;

        // 다트 끝부분 위치에서 hitCheckRadius(1cm) 반경 내에 있는 모든 콜라이더를 검사합니다.
        Collider[] hitColliders = Physics.OverlapSphere(dartTip.position, hitCheckRadius);

        foreach (Collider hitCol in hitColliders)
        {
            // 그 중 DartMiddle 태그를 가진 트리거가 있다면?
            if (hitCol.CompareTag("DartMiddle"))
            {
                dartHitChannel.RaiseEvent();
                darthit_Middle.achived = true;
                Debug.Log("중앙 명중");
                return; // 점수를 확인했으니 반복문 종료
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isStuck) return;

        if (!collision.gameObject.CompareTag("DartBoard") && !collision.gameObject.CompareTag("DartMiddle"))
        {
            isFlying = false;
        }
    }

    public void ForceReset()
    {
        isFlying = false;
        isStuck = false;
    }

    // 씬 창에서 다트 끝부분의 판정 반경과 레이저를 눈으로 볼 수 있게 해줍니다.
    private void OnDrawGizmos()
    {
        Rigidbody tempRb = GetComponent<Rigidbody>();
        if (tempRb != null)
        {
            // [수정 2] 기즈모 레이저의 시작점도 다트 끝(dartTip)으로 맞춥니다.
            Vector3 startPos = dartTip != null ? dartTip.position : transform.position;

            // 다트가 멈춰있을 때 선이 완전히 사라지는 착시를 방지하기 위해 최소 길이를 보장합니다.
            float speed = tempRb.linearVelocity.magnitude;
            float rayDistance = (speed > 0.1f ? speed * Time.fixedDeltaTime : 0.2f) + raycastrength;

            // 파란 선 그리기
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(startPos, transform.forward * rayDistance);

            // 레이저 끝부분 빨간 구슬
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(startPos + transform.forward * rayDistance, 0.01f);

            // (보너스) 다트 끝부분(Tip)에 점수 판정(OverlapSphere) 범위도 노란색으로 그려줍니다.
            if (dartTip != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(dartTip.position, hitCheckRadius);
            }
        }
    }
}