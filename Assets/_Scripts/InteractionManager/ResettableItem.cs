using UnityEngine;

public class ResettableItem : MonoBehaviour
{
    public enum ItemType { Jenga, Dart, Card }

    [Header("이 아이템의 종류를 선택하세요")]
    public ItemType itemType;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    // FixedUpdate에서 안전하게 리셋하기 위한 신호등(플래그)
    private bool needsReset = false;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    // 버튼이 이 함수를 호출합니다.
    public void ResetIfMatchingType(ItemType targetType)
    {
        if (itemType == targetType)
        {
            // 즉시 이동시키지 않고, 리셋 예약만 걸어둡니다.
            needsReset = true;
        }
    }

    // 물리 엔진이 계산되는 타이밍에 맞춰 실행됩니다.
    void FixedUpdate()
    {
        // 예약된 리셋이 있다면 여기서 처리합니다.
        if (needsReset)
        {
            PerformPhysicsReset();
            needsReset = false; // 완료 후 다시 false로 되돌림
        }
    }

    // 실제 이동과 물리 초기화 로직
    private void PerformPhysicsReset()
    {
        if (rb != null)
        {
            // 이동 중 충돌 방지를 위해 물리를 잠깐 끕니다.
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }

        // 안전한 물리 타이밍에 위치와 회전을 복구합니다.
        transform.position = startPosition;
        transform.rotation = startRotation;

        if (rb != null)
        {
            // 이동 완료 후 다시 물리를 켭니다.
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        SendMessage("ForceReset", SendMessageOptions.DontRequireReceiver);
    }
}