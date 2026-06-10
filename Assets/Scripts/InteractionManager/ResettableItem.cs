using UnityEngine;

public class ResettableItem : MonoBehaviour
{
    // 아이템 종류 정의
    public enum ItemType { Jenga, Dart, Card }

    [Header("이 아이템의 종류를 선택하세요")]
    public ItemType itemType;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    public void ResetIfMatchingType(ItemType targetType)
    {
        // 버튼이 요청한 종류와 내 종류가 일치할 때만 리셋 진행
        if (itemType == targetType)
        {
            ResetToOrigin();
        }
    }

    private void ResetToOrigin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 다트 상태 초기화 (다트 스크립트가 있을 때만 실행)
        SendMessage("ForceReset", SendMessageOptions.DontRequireReceiver);
    }
}