using DG.Tweening;
using UnityEngine;

public class CycleEvent_WaterPlant : CycleEvent
{
    [SerializeField] VoidEventChannel wateredEvent;
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
        Debug.Log("꽃이 시든다...");
        var value = Mathf.Max(plantScalingParent.transform.localScale.x - 0.2f, 0f);
        plantScalingParent.transform.DOScale(value, 1f);
    }

    void HandleWatered()
    {
        if ((transform.position - wateringCan.transform.position).magnitude <= wateringDistanceThreshold 
            && Vector3.Dot((transform.position - wateringCan.transform.position), wateringCan.transform.forward) > 0)
        {
            Debug.Log("꽃이 자란다...");
            var value = 1;
            plantScalingParent.transform.DOScale(value, 1f);
        }
    }
    
}
