using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> inputPoints, List<Line> lines, List<Polygon> polygons, ref List<Point> convexHullPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int pointCount = inputPoints.Count;

            // If there are fewer than 3 points, the convex hull is not defined
            if (pointCount < 3)
            {
                convexHullPoints = inputPoints;
                return;
            }

            HashSet<Point> hull = new HashSet<Point>();

            // Find the points with minimum and maximum x-coordinates
            getBottomMostPoint(inputPoints, pointCount, out int minXIndex, out int maxXIndex);

            // Recursively find convex hull points on the other side of the line
            Recurion(inputPoints, pointCount, inputPoints[minXIndex], inputPoints[maxXIndex], -1, ref hull);


            // Recursively find convex hull points on one side of the line joining inputPoints[minXIndex] and inputPoints[maxXIndex]
            Recurion(inputPoints, pointCount, inputPoints[minXIndex], inputPoints[maxXIndex], 1, ref hull);


            // Set the output convex hull points
            convexHullPoints = hull.ToList();
        }

        private static void getBottomMostPoint(List<Point> inputPoints, int pointCount, out int minXIndex, out int maxXIndex)
        {
            minXIndex = 0;
            maxXIndex = 0;
            for (int i = 1; i < pointCount; i++)
            {
                if (inputPoints[i].X < inputPoints[minXIndex].X)
                    minXIndex = i;
                if (inputPoints[i].X > inputPoints[maxXIndex].X)
                    maxXIndex = i;
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }

        // Helper method for the QuickHull algorithm
        private void Recurion(List<Point> points, int pointCount, Point startPoint, Point endPoint, int side, ref HashSet<Point> hull)
        {
            int farthestPointIndex = -1;
            int maxDistance = 0;

            // Finding the point with the maximum distance from the line
            GetMaxPoint(points, pointCount, startPoint, endPoint, side, ref farthestPointIndex, ref maxDistance);

            // If no point is found, add the end points of the line to the convex hull
            if (farthestPointIndex == -1)
            {
                hull.Add(startPoint);
                hull.Add(endPoint);
                return;
            }

            // Recur for the two parts divided by points[farthestPointIndex]
            Recurion(points, pointCount, points[farthestPointIndex], startPoint, -GetSide(points[farthestPointIndex], startPoint, endPoint), ref hull);
            Recurion(points, pointCount, points[farthestPointIndex], endPoint, -GetSide(points[farthestPointIndex], endPoint, startPoint), ref hull);
        }

        private void GetMaxPoint(List<Point> points, int pointCount, Point startPoint, Point endPoint, int side, ref int farthestPointIndex, ref int maxDistance)
        {
            for (int i = pointCount - 1; i >= 0; i--)
            {
                int distance = DisBetweenPointAndLine(startPoint, endPoint, points[i]);
                if (GetSide(startPoint, endPoint, points[i]) == side && distance > maxDistance)
                {
                    farthestPointIndex = i;
                    maxDistance = distance;
                }
            }
        }

        // Helper method to find the side of a point with respect to a line
        private int GetSide(Point startPoint, Point endPoint, Point point)
        {
            int value = (int)((point.Y - startPoint.Y) * (endPoint.X - startPoint.X) - (endPoint.Y - startPoint.Y) * (point.X - startPoint.X));
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
            return 0;
        }

        // Helper method to find a value proportional to the distance between a point and a line
        private int DisBetweenPointAndLine(Point startPoint, Point endPoint, Point point)
        {
            return (int)Math.Abs((point.Y - startPoint.Y) * (endPoint.X - startPoint.X) - (endPoint.Y - startPoint.Y) * (point.X - startPoint.X));
        }
    }
}
