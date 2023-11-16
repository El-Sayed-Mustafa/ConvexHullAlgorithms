using CGUtilities;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            points = points.OrderBy(point => point.X).ThenBy(point => point.Y).ToList();
            outPoints = Divide(points);


        }
        public List<Point> Divide(List<Point> divisiblePoints)
        {

            if (divisiblePoints.Count <= 1)
            {
                return divisiblePoints;
            }

            List<Point> leftPoints = new List<Point>();
            List<Point> rightPoints = new List<Point>();

            for (int i = 0; i < divisiblePoints.Count / 2; i++)
            {
                leftPoints.Add(divisiblePoints[i]);
            }

            for (int i = divisiblePoints.Count / 2; i < divisiblePoints.Count; i++)
            {
                rightPoints.Add(divisiblePoints[i]);
            }

            List<Point> convexHull = Combine(Divide(leftPoints), Divide(rightPoints));

            return convexHull;
        }
        public List<Point> Combine(List<Point> leftPoints, List<Point> rightPoints)
        {

            int leftMostIndex = 0, rightMostIndex = 0, Lcount = leftPoints.Count, Rcount = rightPoints.Count;

            //detecting leftmost and righmost points of divided points
            for (int i = 1; i < Lcount; i++)
            {
                if (leftPoints[i].X > leftPoints[rightMostIndex].X ||
                    leftPoints[i].X == leftPoints[rightMostIndex].X && leftPoints[i].Y > leftPoints[rightMostIndex].Y)

                    rightMostIndex = i;
            }

            for (int i = 1; i < Rcount; i++)
            {
                if (rightPoints[i].X < rightPoints[leftMostIndex].X ||
                    rightPoints[i].X == rightPoints[leftMostIndex].X && rightPoints[i].Y < rightPoints[leftMostIndex].Y)

                    leftMostIndex = i;
            }

            //upper tangent setup
            int upTanA = rightMostIndex;
            int upTanB = leftMostIndex;
            bool found = false;

            //changing the tangent points until both of them make a line doesn't cross the polygon 
            while (!found)
            {
                found = true;

                while (CGUtilities.HelperMethods.CheckTurn(new Line(rightPoints[upTanB].X, rightPoints[upTanB].Y, leftPoints[upTanA].X, leftPoints[upTanA].Y),
                          leftPoints[(upTanA + 1) % Lcount]) == Enums.TurnType.Right)
                {
                    upTanA = (upTanA + 1) % Lcount;
                    found = false;
                }
                //handling special case if the 3 points marked are colinear
                if ((CGUtilities.HelperMethods.CheckTurn(new Line(rightPoints[upTanB].X, rightPoints[upTanB].Y, leftPoints[upTanA].X, leftPoints[upTanA].Y),
                             leftPoints[(upTanA + 1) % Lcount]) == Enums.TurnType.Colinear))
                {
                    upTanA = (upTanA + 1) % Lcount;
                }

                while (CGUtilities.HelperMethods.CheckTurn(new Line(leftPoints[upTanA].X, leftPoints[upTanA].Y, rightPoints[upTanB].X, rightPoints[upTanB].Y),
                    rightPoints[(Rcount + upTanB - 1) % Rcount]) == Enums.TurnType.Left)
                {
                    upTanB = (Rcount + upTanB - 1) % Rcount;
                    found = false;

                }

                if ((CGUtilities.HelperMethods.CheckTurn(new Line(leftPoints[upTanA].X, leftPoints[upTanA].Y, rightPoints[upTanB].X, rightPoints[upTanB].Y),
                    rightPoints[(upTanB - 1 + Rcount) % Rcount]) == Enums.TurnType.Colinear))
                {
                    upTanB = (upTanB - 1 + Rcount) % Rcount;
                }
            }

            //lower tangent setup
            int lowTanA = rightMostIndex;
            int lowTanB = leftMostIndex;
            found = false;

            //same for lower tangent
            while (!found)
            {
                found = true;
                while (CGUtilities.HelperMethods.CheckTurn(new Line(rightPoints[lowTanB].X, rightPoints[lowTanB].Y, leftPoints[lowTanA].X, leftPoints[lowTanA].Y)
                    , leftPoints[(lowTanA + Lcount - 1) % Lcount]) == Enums.TurnType.Left)
                {
                    lowTanA = (lowTanA + Lcount - 1) % Lcount;
                    found = false;
                }

                if ((CGUtilities.HelperMethods.CheckTurn(new Line(rightPoints[lowTanB].X, rightPoints[lowTanB].Y, leftPoints[lowTanA].X, leftPoints[lowTanA].Y),
                                leftPoints[(lowTanA + Lcount - 1) % Lcount]) == Enums.TurnType.Colinear))
                {
                    lowTanA = (lowTanA + Lcount - 1) % Lcount;
                }

                while (CGUtilities.HelperMethods.CheckTurn(new Line(leftPoints[lowTanA].X, leftPoints[lowTanA].Y, rightPoints[lowTanB].X, rightPoints[lowTanB].Y),
                    rightPoints[(lowTanB + 1) % Rcount]) == Enums.TurnType.Right)
                {
                    lowTanB = (lowTanB + 1) % Rcount;
                    found = false;

                }

                if ((CGUtilities.HelperMethods.CheckTurn(new Line(leftPoints[lowTanA].X, leftPoints[lowTanA].Y, rightPoints[lowTanB].X, rightPoints[lowTanB].Y),
                    rightPoints[(lowTanB + 1) % Rcount]) == Enums.TurnType.Colinear))
                {
                    lowTanB = (lowTanB + 1) % Rcount;
                }
            }

            List<Point> convexHull = new List<Point>();

            //brute force algorithm to gather points to form convex hull 

            convexHull.Add(leftPoints[upTanA]);

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



        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}