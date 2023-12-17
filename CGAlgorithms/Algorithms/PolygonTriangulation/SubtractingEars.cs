using CGUtilities;
using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        private PolygonProcessor polygonProcessor;

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (polygons.Count == 0)
                return;

            polygonProcessor = new PolygonProcessor(polygons[0]);
            polygonProcessor.ProcessPolygon(ref outLines);
        }

        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }

    class PolygonProcessor
    {
        private LinkedList<PointWithStatus> polygonVertices;

        public PolygonProcessor(Polygon polygon)
        {
            InitializePolygon(polygon);
            CheckAndReversePolygonIfNeeded();
        }

        public void ProcessPolygon(ref List<Line> outLines)
        {
            Queue<LinkedListNode<PointWithStatus>> earQueue = EarProcessor.GetAllEars(polygonVertices);

            while (earQueue.Count > 0)
            {
                EarProcessor.SubtractEar(earQueue, polygonVertices, ref outLines);
            }
        }

        private void InitializePolygon(Polygon polygon)
        {
            polygonVertices = new LinkedList<PointWithStatus>();

            for (int i = 0; i < polygon.lines.Count; ++i)
            {
                polygonVertices.AddLast(new PointWithStatus(polygon.lines[i].Start));
            }
        }

        private void CheckAndReversePolygonIfNeeded()
        {
            LinkedListNode<PointWithStatus> minPoint = EarProcessor.FindMinimumXPoint(polygonVertices);
            LinkedListNode<PointWithStatus> next = minPoint.Next == null ? polygonVertices.First : minPoint.Next;
            LinkedListNode<PointWithStatus> prev = minPoint.Previous == null ? polygonVertices.Last : minPoint.Previous;

            if (HelperMethods.CheckTurn(new Line(prev.Value, next.Value), minPoint.Value) == Enums.TurnType.Left)
                ReversePolygon();
        }

        private void ReversePolygon()
        {
            LinkedList<PointWithStatus> reversedPolygon = new LinkedList<PointWithStatus>();
            LinkedListNode<PointWithStatus> vertex = polygonVertices.Last;

            while (vertex != polygonVertices.First.Previous)
            {
                reversedPolygon.AddLast(vertex.Value);
                vertex = vertex.Previous;
            }
            polygonVertices = new LinkedList<PointWithStatus>(reversedPolygon);
        }
    }

    class EarProcessor
    {
        public static Queue<LinkedListNode<PointWithStatus>> GetAllEars(LinkedList<PointWithStatus> polygonVertices)
        {
            Queue<LinkedListNode<PointWithStatus>> ears = new Queue<LinkedListNode<PointWithStatus>>();
            LinkedListNode<PointWithStatus> vertex = polygonVertices.First;

            while (vertex != polygonVertices.Last.Next)
            {
                if (IsEar(vertex, polygonVertices))
                    ears.Enqueue(vertex);

                vertex = vertex.Next;
            }
            return ears;
        }

        public static void SubtractEar(Queue<LinkedListNode<PointWithStatus>> earQueue, LinkedList<PointWithStatus> polygonVertices, ref List<Line> outLines)
        {
            LinkedListNode<PointWithStatus> currentEar = earQueue.Dequeue();

            if (currentEar == null || !IsEar(currentEar, polygonVertices) || (currentEar.Next == null && currentEar.Previous == null))
                return;

            if (polygonVertices.Count == 3)
            {
                earQueue.Clear();
                return;
            }

            LinkedListNode<PointWithStatus> next = currentEar.Next == null ? polygonVertices.First : currentEar.Next;
            LinkedListNode<PointWithStatus> prev = currentEar.Previous == null ? polygonVertices.Last : currentEar.Previous;

            polygonVertices.Remove(currentEar);
            outLines.Add(new Line(prev.Value, next.Value));

            earQueue.Enqueue(prev);
            earQueue.Enqueue(next);
        }

        public static bool IsEar(LinkedListNode<PointWithStatus> vertex, LinkedList<PointWithStatus> polygonVertices)
        {
            if (!IsConvex(vertex, polygonVertices))
                return false;

            LinkedListNode<PointWithStatus> next = vertex.Next == null ? polygonVertices.First : vertex.Next;
            LinkedListNode<PointWithStatus> prev = vertex.Previous == null ? polygonVertices.Last : vertex.Previous;

            LinkedListNode<PointWithStatus> cur = polygonVertices.First;

            while (cur != polygonVertices.Last.Next)
            {
                if (HelperMethods.PointInTriangle(cur.Value, prev.Value, next.Value, vertex.Value) == Enums.PointInPolygon.Inside)
                    return false;

                cur = cur.Next;
            }
            return true;
        }

        private static bool IsConvex(LinkedListNode<PointWithStatus> vertex, LinkedList<PointWithStatus> polygonVertices)
        {
            LinkedListNode<PointWithStatus> next = vertex.Next == null ? polygonVertices.First : vertex.Next;
            LinkedListNode<PointWithStatus> prev = vertex.Previous == null ? polygonVertices.Last : vertex.Previous;

            return HelperMethods.CheckTurn(new Line(prev.Value, next.Value), vertex.Value) == Enums.TurnType.Right;
        }

        public static LinkedListNode<PointWithStatus> FindMinimumXPoint(LinkedList<PointWithStatus> polygonVertices)
        {
            LinkedListNode<PointWithStatus> minPoint = new LinkedListNode<PointWithStatus>(new PointWithStatus(double.MaxValue, 0));
            LinkedListNode<PointWithStatus> vertex = polygonVertices.First;

            while (vertex != polygonVertices.Last.Next)
            {
                if (vertex.Value.X < minPoint.Value.X)
                    minPoint = vertex;

                vertex = vertex.Next;
            }
            return minPoint;
        }
    }

    public class PointWithStatus : Point
    {
        public EarStatus EarStatus { get; set; }

        public PointWithStatus(double x, double y) : base(x, y)
        {
            EarStatus = EarStatus.Unknown;
        }

        public PointWithStatus(Point point) : base(point.X, point.Y)
        {
            EarStatus = EarStatus.Unknown;
        }
    }

    public enum EarStatus
    {
        Unknown,
        Ear,
        NotEar
    }
}
