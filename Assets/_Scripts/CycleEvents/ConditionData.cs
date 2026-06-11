
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionData", menuName = "Scriptable Objects/ConditionData")]
public class ConditionData : ScriptableObject
{
    [SerializeField] string _id;
    public string Id { get => _id; }

    // Runtime
    [NonSerialized] public bool achived;
}