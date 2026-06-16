using UnityEngine;
using UnityEngine.Rendering;

public class SimpleCupBehavior : MonoBehaviour
{
    [Header("술 오브젝트 세팅")]
    public GameObject liquidObject;

    [Header("마시기 조건 설정")]
    public float drinkAngle = 90f;
    public float drinkDistance = 0.25f;

    [Header("취기 효과 (URP Volume)")]
    public Volume drunkVolume;
    public float drunkDuration = 5f;

    private Transform playerCamera;
    private bool isFull = false;
    private float drunkTimer = 0f;
    private int drinkCount = 0;

    public VoidEventChannel Drunkchannel;
    public ConditionData drunkCondition;

    private void Start()
    {
        if (Camera.main != null) playerCamera = Camera.main.transform;
        if (drunkVolume != null) drunkVolume.weight = 0f;

        if (liquidObject != null)
        {
            isFull = liquidObject.activeSelf;
        }
    }

    private void Update()
    {
        CheckDrinking();
        HandleSoberUp();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!isFull && liquidObject != null)
        {
            isFull = true;
            liquidObject.SetActive(true);
            Debug.Log("[술잔] 술이 가득 채워졌습니다!");
        }
    }

    private void CheckDrinking()
    {
        if (playerCamera == null || !isFull) return;

        float tiltAngle = Vector3.Angle(transform.up, Vector3.up);
        float distanceToFace = Vector3.Distance(transform.position, playerCamera.position);

        if (tiltAngle >= drinkAngle && distanceToFace <= drinkDistance)
        {
            isFull = false;

            if (liquidObject != null)
            {
                liquidObject.SetActive(false);
            }

            drinkCount++;
            Debug.Log($"[원샷!] 술을 마셨습니다. 누적 횟수: {drinkCount}/3");

            if (drinkCount >= 3)
            {
                if (drunkVolume != null)
                {
                    drunkVolume.weight = 1f;
                }
                
                drunkTimer = drunkDuration;
                drunkCondition.achived = true;
                Drunkchannel.RaiseEvent();
                Debug.Log("[취기 발동] 3번 마셔서 어질어질해집니다!");

            }
        }
    }

    private void HandleSoberUp()
    {
        if (drunkTimer > 0f)
        {
            drunkTimer -= Time.deltaTime;

            if (drunkTimer <= 0f)
            {
                drunkTimer = 0f;
                drinkCount = 0;

                if (drunkVolume != null)
                {
                    drunkVolume.weight = 0f;
                }
                Debug.Log("[술 깨기] 지속 시간이 다 되어 술이 한 번에 깨고 스택이 초기화되었습니다!");
            }
        }
    }
}