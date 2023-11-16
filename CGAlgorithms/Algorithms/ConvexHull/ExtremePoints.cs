using CGUtilities;
using System.Collections.Generic;
using System.Reflection;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> inputPoints, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // Check if the number of input points is less than or equal to 3
            // If true, the input points are the convex hull
            if (inputPoints.Count <= 3)
            {
                outPoints = inputPoints;
                return;
            }

            // Copy the input points to the output points list
            outPoints = new List<Point>(inputPoints);
            bool flag = false;

            // Iterate through each point
            for (int i = 0; i < inputPoints.Count; i++)
            {
                // Iterate through each pair of points (j, k)
                for (int j = 0; j < inputPoints.Count; j++)
                {
                    if (flag == true)
                    {
                        // If the flag is set, exit the loop
                        flag = false;
                        break;
                    }

                    for (int k = 0; k < inputPoints.Count; k++)
                    {
                        if (j != i && k != i && j != k)
                        {
                            // Find a point m other than j and k
                            int m;
                            for (m = 0; m < inputPoints.Count; m++)
                            {
                                if (m != j && m != k && m != i)
                                {
                                    break;
                                }
                            }

                            // Check if the current point (i) lies inside the triangle formed by other points (j, k, m)
                            Enums.PointInPolygon position = HelperMethods.PointInTriangle(inputPoints[i], inputPoints[j], inputPoints[k], inputPoints[m]);

                            // Check if we already have 3 points in the convex hull
                            if (outPoints.Count == 3)
                            {
                                flag = true;
                                break;
                            }

                            // If the current point lies inside or on the edge of the triangle, remove it from the convex hull
                            if (position == Enums.PointInPolygon.Inside || position == Enums.PointInPolygon.OnEdge)
                            {
                                outPoints.Remove(inputPoints[i]);
                                flag = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
