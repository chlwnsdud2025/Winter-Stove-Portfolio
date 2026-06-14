using UnityEngine;

public class CatSleepState : CatBaseState
{
    private readonly int sleepStartHash = Animator.StringToHash("SleepStart");
    private readonly int tieredSleepHash = Animator.StringToHash("Tiered_Sleep");
    private readonly int sleepLoopHash = Animator.StringToHash("Sleep");
    private readonly int wakeUpHash = Animator.StringToHash("Wake_Up");

    // 수면 단계를 내부적으로 나눕니다.
    private enum SleepPhase { LieDown, Sleeping, WakingUp }
    private SleepPhase currentPhase;

    private float sleepTimer;
    private float sleepDuration;

    public CatSleepState(CatController cat, CatStateMachine stateMachine) : base(cat, stateMachine) { }

    public override void Enter()
    {
        if (cat.agent.hasPath) cat.agent.ResetPath();
        cat.agent.isStopped = true;

        currentPhase = SleepPhase.LieDown; // 1단계: 눕기 시작

        // 랜덤으로 눕는 애니메이션 재생
        if (Random.value > 0.5f)
            cat.animator.CrossFadeInFixedTime(sleepStartHash, 0.1f);
        else
            cat.animator.CrossFadeInFixedTime(tieredSleepHash, 0.1f);
    }

    public override void Update()
    {
        AnimatorStateInfo stateInfo = cat.animator.GetCurrentAnimatorStateInfo(0);

        switch (currentPhase)
        {
            case SleepPhase.LieDown:
                // 눕는 애니메이션이 거의 끝났다면 (90% 진행)
                if (IsAnimationFinished(stateInfo, sleepStartHash) || IsAnimationFinished(stateInfo, tieredSleepHash))
                {
                    currentPhase = SleepPhase.Sleeping; // 2단계: 꿀잠 시작
                    cat.animator.CrossFadeInFixedTime(sleepLoopHash, 0.1f);

                    sleepTimer = 0f;
                    sleepDuration = Random.Range(15f, 30f); // 15~30초 수면
                }
                break;

            case SleepPhase.Sleeping:
                sleepTimer += Time.deltaTime;
                // 수면 시간이 다 찼다면
                if (sleepTimer >= sleepDuration)
                {
                    currentPhase = SleepPhase.WakingUp; // 3단계: 일어나기 시작
                    cat.animator.CrossFadeInFixedTime(wakeUpHash, 0.1f);
                }
                break;

            case SleepPhase.WakingUp:
                // 일어나는 애니메이션이 거의 끝났다면
                if (IsAnimationFinished(stateInfo, wakeUpHash))
                {
                    // 모든 수면 시퀀스가 끝났으므로 비로소 상태 전환!
                    stateMachine.ChangeState(cat.idleState);
                }
                break;
        }
    }

    public override void Exit()
    {
        cat.agent.isStopped = false; // 완전히 다 일어난 뒤에야 이동 가능
    }

    // 애니메이션 종료 여부를 체크하는 유틸리티 함수
    private bool IsAnimationFinished(AnimatorStateInfo stateInfo, int animHash)
    {
        return stateInfo.shortNameHash == animHash && stateInfo.normalizedTime >= 1f && !cat.animator.IsInTransition(0);
    }
}