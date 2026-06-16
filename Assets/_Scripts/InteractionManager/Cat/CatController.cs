using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class CatController : MonoBehaviour
{
    [SerializeField] AudioClip meowAudio;
    [SerializeField] AudioClip angryMeowAudio;
    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Movement Settings")]
    public float walkRadius = 5f;
    public float walkSpeed = 1.0f;
    public float runSpeed = 3.5f;

    [HideInInspector] public bool wasSleepingBeforeGrab = false;

    // 1. 상태 머신 및 개별 상태들 선언 (프로퍼티와 필드 혼용 스타일)
    public CatStateMachine stateMachine { get; private set; }
    public CatIdleState idleState { get; private set; }
    public CatWalkState walkState { get; private set; }
    public CatRunState runState { get; private set; }

    public CatPettedState pettedState { get; private set; }
    public CatGrabbedState grabbedState { get; private set; }

    public CatPutDownState putDownState { get; private set; }
    public CatSleepState sleepState { get; private set; }

    public CatStrechState strechState { get; private set; }


    // (향후 추가될 상태들 미리 예시 선언)
    // public CatSleepState sleepState { get; private set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 2. 상태 머신 인스턴스화
        stateMachine = new CatStateMachine();

        // 3. 모든 상태 클래스 조립 및 메모리 할당
        idleState = new CatIdleState(this, stateMachine);
        walkState = new CatWalkState(this, stateMachine);
        runState = new CatRunState(this, stateMachine);
        pettedState = new CatPettedState(this, stateMachine);
        grabbedState = new CatGrabbedState(this, stateMachine);
        putDownState = new CatPutDownState(this, stateMachine);
        sleepState = new CatSleepState(this, stateMachine);
        strechState = new CatStrechState(this, stateMachine);
}

    private void Start()
    {
        // 4. 최초 상태를 Idle로 가동 시작!
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        // 5. 현재 상태의 코드를 매 프레임 실행
        stateMachine.Update();
    }

    public Vector3 GetRandomNavMeshPosition(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }


    // 손이 닿았을 때 (XR Simple/Grab Interactable의 Hover Entered에 연결)
    public void OnHoverEntered()
    {
        // 고양이가 들려있는 상태가 아닐 때만 애교를 부립니다.
        if (stateMachine.CurrentState != grabbedState)
        {
            SoundManager.Instance?.PlaySFX(meowAudio);
            stateMachine.ChangeState(pettedState);
        }
    }

    // 고양이를 집어 들었을 때 (XR Grab Interactable의 Select Entered에 연결)
    public void OnGrabEntered()
    {
        SoundManager.Instance?.PlaySFX(angryMeowAudio, transform.position);
        // 잡히는 순간, 현재 상태가 SleepState였다면 true로 기록해 둡니다.
        // (SleepState 스크립트를 구현하셨다는 가정 하에 작성)
        wasSleepingBeforeGrab = (stateMachine.CurrentState == sleepState);

        stateMachine.ChangeState(grabbedState);
    }

    public void OnGrabExited()
    {
        // 바닥에 놓으면 즉시 Idle이 아니라, 내려놓는 애니메이션 상태로 보냅니다.
        stateMachine.ChangeState(putDownState);
    }
}