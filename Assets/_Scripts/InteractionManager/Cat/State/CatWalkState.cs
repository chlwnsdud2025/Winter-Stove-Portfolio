using UnityEngine;

public class CatWalkState : CatBaseState
{
    private readonly int walkAnimHash = Animator.StringToHash("Walk");

    public CatWalkState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        // 1. 걷기 애니메이션 부드럽게 재생
        cat.animator.CrossFadeInFixedTime(walkAnimHash, 0.1f);

        // 2. 이동 속도를 걷기 속도로 지정 및 에이전트 가동
        cat.agent.speed = cat.walkSpeed;
        cat.agent.isStopped = false;

        // 3. CatController에 정의된 함수를 이용해 주변의 랜덤 목적지 설정
        Vector3 targetPosition = cat.GetRandomNavMeshPosition(cat.transform.position, cat.walkRadius);
        cat.agent.SetDestination(targetPosition);
    }

    public override void Update()
    {
        // 4. 에이전트가 경로 계산을 끝냈고, 남은 거리가 멈춤 기준 거리보다 작다면 (목적지 도착)
        if (!cat.agent.pathPending && cat.agent.remainingDistance <= cat.agent.stoppingDistance)
        {
            // 대기 상태로 전환
            stateMachine.ChangeState(cat.idleState);
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