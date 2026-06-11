using UnityEngine;
using System.Collections.Generic; // 리스트 사용을 위해 추가

public class VRMarker : MonoBehaviour
{
    [Header("드로잉 세팅")]
    public Transform tip;
    public GameObject linePrefab;
    public float drawDistance = 0.05f;
    public LayerMask boardLayer;

    // [수정] 생성된 선들을 자식으로 넣을 부모 오브젝트 (빈 오브젝트 하나 만들어서 연결)
    public Transform linesParent;

    private LineRenderer currentLine;
    private Vector3 previousPosition;

    void Update()
    {
        Vector3 rayOrigin = tip.position - tip.forward * 0.1f;
        float actualDistance = 0.1f + drawDistance;

        if (Physics.Raycast(rayOrigin, tip.forward, out RaycastHit hit, actualDistance, boardLayer))
        {
            float distToBoard = Vector3.Distance(tip.position, hit.point);
            if (distToBoard <= drawDistance)
            {
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
                currentLine = null;
            }
        }
        else
        {
            currentLine = null;
        }
    }

    private void StartDrawing(Vector3 startPos)
    {
        GameObject lineObj = Instantiate(linePrefab);

        // [수정] 생성된 선을 부모 오브젝트의 자식으로 등록합니다.
        if (linesParent != null)
        {
            lineObj.transform.SetParent(linesParent);
        }
        else
        {
            Debug.LogWarning("Lines Parent가 연결되지 않아 리셋 기능이 작동하지 않습니다!");
        }

        currentLine = lineObj.GetComponent<LineRenderer>();
        currentLine.positionCount = 1;
        currentLine.SetPosition(0, startPos);

        previousPosition = startPos;
    }

    private void UpdateDrawing(Vector3 currentPos)
    {
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