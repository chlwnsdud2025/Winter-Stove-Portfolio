using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


[RequireComponent(typeof(XRGrabInteractable))]
public class Coin : MonoBehaviour
{
    [Header("코인 퀘스트")]
    [SerializeField] private ConditionData coinmilk;

    private XRGrabInteractable _grabInteractable;
    public Action Oncoinmilk;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
          Oncoinmilk?.Invoke();
          coinmilk.achived = true;
        Debug.Log("coin");
    }
}