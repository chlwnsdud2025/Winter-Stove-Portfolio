using UnityEngine;

public class CatRunState : CatBaseState
{
    private readonly int runAnimHash = Animator.StringToHash("Run");

    public CatRunState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        // 1. 뛰기 애니메이션 부드럽게 재생
        cat.animator.CrossFadeInFixedTime(runAnimHash, 0.1f);

        // 2. 이동 속도를 뛰기 속도로 지정 및 에이전트 가동
        cat.agent.speed = cat.runSpeed;
        cat.agent.isStopped = false;

        // 3. 걷기보다 2배 더 넓은 반경으로 목적지를 탐색하여 멀리 뛰게 만듭니다.
        Vector3 targetPosition = cat.GetRandomNavMeshPosition(cat.transform.position, cat.walkRadius * 2f);
        cat.agent.SetDestination(targetPosition);
    }

    public override void Update()
    {
        // 4. 목적지에 무사히 도착했다면
        if (!cat.agent.pathPending && cat.agent.remainingDistance <= cat.agent.stoppingDistance)
        {
            // 달린 후에는 숨을 고르기 위해 대기 상태로 전환
            stateMachine.ChangeState(cat.idleState);
        }
    }

    public override void Exit()
    {
        if (cat.agent.hasPath)
        {
            cat.agent.ResetPath();
        }
    }
}