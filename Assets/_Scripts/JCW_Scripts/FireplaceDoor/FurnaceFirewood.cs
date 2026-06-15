using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System;


/// <summary>
/// 장작 오브젝트 — Grab 가능, 소켓에 투입 시 소비됨
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class FurnaceFirewood : MonoBehaviour
{
    [Header("Firewood Settings")]
    [SerializeField] private float fuelAmount = 30f; // 이 장작이 추가하는 연료량(초)
    [SerializeField] ConditionData consumeWood;

    public float FuelAmount => fuelAmount;
    public Action OnConsumed;

    private XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    /// <summary>
    /// FurnaceSocket에서 호출 — 장작 소비 처리
    /// </summary>
    public void Consume()
    {
        OnConsumed?.Invoke();
        consumeWood.achived = true;
        _grabInteractable.enabled = false; // Grab 비활성화
        Destroy(gameObject, 0.1f);
    }
}