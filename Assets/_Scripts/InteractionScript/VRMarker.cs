using UnityEngine;
using System.Collections.Generic;

public class VRMarker : MonoBehaviour
{
    [Header("드로잉 세팅")]
    public Transform tip;
    public GameObject linePrefab;
    public float drawDistance = 0.05f;
    public LayerMask boardLayer;
    public Transform linesParent;

    [Header("제스처 인식 최소 점수 (0.7 ~ 0.8 추천)")]
    public float recognitionThreshold = 0.30f;

    private LineRenderer currentLine;
    private Vector3 previousPosition;

    public VoidEventChannel drawTrigangle;

    // 현재 그리고 있는 선의 3D 점들을 기록할 리스트
    private List<Vector3> currentLinePositions = new List<Vector3>();

    // $1 인식기 인스턴스
    private DollarRecognizer recognizer;

    public ConditionData DrawSuccess;

    void Awake()
    {
        recognizer = new DollarRecognizer();
        RegisterTemplates(); // 3단계에서 만들 패턴 등록 함수
    }

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
                EndDrawing(); // 보드 레이저 범위에서 벗어나면 선 종료 및 판단
            }
        }
        else
        {
            EndDrawing(); // 보드에서 펜을 떼면 선 종료 및 판단
        }
    }

    private void StartDrawing(Vector3 startPos)
    {
        GameObject lineObj = Instantiate(linePrefab);
        if (linesParent != null) lineObj.transform.SetParent(linesParent);

        currentLine = lineObj.GetComponent<LineRenderer>();
        currentLine.positionCount = 1;
        currentLine.SetPosition(0, startPos);

        previousPosition = startPos;

        // 새로운 선이 시작될 때 리스트 초기화 후 첫 점 추가
        currentLinePositions.Clear();
        currentLinePositions.Add(startPos);
    }

    private void UpdateDrawing(Vector3 currentPos)
    {
        if (Vector3.Distance(previousPosition, currentPos) > 0.005f)
        {
            currentLine.positionCount++;
            currentLine.SetPosition(currentLine.positionCount - 1, currentPos);
            previousPosition = currentPos;

            // 선이 이어질 때마다 3D 좌표 저장
            currentLinePositions.Add(currentPos);
        }
    }

    // [핵심 추가] 선 그리기가 끝났을 때 그림을 판독하는 로직
    private void EndDrawing()
    {
        if (currentLine == null) return;

        // 점의 개수가 최소한 어느 정도 쌓였을 때만 판독 (점 10개 미만은 노이즈로 취급)
        if (currentLinePositions.Count > 10)
        {
            AnalyzeGesture();
        }

        currentLine = null;
    }

    private void AnalyzeGesture()
    {
        List<DollarRecognizer.Point> dollarPoints = new List<DollarRecognizer.Point>();

        // 3D 공간의 월드 좌표를 마커펜(또는 화이트보드)의 로컬 좌표계 2D(X, Y)로 변환합니다.
        foreach (Vector3 worldPos in currentLinePositions)
        {
            // 화이트보드 평면 기준 로컬 좌표로 변환하여 Z축(깊이)을 무시합니다.
            Vector3 localPos = transform.InverseTransformPoint(worldPos);
            dollarPoints.Add(new DollarRecognizer.Point(localPos.x, localPos.y));
        }

        // 알고리즘 작동!
        DollarRecognizer.Result result = recognizer.Recognize(dollarPoints);

        // 내가 정한 합격 점수(Threshold)를 넘겼을 때만 인정
        if (result.Score >= recognitionThreshold)
        {
            drawTrigangle.RaiseEvent();
            DrawSuccess.achived = true;
            Debug.Log($"[그림 인식 성공] 형태: {result.Name} (일치율: {result.Score * 100:F1}%)");

            // 실전 활용 예시 코드:
            // if(result.Name == "Triangle") TriggerTriangleMagic();
        }
        else
        {
            Debug.Log($"❓ [인식 실패] 가장 비슷한 형태: {result.Name} (하지만 점수가 낮음: {result.Score * 100:F1}%)");
        }
    }

    // 3. 템플릿 패턴 등록 (여기서 기준이 될 세모, 네모를 코드로 입력해 줍니다.)
    private void RegisterTemplates()
    {
        // 예시: 삼각형 (세 개의 꼭짓점을 정방향 한 붓 그리기로 좌표 표현)
        List<DollarRecognizer.Point> triangle = new List<DollarRecognizer.Point> {
            // 1. 밑변 왼쪽에서 시작해서 위로 올라가는 선 (왼쪽 변)
            new DollarRecognizer.Point(0, 0),
            new DollarRecognizer.Point(10, 20),
            new DollarRecognizer.Point(20, 40),
            new DollarRecognizer.Point(30, 60),
            new DollarRecognizer.Point(40, 80),
            new DollarRecognizer.Point(50, 100), // 꼭짓점

            // 2. 꼭짓점에서 오른쪽 아래로 내려가는 선 (오른쪽 변)
            new DollarRecognizer.Point(60, 80),
            new DollarRecognizer.Point(70, 60),
            new DollarRecognizer.Point(80, 40),
            new DollarRecognizer.Point(90, 20),
            new DollarRecognizer.Point(100, 0), // 우측 아래 꼭짓점

            // 3. 오른쪽 아래에서 다시 시작점(0,0)으로 돌아가는 선 (밑변)
            new DollarRecognizer.Point(80, 0),
            new DollarRecognizer.Point(60, 0),
            new DollarRecognizer.Point(40, 0),
            new DollarRecognizer.Point(20, 0),
            new DollarRecognizer.Point(0, 0) // 완전히 닫힘
        };
        recognizer.AddTemplate("Triangle", triangle);

        //// 예시: 사각형 (네 개의 꼭짓점 한 붓 그리기)
        //List<DollarRecognizer.Point> rectangle = new List<DollarRecognizer.Point> {
        //    new DollarRecognizer.Point(0, 0),
        //    new DollarRecognizer.Point(0, 100),
        //    new DollarRecognizer.Point(100, 100),
        //    new DollarRecognizer.Point(100, 0),
        //    new DollarRecognizer.Point(0, 0)
        //};
        //recognizer.AddTemplate("Rectangle", rectangle);

        //// 조언: 하트나 별처럼 곡선이 많은 것은 점을 더 촘촘하게 배열하여 등록하면 됩니다.
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