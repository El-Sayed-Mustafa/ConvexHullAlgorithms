using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        /// <summary>
        /// Calculates the dot product of two 2D vectors represented by points.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static double DotProduct(Point v1, Point v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            points.Sort(CompareXY);
            points = RemoveDuplicates(points);

            FindLowestYPointIndex(points, out double MinimY, out double x, out int ind);

            Line MakeLIne = new Line(new Point(x, MinimY), new Point(x + 1000.0, MinimY));

            List<KeyValuePair<double, int>> list = CalculateAngles(points, ind, MakeLIne);


            list.Sort(CompareAngles);

            Stack<int> st = new Stack<int>();
            st.Push(ind);
            if (list.Count > 0)
                st.Push(list[0].Value);
            MakeLIne = BuildConvexHull(points, MakeLIne, list, st);

            PopulateConvexHullPoints(points, outPoints, st);
        }

        /// <summary>
        /// Populates the convex hull points from the stack to the output list.
        /// </summary>
        private static void PopulateConvexHullPoints(List<Point> inputPoints, List<Point> convexHullPoints, Stack<int> convexHullStack)
        {
            // Pop elements from the stack and add corresponding points to the convex hull list
            while (convexHullStack.Count > 0)
            {
                convexHullPoints.Add(inputPoints[convexHullStack.Peek()]);
                convexHullStack.Pop();
            }
        }


        /// <summary>
        /// Builds the convex hull using the Graham Scan algorithm.
        /// </summary>
        /// <param name="points">The list of input points.</param>
        /// <param name="currentLine">The current line used in the algorithm.</param>
        /// <param name="sortedAngles">The list of angles and corresponding indices sorted in ascending order.</param>
        /// <param name="convexHullStack">The stack containing the indices of convex hull points.</param>
        /// <returns>The final line used in the convex hull.</returns>
        private static Line BuildConvexHull(List<Point> points, Line currentLine, List<KeyValuePair<double, int>> sortedAngles, Stack<int> convexHullStack)
        {
            // Iterate through the sorted angles and update the convex hull stack
            for (int i = 1; i < points.Count - 1 && convexHullStack.Count >= 2; i++)
            {
                int previousIndex1 = convexHullStack.Peek();
                Point previousPoint1 = points[previousIndex1];
                convexHullStack.Pop();

                int previousIndex2 = convexHullStack.Peek();
                Point previousPoint2 = points[previousIndex2];

                convexHullStack.Push(previousIndex1);

                currentLine = new Line(previousPoint2, previousPoint1);

                // Check the turn type and update the stack accordingly
                if (HelperMethods.CheckTurn(currentLine, points[sortedAngles[i].Value]) == Enums.TurnType.Left)
                {
                    convexHullStack.Push(sortedAngles[i].Value);
                }
                else if (HelperMethods.CheckTurn(currentLine, points[sortedAngles[i].Value]) == Enums.TurnType.Colinear)
                {
                    convexHullStack.Pop();
                    convexHullStack.Push(sortedAngles[i].Value);
                }
                else
                {
                    convexHullStack.Pop();
                    i--;
                }
            }

            return currentLine;
        }


        /// <summary>
        /// Calculates the angles between the reference line and other points.
        /// </summary>
        /// <param name="points">The list of input points.</param>
        /// <param name="referenceIndex">The index of the reference point.</param>
        /// <param name="referenceLine">The reference line.</param>
        /// <returns>A list of angles and corresponding indices, sorted in ascending order.</returns>
        private List<KeyValuePair<double, int>> CalculateAngles(List<Point> points, int referenceIndex, Line referenceLine)
        {
            List<KeyValuePair<double, int>> angleList = new List<KeyValuePair<double, int>>();

            // Calculate angles for each point and add them to the list
            for (int i = 0; i < points.Count; i++)
            {
                if (i == referenceIndex)
                    continue;

                Point vector1 = PointToVector(referenceLine.Start, referenceLine.End);
                Point vector2 = PointToVector(referenceLine.Start, points[i]);

                double crossProduct = HelperMethods.CrossProduct(vector1, vector2);
                double dotProduct = DotProduct(vector1, vector2);

                double angle = Math.Atan2(crossProduct, dotProduct) * (180.00 / Math.PI);
                if (angle < 0)
                    angle += 360;

                angleList.Add(new KeyValuePair<double, int>(angle, i));
            }

            // Sort the list by angles in ascending order
            angleList.Sort((a, b) => a.Key.CompareTo(b.Key));

            return angleList;
        }

        /// <summary>
        /// Finds the index of the point with the lowest Y coordinate.
        /// </summary>
        /// <param name="points">The list of input points.</param>
        /// <param name="lowestY">Output parameter for the minimum Y coordinate.</param>
        /// <param name="xCoordinate">Output parameter for the corresponding X coordinate.</param>
        /// <param name="lowestYIndex">Output parameter for the index of the point with the lowest Y coordinate.</param>
        private static void FindLowestYPointIndex(List<Point> points, out double lowestY, out double xCoordinate, out int lowestYIndex)
        {
            lowestY = double.MaxValue;
            xCoordinate = 0;
            bool[] visited = new bool[points.Count];

            // Initialize the visited array
            for (int i = 0; i < points.Count; i++)
                visited[i] = false;

            lowestYIndex = 0;

            // Find the point with the lowest Y coordinate
            for (int i = 0; i < points.Count; i++)
            {
                if (lowestY > points[i].Y)
                {
                    lowestY = points[i].Y;
                    xCoordinate = points[i].X;
                    lowestYIndex = i;
                }
            }
        }


        /// <summary>
        /// Removes duplicate points from the input list.
        /// </summary>
        /// <param name="points">The list of input points.</param>
        /// <returns>A list of unique points without duplicates.</returns>
        private static List<Point> RemoveDuplicates(List<Point> points)
        {
            List<Point> uniquePoints = new List<Point> { points[0] };

            // Iterate through the points to find and add unique points
            for (int i = 1; i < points.Count; i++)
            {
                // Check if the current point is the same as the previous one
                if (points[i].Equals(points[i - 1]))
                {
                    continue;
                }

                // Add the unique point to the list
                uniquePoints.Add(points[i]);
            }

            // Update the input list with the unique points
            points = uniquePoints;

            return points;
        }

        /// <summary>
        /// Custom comparison method for sorting a list of key-value pairs based on their keys.
        /// </summary>
        /// <param name="a">The first key-value pair.</param>
        /// <param name="b">The second key-value pair.</param>
        /// <returns>An integer indicating the relative order of the key-value pairs.</returns>
        static int CompareAngles(KeyValuePair<double, int> a, KeyValuePair<double, int> b)
        {
            if (a.Key == b.Key)
            {
                return a.Value.CompareTo(b.Value);
            }

            return a.Key.CompareTo(b.Key);
        }

        /// <summary>
        /// Converts two points into a vector.
        /// </summary>
        /// <param name="a">The starting point of the vector.</param>
        /// <param name="b">The ending point of the vector.</param>
        /// <returns>A vector representing the difference between the two points.</returns>
        Point PointToVector(Point a, Point b)
        {
            return new Point(b.X - a.X, b.Y - a.Y);
        }

        /// <summary>
        /// Custom comparison method for sorting points based on their X and Y coordinates.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>An integer indicating the relative order of the points.</returns>
        static int CompareXY(Point a, Point b)
        {
            if (a.X == b.X)
            {
                return a.Y.CompareTo(b.Y);
            }

            return a.X.CompareTo(b.X);
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
        
    }
}