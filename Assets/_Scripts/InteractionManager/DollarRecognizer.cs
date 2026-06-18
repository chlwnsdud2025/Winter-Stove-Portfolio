using System;
using System.Collections.Generic;
using UnityEngine;

public class DollarRecognizer
{
    public class Point
    {
        public float X, Y;
        public Point(float x, float y) { X = x; Y = y; }
    }

    public class Result
    {
        public string Name;
        public float Score;
        public Result(string name, float score) { Name = name; Score = score; }
    }

    public class Template
    {
        public string Name;
        public List<Point> Points;
        public Template(string name, List<Point> points)
        {
            Name = name;
            Points = DollarRecognizer.Resample(points, 64);
            Points = DollarRecognizer.RotateToZero(Points);
            Points = DollarRecognizer.ScaleToSquare(Points, 250f);
            Points = DollarRecognizer.TranslateToOrigin(Points);
        }
    }

    private List<Template> templates = new List<Template>();

    // 템플릿 등록 함수
    public void AddTemplate(string name, List<Point> points)
    {
        templates.Add(new Template(name, points));
    }

    // 인식 실행 함수 (시작점 순환 및 역방향 검사 적용 완료)
    public Result Recognize(List<Point> rawPoints)
    {
        if (rawPoints.Count < 2) return new Result("Unknown", 0f);

        // 1. 원본 점들을 정방향과 역방향으로 각각 준비
        List<Point> forwardPoints = new List<Point>(rawPoints);
        List<Point> backwardPoints = new List<Point>(rawPoints);
        backwardPoints.Reverse();

        // 2. 64개의 점으로 리샘플링 (공통 수행)
        int targetPointCount = 64;
        forwardPoints = Resample(forwardPoints, targetPointCount);
        backwardPoints = Resample(backwardPoints, targetPointCount);

        float bestDistance = float.MaxValue;
        int bestTemplateIndex = -1;

        // 3. 8칸씩 건너뛰며(총 8개의 가상 시작점) 테스트
        int shiftStep = 8;
        float boxSize = 250f;

        for (int step = 0; step < targetPointCount; step += shiftStep)
        {
            // 정방향: 배열 순환 및 전처리
            List<Point> shiftedForward = ShiftArray(forwardPoints, step);
            shiftedForward = RotateToZero(shiftedForward);
            shiftedForward = ScaleToSquare(shiftedForward, boxSize);
            shiftedForward = TranslateToOrigin(shiftedForward);

            // 역방향: 배열 순환 및 전처리
            List<Point> shiftedBackward = ShiftArray(backwardPoints, step);
            shiftedBackward = RotateToZero(shiftedBackward);
            shiftedBackward = ScaleToSquare(shiftedBackward, boxSize);
            shiftedBackward = TranslateToOrigin(shiftedBackward);

            // 등록된 템플릿들과 비교하여 최단 거리 찾기
            for (int i = 0; i < templates.Count; i++)
            {
                float distanceForward = DistanceAtBestAngle(shiftedForward, templates[i], -45f, 45f, 2f);
                float distanceBackward = DistanceAtBestAngle(shiftedBackward, templates[i], -45f, 45f, 2f);

                float minDistance = Mathf.Min(distanceForward, distanceBackward);
                if (minDistance < bestDistance)
                {
                    bestDistance = minDistance;
                    bestTemplateIndex = i;
                }
            }
        }

        if (bestTemplateIndex == -1) return new Result("Unknown", 0f);

        // 0~1 사이의 점수 계산 (1에 가까울수록 완벽히 일치)
        float halfDiagonal = 0.5f * Mathf.Sqrt(boxSize * boxSize + boxSize * boxSize);
        float score = 1f - (bestDistance / halfDiagonal);
        return new Result(templates[bestTemplateIndex].Name, score);
    }

    #region 알고리즘 내부 연산 (수학적 전처리)

    // 배열을 shift 만큼 순환시켜 새로운 시작점을 만드는 함수
    private static List<Point> ShiftArray(List<Point> points, int shiftAmount)
    {
        List<Point> newPoints = new List<Point>(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            newPoints.Add(points[(i + shiftAmount) % points.Count]);
        }
        return newPoints;
    }

    // 선의 길이를 기반으로 점의 개수를 일정하게 맞추는 함수
    private static List<Point> Resample(List<Point> points, int targetPointCount)
    {
        float intervalLength = CalculatePathLength(points) / (targetPointCount - 1);
        float accumulatedDistance = 0f;

        List<Point> resampledPoints = new List<Point> { points[0] };

        for (int i = 1; i < points.Count; i++)
        {
            float segmentDistance = CalculateDistance(points[i - 1], points[i]);

            if ((accumulatedDistance + segmentDistance) >= intervalLength)
            {
                float newX = points[i - 1].X + ((intervalLength - accumulatedDistance) / segmentDistance) * (points[i].X - points[i - 1].X);
                float newY = points[i - 1].Y + ((intervalLength - accumulatedDistance) / segmentDistance) * (points[i].Y - points[i - 1].Y);

                Point newPoint = new Point(newX, newY);
                resampledPoints.Add(newPoint);
                points.Insert(i, newPoint);
                accumulatedDistance = 0f;
            }
            else
            {
                accumulatedDistance += segmentDistance;
            }
        }

        // 부동소수점 오차로 인해 마지막 점이 누락된 경우 강제 추가
        if (resampledPoints.Count == targetPointCount - 1)
        {
            resampledPoints.Add(points[points.Count - 1]);
        }

        return resampledPoints;
    }

    // 중심점과 첫 번째 점의 각도를 0도로 회전시키는 함수
    private static List<Point> RotateToZero(List<Point> points)
    {
        Point center = GetCentroid(points);
        float angleToFirstPoint = Mathf.Atan2(center.Y - points[0].Y, center.X - points[0].X);
        return RotateByAngle(points, -angleToFirstPoint);
    }

    // 주어진 각도만큼 점들을 회전시키는 함수
    private static List<Point> RotateByAngle(List<Point> points, float angle)
    {
        Point center = GetCentroid(points);
        float cosAngle = Mathf.Cos(angle);
        float sinAngle = Mathf.Sin(angle);

        List<Point> rotatedPoints = new List<Point>();
        foreach (Point p in points)
        {
            float newX = (p.X - center.X) * cosAngle - (p.Y - center.Y) * sinAngle + center.X;
            float newY = (p.X - center.X) * sinAngle + (p.Y - center.Y) * cosAngle + center.Y;
            rotatedPoints.Add(new Point(newX, newY));
        }
        return rotatedPoints;
    }

    // 도형을 지정된 크기의 정사각형 비율로 변환하는 함수
    private static List<Point> ScaleToSquare(List<Point> points, float targetSize)
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (Point p in points)
        {
            if (p.X < minX) minX = p.X;
            if (p.X > maxX) maxX = p.X;
            if (p.Y < minY) minY = p.Y;
            if (p.Y > maxY) maxY = p.Y;
        }

        float boundingBoxWidth = maxX - minX;
        float boundingBoxHeight = maxY - minY;

        List<Point> scaledPoints = new List<Point>();
        foreach (Point p in points)
        {
            float newX = p.X * (targetSize / boundingBoxWidth);
            float newY = p.Y * (targetSize / boundingBoxHeight);
            scaledPoints.Add(new Point(newX, newY));
        }
        return scaledPoints;
    }

    // 중심점을 원점(0,0)으로 이동시키는 함수
    private static List<Point> TranslateToOrigin(List<Point> points)
    {
        Point center = GetCentroid(points);
        List<Point> translatedPoints = new List<Point>();
        foreach (Point p in points)
        {
            translatedPoints.Add(new Point(p.X - center.X, p.Y - center.Y));
        }
        return translatedPoints;
    }

    // 황금분할 탐색을 사용하여 최적의 회전 각도와 최소 거리를 찾는 함수
    private static float DistanceAtBestAngle(List<Point> points, Template template, float startAngle, float endAngle, float angleThreshold)
    {
        float goldenRatio = 0.5f * (Mathf.Sqrt(5f) - 1f);

        float testAngle1 = goldenRatio * startAngle + (1f - goldenRatio) * endAngle;
        float distanceAtAngle1 = DistanceAtAngle(points, template, testAngle1);

        float testAngle2 = (1f - goldenRatio) * startAngle + goldenRatio * endAngle;
        float distanceAtAngle2 = DistanceAtAngle(points, template, testAngle2);

        while (Mathf.Abs(endAngle - startAngle) > angleThreshold)
        {
            if (distanceAtAngle1 < distanceAtAngle2)
            {
                endAngle = testAngle2;
                testAngle2 = testAngle1;
                distanceAtAngle2 = distanceAtAngle1;

                testAngle1 = goldenRatio * startAngle + (1f - goldenRatio) * endAngle;
                distanceAtAngle1 = DistanceAtAngle(points, template, testAngle1);
            }
            else
            {
                startAngle = testAngle1;
                testAngle1 = testAngle2;
                distanceAtAngle1 = distanceAtAngle2;

                testAngle2 = (1f - goldenRatio) * startAngle + goldenRatio * endAngle;
                distanceAtAngle2 = DistanceAtAngle(points, template, testAngle2);
            }
        }
        return Mathf.Min(distanceAtAngle1, distanceAtAngle2);
    }

    // 특정 각도로 회전했을 때 템플릿과의 거리를 계산하는 함수
    private static float DistanceAtAngle(List<Point> points, Template template, float angle)
    {
        List<Point> rotatedPoints = RotateByAngle(points, angle);
        return CalculatePathDistance(rotatedPoints, template.Points);
    }

    // 전체 선의 길이를 계산하는 함수
    private static float CalculatePathLength(List<Point> points)
    {
        float totalLength = 0f;
        for (int i = 1; i < points.Count; i++)
        {
            totalLength += CalculateDistance(points[i - 1], points[i]);
        }
        return totalLength;
    }

    // 두 점 리스트 사이의 평균 거리를 계산하는 함수
    private static float CalculatePathDistance(List<Point> points1, List<Point> points2)
    {
        float totalDistance = 0f;
        for (int i = 0; i < points1.Count; i++)
        {
            totalDistance += CalculateDistance(points1[i], points2[i]);
        }
        return totalDistance / points1.Count;
    }

    // 두 점 사이의 유클리드 거리를 계산하는 함수
    private static float CalculateDistance(Point p1, Point p2)
    {
        float deltaX = p2.X - p1.X;
        float deltaY = p2.Y - p1.Y;
        return Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    // 점들의 무게중심(Centroid)을 계산하는 함수
    private static Point GetCentroid(List<Point> points)
    {
        float sumX = 0f, sumY = 0f;
        foreach (Point p in points)
        {
            sumX += p.X;
            sumY += p.Y;
        }
        return new Point(sumX / points.Count, sumY / points.Count);
    }
    #endregion
}