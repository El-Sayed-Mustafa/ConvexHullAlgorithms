using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    {
        public override void Run(List<Point> inputPoints, List<Line> inputLines, List<Polygon> inputPolygons,
            ref List<Point> outputPoints, ref List<Line> outputLines, ref List<Polygon> outputPolygons)
        {
            Polygon polygon = new Polygon(inputLines);
            polygon = MakePolygonCounterClockwise(polygon);
            bool isMonotone = CheckMonotone(polygon);

            for (int i = 0; i < polygon.lines.Count; i++)
                outputPoints.Add(polygon.lines[i].Start);

            // Sort the points based on max Y and max X in case of a tie O(n)
            List<Point> sortedPoints = new List<Point>();
            for (int i = 0; i < polygon.lines.Count; i++)
                sortedPoints.Add(polygon.lines[i].Start);
            sortedPoints.Sort(PointSort);

            Stack<Point> stack = new Stack<Point>();
            stack.Push(sortedPoints[0]);
            stack.Push(sortedPoints[1]);
            TriangulateMonotonePolygon(outputPoints, outputLines, polygon, sortedPoints, stack);
        }

        private void TriangulateMonotonePolygon(List<Point> outputPoints, List<Line> outputLines, Polygon polygon, List<Point> sortedPoints, Stack<Point> stack)
        {
            int current = 2;

            while (current != polygon.lines.Count)
            {
                Point currentPoint = sortedPoints[current];
                Point top = stack.Peek();

                bool sameSide = CheckSameSide(sortedPoints, currentPoint, top);

                if (sameSide)
                {
                    current = HandleSameSide(outputPoints, outputLines, polygon, stack, current, currentPoint, top);
                }
                else
                {
                    HandleDifferentSide(outputLines, stack, currentPoint, top);
                }
            }
        }

        private static void HandleDifferentSide(List<Line> outputLines, Stack<Point> stack, Point currentPoint, Point top)
        {
            while (stack.Count != 1)
            {
                Point lastTop = stack.Pop();
                outputLines.Add(new Line(currentPoint, lastTop ));
            }
            stack.Pop();
            stack.Push(top);
            stack.Push(currentPoint);
        }

        private int HandleSameSide(List<Point> outputPoints, List<Line> outputLines, Polygon polygon, Stack<Point> stack, int current, Point currentPoint, Point top)
        {
            stack.Pop();
            Point lastTop = stack.Peek();

            int index = outputPoints.IndexOf(top);
            if (IsConvex(polygon, index))
            {
                outputLines.Add(new Line(currentPoint, lastTop));
                if (stack.Count == 1)
                {
                    stack.Push(currentPoint);
                    current++;
                }
            }
            else
            {
                stack.Push(top);
                stack.Push(currentPoint);
                current++;
            }

            return current;
        }

        private static bool CheckSameSide(List<Point> sortedPoints, Point currentPoint, Point top)
        {
            bool sameSide = false;
            if (currentPoint.X < sortedPoints[0].X && top.X < sortedPoints[0].X)
                sameSide = true;
            else if (currentPoint.X > sortedPoints[0].X && top.X > sortedPoints[0].X)
                sameSide = true;
            return sameSide;
        }

        // Check the orientation of the polygon
        public Polygon MakePolygonCounterClockwise(Polygon polygon)
        {
            double signedArea = 0;
            for (int i = 0; i < polygon.lines.Count; i++)
                signedArea += (polygon.lines[i].End.X - polygon.lines[i].Start.X) * (polygon.lines[i].End.Y + polygon.lines[i].Start.Y);
            signedArea /= 2;

            if (signedArea > 0)
            {
                polygon.lines.Reverse();
                for (int i = 0; i < polygon.lines.Count; i++)
                {
                    Point temp = polygon.lines[i].Start;
                    polygon.lines[i].Start = polygon.lines[i].End;
                    polygon.lines[i].End = temp;
                }
            }
            return polygon;
        }

        // Check Monotone: this function returns true if and only if no cusp points
        public bool CheckMonotone(Polygon polygon)
        {
            int count = 0;

            for (int i = 0; i < polygon.lines.Count; i++)
            {
                int previous = ((i - 1) + polygon.lines.Count) % polygon.lines.Count;
                int next = (i + 1) % polygon.lines.Count;

                Point currentPoint = polygon.lines[i].Start;
                Point prevPoint = polygon.lines[previous].Start;
                Point nextPoint = polygon.lines[next].Start;

                if (nextPoint.Y < currentPoint.Y && prevPoint.Y < currentPoint.Y && !IsConvex(polygon, i))
                    count++;
                else if (nextPoint.Y > currentPoint.Y && prevPoint.Y > currentPoint.Y && !IsConvex(polygon, i))
                    count++;
            }

            return count == 0;
        }

        // Check Convex point 
        public bool IsConvex(Polygon polygon, int currentIndex)
        {
            int previous = ((currentIndex - 1) + polygon.lines.Count) % polygon.lines.Count;
            int next = (currentIndex + 1) % polygon.lines.Count;

            Point p1 = polygon.lines[previous].Start;
            Point p2 = polygon.lines[currentIndex].Start;
            Point p3 = polygon.lines[next].Start;
            Line line = new Line(p1, p2);

            return HelperMethods.CheckTurn(line, p3) == Enums.TurnType.Left;
        }

        // Sort the points based on max Y and max X in case of a tie O(n)
        public static int PointSort(Point a, Point b)
        {
            if (a.Y == b.Y)
                return -a.X.CompareTo(b.X);
            else
                return -a.Y.CompareTo(b.Y);
        }

        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}
