using UnityEngine;

public class CatStateMachine
{
    // 현재 실행 중인 고양이의 상태
    public CatBaseState CurrentState { get; private set; }

    // 최초 시작 상태를 지정하며 초기화합니다.
    public void Initialize(CatBaseState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    // 현재 상태를 종료하고 새로운 상태로 전환합니다.
    public void ChangeState(CatBaseState newState)
    {
        if (CurrentState == newState) return;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    // 메인 컨트롤러의 Update에서 호출되어 현재 상태의 틱을 골고루 굴려줍니다.
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}