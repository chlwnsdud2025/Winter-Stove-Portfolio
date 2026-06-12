using UnityEngine;

public class AvatarManager : MonoBehaviour
{
    public static AvatarManager Instance;

    [Header("Avatar")]
    [SerializeField] private Transform avatarRoot;
    [SerializeField] private GameObject defaultAvatar;

    [Header("XR Targets")]
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform rightHandTarget;

    private GameObject currentAvatar;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (defaultAvatar != null)
        {
            ChangeAvatar(defaultAvatar);
        }
        else
        {
            Debug.LogWarning("Default Avatar가 지정되지 않았습니다.");
        }
    }

    public void ChangeAvatar(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("변경할 Avatar Prefab이 비어있습니다.");
            return;
        }

        if (currentAvatar != null)
        {
            Destroy(currentAvatar);
        }

        currentAvatar = Instantiate(prefab, avatarRoot);

        VRIKCalibrator calibrator =
            currentAvatar.GetComponent<VRIKCalibrator>();

        if (calibrator == null)
        {
            Debug.LogError(
                currentAvatar.name +
                " 에 VRIKCalibrator 컴포넌트가 없습니다.");

            return;
        }

        calibrator.Initialize(
            headTarget,
            leftHandTarget,
            rightHandTarget);

        Debug.Log("아바타 변경 완료 : " + currentAvatar.name);
    }
}