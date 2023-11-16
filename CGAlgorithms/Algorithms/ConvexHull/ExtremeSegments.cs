using CGUtilities;
using System.Collections.Generic;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {/// <summary>
     /// Removes points that lie on segments of the convex hull.
     /// </summary>
     /// <param name="inputPoints">The input list of points.</param>
     /// <returns>The list of points after removing those on segments.</returns>
        private List<Point> RemovePointsOnSegments(List<Point> inputPoints)
        {
            for (int currentIndex = 0; currentIndex < inputPoints.Count; currentIndex++)
            {
                bool isExtreme = true;
                for (int compareIndex1 = 0; compareIndex1 < inputPoints.Count; compareIndex1++)
                {
                    for (int compareIndex2 = 0; compareIndex2 < inputPoints.Count; compareIndex2++)
                    {
                        if (inputPoints[currentIndex] != inputPoints[compareIndex1] && inputPoints[currentIndex] != inputPoints[compareIndex2])
                        {
                            // Check if the current point lies on the segment formed by two other points.
                            bool isOnSegment = HelperMethods.PointOnSegment(inputPoints[currentIndex], inputPoints[compareIndex1], inputPoints[compareIndex2]);
                            if (isOnSegment)
                            {
                                // Remove the current point if it lies on a segment.
                                inputPoints.Remove(inputPoints[currentIndex]);
                                isExtreme = false;
                                break;
                            }
                        }
                    }
                    if (!isExtreme)
                        break;
                }
                if (!isExtreme)
                    currentIndex--;
            }
            return inputPoints;
        }

        /// <summary>
        /// Finds convex hull points based on extreme segments.
        /// </summary>
        /// <param name="inputPoints">The input list of points.</param>
        /// <param name="convexHullPoints">The list to store convex hull points.</param>
        private void FindConvexHullPoints(List<Point> inputPoints, ref List<Point> convexHullPoints)
        {
            for (int firstIndex = 0; firstIndex < inputPoints.Count; firstIndex++)
            {
                for (int secondIndex = 0; secondIndex < inputPoints.Count; secondIndex++)
                {
                    int leftCount = 0, rightCount = 0;
                    for (int thirdIndex = 0; thirdIndex < inputPoints.Count; thirdIndex++)
                    {
                        Line line = new Line(inputPoints[firstIndex], inputPoints[secondIndex]);
                        if (thirdIndex != firstIndex && thirdIndex != secondIndex)
                        {
                            // Determine the turn type of the third point relative to the line formed by the first two points.
                            Enums.TurnType turnTest = HelperMethods.CheckTurn(line, inputPoints[thirdIndex]);
                            if (turnTest == Enums.TurnType.Left)
                                leftCount++;
                            else if (turnTest == Enums.TurnType.Right)
                                rightCount++;
                        }
                    }
                    // Check if the current line segment is extreme based on the count of left and right turns.
                    if ((leftCount == 0 && rightCount > 0) || (rightCount == 0 && leftCount > 0) && firstIndex != secondIndex)
                    {
                        if (!convexHullPoints.Contains(inputPoints[firstIndex]))
                            convexHullPoints.Add(inputPoints[firstIndex]);
                        if (!convexHullPoints.Contains(inputPoints[secondIndex]))
                            convexHullPoints.Add(inputPoints[secondIndex]);
                    }
                }
            }

        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Check if the number of points is less than or equal to 2, in which case, they are already the convex hull.
            if (points.Count <= 2)
            {
                outPoints = points;
                return;
            }

            // Find the convex hull points based on the extreme segments.
            FindConvexHullPoints(points, ref outPoints);

            // Remove points that lie on segments of the convex hull.
            outPoints = RemovePointsOnSegments(outPoints);
        }

     

        
        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
