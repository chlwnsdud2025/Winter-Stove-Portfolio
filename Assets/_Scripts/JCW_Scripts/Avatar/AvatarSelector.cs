using UnityEngine;

public class AvatarSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject avatarPrefab;

    public void SelectAvatar()
    {
        AvatarManager.Instance
            .ChangeAvatar(avatarPrefab);
    }
}