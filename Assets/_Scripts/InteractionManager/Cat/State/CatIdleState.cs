using UnityEngine;

public class CatIdleState : CatBaseState
{
    // 애니메이션 해시 세팅 (기존 작성 스타일에 맞춤)
    private readonly int idleAnimHash = Animator.StringToHash("Idle");

    private float idleTimer;
    private float idleDuration;

    public CatIdleState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        // 1. 대기 애니메이션 부드럽게 재생
        cat.animator.CrossFadeInFixedTime(idleAnimHash, 0.1f);

        // 2. NavMeshAgent 이동 정지 및 기존 경로 초기화
        if (cat.agent.hasPath)
        {
            cat.agent.ResetPath();
        }
        cat.agent.isStopped = true;

        // 3. 이번 대기 상태에서 얼마나 멍때릴지 결정 (2초 ~ 5초 사이 랜덤)
        idleTimer = 0f;
        idleDuration = 1f;
    }

    public override void Update()
    {
        idleTimer += Time.deltaTime;

        // 설정한 대기 시간이 지나면 다음 행동을 선택하여 전환
        if (idleTimer >= idleDuration)
        {
            ChooseNextState();
        }
    }

    public override void Exit()
    {
        // 대기 상태를 빠져나갈 때, 다음 상태가 이동할 수 있도록 agent를 다시 켜줍니다.
        cat.agent.isStopped = false;
    }

    /// <summary>
    /// 대기가 끝난 후 확률에 따라 다음 상태를 결정하는 함수
    /// </summary>
    private void ChooseNextState()
    {
        int rand = Random.Range(0, 100);

        // 확률 분기 (CatController에 정의될 상태들을 호출)
        if (rand < 40)
        {
            // 40% 확률로 주변을 걸어다님
            stateMachine.ChangeState(cat.walkState);
        }
        else if (rand < 50)
        {
            // 10% 확률로 낮잠 청하기
            stateMachine.ChangeState(cat.sleepState);
        }
        else if (rand < 70)
        {
            // 20% 확률로 우다다 뛰어다님
            stateMachine.ChangeState(cat.runState);
        }
        else if( rand < 90)
        {
            stateMachine.ChangeState(cat.strechState);
        }
        else
        {
            // 10% 확률로 제자리에서 한 번 더 멍 때리기 (타이머만 새로 갱신)
            idleTimer = 0f;
            idleDuration = Random.Range(2f, 4f);
        }
    }
}