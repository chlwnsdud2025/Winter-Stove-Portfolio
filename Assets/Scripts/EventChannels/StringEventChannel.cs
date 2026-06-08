
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StringEventChannel", menuName = "Scriptable Objects/EventChannel/StringEventChannel")]
public class StringEventChannel : ScriptableObject
{
    public Action<string> OnEventRaised;

    public void RaiseEvent(string value) => OnEventRaised?.Invoke(value);
}