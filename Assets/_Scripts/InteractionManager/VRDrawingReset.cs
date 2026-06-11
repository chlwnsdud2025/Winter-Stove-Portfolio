using UnityEngine;

public class VRDrawingReset : MonoBehaviour
{
    // VRMarker스크립트에 연결한 것과 동일한 'DrawingLines' 부모 오브젝트를 연결합니다.
    public Transform linesParent;

    // 버튼 온클릭 이벤트에 연결할 함수
    public void ResetAllDrawings()
    {
        if (linesParent == null) return;

        // 부모 오브젝트 아래에 있는 모든 자식(선)들을 순회하며 파괴합니다.
        foreach (Transform child in linesParent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("[화이트보드] 그림 전체 리셋 완료.");
    }
}