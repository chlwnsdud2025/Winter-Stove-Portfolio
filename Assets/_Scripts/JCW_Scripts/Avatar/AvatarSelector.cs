using UnityEngine;

public class AvatarSelector : MonoBehaviour
{
    [SerializeField]
    AudioClip changeAvatarAudio;
    [SerializeField]
    private GameObject avatarPrefab;

    public void SelectAvatar()
    {
        SoundManager.Instance?.PlaySFX(changeAvatarAudio, transform.position);
        AvatarManager.Instance
            .ChangeAvatar(avatarPrefab);
    }
}