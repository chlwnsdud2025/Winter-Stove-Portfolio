using UnityEngine;
using RootMotion.FinalIK;

public class VRIKCalibrator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VRIK vrik;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    [Header("Position Offset (스케일 1 기준 기본값)")]
    [SerializeField] private Vector3 headOffset = new Vector3(0, -0.1f, -0.1f);
    [SerializeField] private float heightOffset = 0f;

    [Header("Hand Rotation Offset")]
    [SerializeField] private Vector3 leftHandRotOffset;
    [SerializeField] private Vector3 rightHandRotOffset;

    [Header("Arm Limit (스케일 1 기준 기본값)")]
    [SerializeField] private float maxHandDistance = 0.6f;
    [SerializeField] private float shoulderHeight = 1.4f;

    [Header("Hand Position Offset (스케일 1 기준 기본값)")]
    [SerializeField] private Vector3 handPositionOffset = new Vector3(0, -0.05f, 0.1f);

    private void Start()
    {
        if (vrik != null) SetupVRIK(vrik);
    }

    public void Initialize(Transform head, Transform leftHand, Transform rightHand)
    {
        headTarget = head;
        leftHandTarget = leftHand;
        rightHandTarget = rightHand;
        SetupVRIK(vrik);
    }

    private void LateUpdate()
    {
        if (headTarget == null || vrik == null) return;

        // 현재 오브젝트의 Y축 스케일(크기)을 가져옴
        float currentScale = transform.localScale.y;

        // 1. 캐릭터 위치 동기화 (스케일을 반영한 headOffset 적용)
        Vector3 scaledHeadOffset = headOffset * currentScale;
        Vector3 headPos = headTarget.position + scaledHeadOffset;

        transform.position = new Vector3(
            headPos.x,
            transform.position.y + (heightOffset * currentScale),
            headPos.z
        );

        // 좌우 회전 반영
        float yRotation = headTarget.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // 2. 손 위치 설정 (+= 누적 연산 대신 = 대입 연산으로 변경하고 스케일 반영)
        // 원래 타겟 위치를 기준으로 스케일된 오프셋을 더해줍니다.
        if (leftHandTarget != null && vrik.solver.leftArm.target != null)
        {
            vrik.solver.leftArm.target.position = leftHandTarget.position +
                leftHandTarget.TransformDirection(handPositionOffset * currentScale);

            vrik.solver.leftArm.target.rotation = leftHandTarget.rotation * Quaternion.Euler(leftHandRotOffset);
            LimitHandDistance(vrik.solver.leftArm.target, currentScale);
        }

        if (rightHandTarget != null && vrik.solver.rightArm.target != null)
        {
            vrik.solver.rightArm.target.position = rightHandTarget.position +
                rightHandTarget.TransformDirection(handPositionOffset * currentScale);

            vrik.solver.rightArm.target.rotation = rightHandTarget.rotation * Quaternion.Euler(rightHandRotOffset);
            LimitHandDistance(vrik.solver.rightArm.target, currentScale);
        }
    }

    // 인자로 현재 스케일을 받아와서 동적으로 거리를 제한하도록 수정
    private void LimitHandDistance(Transform handTarget, float scale)
    {
        // 스케일이 반영된 어깨 높이 계산
        Vector3 shoulderPos = transform.position + Vector3.up * (shoulderHeight * scale);
        Vector3 dir = handTarget.position - shoulderPos;

        // 스케일이 반영된 최대 팔 길이 계산
        float scaledMaxDistance = maxHandDistance * scale;

        if (dir.magnitude > scaledMaxDistance)
        {
            handTarget.position = shoulderPos + dir.normalized * scaledMaxDistance;
        }
    }

    public void SetupVRIK(VRIK newVrik)
    {
        vrik = newVrik;
        if (vrik == null) return;

        vrik.solver.spine.headTarget = headTarget;
        vrik.solver.leftArm.target = leftHandTarget;
        vrik.solver.rightArm.target = rightHandTarget;
    }
}