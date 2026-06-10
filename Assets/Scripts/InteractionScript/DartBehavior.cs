using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class DartBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private bool isFlying = false;
    private bool isStuck = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // XRI 이벤트 구독
        grabInteractable.selectExited.AddListener(OnThrow);
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    // 다트를 손에서 놓았을 때 (던질 때)
    private void OnThrow(SelectExitEventArgs args)
    {
        isFlying = true;
        isStuck = false;

        // 던지는 순간 물리 연산 활성화
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    // 다트를 다시 잡았을 때 (뽑을 때)
    private void OnGrab(SelectEnterEventArgs args)
    {
        if (isStuck)
        {
            // 상태만 초기화해 줍니다.
            isStuck = false;
            isFlying = false;

            // [수정 핵심 1] 
            // 문제를 일으키던 SetParent(null)을 하지 않습니다 (애초에 부모 설정을 안 하므로).
            // 또한, 손에 쥐고 있는 동안 물리를 강제로 켜던 코루틴(ResetPhysicsAfterGrab)을 완전히 제거했습니다.
            // 손에 쥐고 있는 동안의 물리는 XRI 내부 시스템이 완벽하게 안전하게 제어하도록 내버려 둡니다.
        }
    }

    void FixedUpdate()
    {
        if (isFlying && !isStuck)
        {
            // 관통 방지: 다음 프레임에 이동할 거리만큼 미리 감지 (Raycast)
            float rayDistance = rb.linearVelocity.magnitude * Time.fixedDeltaTime + 0.1f;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance))
            {
                if (hit.collider.CompareTag("DartBoard"))
                {
                    StickToBoard();
                    return; // 꽂혔으므로 아래 비행 로직 건너뜀
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

        // [수정 핵심 2] SetParent를 쓰지 않고 Rigidbody를 완벽히 고정(Kinematic)하여 공중에 멈춥니다.
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isStuck) return;

        // 다트보드 이외의 물체에 부딪혔을 때 비행 중단
        if (!collision.gameObject.CompareTag("DartBoard"))
        {
            isFlying = false;
        }
    }

    public void ForceReset()
    {
        isFlying = false;
        isStuck = false; // 꽂힌 상태 해제
    }
}