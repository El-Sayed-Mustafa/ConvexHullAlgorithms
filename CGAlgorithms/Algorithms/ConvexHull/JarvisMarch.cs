using CGUtilities;
using System;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> inputPoints, List<Line> lines, List<Polygon> polygons, ref List<Point> convexHullPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Get the number of input points
            int numPoints = inputPoints.Count;

            // Check if there are at least 3 points for a convex hull
            if (numPoints < 3)
            {
                // If not enough points, the input points are the convex hull
                convexHullPoints = inputPoints;
                return;
            }

            // Initialize the result convex hull list
            List<Point> convexHull = new List<Point>();

            // Find the leftmost point as the starting point for the convex hull
            int leftmostIndex = FindLeftmostPointIndex(inputPoints);

            // Build the convex hull starting from the leftmost point
            BuildConvexHull(inputPoints, numPoints, leftmostIndex, ref convexHull);

            // Set the output convex hull points
            convexHullPoints = convexHull;
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
        // Determines the orientation of three points.
        // Returns -1 for counterclockwise, 0 for colinear, and 1 for clockwise.
        private int DetermineOrientation(Point p, Point q, Point r)
        {
            double crossProduct = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            // Check for colinearity
            if (Math.Abs(crossProduct) < double.Epsilon)
                return 0; // Colinear

            return (crossProduct > 0) ? -1 : 1; // Counterclockwise or clockwise
        }

        // Calculates the squared Euclidean distance between two points.
        private double CalculateSquaredDistance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;

            return dx * dx + dy * dy;
        }

        // Helper method to find the index of the leftmost point.
        private int FindLeftmostPointIndex(List<Point> points)
        {
            int leftmostIndex = 0;

            for (int i = 1; i < points.Count; i++)
            {
                // Compare X coordinates; if equal, compare Y coordinates
                if (points[i].X < points[leftmostIndex].X ||
                   (points[i].X == points[leftmostIndex].X && points[i].Y < points[leftmostIndex].Y))
                {
                    leftmostIndex = i;
                }
            }

            return leftmostIndex;
        }

        // Helper method to build the convex hull starting from a given point
        private void BuildConvexHull(List<Point> points, int totalPoints, int startingPointIndex, ref List<Point> convexHull)
        {
            int currentPointIndex = startingPointIndex;
            int nextPointIndex;

            do
            {
                // Add the current point to the convex hull
                convexHull.Add(points[currentPointIndex]);

                // Find the next point in the convex hull
                nextPointIndex = FindNextHullPoint(points, totalPoints, currentPointIndex);

                // Update the current point for the next iteration
                currentPointIndex = nextPointIndex;

            } while (currentPointIndex != startingPointIndex);  // Continue until reaching the starting point again
        }

        // Helper method to find the next point in the convex hull
        private int FindNextHullPoint(List<Point> points, int totalPoints, int currentPointIndex)
        {
            int nextPointIndex = (currentPointIndex + 1) % totalPoints;

            for (int i = 0; i < totalPoints; i++)
            {
                int turn = DetermineOrientation(points[currentPointIndex], points[i], points[nextPointIndex]);

                if (turn == -1 || (turn == 0 && CalculateSquaredDistance(points[currentPointIndex], points[i]) > CalculateSquaredDistance(points[currentPointIndex], points[nextPointIndex])))
                    nextPointIndex = i;
            }

            return nextPointIndex;
        }

    }
}
