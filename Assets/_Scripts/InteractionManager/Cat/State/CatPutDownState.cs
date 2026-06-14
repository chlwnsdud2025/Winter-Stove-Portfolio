using UnityEngine;

public class CatPutDownState : CatBaseState
{
    private readonly int awakeDownHash = Animator.StringToHash("PlayerDown");
    private readonly int sleepDownHash = Animator.StringToHash("PlayerDown_2");

    public CatPutDownState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        // 내려놓는 애니메이션 재생
        if (cat.wasSleepingBeforeGrab)
        {
            cat.animator.CrossFadeInFixedTime(sleepDownHash, 0.1f);
        }
        else
        {
            cat.animator.CrossFadeInFixedTime(awakeDownHash, 0.1f);
        }
    }

    public override void Update()
    {
        // 현재 재생 중인 애니메이션의 정보를 가져옵니다.
        AnimatorStateInfo stateInfo = cat.animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션이 거의 끝났다면 (90% 이상 재생 완료)
        if (stateInfo.shortNameHash == awakeDownHash || stateInfo.shortNameHash == sleepDownHash)
        {
            if (stateInfo.normalizedTime >= 0.9f && !cat.animator.IsInTransition(0))
            {
                // 완전히 내려놓았으므로 일상(Idle)으로 복귀!
                stateMachine.ChangeState(cat.idleState);
            }
        }
    }

    public override void Exit()
    {
        // 내려놓기 애니메이션이 모두 끝나고 Idle로 돌아갈 때 비로소 길찾기(AI)를 다시 켜줍니다.
        if (!cat.agent.enabled)
        {
            cat.agent.enabled = true;
        }

        // 플래그 초기화 (다음 번에 잡힐 때를 위해)
        cat.wasSleepingBeforeGrab = false;
    }
}