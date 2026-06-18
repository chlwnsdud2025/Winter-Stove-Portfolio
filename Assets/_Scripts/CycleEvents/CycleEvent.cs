
using UnityEngine;

public abstract class CycleEvent : MonoBehaviour
{
    public CycleEventData data;

    public bool IsPlaying { get; protected set; }
    public float StartTime { get; protected set; }
    public bool IsFirstEvent { get; protected set; }

    private float postponeAmount = 0f;
    public float SetRemained(float value) => postponeAmount = value - (IsFirstEvent ? data.elapseTimeNeed : data.termTimeNeed) + (Time.time - StartTime);
    public float GetRemained() => ((IsFirstEvent ? data.elapseTimeNeed : data.termTimeNeed) + postponeAmount) - (Time.time - StartTime);
    protected void Postpone(float value) => postponeAmount += value;
    protected abstract void Raise();
    
    private void Start()
    {
        StartTime = Time.time;
        IsFirstEvent = true;
    }
    protected virtual void Update()
    {
        if (IsPlaying || CheckRequired() == false || CycleEvent_Nuclear.IsRaised)
        {
            StartTime = Time.time;
            return;
        }
        if(IsFirstEvent && Time.time - StartTime > data.elapseTimeNeed + postponeAmount)
        {
            postponeAmount = 0;
            StartTime = Time.time;
            IsFirstEvent = false;
            if(Random.value <= data.rate) Raise();
        }
        else if(Time.time - StartTime > data.termTimeNeed + postponeAmount)
        {
            postponeAmount = 0;
            StartTime = Time.time;
            if(Random.value <= data.rate) Raise();
        }
    }
    bool CheckRequired()
    {
        for(int i = 0;i  < data.requires.Length; i++)
        {
            if (data.requires[i].achived == false) return false;
        }
        return true;
    }
}