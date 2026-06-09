using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class DartBehavior : MonoBehaviour
{
    private Rigidbody rb;
    private bool isFlying = false;
    private bool isStuck = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 던지는 순간을 감지하기 위해 XRGrabInteractable의 이벤트를 구독
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().selectExited.AddListener(OnThrow);
    }

    private void OnThrow(SelectExitEventArgs args)
    {
        isFlying = true;
        isStuck = false;
    }

    void FixedUpdate()
    {
        // 다트가 날아가는 중일 때만 작동
        if (isFlying && !isStuck && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            // 이동 방향(velocity)을 전방(forward)으로 정렬
            transform.forward = rb.linearVelocity.normalized;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 이미 꽂혀있거나 꽂히는 동작 중이라면 무시
        if (isStuck) return;

        // 1. 다트보드에 맞았을 때: 꽂히는 로직
        if (collision.gameObject.CompareTag("DartBoard"))
        {
            isFlying = false;
            isStuck = true;

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            transform.SetParent(collision.transform);
        }
        // 2. 다트보드가 아닌 다른 곳(벽, 바닥 등)에 맞았을 때
        else
        {
            // 비행 상태 종료 -> FixedUpdate의 방향 보정 로직이 즉시 멈춤
            isFlying = false;

            // 추가 팁: 통통 튀는 게 보기 싫다면 멈춰줄 수도 있음
            // rb.velocity = Vector3.zero; 

            Debug.Log("다트가 빗나갔습니다.");
        }
    }
}