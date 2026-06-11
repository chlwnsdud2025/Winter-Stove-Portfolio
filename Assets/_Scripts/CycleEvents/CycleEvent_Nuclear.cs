using UnityEngine;
using UnityEngine.Playables;

public class CycleEvent_Nuclear : CycleEvent
{
    protected override void Raise()
    {
        GetComponent<PlayableDirector>().Play();
    }
}
