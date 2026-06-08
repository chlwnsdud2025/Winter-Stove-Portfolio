
using UnityEngine;

public abstract class CycleEvent : MonoBehaviour
{
    public CycleEventData data;
    
    public bool IsPlaying { get; protected set; }
    public float StartTime { get; protected set; }
    public bool IsFirstEvent { get; protected set; }

    protected abstract void Raise();
    
    private void Start()
    {
        StartTime = Time.time;
        IsFirstEvent = true;
    }
    protected virtual void Update()
    {
        // ensure no multiple event occur
        if (IsPlaying)
        {
            StartTime = Time.time;
            return;
        }
        if(IsFirstEvent && Time.time - StartTime > data.elapseTimeNeed)
        {
            StartTime = Time.time;
            IsFirstEvent = false;
            if(Random.value <= data.rate) Raise();
        }
        else if(Time.time - StartTime > data.termTimeNeed)
        {
            StartTime = Time.time;
            if(Random.value <= data.rate) Raise();
        }
    }
}