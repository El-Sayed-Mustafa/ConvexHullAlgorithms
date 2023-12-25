using CGUtilities;
using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    // Main algorithm class
    class SubtractingEars : Algorithm
    {
        private PolygonProcessor polygonProcessor;

        // Main entry point for the algorithm
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Check if there are polygons to process
            if (polygons.Count == 0)
                return;

            // Create a PolygonProcessor for the input polygon
            polygonProcessor = new PolygonProcessor(polygons[0]);

            // Process the polygon and update the output lines
            polygonProcessor.ProcessPolygon(ref outLines);
        }

        // Override ToString to provide a descriptive name for the algorithm
        public override string ToString()
        {
            return "Subtracting Ears";
        }
    }

    // Class to process the polygon
    class PolygonProcessor
    {
        private LinkedList<PointWithStatus> polygonVertices;

        // Constructor to initialize the PolygonProcessor with a polygon
        public PolygonProcessor(Polygon polygon)
        {
            // Initialize the polygon vertices with ear status
            InitializePolygon(polygon);

            // Check and reverse the polygon if needed
            ReversePolygon();
        }

        // Function to process the polygon and update output lines
        public void ProcessPolygon(ref List<Line> outLines)
        {
            // Get a queue of all ears in the polygon
            Queue<LinkedListNode<PointWithStatus>> earQueue = EarProcessor.GetAllEars(polygonVertices);

            // Process each ear in the queue
            while (earQueue.Count > 0)
            {
                EarProcessor.SubtractEar(earQueue, polygonVertices, ref outLines);
            }
        }

        // Function to initialize the polygon vertices with ear status
        private void InitializePolygon(Polygon polygon)
        {
            polygonVertices = new LinkedList<PointWithStatus>();

            // Iterate through polygon lines and add vertices with unknown ear status
            for (int i = 0; i < polygon.lines.Count; ++i)
            {
                polygonVertices.AddLast(new PointWithStatus(polygon.lines[i].Start));
            }
        }


        // Function to reverse the order of vertices in the polygon
        private void ReversePolygon()
        {
            LinkedList<PointWithStatus> reversedPolygon = new LinkedList<PointWithStatus>();
            LinkedListNode<PointWithStatus> vertex = polygonVertices.Last;

            // Iterate through vertices and add them in reverse order
            while (vertex != polygonVertices.First.Previous)
            {
                reversedPolygon.AddLast(vertex.Value);
                vertex = vertex.Previous;
            }
            polygonVertices = new LinkedList<PointWithStatus>(reversedPolygon);
        }
    }

    // Class to process ears in the polygon
    class EarProcessor
    {
        // Function to get a queue of all ears in the polygon
        public static Queue<LinkedListNode<PointWithStatus>> GetAllEars(LinkedList<PointWithStatus> polygonVertices)
        {
            Queue<LinkedListNode<PointWithStatus>> ears = new Queue<LinkedListNode<PointWithStatus>>();
            LinkedListNode<PointWithStatus> vertex = polygonVertices.First;

            // Iterate through all vertices and enqueue ears
            while (vertex != polygonVertices.Last.Next)
            {
                if (IsEar(vertex, polygonVertices))
                    ears.Enqueue(vertex);

                vertex = vertex.Next;
            }
            return ears;
        }

        // Function to process a single ear
        public static void SubtractEar(Queue<LinkedListNode<PointWithStatus>> earQueue, LinkedList<PointWithStatus> polygonVertices, ref List<Line> outLines)
        {
            LinkedListNode<PointWithStatus> currentEar = earQueue.Dequeue();

            // Check if the ear is valid and has neighbors
            if (currentEar == null || !IsEar(currentEar, polygonVertices) || (currentEar.Next == null && currentEar.Previous == null))
                return;

            // Check if there are only three vertices remaining, clear the queue and return
            if (polygonVertices.Count == 3)
            {
                earQueue.Clear();
                return;
            }

            // Get the neighbors of the current ear
            LinkedListNode<PointWithStatus> next = currentEar.Next == null ? polygonVertices.First : currentEar.Next;
            LinkedListNode<PointWithStatus> prev = currentEar.Previous == null ? polygonVertices.Last : currentEar.Previous;

            // Remove the current ear and add the diagonal to the output
            polygonVertices.Remove(currentEar);
            outLines.Add(new Line(prev.Value, next.Value));

            // Enqueue the neighbors for further processing
            earQueue.Enqueue(prev);
            earQueue.Enqueue(next);
        }

        // Function to check if a vertex is an ear
        public static bool IsEar(LinkedListNode<PointWithStatus> vertex, LinkedList<PointWithStatus> polygonVertices)
        {
            if (!IsConvex(vertex, polygonVertices))
                return false;

            LinkedListNode<PointWithStatus> next = vertex.Next == null ? polygonVertices.First : vertex.Next;
            LinkedListNode<PointWithStatus> prev = vertex.Previous == null ? polygonVertices.Last : vertex.Previous;

            LinkedListNode<PointWithStatus> current = polygonVertices.First;

            // Check if any other vertex is inside the triangle formed by the current vertex and its neighbors
            while (current != polygonVertices.Last.Next)
            {
                if (HelperMethods.PointInTriangle(current.Value, prev.Value, next.Value, vertex.Value) == Enums.PointInPolygon.Inside)
                    return false;

                current = current.Next;
            }
            return true;
        }

        // Function to check if a vertex is convex
        private static bool IsConvex(LinkedListNode<PointWithStatus> vertex, LinkedList<PointWithStatus> polygonVertices)
        {
            LinkedListNode<PointWithStatus> next = vertex.Next == null ? polygonVertices.First : vertex.Next;
            LinkedListNode<PointWithStatus> prev = vertex.Previous == null ? polygonVertices.Last : vertex.Previous;

            // Check the turn type to determine convexity
            return HelperMethods.CheckTurn(new Line(prev.Value, next.Value), vertex.Value) == Enums.TurnType.Right;
        }

        // Function to find the minimum X point in the polygon
        public static LinkedListNode<PointWithStatus> FindMinimumXPoint(LinkedList<PointWithStatus> polygonVertices)
        {
            LinkedListNode<PointWithStatus> minPoint = new LinkedListNode<PointWithStatus>(new PointWithStatus(double.MaxValue, 0));
            LinkedListNode<PointWithStatus> vertex = polygonVertices.First;

            // Iterate through all vertices and find the minimum X point
            while (vertex != polygonVertices.Last.Next)
            {
                if (vertex.Value.X < minPoint.Value.X)
                    minPoint = vertex;

                vertex = vertex.Next;
            }
            return minPoint;
        }
    }

    // Class representing a point with an associated ear status
    public class PointWithStatus : Point
    {
        // Ear status property
        public EarStatus EarStatus { get; set; }

        // Constructor with X, Y coordinates
        public PointWithStatus(double x, double y) : base(x, y)
        {
            EarStatus = EarStatus.Unknown;
        }

        // Constructor with a Point object
        public PointWithStatus(Point point) : base(point.X, point.Y)
        {
            EarStatus = EarStatus.Unknown;
        }
    }

    // Enum representing ear status
    public enum EarStatus
    {
        Unknown,
        Ear,
        NotEar
    }
}
