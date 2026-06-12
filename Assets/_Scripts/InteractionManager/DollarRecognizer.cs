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
            Points = DollarRecognizer.ScaleToSquare(Points, 250);
            Points = DollarRecognizer.TranslateToOrigin(Points);
        }
    }

    private List<Template> templates = new List<Template>();

    // 템플릿 등록 함수
    public void AddTemplate(string name, List<Point> points)
    {
        templates.Add(new Template(name, points));
    }

    // 인식 실행 함수
    public Result Recognize(List<Point> points)
    {
        if (points.Count < 2) return new Result("Unknown", 0f);

        points = Resample(points, 64);
        points = RotateToZero(points);
        points = ScaleToSquare(points, 250);
        points = TranslateToOrigin(points);

        float b = float.MaxValue;
        int t = -1;

        for (int i = 0; i < templates.Count; i++)
        {
            float d = DistanceAtBestAngle(points, templates[i], -45f, 45f, 2f);
            if (d < b)
            {
                b = d;
                t = i;
            }
        }

        if (t == -1) return new Result("Unknown", 0f);

        // 0~1 사이의 점수 계산 (1에 가까울수록 완벽히 일치)
        float score = 1f - (b / (0.5f * Mathf.Sqrt(250 * 250 + 250 * 250)));
        return new Result(templates[t].Name, score);
    }

    #region 알고리즘 내부 연산 (수학적 전처리)
    private static List<Point> Resample(List<Point> points, int n)
    {
        float I = PathLength(points) / (n - 1);
        float D = 0f;
        List<Point> newPoints = new List<Point> { points[0] };
        for (int i = 1; i < points.Count; i++)
        {
            float d = Distance(points[i - 1], points[i]);
            if ((D + d) >= I)
            {
                float qx = points[i - 1].X + ((I - D) / d) * (points[i].X - points[i - 1].X);
                float qy = points[i - 1].Y + ((I - D) / d) * (points[i].Y - points[i - 1].Y);
                Point q = new Point(qx, qy);
                newPoints.Add(q);
                points.Insert(i, q);
                D = 0f;
            }
            else D += d;
        }
        if (newPoints.Count == n - 1) newPoints.Add(points[points.Count - 1]);
        return newPoints;
    }

    private static List<Point> RotateToZero(List<Point> points)
    {
        Point c = Centroid(points);
        float theta = Mathf.Atan2(c.Y - points[0].Y, c.X - points[0].X);
        return RotateBy(points, -theta);
    }

    private static List<Point> RotateBy(List<Point> points, float theta)
    {
        Point c = Centroid(points);
        float cos = Mathf.Cos(theta);
        float sin = Mathf.Sin(theta);
        List<Point> newPoints = new List<Point>();
        foreach (Point p in points)
        {
            float qx = (p.X - c.X) * cos - (p.Y - c.Y) * sin + c.X;
            float qy = (p.X - c.X) * sin + (p.Y - c.Y) * cos + c.Y;
            newPoints.Add(new Point(qx, qy));
        }
        return newPoints;
    }

    private static List<Point> ScaleToSquare(List<Point> points, float size)
    {
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        foreach (Point p in points)
        {
            if (p.X < minX) minX = p.X; if (p.X > maxX) maxX = p.X;
            if (p.Y < minY) minY = p.Y; if (p.Y > maxY) maxY = p.Y;
        }
        float bWidth = maxX - minX;
        float bHeight = maxY - minY;
        List<Point> newPoints = new List<Point>();
        foreach (Point p in points)
        {
            float qx = p.X * (size / bWidth);
            float qy = p.Y * (size / bHeight);
            newPoints.Add(new Point(qx, qy));
        }
        return newPoints;
    }

    private static List<Point> TranslateToOrigin(List<Point> points)
    {
        Point c = Centroid(points);
        List<Point> newPoints = new List<Point>();
        foreach (Point p in points)
        {
            newPoints.Add(new Point(p.X - c.X, p.Y - c.Y));
        }
        return newPoints;
    }

    private static float DistanceAtBestAngle(List<Point> points, Template T, float a, float b, float threshold)
    {
        float phi = 0.5f * (Mathf.Sqrt(5f) - 1f);
        float x1 = phi * a + (1f - phi) * b;
        float f1 = DistanceAtAngle(points, T, x1);
        float x2 = (1f - phi) * a + phi * b;
        float f2 = DistanceAtAngle(points, T, x2);

        while (Mathf.Abs(b - a) > threshold)
        {
            if (f1 < f2)
            {
                b = x2; x2 = x1; f2 = f1;
                x1 = phi * a + (1f - phi) * b;
                f1 = DistanceAtAngle(points, T, x1);
            }
            else
            {
                a = x1; x1 = x2; f1 = f2;
                x2 = (1f - phi) * a + phi * b;
                f2 = DistanceAtAngle(points, T, x2);
            }
        }
        return Mathf.Min(f1, f2);
    }

    private static float DistanceAtAngle(List<Point> points, Template T, float r)
    {
        List<Point> newPoints = RotateBy(points, r);
        return PathDistance(newPoints, T.Points);
    }

    private static float PathLength(List<Point> points)
    {
        float d = 0f;
        for (int i = 1; i < points.Count; i++) d += Distance(points[i - 1], points[i]);
        return d;
    }

    private static float PathDistance(List<Point> pts1, List<Point> pts2)
    {
        float d = 0f;
        for (int i = 0; i < pts1.Count; i++) d += Distance(pts1[i], pts2[i]);
        return d / pts1.Count;
    }

    private static float Distance(Point p1, Point p2)
    {
        float dx = p2.X - p1.X;
        float dy = p2.Y - p1.Y;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    private static Point Centroid(List<Point> points)
    {
        float x = 0f, y = 0f;
        foreach (Point p in points) { x += p.X; y += p.Y; }
        return new Point(x / points.Count, y / points.Count);
    }
    #endregion
}