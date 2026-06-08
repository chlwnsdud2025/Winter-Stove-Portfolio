
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IntEventChannel", menuName = "Scriptable Objects/EventChannel/IntEventChannel")]
public class IntEventChannel : ScriptableObject
{
    public Action<int> OnEventRaised;

    public void RaiseEvent(int value) => OnEventRaised?.Invoke(value);
}