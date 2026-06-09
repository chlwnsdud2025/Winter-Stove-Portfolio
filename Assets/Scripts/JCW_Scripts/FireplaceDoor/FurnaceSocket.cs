using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// 난로 내부 소켓 — 문이 열렸을 때만 활성화
/// 장작 투입 시 FurnaceManager에 연료 추가
/// </summary>
[RequireComponent(typeof(XRSocketInteractor))]
public class FurnaceSocket : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FurnaceDoor furnaceDoor;
    [SerializeField] private FurnaceManager furnaceManager;

    private XRSocketInteractor _socket;

    private void Awake()
    {
        _socket = GetComponent<XRSocketInteractor>();
        _socket.selectEntered.AddListener(OnFirewoodInserted);
    }

    private void OnDestroy()
    {
        _socket.selectEntered.RemoveListener(OnFirewoodInserted);
    }

    private void Update()
    {
        // 문이 열렸을 때만 소켓 활성화
        _socket.socketActive = furnaceDoor.IsOpen;
    }

    private void OnFirewoodInserted(SelectEnterEventArgs args)
    {
        // 장작인지 확인
        if (args.interactableObject.transform
                .TryGetComponent<FurnaceFirewood>(out var firewood))
        {
            furnaceManager.AddFuel(firewood.FuelAmount);
            firewood.Consume();
        }
    }
}