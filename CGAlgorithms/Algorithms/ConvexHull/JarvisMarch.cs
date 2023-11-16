using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int n = points.Count;

            // There must be at least 3 points for convex hull
            if (n < 3)
            {
                outPoints = points;
                return;
            }

            // Initialize Result
            List<Point> hull = new List<Point>();

            // Find the leftmost point
            int l = 0;
            for (int i = 1; i < n; i++)
            {
                if (points[i].X < points[l].X || (points[i].X == points[l].X && points[i].Y < points[l].Y))
                    l = i;
            }

            // Start from the leftmost point, keep moving counterclockwise until reach the start point again
            int p = l, q;
            do
            {
                // Add the current point to the result
                hull.Add(points[p]);

                // Search for a point 'q' such that orientation(p, x, q) is counterclockwise for all points 'x'
                q = (p + 1) % n;
                for (int i = 0; i < n; i++)
                {
                    // If i is more counterclockwise than current q, then update q
                    int turn = TurnOrientation(points[p], points[i], points[q]);
                    if (turn == -1 || (turn == 0 && Distance(points[p], points[i]) > Distance(points[p], points[q])))
                        q = i;
                }

                // Now q is the most counterclockwise with respect to p
                // Set p as q for the next iteration, so that q is added to the result 'hull'
                p = q;

            } while (p != l);  // While we don't come to the first point

            // Set the output convex hull points
            outPoints = hull;

            // Optionally, you can create lines or polygons based on the convex hull points
            // For simplicity, the lines and polygons lists are not used in this example.
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }

        // Function to determine the orientation of three points
        // Returns -1 for counterclockwise, 0 for colinear, and 1 for clockwise
        private int TurnOrientation(Point p, Point q, Point r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
            if (Math.Abs(val) < double.Epsilon) return 0;     // colinear
            return (val > 0) ? -1 : 1;   // counterclockwise or clockwise
        }

        // Function to calculate the squared Euclidean distance between two points
        private double Distance(Point a, Point b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }
    }
}