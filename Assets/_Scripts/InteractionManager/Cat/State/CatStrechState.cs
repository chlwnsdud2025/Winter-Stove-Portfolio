using UnityEngine;

public class CatStrechState : CatBaseState
{
    private readonly int strechAnimHash = Animator.StringToHash("Streching (1)");

    public CatStrechState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        // 1. 걷기 애니메이션 부드럽게 재생
        cat.animator.CrossFadeInFixedTime(strechAnimHash, 0.1f);

        // 2. 이동 즉시 정지
        if (cat.agent.enabled)
        {
            if (cat.agent.hasPath) cat.agent.ResetPath();
            cat.agent.isStopped = true;
        }

        

    }

    public override void Update()
    {
        // 현재 재생 중인 애니메이션의 정보를 가져옵니다.
        AnimatorStateInfo stateInfo = cat.animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션이 거의 끝났다면 (90% 이상 재생 완료)
        if (stateInfo.shortNameHash == strechAnimHash)
        {
            if (stateInfo.normalizedTime >= 0.95f && !cat.animator.IsInTransition(0))
            {
                // 완전히 내려놓았으므로 일상(Idle)으로 복귀!
                stateMachine.ChangeState(cat.idleState);
            }
        }
    }

    public override void Exit()
    {
        // 이동이 끝났으므로 경로를 깨끗하게 비워줍니다.
        if (cat.agent.hasPath)
        {
            cat.agent.ResetPath();
        }
    }
}