using UnityEngine;

public class WateringCan : MonoBehaviour
{
    [SerializeField] VoidEventChannel wateringEvent;
    bool isPooring;
    bool isIntersected;
    void TryRaiseWaterEvent()
    {
        if (isPooring && isIntersected) wateringEvent.RaiseEvent();
    }
    public void StartPooring()
    {
        isPooring = true;
        TryRaiseWaterEvent();
    }
    public void EndPooring()
    {
        isPooring = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FlowerPot") == false) return;
        isIntersected = true;
        TryRaiseWaterEvent();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FlowerPot") == false) return;
        isIntersected = false;
    }
}
