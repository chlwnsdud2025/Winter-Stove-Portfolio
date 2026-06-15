using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundEventChannel", menuName = "Scriptable Objects/EventChannel/SoundEventChannel")]
public class SoundEventChannel : ScriptableObject
{
    public Action<Vector3, SoundType> OnEventRaised;

    public void RaiseEvent(Vector3 value, SoundType type) => OnEventRaised?.Invoke(value, type);
}