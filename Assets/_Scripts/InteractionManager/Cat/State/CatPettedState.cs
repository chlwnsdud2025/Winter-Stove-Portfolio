using UnityEngine;

public class CatPettedState : CatBaseState
{
    private readonly int happyAnimHash = Animator.StringToHash("Take 001 (1)"); // 애교 애니메이션

    private float pettedTimer;
    private float pettedDuration = 3.0f; // 3초간 애교 부리기

    public CatPettedState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        // 1. 애교 애니메이션 재생
        cat.animator.CrossFadeInFixedTime(happyAnimHash, 0.1f);

        // 2. 이동 즉시 정지
        if (cat.agent.enabled)
        {
            if (cat.agent.hasPath) cat.agent.ResetPath();
            cat.agent.isStopped = true;
        }

        pettedTimer = 0f;
    }

    public override void Update()
    {
        pettedTimer += Time.deltaTime;

        // 시간이 다 되면 다시 대기(Idle) 상태로 돌아가서 다음 행동을 생각함
        if (pettedTimer >= pettedDuration)
        {
            stateMachine.ChangeState(cat.idleState);
        }
    }

    public override void Exit()
    {
        if (cat.agent.enabled)
        {
            cat.agent.isStopped = false;
        }
    }
}