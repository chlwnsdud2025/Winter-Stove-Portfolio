using DG.Tweening;
using UnityEngine;

public class CycleEvent_WaterPlant : CycleEvent
{
    [SerializeField] VoidEventChannel wateredEvent;
    [SerializeField] ConditionData watered;
    [SerializeField] GameObject wateringCan;
    [SerializeField] Transform plantScalingParent;
    [SerializeField] float wateringDistanceThreshold = 1f;
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
        if ((transform.position - wateringCan.transform.position).magnitude <= wateringDistanceThreshold 
            && Vector3.Dot((transform.position - wateringCan.transform.position), wateringCan.transform.forward) > 0)
        {
            var value = 1;
            plantScalingParent.transform.DOScale(value, 1f);
            watered.achived = true;
        }
    }
    
}
