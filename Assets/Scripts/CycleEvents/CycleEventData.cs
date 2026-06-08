using UnityEngine;

[CreateAssetMenu(fileName = "CycleEventData", menuName = "Scriptable Objects/CycleEventData")]
public class CycleEventData : ScriptableObject
{
    public string id;
    public float elapseTimeNeed;
    public float termTimeNeed;
    public float rate;// 0~1
    public ConditionData[] requires;
}