using UnityEngine;

public class CatGrabbedState : CatBaseState
{
    private readonly int awakeUpHash = Animator.StringToHash("Player_Up");
    private readonly int sleepUpHash = Animator.StringToHash("PlayerUP_2");

    public CatGrabbedState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        // NavMeshAgent 끄기 (물리 충돌 방지)
        if (cat.agent.enabled) cat.agent.enabled = false;

        // 자고 있었는지 확인 후 다른 애니메이션 재생
        if (cat.wasSleepingBeforeGrab)
        {
            cat.animator.CrossFadeInFixedTime(sleepUpHash, 0.1f);
        }
        else
        {
            cat.animator.CrossFadeInFixedTime(awakeUpHash, 0.1f);
        }

        // (참고) 이후 Uping / Uping_2 로 넘어가는 것은 아까 설정한 Animator의 Exit Time이 알아서 해줍니다!
    }

    public override void Update()
    {
        // 들려있는 동안은 특별한 로직 없이 대기합니다.
    }

    public override void Exit()
    {
        // 여기서는 아무것도 하지 않습니다. Agent를 켜는 건 내려놓기가 완전히 끝난 후(PutDownState)에 해야 합니다.
    }
}