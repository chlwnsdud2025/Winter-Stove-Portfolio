using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class GunBehavior : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    public VoidEventChannel GunshotChannel;
    //public ConditionData gunshot;

    [Header("발사 위치 (총구 끝에 배치할 빈 오브젝트)")]
    public Transform firePoint;

    [Header("시각 효과 프리팹 (Muzzle Flash Prefab)")]
    public GameObject muzzleFlashPrefab;

    [Header("이펙트 삭제 대기 시간 (초)")]
    public float destroyDelay = 0.5f;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // activated: 물건을 '잡은 상태에서' 트리거를 누를 때 실행되는 XRI 내장 이벤트
        grabInteractable.activated.AddListener(OnFire);
    }

    // 총 발사 로직
    private void OnFire(ActivateEventArgs args)
    {
        // 프리팹과 위치가 모두 지정되어 있을 때만 실행
        if (muzzleFlashPrefab != null && firePoint != null)
        {
            // 1. 지정된 FirePoint의 위치와 회전값으로 이펙트 프리팹 동적 생성
            // (파티클 프리팹에 'Play On Awake'가 켜져 있으면 생성되자마자 바로 불꽃이 터집니다)
            GameObject flashInstance = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);

            GunshotChannel.RaiseEvent();
            //gunshot.achived = true;

            // 2. [중요] 프리팹 방식의 필수 처리! 
            // 찌꺼기 오브젝트가 Hierarchy에 계속 쌓여 렉이 걸리지 않도록 일정 시간 뒤 자동으로 삭제합니다.
            Destroy(flashInstance, destroyDelay);
        }

        Debug.Log("탕! 총이 발사되었습니다.");
    }
}