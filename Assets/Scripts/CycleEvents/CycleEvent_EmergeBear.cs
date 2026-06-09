
using UnityEngine;

public class CycleEvent_EmergeBear : CycleEvent
{
    [SerializeField] VoidEventChannel gunShotEvent;
    [SerializeField] RuntimeConditionData bearAlive;
    [SerializeField] Bear bear;
    [SerializeField] string onShotAnimatorTrigger = "OnShotted";
    int bearLife = 3;

    private void Awake()
    {
        bear.gameObject.SetActive(false);
    }
    protected override void Raise()
    {
        if(ConditionChecker.Check(data.requires, bearAlive))
        {
            bear.gameObject.SetActive(true);
            bear.anim.ResetTrigger(onShotAnimatorTrigger);
            IsPlaying = true;
            gunShotEvent.OnEventRaised -= HandleGunShotted;
            gunShotEvent.OnEventRaised += HandleGunShotted;
        }
    }
    void HandleGunShotted()
    {
        bear.anim.SetTrigger(onShotAnimatorTrigger);
        gunShotEvent.OnEventRaised -= HandleGunShotted;
        IsPlaying = false;
        if (bearAlive.value)
        {
            bearLife--;
            if(bearLife == 0)
            {
                bearAlive.value = false;
                bear.gameObject.SetActive(false);
            }
        }
    }
    private void OnDisable()
    {
        gunShotEvent.OnEventRaised -= HandleGunShotted;
    }
}
