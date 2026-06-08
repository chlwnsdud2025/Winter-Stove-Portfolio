
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FloatEventChannel", menuName = "Scriptable Objects/EventChannel/FloatEventChannel")]
public class FloatEventChannel : ScriptableObject
{
    public Action<float> OnEventRaised;

    public void RaiseEvent(float value) => OnEventRaised?.Invoke(value);
}