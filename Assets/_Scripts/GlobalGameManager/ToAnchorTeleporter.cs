using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class ToAnchorTeleporter : MonoBehaviour
{
    [SerializeField] TeleportationProvider teleportationProvider;
    [SerializeField] private GameObject xrOrigin; // Your main XR Origin (XR Rig) object
    [SerializeField] Transform targetTransform;
    [SerializeField] AudioClip nextAudio;
    public void Teleport()
    {
        SoundManager.Instance.PlayBGM(nextAudio, 1.5f);
        TeleportRequest req = new TeleportRequest
        {
            destinationPosition = targetTransform.position,
            destinationRotation = targetTransform.rotation
        };

        teleportationProvider.QueueTeleportRequest(req);
    }
}
