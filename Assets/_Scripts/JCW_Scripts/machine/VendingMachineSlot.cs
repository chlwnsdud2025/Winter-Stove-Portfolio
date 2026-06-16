using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// 동전 투입구 — 동전 소켓에 투입 시 자판기 작동
/// </summary>
[RequireComponent(typeof(XRSocketInteractor))]
public class VendingMachineSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VendingMachine vendingMachine;

    private XRSocketInteractor _socket;

    private void Awake()
    {
        _socket = GetComponent<XRSocketInteractor>();
        _socket.selectEntered.AddListener(OnCoinInserted);
    }

    private void OnDestroy()
    {
        _socket.selectEntered.RemoveListener(OnCoinInserted);
    }

    private void OnCoinInserted(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform
                .TryGetComponent<Coin>(out var coin))
        {
            vendingMachine.InsertCoin();

            // 동전 소비
            Destroy(coin.gameObject, 0.1f);
        }
    }
}