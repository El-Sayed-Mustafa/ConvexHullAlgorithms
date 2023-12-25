using System;
using System.Collections.Generic;
using System.Linq;
using CGUtilities;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    public class SweepLine : Algorithm
    {
        public override string ToString()
        {
            return "Sweep Line";
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Preprocess lines to ensure Start.X is always less than End.X
            lines = LineHandler.HandleLines(lines);
            // Generate event points from the lines
            List<(Point Point, string EventType, int Index)> eventPoints = EventPointGenerator.GenerateEventPoints(lines);

            // Sort event points based on X-axis in descending order
            eventPoints = EventPointSorter.SortEventPoints(eventPoints);
            eventPoints.Reverse();

            // Initialize data structures for the sweep line algorithm
            List<(Point Point, string EventType, int Index)> activitySet = new List<(Point, string, int)>();
            List<(Point Point, string EventType, int Index)> allBelow = new List<(Point, string, int)>();
            List<(Point Point, string EventType, int Index)> allAbove = new List<(Point, string, int)>();

            foreach (var eventPoint in eventPoints)
            {
                allBelow.Clear();
                allAbove.Clear();

                if (eventPoint.EventType == "StartPoint")
                {
                    activitySet.Add(eventPoint);

                    foreach (var point in activitySet.FindAll(p => p != eventPoint))
                    {
                        if (point.Point.Y > eventPoint.Point.Y)
                            allAbove.Add(point);
                        else
                            allBelow.Add(point);
                    }

                    // Check for intersections above and below the current point
                    CheckAndAddIntersection(lines, eventPoint, allAbove, outPoints);
                    CheckAndAddIntersection(lines, eventPoint, allBelow, outPoints);
                }
                else // EndPoint
                {
                    // Remove the corresponding StartPoint from the activity set
                    int startIndex = activitySet.FindIndex(p => p.Index == eventPoint.Index && p.EventType != eventPoint.EventType);
                    activitySet.RemoveAt(startIndex);

                    foreach (var point in activitySet)
                    {
                        if (point.Point.Y > eventPoint.Point.Y)
                            allAbove.Add(point);
                        else
                            allBelow.Add(point);
                    }

                    // Check for intersections above and below the current point
                    CheckAndAddIntersection(lines, eventPoint, allAbove, outPoints);
                    CheckAndAddIntersection(lines, eventPoint, allBelow, outPoints);
                }
            }
        }

        // Check for intersections and add intersection points to the result list
        private void CheckAndAddIntersection(List<Line> lines, (Point Point, string EventType, int Index) eventPoint, List<(Point Point, string EventType, int Index)> otherPoints, List<Point> outPoints)
        {
            foreach (var otherPoint in otherPoints)
            {
                if (DoIntersect(lines[eventPoint.Index], lines[otherPoint.Index]))
                {
                    Point intersectionPoint = ComputeIntersectionPoint(lines[eventPoint.Index], lines[otherPoint.Index]);
                    if (!outPoints.Contains(intersectionPoint))
                        outPoints.Add(intersectionPoint);
                }
            }
        }
        private bool DoIntersect(Line line1, Line line2)
        {
            // Extract points from lines
            Point p1 = line1.Start;
            Point q1 = line1.End;
            Point p2 = line2.Start;
            Point q2 = line2.End;

            // Find orientations
            int orientation1 = Orientation(p1, q1, p2);
            int orientation2 = Orientation(p1, q1, q2);
            int orientation3 = Orientation(p2, q2, p1);
            int orientation4 = Orientation(p2, q2, q1);

            // General case
            if (orientation1 != orientation2 && orientation3 != orientation4)
                return true;

            // Special Cases

            // p1, q1, and p2 are collinear and p2 lies on segment p1q1
            if (orientation1 == 0 && OnSegment(p1, p2, q1))
                return true;

            // p1, q1, and q2 are collinear and q2 lies on segment p1q1
            if (orientation2 == 0 && OnSegment(p1, q2, q1))
                return true;

            // p2, q2, and p1 are collinear and p1 lies on segment p2q2
            if (orientation3 == 0 && OnSegment(p2, p1, q2))
                return true;

            // p2, q2, and q1 are collinear and q1 lies on segment p2q2
            if (orientation4 == 0 && OnSegment(p2, q1, q2))
                return true;

            return false; // Doesn't fall in any of the above cases
        }

        private int Orientation(Point p, Point q, Point r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0; // collinear
            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        private bool OnSegment(Point p, Point q, Point r)
        {
            return q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                   q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
        }

        private Point ComputeIntersectionPoint(Line line1, Line line2)
        {
            double x1 = line1.Start.X;
            double y1 = line1.Start.Y;
            double x2 = line1.End.X;
            double y2 = line1.End.Y;

            double x3 = line2.Start.X;
            double y3 = line2.Start.Y;
            double x4 = line2.End.X;
            double y4 = line2.End.Y;

            // Use the line intersection formula
            double x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) /
                       ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

            double y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) /
                       ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));

            return new Point(x, y);
        }

    }


    // Helper class to handle lines preprocessing
    public static class LineHandler
    {
        public static List<Line> HandleLines(List<Line> unhandled)
        {
            // Ensure Start.X is always less than End.X
            for (int i = 0; i < unhandled.Count; i++)
            {
                if (unhandled[i].Start.X > unhandled[i].End.X)
                {
                    Point temp = unhandled[i].Start;
                    unhandled[i].Start = unhandled[i].End;
                    unhandled[i].End = temp;
                }
            }
            return unhandled;
        }
    }

    // Helper class to sort event points based on X-axis
    public static class EventPointSorter
    {
        public static List<(Point Point, string EventType, int Index)> SortEventPoints(List<(Point Point, string EventType, int Index)> unsortedList)
        {
            // Sort in descending order based on X-axis
            unsortedList.Sort((p1, p2) => p2.Point.X.CompareTo(p1.Point.X));
            return unsortedList;
        }
    }

    // Helper class to find extreme points in Y-axis
    public static class EventPointFinder
    {
        public static (Point Point, string EventType, int Index) FindMinY(List<(Point Point, string EventType, int Index)> above)
        {
            var minPoint = above[0];
            for (int i = 1; i < above.Count; i++)
            {
                if (minPoint.Point.Y > above[i].Point.Y)
                    minPoint = above[i];
            }
            return minPoint;
        }

        public static (Point Point, string EventType, int Index) FindMaxY(List<(Point Point, string EventType, int Index)> above)
        {
            var maxPoint = above[0];
            for (int i = 1; i < above.Count; i++)
            {
                if (maxPoint.Point.Y < above[i].Point.Y)
                    maxPoint = above[i];
            }
            return maxPoint;
        }
    }

    // Helper class to generate event points from lines
    public static class EventPointGenerator
    {
        public static List<(Point Point, string EventType, int Index)> GenerateEventPoints(List<Line> lines)
        {
            List<(Point Point, string EventType, int Index)> eventPoints = new List<(Point, string, int)>();

            for (int i = 0; i < lines.Count; i++)
            {
                // Generate StartPoint and EndPoint for each line
                eventPoints.Add((lines[i].Start, "StartPoint", i));
                eventPoints.Add((lines[i].End, "EndPoint", i));
            }

            return eventPoints;
        }
    }

}
