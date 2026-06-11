using System;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    public string text;
    public ConditionData condition;
    public QuestData[] preQuests;
    [NonSerialized] public QuestStates state = QuestStates.Sleep;
}
public enum QuestStates
{
    Sleep,
    Running,
    Cleared,
}
