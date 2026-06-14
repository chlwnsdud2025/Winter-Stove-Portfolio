using UnityEngine;
using UnityEngine.XR;

public class VRHandAnimator : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XRNode handNode = XRNode.LeftHand;

    [Header("엄지")]
    [SerializeField] private Transform[] thumb = new Transform[3];

    [Header("검지")]
    [SerializeField] private Transform[] index = new Transform[3];

    [Header("중지")]
    [SerializeField] private Transform[] middle = new Transform[3];

    [Header("약지")]
    [SerializeField] private Transform[] ring = new Transform[3];

    [Header("새끼")]
    [SerializeField] private Transform[] pinky = new Transform[3];

    [Header("엄지 회전")]
    [SerializeField] private Vector3 thumb0Offset;
    [SerializeField] private Vector3 thumb1Offset;
    [SerializeField] private Vector3 thumb2Offset;

    [Header("검지 회전")]
    [SerializeField] private Vector3 index0Offset;
    [SerializeField] private Vector3 index1Offset;
    [SerializeField] private Vector3 index2Offset;

    [Header("중지 회전")]
    [SerializeField] private Vector3 middle0Offset;
    [SerializeField] private Vector3 middle1Offset;
    [SerializeField] private Vector3 middle2Offset;

    [Header("약지 회전")]
    [SerializeField] private Vector3 ring0Offset;
    [SerializeField] private Vector3 ring1Offset;
    [SerializeField] private Vector3 ring2Offset;

    [Header("새끼 회전")]
    [SerializeField] private Vector3 pinky0Offset;
    [SerializeField] private Vector3 pinky1Offset;
    [SerializeField] private Vector3 pinky2Offset;

    [Header("애니메이션")]
    [SerializeField] private float animationSpeed = 15f;

    private InputDevice device;

    private float gripValue;
    private float triggerValue;

    private Quaternion[] thumbBaseRot;
    private Quaternion[] indexBaseRot;
    private Quaternion[] middleBaseRot;
    private Quaternion[] ringBaseRot;
    private Quaternion[] pinkyBaseRot;

    private void Awake()
    {
        thumbBaseRot = CacheRotations(thumb);
        indexBaseRot = CacheRotations(index);
        middleBaseRot = CacheRotations(middle);
        ringBaseRot = CacheRotations(ring);
        pinkyBaseRot = CacheRotations(pinky);
    }

    private void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(handNode);
    }

    private void Update()
    {
        if (!device.isValid)
        {
            device = InputDevices.GetDeviceAtXRNode(handNode);
        }

        device.TryGetFeatureValue(CommonUsages.grip, out gripValue);
        device.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);

        AnimateBone(index, indexBaseRot, 0, triggerValue, index0Offset);
        AnimateBone(index, indexBaseRot, 1, triggerValue, index1Offset);
        AnimateBone(index, indexBaseRot, 2, triggerValue, index2Offset);

        AnimateBone(middle, middleBaseRot, 0, gripValue, middle0Offset);
        AnimateBone(middle, middleBaseRot, 1, gripValue, middle1Offset);
        AnimateBone(middle, middleBaseRot, 2, gripValue, middle2Offset);

        AnimateBone(ring, ringBaseRot, 0, gripValue, ring0Offset);
        AnimateBone(ring, ringBaseRot, 1, gripValue, ring1Offset);
        AnimateBone(ring, ringBaseRot, 2, gripValue, ring2Offset);

        AnimateBone(pinky, pinkyBaseRot, 0, gripValue, pinky0Offset);
        AnimateBone(pinky, pinkyBaseRot, 1, gripValue, pinky1Offset);
        AnimateBone(pinky, pinkyBaseRot, 2, gripValue, pinky2Offset);

        AnimateBone(thumb, thumbBaseRot, 0, gripValue, thumb0Offset);
        AnimateBone(thumb, thumbBaseRot, 1, gripValue, thumb1Offset);
        AnimateBone(thumb, thumbBaseRot, 2, gripValue, thumb2Offset);
    }

    private Quaternion[] CacheRotations(Transform[] bones)
    {
        Quaternion[] rotations = new Quaternion[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                rotations[i] = bones[i].localRotation;
            }
        }

        return rotations;
    }

    private void AnimateBone(
        Transform[] bones,
        Quaternion[] baseRotations,
        int boneIndex,
        float inputValue,
        Vector3 offset)
    {
        if (boneIndex >= bones.Length)
            return;

        if (bones[boneIndex] == null)
            return;

        Quaternion targetRotation =
            baseRotations[boneIndex] *
            Quaternion.Euler(offset * inputValue);

        bones[boneIndex].localRotation =
            Quaternion.Slerp(
                bones[boneIndex].localRotation,
                targetRotation,
                Time.deltaTime * animationSpeed);
    }
}