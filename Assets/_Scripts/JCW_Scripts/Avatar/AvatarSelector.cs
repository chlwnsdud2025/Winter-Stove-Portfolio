using UnityEngine;

public class AvatarSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject avatarPrefab;

    public void SelectAvatar()
    {
        AvatarManager.Instance
            .ChangeAvatar(avatarPrefab);

        Debug.Log("아바타교체");
    }
}