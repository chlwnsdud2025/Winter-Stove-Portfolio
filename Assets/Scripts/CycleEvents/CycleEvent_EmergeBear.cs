
using UnityEngine;

public class CycleEvent_EmergeBear : CycleEvent
{
    [SerializeField] VoidEventChannel gunShotEvent;
    [SerializeField] RuntimeConditionData bearAlive;
    [SerializeField] GameObject Bear;

    int bearLife = 3;

    private void Awake()
    {
        Bear.SetActive(false);
    }
    protected override void Raise()
    {
        if(ConditionChecker.Check(data.requires, bearAlive))
        {
            Bear.SetActive(true);
            IsPlaying = true;
            gunShotEvent.OnEventRaised -= HandleGunShotted;
            gunShotEvent.OnEventRaised += HandleGunShotted;
        }
    }
    void HandleGunShotted()
    {
        gunShotEvent.OnEventRaised -= HandleGunShotted;
        IsPlaying = false;
        if (bearAlive.value)
        {
            bearLife--;
            if(bearLife == 0)
            {
                bearAlive.value = false;
                Bear.SetActive(false);
            }
        }
    }
    private void OnDisable()
    {
        gunShotEvent.OnEventRaised -= HandleGunShotted;
    }
}
