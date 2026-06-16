using UnityEngine;
using UnityEngine.Playables;

public class CycleEvent_Nuclear : CycleEvent
{
    static CycleEvent_Nuclear instance;
    [SerializeField] Clock clock;
    const int POSTPONE_AMOUNT = 60;
    bool disabled;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        if (clock != null)
            clock.seconds -= (int)data.elapseTimeNeed;
    }
    protected override void Raise()
    {
        if (disabled) return;
        GetComponent<PlayableDirector>().Play();
    }
    public static void Disable()
    {
        instance.clock.Stop = true;
        instance.disabled = true;
    }
    public static void Postpone()
    {
        if (instance != null)
        {
            instance.Postpone(POSTPONE_AMOUNT);
            instance.clock.seconds -= POSTPONE_AMOUNT;
        }
    }
}
