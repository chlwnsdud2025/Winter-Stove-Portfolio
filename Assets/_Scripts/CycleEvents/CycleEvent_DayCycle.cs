using DG.Tweening;
using UnityEngine;

public class CycleEvent_DayCycle : CycleEvent
{
    [SerializeField] Material skybox;
    [SerializeField] float duration = 2f;

    float blend = 0f;
    protected override void Update()
    {
        base.Update();
        skybox.SetFloat("_blend", blend);
    }
    protected override void Raise()
    {
        // night
        if(blend > .5f)
        {
            DOTween.To(() => blend, x => blend = x, 0f, duration);
        }
        // morning
        else
        {
            DOTween.To(() => blend, x => blend = x, 1f, duration);
        }
    }
    private void OnDestroy()
    {
        skybox.SetFloat("_blend", 0);
    }
}
