using CGUtilities;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    /// <summary>
    /// Implementation of the Divide and Conquer algorithm for computing the convex hull.
    /// </summary>
    public class DivideAndConquer : Algorithm
    {
        private readonly DivideHelper divideHelper;

        public DivideAndConquer()
        {
            divideHelper = new DivideHelper();
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Sort the input points by X and then by Y.
            points = points.OrderBy(point => point.X).ThenBy(point => point.Y).ToList();

            // Use the DivideHelper to compute the convex hull.
            outPoints = divideHelper.Divide(points);
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }
    }
    /// <summary>
    /// Helper class for the Divide and Conquer algorithm, responsible for dividing the list of points and combining their convex hulls.
    /// </summary>
    public class DivideHelper
    {
        /// <summary>
        /// Divides a list of points into two halves recursively and combines their convex hulls.
        /// </summary>
        /// <param name="divisiblePoints">The list of points to divide.</param>
        /// <returns>The convex hull of the combined points.</returns>
        public List<Point> Divide(List<Point> divisiblePoints)
        {
            // Base case: If there is only one point or none, return the list as it is already a convex hull.
            if (divisiblePoints.Count <= 1)
            {
                return divisiblePoints;
            }

            // Divide the points into left and right halves.
            List<Point> leftPoints = GetLeftHalf(divisiblePoints);
            List<Point> rightPoints = GetRightHalf(divisiblePoints);

            // Recursively divide and combine the convex hulls of the left and right halves.
            List<Point> convexHull = new CombineHelper().Combine(Divide(leftPoints), Divide(rightPoints));

            return convexHull;
        }

        /// <summary>
        /// Retrieves the left half of a list of points.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <returns>The left half of the points.</returns>
        private List<Point> GetLeftHalf(List<Point> points)
        {
            // Create a new list to store the left half of the points.
            List<Point> leftPoints = new List<Point>();

            // Iterate through the first half of the points and add them to the left half.
            for (int i = 0; i < points.Count / 2; i++)
            {
                leftPoints.Add(points[i]);
            }

            return leftPoints;
        }

        /// <summary>
        /// Retrieves the right half of a list of points.
        /// </summary>
        /// <param name="points">The list of points.</param>
        /// <returns>The right half of the points.</returns>
        private List<Point> GetRightHalf(List<Point> points)
        {
            // Create a new list to store the right half of the points.
            List<Point> rightPoints = new List<Point>();

            // Iterate through the second half of the points and add them to the right half.
            for (int i = points.Count / 2; i < points.Count; i++)
            {
                rightPoints.Add(points[i]);
            }

            return rightPoints;
        }
    }
    /// <summary>
    /// Helper class for the Divide and Conquer algorithm, responsible for combining the convex hulls of left and right halves.
    /// </summary>
    public class CombineHelper
    {
        /// <summary>
        /// Combines the convex hulls of left and right halves.
        /// </summary>
        /// <param name="leftPoints">The convex hull of the left half.</param>
        /// <param name="rightPoints">The convex hull of the right half.</param>
        /// <returns>The combined convex hull.</returns>
        public List<Point> Combine(List<Point> leftPoints, List<Point> rightPoints)
        {
            int rightmostIndexLeft = 0, leftmostIndexRight = 0, leftCount = leftPoints.Count, rightCount = rightPoints.Count;

            // Finding the rightmost point in the left convex hull.
            rightmostIndexLeft = FindRightMostPoint(leftPoints, rightmostIndexLeft, leftCount);

            // Finding the leftmost point in the right convex hull.
            leftmostIndexRight = FindLeftMostPoint(rightPoints, leftmostIndexRight, rightCount);

            int upperTanA = rightmostIndexLeft;
            int upperTanB = leftmostIndexRight;
            bool found = false;

            // Updating the upper tangent points until both of them make a line that doesn't cross the polygon.
            while (!found)
            {
                found = true;
                UpdateTangentA(leftPoints, rightPoints, leftCount, ref upperTanA, upperTanB, ref found);
                upperTanA = AdjustUpperTangentA(leftPoints, rightPoints, leftCount, upperTanA, upperTanB);
                UpdateTangentB(leftPoints, rightPoints, rightCount, upperTanA, ref upperTanB, ref found);
                upperTanB = AdjustUpperTangentB(leftPoints, rightPoints, rightCount, upperTanA, upperTanB);
            }

            int lowerTanA = rightmostIndexLeft;
            int lowerTanB = leftmostIndexRight;
            found = false;

            // Updating the lower tangent points until both of them make a line that doesn't cross the polygon.
            while (!found)
            {
                found = UpdateLowerTangentA(leftPoints, rightPoints, leftCount, ref lowerTanA, lowerTanB);
                lowerTanA = AdjustLowerTangentA(leftPoints, rightPoints, leftCount, lowerTanA, lowerTanB);
                UpdateLowerTangentB(leftPoints, rightPoints, rightCount, ref found, lowerTanA, ref lowerTanB);
                lowerTanB = AdjustLowerTangentB(leftPoints, rightPoints, rightCount, lowerTanA, lowerTanB);
            }

            List<Point> convexHull = new List<Point>
                {
                    leftPoints[upperTanA]
                };

            return BruteForce(leftPoints, rightPoints, leftCount, rightCount, ref upperTanA, upperTanB, lowerTanA, ref lowerTanB, convexHull);
        }

        /// <summary>
        /// Performs the brute force step to complete the convex hull.
        /// </summary>
        private List<Point> BruteForce(List<Point> leftPoints, List<Point> rightPoints, int Lcount, int Rcount, ref int upTanA, int upTanB, int lowTanA, ref int lowTanB, List<Point> convexHull)
        {
            while (upTanA != lowTanA)
            {
                upTanA = (upTanA + 1) % Lcount;

                if (!convexHull.Contains(leftPoints[upTanA]))
                    convexHull.Add(leftPoints[upTanA]);
            }

            convexHull.Add(rightPoints[lowTanB]);

            while (lowTanB != upTanB)
            {
                lowTanB = (lowTanB + 1) % Rcount;

                if (!convexHull.Contains(rightPoints[lowTanB]))
                    convexHull.Add(rightPoints[lowTanB]);
            }

            return convexHull;
        }

        /// <summary>
        /// Updates the lower tangent point A until it forms a non-crossing line.
        /// </summary>
        private bool UpdateLowerTangentA(List<Point> leftPoints, List<Point> rightPoints, int leftCount, ref int lowerTanA, int lowerTanB)
        {
            bool isFound = true;

            while (Enums.TurnType.Left == HelperMethods.CheckTurn(new Line(rightPoints[lowerTanB].X, rightPoints[lowerTanB].Y, leftPoints[lowerTanA].X, leftPoints[lowerTanA].Y)
                , leftPoints[(lowerTanA + leftCount - 1) % leftCount]))
            {
                lowerTanA = (lowerTanA + leftCount - 1) % leftCount;
                isFound = false;
            }

            return isFound;
        }


        /// <summary>
        /// Updates the lower tangent point B until it forms a non-crossing line.
        /// </summary>
        private void UpdateLowerTangentB(List<Point> leftPoints, List<Point> rightPoints, int rightCount, ref bool isFound, int lowerTanA, ref int lowerTanB)
        {
            while (HelperMethods.CheckTurn(new Line(leftPoints[lowerTanA].X, leftPoints[lowerTanA].Y, rightPoints[lowerTanB].X, rightPoints[lowerTanB].Y),
                rightPoints[(lowerTanB + 1) % rightCount]) == Enums.TurnType.Right)
            {
                lowerTanB = (lowerTanB + 1) % rightCount;
                isFound = false;
            }
        }


        /// <summary>
        /// Updates the upper tangent point A until it forms a non-crossing line.
        /// </summary>
        private bool UpdateTangentA(List<Point> leftPoints, List<Point> rightPoints, int leftCount, ref int upperTanA, int upperTanB, ref bool isFound)
        {
            while (HelperMethods.CheckTurn(new Line(rightPoints[upperTanB].X, rightPoints[upperTanB].Y, leftPoints[upperTanA].X, leftPoints[upperTanA].Y),
                                  leftPoints[(upperTanA + 1) % leftCount]) == Enums.TurnType.Right)
            {
                upperTanA = (upperTanA + 1) % leftCount;
                isFound = false;
            }

            return isFound;
        }


        /// <summary>
        /// Updates the upper tangent point B until it forms a non-crossing line.
        /// </summary>
        private void UpdateTangentB(List<Point> leftPoints, List<Point> rightPoints, int rightCount, int upperTanA, ref int upperTanB, ref bool isFound)
        {
            while (HelperMethods.CheckTurn(new Line(leftPoints[upperTanA].X, leftPoints[upperTanA].Y, rightPoints[upperTanB].X, rightPoints[upperTanB].Y),
                            rightPoints[(rightCount + upperTanB - 1) % rightCount]) == Enums.TurnType.Left)
            {
                upperTanB = (rightCount + upperTanB - 1) % rightCount;
                isFound = false;
            }
        }

        /// <summary>
        /// Checks and adjusts the index of the upper tangent point B if it is colinear with the previous and next points.
        /// </summary>
        private int AdjustUpperTangentB(List<Point> leftConvexHull, List<Point> rightConvexHull, int rightHullCount, int upperTangentA, int upperTangentB)
        {
            if ((HelperMethods.CheckTurn(new Line(leftConvexHull[upperTangentA].X, leftConvexHull[upperTangentA].Y, rightConvexHull[upperTangentB].X, rightConvexHull[upperTangentB].Y),
                rightConvexHull[(upperTangentB - 1 + rightHullCount) % rightHullCount]) == Enums.TurnType.Colinear))
            {
                upperTangentB = (upperTangentB - 1 + rightHullCount) % rightHullCount;
            }

            return upperTangentB;
        }


        /// <summary>
        /// Checks and adjusts the index of the upper tangent point A if it is colinear with the previous and next points.
        /// </summary>
        private int AdjustUpperTangentA(List<Point> leftConvexHull, List<Point> rightConvexHull, int leftHullCount, int upperTangentA, int upperTangentB)
        {
            if ((HelperMethods.CheckTurn(new Line(rightConvexHull[upperTangentB].X, rightConvexHull[upperTangentB].Y, leftConvexHull[upperTangentA].X, leftConvexHull[upperTangentA].Y),
                leftConvexHull[(upperTangentA + 1) % leftHullCount]) == Enums.TurnType.Colinear))
            {
                upperTangentA = (upperTangentA + 1) % leftHullCount;
            }

            return upperTangentA;
        }


        /// <summary>
        /// Checks and adjusts the index of the lower tangent point A if it is colinear with the previous and next points.
        /// </summary>
        private int AdjustLowerTangentA(List<Point> leftConvexHull, List<Point> rightConvexHull, int leftHullCount, int lowerTangentA, int lowerTangentB)
        {
            if (HelperMethods.CheckTurn(new Line(rightConvexHull[lowerTangentB].X, rightConvexHull[lowerTangentB].Y, leftConvexHull[lowerTangentA].X, leftConvexHull[lowerTangentA].Y),
                leftConvexHull[(lowerTangentA + leftHullCount - 1) % leftHullCount]) == Enums.TurnType.Colinear)
            {
                lowerTangentA = (lowerTangentA + leftHullCount - 1) % leftHullCount;
            }

            return lowerTangentA;
        }


        /// <summary>
        /// Checks and adjusts the index of the lower tangent point B if it is colinear with the previous and next points.
        /// </summary>
        private int AdjustLowerTangentB(List<Point> leftConvexHull, List<Point> rightConvexHull, int rightHullCount, int lowerTangentA, int lowerTangentB)
        {
            if ((HelperMethods.CheckTurn(new Line(leftConvexHull[lowerTangentA].X, leftConvexHull[lowerTangentA].Y, rightConvexHull[lowerTangentB].X, rightConvexHull[lowerTangentB].Y),
                rightConvexHull[(lowerTangentB + 1) % rightHullCount]) == Enums.TurnType.Colinear))
            {
                lowerTangentB = (lowerTangentB + 1) % rightHullCount;
            }

            return lowerTangentB;
        }



        /// <summary>
        /// Finds the index of the leftmost point in the right convex hull.
        /// </summary>
        private int FindLeftMostPoint(List<Point> points, int initialIndex, int count)
        {
            int leftMostIndex = initialIndex;

            for (int i = count - 1; i >= 1; i--)
            {
                if (points[i].X < points[leftMostIndex].X ||
                    (points[i].X == points[leftMostIndex].X && points[i].Y < points[leftMostIndex].Y))
                {
                    leftMostIndex = i;
                }
            }

            return leftMostIndex;
        }


        /// <summary>
        /// Finds the index of the rightmost point in the left convex hull.
        /// </summary>
        private int FindRightMostPoint(List<Point> points, int initialIndex, int count)
        {
            int rightMostIndex = initialIndex;

            for (int i = count - 1; i >= 1; i--)
            {
                if (points[i].X > points[rightMostIndex].X ||
                    (points[i].X == points[rightMostIndex].X && points[i].Y > points[rightMostIndex].Y))
                {
                    rightMostIndex = i;
                }
            }

            return rightMostIndex;
        }

    }

}
