using CGUtilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }

            Point point1, point2, point3, points4;
            outPoints = new List<Point> (points);
            bool flag = false;
            // Iterate through each point
            for (int i = 0; i < points.Count; i++)
            {
              
                // Iterate through each pair of points (j, k)
                for (int j = 0; j < points.Count; j++)
                {
                    if (flag == true)
                    {
                        flag = false;
                        break;
                    }
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (j != i && k != i && j != k)
                        {
                            // Find a point m other than j and k
                            int m;
                            for (m = 0; m < points.Count; m++)
                            {
                                if (m != j && m != k && m != i)
                                {
                                    break;
                                }
                            }

                            // Check if the current point (i) lies inside the triangle formed by other points (j, k, m)
                            Enums.PointInPolygon position = HelperMethods.PointInTriangle(points[i], points[j], points[k], points[m]);
                            point1 = points[i];
                            point2 = points[j];
                            point3 = points[k];
                            points4 = points[m];

                            if (outPoints.Count==3)
                            {
                                flag = true;

                                break;
                            }
                            if (position == Enums.PointInPolygon.Inside || position == Enums.PointInPolygon.OnEdge)
                            {
                                // If it lies inside, remove the current point from the result
                                outPoints.Remove(points[i]);
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