using UnityEngine;

public class JengaBlock : MonoBehaviour
{
    private bool isExtracted = false; // 이미 성공 판정이 났는지 체크

    // 트리거 영역을 완전히 벗어나는 순간 호출됩니다.
    private void OnTriggerExit(Collider other)
    {
        // 이미 뺀 블록이거나, 게임오버 상태면 무시
        if (isExtracted) return;
        if (JengaManager.Instance != null && JengaManager.Instance.isGameOver) return;

        // 벗어난 영역의 태그가 JengaZone인지 확인
        if (other.CompareTag("JengaZone"))
        {
            SuccessfullyRemoved();
        }
    }

    private void SuccessfullyRemoved()
    {
        isExtracted = true;

        // 매니저에게 성공 알림 (카운트 증가)
        if (JengaManager.Instance != null)
        {
            JengaManager.Instance.AddSuccessCount();
        }

        Debug.Log($"{gameObject.name} 블록 탈출 성공!");

        // (선택 사항) 성공한 블록은 더 이상 감지할 필요가 없으므로 콜라이더를 끄거나 
        // 레이어를 변경하여 물리 연산을 가볍게 최적화할 수도 있습니다.
    }

    public void ResetBlockState()
    {
        isExtracted = false;
    }
}