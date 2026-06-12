using DG.Tweening;
using UnityEngine;

public class CycleEvent_WaterPlant : CycleEvent
{
    [SerializeField] VoidEventChannel wateredEvent;
    [SerializeField] ConditionData watered;
    [SerializeField] Transform plantScalingParent;
    [SerializeField] float witheringPower = 0.2f;
    private void Start()
    {
        wateredEvent.OnEventRaised += HandleWatered;
    }
    protected override void Raise()
    {
        var value = Mathf.Max(plantScalingParent.transform.localScale.x - witheringPower, 0f);
        plantScalingParent.transform.DOScale(value, 1f);
    }

    void HandleWatered()
    {
        var value = 1;
        plantScalingParent.transform.DOScale(value, 1f);
        watered.achived = true;
    }
    
}
