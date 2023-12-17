using CGUtilities;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace CGAlgorithms.Algorithms.ConvexHull
{
 
    public class Incremental : Algorithm
    {
        int[] next;  // Declare next at class level
        int[] prev;  // Declare prev at class level

        /// <summary>
        /// Compares two points for sorting purposes.
        /// </summary>

        /// <returns>
        ///     0 if the points are considered equal within a small epsilon value,
        ///    -1 if the first point is less than the second, and
        ///     1 if the first point is greater than the second.
        /// </returns>
        public int ComparePoints(Point a, Point b)
        {
            // Check if the points are almost equal within a small epsilon value
            if (Math.Abs(a.X - b.X) <= Constants.Epsilon && Math.Abs(a.Y - b.Y) <= Constants.Epsilon)
                return 0;

            // Compare points based on their X coordinates
            if (a.X == b.X)
            {
                // If X coordinates are equal, compare based on Y coordinates
                if (a.Y < b.Y)
                    return -1;
                return 1;
            }
            else if (a.X < b.X)
            {
                // If the X coordinate of the first point is less than the second, return -1
                return -1;
            }

            // If the X coordinate of the first point is greater than the second, return 1
            return 1;
        }

        /// <summary>
        /// Runs the Incremental Convex Hull algorithm on the provided set of points.
        /// </summary>

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Handle special cases 
            if (points.Count == 0)
            {
                // No points to process
                return;
            }

            if (points.Count == 1)
            {
                // Only one point, it is the convex hull
                outPoints.Add(points[0]);
                return;
            }

            // Sort the points lexicographically for the convex hull algorithm
            points.Sort(ComparePoints);

            // Representation of the convex hull 
            InitializeHullRepresentation(points);

            // Find the index of the first non-collinear point
            int index = FindFirstNonCollinearPoint(points);

            if (index != points.Count)
            {
                // Initialize the convex hull with the first non-collinear point
                InitializeFirstHullPoints(index);
            }
            else
            {
                // All points are collinear, the convex hull is a single point
                outPoints.Add(points[0]);
                return;
            }

            // Build the rest of the convex hull incrementally 
            index = IncrementalHull(points, index);

            // Build the result convex hull
            BuildResultHull(points, outPoints, index);
        }

        /// <summary>
        /// Initializes the data structures representing the convex hull.
        /// </summary>
        private void InitializeHullRepresentation(List<Point> points)
        {
            // Create arrays to represent the next and previous points in the convex hull
            next = new int[points.Count];
            prev = new int[points.Count];
        }

        /// <summary>
        /// Finds the index of the first point that is not collinear with the first point.
        /// </summary>
        private int FindFirstNonCollinearPoint(List<Point> points)
        {
            int index = 1;

            // Increment the index until a non-collinear point is found or the end of the list is reached
            while (index < points.Count && ComparePoints(points[0], points[index]) == 0)
            {
                index += 1;
            }

            return index;
        }

        /// <summary>
        /// Initializes the convex hull with the first non-collinear point.
        /// </summary>
        private void InitializeFirstHullPoints(int index)
        {
            // Connect the first non-collinear point with the initial point to form the initial convex hull edge
            next[0] = index;
            prev[0] = index;
            next[index] = 0;
            prev[index] = 0;
        }

        /// <summary>
        /// Builds the convex hull incrementally by adding points to the existing hull.
        /// </summary>
        private int IncrementalHull(List<Point> points, int index)
        {
            // Initialize the index of the most right point in the hull
            int mostRightPointIndex = index;

            // Iterate through the remaining points to add them to the hull
            for (index = index + 1; index < points.Count; index++)
            {
                Point newPoint = points[index];

                // Ignore points that are equal to the most right point in the hull
                if (ComparePoints(newPoint, points[mostRightPointIndex]) == 0)
                {
                    continue;
                }

                // Update the convex hull edges with the new point
                UpdateHullEdges(points, index, mostRightPointIndex);

                // Update the most right point index
                mostRightPointIndex = index;
            }

            // Return the final index processed during incremental hull construction
            return index;
        }

        /// <summary>
        /// Updates the convex hull edges with the new point during incremental hull construction.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <param name="index">The index of the new point.</param>
        /// <param name="mostRightPointIndex">The index of the most right point in the current convex hull.</param>
        private void UpdateHullEdges(List<Point> points, int index, int mostRightPointIndex)
        {
            Point newPoint = points[index];

            // Determine whether the new point is above or below the most right point in the hull
            if (newPoint.Y >= points[mostRightPointIndex].Y)
            {
                // The new point is above or at the same height as the most right point
                next[index] = next[mostRightPointIndex];
                prev[index] = mostRightPointIndex;
            }
            else
            {
                // The new point is below the most right point
                next[index] = mostRightPointIndex;
                prev[index] = prev[mostRightPointIndex];
            }

            // Update the next and previous pointers of adjacent points in the hull
            next[prev[index]] = index;
            prev[next[index]] = index;

            // Update the upper and lower hull edges based on the new point
            UpdateUpperLine(points, index);
            UpdateLowerLine(points, index);
        }


        /// <summary>
        /// Updates the upper hull line during incremental hull construction based on the new point.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <param name="index">The index of the new point.</param>
        private void UpdateUpperLine(List<Point> points, int index)
        {
            // Continue updating the upper hull line until a left turn is encountered
            while (true)
            {
                // Create a line segment using the current point and the next point in the hull
                Line segment = new Line(points[index], points[next[index]]);

                // Get the next point after the next point in the hull
                Point nextPoint = points[next[next[index]]];

                // Determine the turn type using the HelperMethods.CheckTurn method
                Enums.TurnType turn = HelperMethods.CheckTurn(segment, nextPoint);

                // If the turn is not left, update the hull pointers and check for colinearity
                if (turn != Enums.TurnType.Left)
                {
                    // Update the next pointer of the current point to skip the next point in the hull
                    next[index] = next[next[index]];

                    // Update the previous pointer of the next point to connect it to the current point
                    prev[next[index]] = index;

                    // If the points are colinear, break out of the loop
                    if (turn == Enums.TurnType.Colinear)
                    {
                        break;
                    }
                }
                else
                {
                    // If a left turn is encountered, break out of the loop
                    break;
                }
            }
        }


        /// <summary>
        /// Updates the lower hull line during incremental hull construction based on the new point.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <param name="index">The index of the new point.</param>
        private void UpdateLowerLine(List<Point> points, int index)
        {
            // Continue updating the lower hull line until a right turn is encountered
            while (true)
            {
                // Create a line segment using the current point and the previous point in the hull
                Line segment = new Line(points[index], points[prev[index]]);

                // Get the next point before the previous point in the hull
                Point nextPoint = points[prev[prev[index]]];

                // Determine the turn type using the HelperMethods.CheckTurn method
                Enums.TurnType turn = HelperMethods.CheckTurn(segment, nextPoint);

                // If the turn is not right, update the hull pointers and check for colinearity
                if (turn != Enums.TurnType.Right)
                {
                    // Update the previous pointer of the current point to skip the previous point in the hull
                    prev[index] = prev[prev[index]];

                    // Update the next pointer of the previous point to connect it to the current point
                    next[prev[index]] = index;

                    // If the points are colinear, break out of the loop
                    if (turn == Enums.TurnType.Colinear)
                    {
                        break;
                    }
                }
                else
                {
                    // If a right turn is encountered, break out of the loop
                    break;
                }
            }
        }


        /// <summary>
        /// Builds the final convex hull by traversing the hull edges and adding points to the result list.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <param name="outPoints">The list to store the convex hull points.</param>
        /// <param name="index">The starting index for building the convex hull.</param>
        private void BuildResultHull(List<Point> points, List<Point> outPoints, int index)
        {
            // Initialize the current index to the starting index
            int currentIndex = 0;
                
            // Traverse the hull edges and add points to the result list
            while (true)
            {
                // Add the current point to the result list
                outPoints.Add(points[currentIndex]);

                // Move to the next point in the hull using the next pointer
                currentIndex = next[currentIndex];

                // If the next point is the starting point, break out of the loop
                if (currentIndex == 0)
                {
                    break;
                }
            }
        }


        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
