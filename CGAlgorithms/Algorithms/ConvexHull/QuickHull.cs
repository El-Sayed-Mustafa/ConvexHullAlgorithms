using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int n = points.Count;

            // If there are fewer than 3 points, the convex hull is not defined
            if (n < 3)
            {
                outPoints = points;
                return;
            }

            HashSet<Point> hull = new HashSet<Point>();

            // Finding the point with minimum and maximum x-coordinate
            int minXIndex = 0, maxXIndex = 0;
            for (int i = 1; i < n; i++)
            {
                if (points[i].X < points[minXIndex].X)
                    minXIndex = i;
                if (points[i].X > points[maxXIndex].X)
                    maxXIndex = i;
            }

            // Recursively find convex hull points on one side of the line joining points[minXIndex] and points[maxXIndex]
            QuickHullRecursive(points, n, points[minXIndex], points[maxXIndex], 1, ref hull);

            // Recursively find convex hull points on the other side of the line
            QuickHullRecursive(points, n, points[minXIndex], points[maxXIndex], -1, ref hull);

            // Set the output convex hull points
            outPoints = hull.ToList();
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }

        // Helper method for QuickHull algorithm
        private void QuickHullRecursive(List<Point> a, int n, Point p1, Point p2, int side, ref HashSet<Point> hull)
        {
            int ind = -1;
            int maxDist = 0;

            // Finding the point with maximum distance from the line
            for (int i = 0; i < n; i++)
            {
                int temp = LineDist(p1, p2, a[i]);
                if (FindSide(p1, p2, a[i]) == side && temp > maxDist)
                {
                    ind = i;
                    maxDist = temp;
                }
            }

            // If no point is found, add the end points of the line to the convex hull
            if (ind == -1)
            {
                hull.Add(p1);
                hull.Add(p2);
                return;
            }

            // Recur for the two parts divided by a[ind]
            QuickHullRecursive(a, n, a[ind], p1, -FindSide(a[ind], p1, p2), ref hull);
            QuickHullRecursive(a, n, a[ind], p2, -FindSide(a[ind], p2, p1), ref hull);
        }

        // Helper method to find the side of a point with respect to a line
        private int FindSide(Point p1, Point p2, Point p)
        {
            int val = (int)((p.Y - p1.Y) * (p2.X - p1.X) - (p2.Y - p1.Y) * (p.X - p1.X));
            if (val > 0)
                return 1;
            if (val < 0)
                return -1;
            return 0;
        }

        // Helper method to find a value proportional to the distance between a point and a line
        private int LineDist(Point p1, Point p2, Point p)
        {
            return (int)Math.Abs((p.Y - p1.Y) * (p2.X - p1.X) - (p2.Y - p1.Y) * (p.X - p1.X));
        }
    }
}
