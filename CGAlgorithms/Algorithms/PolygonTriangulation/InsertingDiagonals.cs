using System;
using System.Collections.Generic;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    public class InsertingDiagonals : Algorithm
    {
        // Main entry point for the algorithm
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (polygons.Count == 0)
                return;

            // Extract vertices from the first polygon
            List<Point> polygonVertices = ExtractPolygonVertices(polygons[0]);

            // Ensure counterclockwise order of vertices
            polygonVertices.Reverse();

            // Insert diagonals and update the result lines
            outLines = InsertDiagonals(polygonVertices);
        }

        // Recursive function to insert diagonals into the polygon
        private List<Line> InsertDiagonals(List<Point> polygonVertices)
        {
            if (polygonVertices.Count > 3)
            {
                List<Line> resultLines = new List<Line>();

                // Find a convex vertex
                int convexVertexIndex = FindConvexVertexIndex(polygonVertices);

                // If no convex vertex is found, return an empty list
                if (convexVertexIndex == -1)
                    return new List<Line>();

                // Get the indices of the previous and next vertices
                int prevIndex = GetPreviousIndex(polygonVertices, convexVertexIndex);
                int nextIndex = GetNextIndex(polygonVertices, convexVertexIndex);

                // Find the vertex with the maximum distance inside the triangle formed by the convex vertex and its neighbors
                int maxVertexIndex = FindMaxDistanceVertexIndex(polygonVertices, prevIndex, nextIndex, convexVertexIndex);

                // Add the diagonal to the result lines
                if (maxVertexIndex == -1)
                    resultLines.Add(new Line(polygonVertices[prevIndex], polygonVertices[nextIndex]));
                else
                    resultLines.Add(new Line(polygonVertices[convexVertexIndex], polygonVertices[maxVertexIndex]));

                // Recursively process the two parts of the polygon split by the diagonal
                List<Point> part1 = ExtractVertices(polygonVertices, maxVertexIndex == -1 ? nextIndex : convexVertexIndex, maxVertexIndex == -1 ? prevIndex : maxVertexIndex);
                List<Point> part2 = ExtractVertices(polygonVertices, maxVertexIndex == -1 ? prevIndex : maxVertexIndex, maxVertexIndex == -1 ? nextIndex : convexVertexIndex);

                // Recursively insert diagonals into the two parts
                resultLines.AddRange(InsertDiagonals(part1));
                resultLines.AddRange(InsertDiagonals(part2));

                return resultLines;
            }

            // Return an empty list if the polygon has three or fewer vertices
            return new List<Line>();
        }

        // Find the vertex inside the triangle formed by the convex vertex and its neighbors with the maximum distance
        private int FindMaxDistanceVertexIndex(List<Point> polygonVertices, int prevIndex, int nextIndex, int convexVertexIndex)
        {
            double maxDistance = -1e6;
            int maxIndex = -1;

            for (int i = 0; i < polygonVertices.Count; ++i)
            {
                // Check if the vertex is inside the triangle
                if (HelperMethods.PointInTriangle(polygonVertices[i], polygonVertices[convexVertexIndex], polygonVertices[prevIndex], polygonVertices[nextIndex]) == Enums.PointInPolygon.Inside)
                {
                    // Calculate the distance to the line formed by the neighbors
                    double distance = HelperMethods.LinePointDistance(new Line(polygonVertices[prevIndex], polygonVertices[nextIndex]), polygonVertices[i]);

                    // Update the maximum distance and index if needed
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxIndex = i;
                    }
                }
            }

            return maxIndex;
        }

        // Find the index of the first convex vertex in the polygon
        private int FindConvexVertexIndex(List<Point> polygonVertices)
        {
            for (int i = 0; i < polygonVertices.Count; ++i)
                if (IsConvex(polygonVertices, i))
                    return i;

            return -1;
        }

        // Check if a vertex is convex by examining the turn type
        private bool IsConvex(List<Point> polygonVertices, int index)
        {
            int prevIndex = GetPreviousIndex(polygonVertices, index);
            int nextIndex = GetNextIndex(polygonVertices, index);

            return HelperMethods.CheckTurn(new Line(polygonVertices[prevIndex], polygonVertices[nextIndex]), polygonVertices[index]) == Enums.TurnType.Right;
        }


        // Extract the vertices from a given polygon
        private List<Point> ExtractVertices(List<Point> polygonVertices, int start, int end)
        {
            List<Point> extractedVertices = new List<Point>();

            // Extract vertices between the start and end indices
            for (int i = end; i != start; i = (i + 1) % polygonVertices.Count)
                extractedVertices.Add(polygonVertices[i]);

            // Add the starting vertex to complete the loop
            extractedVertices.Add(polygonVertices[start]);
            return extractedVertices;
        }

        // Get the index of the previous vertex in a cyclic manner
        private int GetPreviousIndex(List<Point> polygonVertices, int currentIndex)
        {
            return (currentIndex - 1 + polygonVertices.Count) % polygonVertices.Count;
        }

        // Get the index of the next vertex in a cyclic manner
        private int GetNextIndex(List<Point> polygonVertices, int currentIndex)
        {
            return (currentIndex + 1) % polygonVertices.Count;
        }

        // Extract the vertices from a given polygon
        private List<Point> ExtractPolygonVertices(Polygon polygon)
        {
            List<Point> vertices = new List<Point>();
            for (int i = 0; i < polygon.lines.Count; ++i)
                vertices.Add(polygon.lines[i].Start);
            return vertices;
        }

        // Return the name of the algorithm
        public override string ToString()
        {
            return "Inserting Diagonals";
        }
    }
}
