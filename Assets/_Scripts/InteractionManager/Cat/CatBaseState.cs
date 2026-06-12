using UnityEngine;

public abstract class CatBaseState
{
    protected CatController cat;
    protected CatStateMachine stateMachine;

    // 생성자를 통해 고양이 컨트롤러와 상태 머신을 주입받습니다.
    public CatBaseState(CatController cat, CatStateMachine stateMachine)
    {
        this.cat = cat;
        this.stateMachine = stateMachine;
    }

    // 상태에 진입할 때 최초 1회 실행
    public virtual void Enter() { }

    // 매 프레임(Update)마다 실행할 로직
    public virtual void Update() { }

    // 다른 상태로 전환되며 현재 상태를 빠져나갈 때 1회 실행
    public virtual void Exit() { }
}