using UnityEngine;

public class WateringCan : MonoBehaviour
{
    [SerializeField] VoidEventChannel wateringEvent;
    public void RaiseWaterEvent() => wateringEvent.RaiseEvent();
}
