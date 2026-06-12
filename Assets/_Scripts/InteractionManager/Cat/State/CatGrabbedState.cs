using UnityEngine;

public class CatGrabbedState : CatBaseState
{
    private readonly int grabbedAnimHash = Animator.StringToHash("Grabbed"); // 대롱대롱 매달린 애니메이션

    public CatGrabbedState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        // 1. 대롱대롱 매달리는 애니메이션 재생
        cat.animator.CrossFadeInFixedTime(grabbedAnimHash, 0.1f);

        // 2. [핵심] 플레이어의 손(물리)에 이끌려가야 하므로 AI 길찾기 컴포넌트를 완전히 끕니다.
        if (cat.agent.enabled)
        {
            cat.agent.enabled = false;
        }
    }

    public override void Update()
    {
        // 들려있는 동안에는 타이머도, 스스로 하는 행동도 없습니다. 
        // 오직 플레이어가 손을 놓을 때까지 기다립니다.
    }

    public override void Exit()
    {
        // 플레이어가 고양이를 바닥에 놓아서 상태를 빠져나갈 때, 다시 AI를 켜줍니다.
        cat.agent.enabled = true;
    }
}