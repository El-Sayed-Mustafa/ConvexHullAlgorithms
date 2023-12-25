using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 4)
            {
                outPoints = points;
            }
            else
            {
                List<Point> polygon = BuildInitialPolygon(points);
                List<Line> polyLines = BuildInitialPolygonLines(polygon);

                for (int j = 0; j < polyLines.Count; j++)
                {
                    Line selectedLine = polyLines[j];
                    List<Point> possiblePoints = GetPossiblePoints(points, selectedLine);

                    if (possiblePoints.Count == 0)
                        continue;

                    int maxElement = FindMaxDistanceElement(selectedLine, possiblePoints);
                    List<Point> allTangents = Tangent(polygon, possiblePoints[maxElement]);

                    UpdatePolygonAndLines(ref polygon, ref polyLines, allTangents, possiblePoints[maxElement]);
                }

                RemoveDuplicateLastPoint(ref polygon);
                outPoints = polygon;
            }
        }

        private List<Point> BuildInitialPolygon(List<Point> points)
        {
            int upper = 0, left = 0, right = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < points[left].X)
                    left = i;
                if (points[i].X > points[right].X)
                    right = i;
                if (points[i].Y > points[upper].Y)
                    upper = i;
            }

            return new List<Point>
            {
                points[upper],
                points[left],
                points[right]
            };
        }

        private List<Line> BuildInitialPolygonLines(List<Point> polygon)
        {
            return new List<Line>
            {
                new Line(polygon[0], polygon[1]),
                new Line(polygon[1], polygon[2]),
                new Line(polygon[2], polygon[0])
            };
        }

        private List<Point> GetPossiblePoints(List<Point> points, Line selectedLine)
        {
            return points.Where(p => HelperMethods.CheckTurn(selectedLine, p) == Enums.TurnType.Right).ToList();
        }

        private int FindMaxDistanceElement(Line selectedLine, List<Point> possiblePoints)
        {
            double maxDistance = 0;
            int maxElement = 0;

            for (int i = 0; i < possiblePoints.Count; i++)
            {
                double distance = DistanacePointLine(possiblePoints[i], selectedLine);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxElement = i;
                }
            }

            return maxElement;
        }

        public static List<Point> Tangent(List<Point> Polygon, Point SelectedPoint)
        {
            List<Point> twoTangents = new List<Point>();

            for (int i = 0; i < Polygon.Count; i++)
            {
                Point X = Polygon[i];
                Point PreX = Polygon[(i == 0) ? Polygon.Count - 1 : i - 1];
                Point NextX = Polygon[(i == Polygon.Count - 1) ? 0 : i + 1];

                if (orientation(PreX, X, SelectedPoint) != orientation(X, NextX, SelectedPoint))
                {
                    twoTangents.Add(X);
                }

                if (twoTangents.Count == 2)
                    break;
            }

            return twoTangents;
        }

        private void UpdatePolygonAndLines(ref List<Point> polygon, ref List<Line> polyLines, List<Point> allTangents, Point maxElementPoint)
        {
            if (orientation(allTangents[0], maxElementPoint, allTangents[1]) == 2)
            {
                UpdatePolygonAndLinesHelper(ref polygon, ref polyLines, allTangents[0], allTangents[1], maxElementPoint);
            }
            else
            {
                UpdatePolygonAndLinesHelper(ref polygon, ref polyLines, allTangents[1], allTangents[0], maxElementPoint);
            }
        }

        private void UpdatePolygonAndLinesHelper(ref List<Point> polygon, ref List<Line> polyLines, Point tangent1, Point tangent2, Point maxElementPoint)
        {
            int index1 = polygon.IndexOf(tangent1) + 1;
            polygon.Insert(index1, maxElementPoint);

            Line line1 = new Line(tangent1, maxElementPoint);
            Line line2 = new Line(maxElementPoint, tangent2);

            polyLines.Add(line1);
            polyLines.Add(line2);
        }

        private void RemoveDuplicateLastPoint(ref List<Point> polygon)
        {
            for (int i = 0; i < polygon.Count - 2; i++)
            {
                if (polygon[i] == polygon[polygon.Count - 1])
                {
                    polygon.RemoveAt(i);
                    break;
                }
            }
        }

        public static double DistanacePointLine(Point P, Line L)
        {
            Point l1 = L.Start;
            Point l2 = L.End;
            return Math.Abs((l2.X - l1.X) * (l1.Y - P.Y) - (l1.X - P.X) * (l2.Y - l1.Y)) / Math.Sqrt(Math.Pow(l2.X - l1.X, 2) + Math.Pow(l2.Y - l1.Y, 2));
        }

        public static int orientation(Point p1, Point p2, Point p3)
        {
            double exp = (p2.Y - p1.Y) * (p3.X - p2.X) - (p2.X - p1.X) * (p3.Y - p2.Y);
            if (exp == 0) return 0;  // collinear
            return (exp > 0) ? 1 : 2; // clockwise or counterclockwise
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
