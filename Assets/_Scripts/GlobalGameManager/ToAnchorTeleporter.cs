using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class ToAnchorTeleporter : MonoBehaviour
{
    [SerializeField] TeleportationProvider teleportationProvider;
    [SerializeField] private GameObject xrOrigin; // Your main XR Origin (XR Rig) object
    [SerializeField] TeleportationAnchor anchor;
    [SerializeField] AudioClip nextAudio;
    public void Teleport()
    {
        SoundManager.Instance.PlayBGM(nextAudio, 1.5f);
        xrOrigin.transform.position = anchor.teleportAnchorTransform.position;
        xrOrigin.transform.rotation = anchor.teleportAnchorTransform.rotation;
        //TeleportRequest req = new TeleportRequest
        //{
        //    destinationPosition = anchor.teleportAnchorTransform.position,
        //    destinationRotation = anchor.teleportAnchorTransform.rotation
        //};

        //teleportationProvider.QueueTeleportRequest(req);
        //// 4. Adjust the orientation if Match Orientation rules require it
        //MatchOrientation orientationMode = anchor.matchOrientation;
        //ApplyOrientation(anchor.teleportAnchorTransform, orientationMode);
    }
    private void ApplyOrientation(Transform targetTransform, MatchOrientation mode)
    {
        if (mode == MatchOrientation.TargetUpAndForward)
        {
            xrOrigin.transform.rotation = targetTransform.rotation;
        }
        else if (mode == MatchOrientation.TargetUp)
        {
            // Retains original forward direction but aligns the vertical up axis
            Vector3 forward = Vector3.ProjectOnPlane(xrOrigin.transform.forward, targetTransform.up).normalized;
            xrOrigin.transform.rotation = Quaternion.LookRotation(forward, targetTransform.up);
        }
    }
}
