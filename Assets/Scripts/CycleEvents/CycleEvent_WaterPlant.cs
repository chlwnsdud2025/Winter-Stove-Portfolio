using UnityEngine;

public class CycleEvent_WaterPlant : CycleEvent
{
    [SerializeField] VoidEventChannel wateredEvent;
    [SerializeField] GameObject wateringCan;
    [SerializeField] float wateringDistanceThreshold;

    private void Start()
    {
        wateredEvent.OnEventRaised += HandleWatered;
    }
    protected override void Raise()
    {
        Debug.Log("식물이 시들고 있다...");
    }

    void HandleWatered()
    {
        if ((transform.position - wateringCan.transform.position).magnitude <= wateringDistanceThreshold)
        {
            Debug.Log("식물이 생기를 되찾고 있다...");
        }
    }
    
}
