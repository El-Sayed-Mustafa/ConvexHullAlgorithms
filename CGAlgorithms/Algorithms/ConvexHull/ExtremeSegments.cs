using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 2)
            {
                outPoints = points;
                return;
            }
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    int left = 0, right = 0;
                    for (int k = 0; k < points.Count; k++)
                    {
                        Line line = new Line(points[i], points[j]);
                        if (k != i && k != j)
                        {
                            Enums.TurnType turnTest = HelperMethods.CheckTurn(line, points[k]);
                            if (turnTest == Enums.TurnType.Left)
                                left++;
                            else if (turnTest == Enums.TurnType.Right)
                                right++;


                        }
                    }
                    if ((left == 0 && right > 0) || (right == 0 && left > 0) && i != j)
                    {
                        if (!outPoints.Contains(points[i]))
                            outPoints.Add(points[i]);
                        if (!outPoints.Contains(points[j]))
                            outPoints.Add(points[j]);
                    }

                }
            }

           outPoints =  deleteOnSegmentPoints(outPoints);

        }

        private List<Point> deleteOnSegmentPoints (List<Point> points){

            for (int i = 0; i < points.Count; i++)
            {
                bool isExtreme = true;
                for (int j = 0; j < points.Count; j++)
                {
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (points[i] != points[j] && points[i] != points[k])
                        {
                            bool isOnSegment = HelperMethods.PointOnSegment(points[i], points[j], points[k]);
                            if (isOnSegment)
                            {
                                points.Remove(points[i]);
                                isExtreme = false;
                                break;
                            }
                        }
                    }
                    if (!isExtreme)
                        break;
                }
                if (!isExtreme)
                    i--;
            }
            return points;
        }
        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}