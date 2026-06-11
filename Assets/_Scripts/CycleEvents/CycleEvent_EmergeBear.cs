
using UnityEngine;

public class CycleEvent_EmergeBear : CycleEvent
{
    [SerializeField] VoidEventChannel gunShotEvent;
    [SerializeField] ConditionData bearDead;
    [SerializeField] Bear bear;
    [SerializeField] string onShotAnimatorTrigger = "OnShotted";
    int bearLife = 3;

    private void Awake()
    {
        bear.gameObject.SetActive(false);
    }
    protected override void Raise()
    {
        if(bearDead.achived == false)
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
        if (bearDead.achived == false)
        {
            bearLife--;
            if(bearLife == 0)
            {
                bearDead.achived = true;
                bear.gameObject.SetActive(false);
            }
        }
    }
    private void OnDisable()
    {
        gunShotEvent.OnEventRaised -= HandleGunShotted;
    }
}
