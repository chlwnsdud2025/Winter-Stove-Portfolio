using UnityEngine;

public class SinkManager : MonoBehaviour
{
    [Header("성공 조건")]
    public int targetPlateCount = 5; // 목표 접시 개수
    private int currentPlateCount = 0; // 현재 소켓에 들어온 접시 개수

    public ConditionData sinkCondition;
    public VoidEventChannel sinkchannel;

    // 소켓에 접시가 안착했을 때 호출할 함수
    public void OnPlatePlaced()
    {
        currentPlateCount++;
        Debug.Log($"[설거지 미션] 접시 안착! 현재 진행도: {currentPlateCount} / {targetPlateCount}");

        // 5개의 접시가 모두 모였다면?
        if (currentPlateCount >= targetPlateCount)
        {
            MissionComplete();
        }
    }

    // 소켓에 있던 접시를 플레이어가 다시 집어 들었을 때 호출할 함수 (예외 처리)
    public void OnPlateRemoved()
    {
        currentPlateCount--;
        // 0 미만으로 떨어지지 않게 방어 코드
        if (currentPlateCount < 0) currentPlateCount = 0;

        Debug.Log($"[설거지 미션] 접시가 빠졌습니다. 현재 진행도: {currentPlateCount} / {targetPlateCount}");
    }

    // 최종 성공 판정
    private void MissionComplete()
    {
        sinkCondition.achived = true;
        sinkchannel.RaiseEvent();
        Debug.Log("[미션 성공] 모든 접시를 깨끗하게 닦아 건조대에 넣었습니다!");

        // (선택) 여기에 성공 팝업 UI를 띄우거나 다음 스테이지로 넘어가는 로직을 넣으시면 됩니다!
    }
}
