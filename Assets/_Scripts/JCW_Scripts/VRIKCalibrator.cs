
using UnityEngine;
using RootMotion.FinalIK;

public class VRIKCalibrator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VRIK vrik;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    [Header("Position Offset")]
    [SerializeField] private Vector3 headOffset = new Vector3(0, -0.1f, -0.1f);
    [SerializeField] private float heightOffset = 0f;

    [Header("Hand Rotation Offset")]
    [SerializeField] private Vector3 leftHandRotOffset;
    [SerializeField] private Vector3 rightHandRotOffset;

    [Header("Arm Limit")]
    [SerializeField] private float maxHandDistance = 0.6f;
    [SerializeField] private float shoulderHeight = 1.4f; // 어깨 높이 조정

    [Header("Hand Position Offset")]
    [SerializeField]
    private Vector3 handPositionOffset = new Vector3(0, -0.05f, 0.1f);

    private void Start()
    {
        vrik.solver.spine.headTarget = headTarget;
        vrik.solver.leftArm.target = leftHandTarget;
        vrik.solver.rightArm.target = rightHandTarget;


      
    }
    public void Initialize(
    Transform head,
    Transform leftHand,
    Transform rightHand)
    {
        headTarget = head;
        leftHandTarget = leftHand;
        rightHandTarget = rightHand;

        SetupVRIK(vrik);
    }
    private void LateUpdate()
    {
        // 캐릭터 위치 동기화
        Vector3 headPos = headTarget.position + headOffset;
        transform.position = new Vector3(
            headPos.x,
            transform.position.y + heightOffset,
            headPos.z
        );

        // 좌우 회전만 반영
        float yRotation = headTarget.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        vrik.solver.leftArm.target.position +=
       leftHandTarget.TransformDirection(handPositionOffset);
        vrik.solver.rightArm.target.position +=
            rightHandTarget.TransformDirection(handPositionOffset);

        // 손 회전 오프셋
        vrik.solver.leftArm.target.rotation =
            vrik.solver.leftArm.target.rotation * Quaternion.Euler(leftHandRotOffset);
        vrik.solver.rightArm.target.rotation =
            vrik.solver.rightArm.target.rotation * Quaternion.Euler(rightHandRotOffset);

        // 팔 최대 거리 제한
        LimitHandDistance(leftHandTarget);
        LimitHandDistance(rightHandTarget);
    }

    private void LimitHandDistance(Transform handTarget)
    {
        // 어깨 기준 위치
        Vector3 shoulderPos = transform.position + Vector3.up * shoulderHeight;
        Vector3 dir = handTarget.position - shoulderPos;

        if (dir.magnitude > maxHandDistance)
            handTarget.position = shoulderPos + dir.normalized * maxHandDistance;
    }
    public void SetupVRIK(VRIK newVrik)
    {
        vrik = newVrik;
        vrik.solver.spine.headTarget = headTarget;
        vrik.solver.leftArm.target = leftHandTarget;
        vrik.solver.rightArm.target = rightHandTarget;
    }
}
