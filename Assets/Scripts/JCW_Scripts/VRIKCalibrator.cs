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

    private void Start()
    {
        vrik.solver.spine.headTarget = headTarget;
        vrik.solver.leftArm.target = leftHandTarget;
        vrik.solver.rightArm.target = rightHandTarget;
    }

    private void LateUpdate()
    {
        Vector3 headPos = headTarget.position + headOffset;
        transform.position = new Vector3(
            headPos.x,
            transform.position.y + heightOffset,
            headPos.z
        );

        float yRotation = headTarget.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // 손 회전 오프셋
        vrik.solver.leftArm.target.rotation =
            vrik.solver.leftArm.target.rotation * Quaternion.Euler(leftHandRotOffset);
        vrik.solver.rightArm.target.rotation =
            vrik.solver.rightArm.target.rotation * Quaternion.Euler(rightHandRotOffset);
    }
}