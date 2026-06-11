using UnityEngine;

public class VRMarker : MonoBehaviour
{
    [Header("드로잉 세팅")]
    public Transform tip;             // 펜촉(Tip) 빈 오브젝트 위치
    public GameObject linePrefab;     // 1단계에서 만든 선 프리팹
    public float drawDistance = 0.05f;// 펜촉이 보드에 닿았다고 인식할 한계 거리
    public LayerMask boardLayer;      // 2단계에서 만든 화이트보드 레이어

    private LineRenderer currentLine;
    private Vector3 previousPosition;

    void Update()
    {
        // [해결책] 레이저의 시작점을 펜촉보다 10cm(0.1f) 뒤로 뺍니다.
        // 이렇게 하면 펜이 칠판을 파고들어도 레이저는 칠판 바깥에서 쏘게 됩니다!
        Vector3 rayOrigin = tip.position - tip.forward * 0.1f;

        // 뒤에서 쏘는 만큼 레이저 길이도 10cm 길게 늘려줍니다.
        float actualDistance = 0.1f + drawDistance;

        // 레이저가 닿았는지 검사
        if (Physics.Raycast(rayOrigin, tip.forward, out RaycastHit hit, actualDistance, boardLayer))
        {
            // [중요] 닿긴 닿았는데, 진짜 펜촉과 칠판 표면 사이의 거리가 너무 멀면 안 그려지게 막습니다.
            // (즉, 허공에서 레이저 포인터처럼 그려지는 것을 방지하고, 펜촉이 보드 근처일 때만 작동)
            float distToBoard = Vector3.Distance(tip.position, hit.point);

            if (distToBoard <= drawDistance)
            {
                // Z-Fighting 방지용 미세 띄움
                Vector3 drawPoint = hit.point + hit.normal * 0.001f;

                if (currentLine == null)
                {
                    StartDrawing(drawPoint);
                }
                else
                {
                    UpdateDrawing(drawPoint);
                }
            }
            else
            {
                // 보드에 맞긴 했지만 펜촉이 너무 떨어져 있으면 끊기
                currentLine = null;
            }
        }
        else
        {
            // 아예 보드를 벗어나면 선 끊기
            currentLine = null;
        }
    }

    private void StartDrawing(Vector3 startPos)
    {
        // 선 프리팹을 씬에 생성합니다.
        GameObject lineObj = Instantiate(linePrefab);
        currentLine = lineObj.GetComponent<LineRenderer>();

        // 시작점 위치를 찍어줍니다.
        currentLine.positionCount = 1;
        currentLine.SetPosition(0, startPos);

        previousPosition = startPos;
    }

    private void UpdateDrawing(Vector3 currentPos)
    {
        // 이전 점과 현재 점의 거리가 일정 수치(예: 5mm) 이상 벌어졌을 때만 점을 추가합니다. 
        // (프레임마다 무조건 찍으면 렉이 발생할 수 있으므로 최적화하는 과정입니다)
        if (Vector3.Distance(previousPosition, currentPos) > 0.005f)
        {
            currentLine.positionCount++;
            currentLine.SetPosition(currentLine.positionCount - 1, currentPos);
            previousPosition = currentPos;
        }
    }

    private void OnDrawGizmos()
    {
        // Tip이 아직 연결되지 않았다면 오류를 막기 위해 실행하지 않습니다.
        if (tip == null) return;

        // Update 함수와 똑같은 레이저 시작점과 길이를 계산합니다.
        Vector3 rayOrigin = tip.position - tip.forward * 0.1f;
        float actualDistance = 0.1f + drawDistance;

        // 1. 레이저 선 그리기 (초록색)
        Gizmos.color = Color.green;
        Gizmos.DrawRay(rayOrigin, tip.forward * actualDistance);

        // 2. 레이저 시작점 (10cm 뒤) 표시 (빨간색 구슬)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(rayOrigin, 0.005f);

        // 3. 실제 펜촉 위치 표시 (파란색 구슬)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(tip.position, 0.005f);

        // 4. 칠판을 인식하는 최대 사거리 끝점 표시 (노란색 구슬)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rayOrigin + tip.forward * actualDistance, 0.005f);
    }
}