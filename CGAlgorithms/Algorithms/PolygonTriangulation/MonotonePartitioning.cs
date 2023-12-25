/*using CGUtilities;
using CGUtilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    public class MonotonePartitioning : Algorithm
    {
        public class Vertex
        {
            public double X { get; set; }
            public double Y { get; set; }

            public Vertex(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            foreach (var polygon in polygons)
            {
                // Convert CGUtilities.Polygon to List<Vertex>
                List<Vertex> polygonVertices = new List<Vertex>();
                CGUtilities.DataStructures.Set<Vertex> linesSet = new Set<Vertex>();
                foreach (var line in polygon.lines)
                {
                    linesSet.Add(new Vertex(line.Start.X, line.Start.Y));
                    linesSet.Add(new Vertex(line.End.X, line.End.Y));
                }
                polygonVertices = linesSet.ToList();
                // Apply Monotone Partitioning to the polygon
                List<List<Vertex>> monotoneChains = PartitionPolygon(polygonVertices);

                // Convert the resulting monotone chains back to CGUtilities.Polygon objects
                foreach (var chain in monotoneChains)
                {
                    List<Line> monotoneLines = new List<Line>();

                    // Convert the list of vertices to a list of lines
                    for (int i = 0; i < chain.Count - 1; i++)
                    {
                        Line line = new Line(new Point(chain[i].X, chain[i].Y), new Point(chain[i + 1].X, chain[i + 1].Y));
                        monotoneLines.Add(line);
                    }

                    //// Create a new Polygon instance and add the lines to its lines property
                    //CGUtilities.Polygon monotonePolygon = new CGUtilities.Polygon(monotoneLines);

                    //// Add the resulting monotone polygon to the output list
                    //outPolygons.Add(monotonePolygon);
                    outLines.AddRange(monotoneLines);
                }
            }
        }

        private static List<List<Vertex>> PartitionPolygon(List<Vertex> polygon)
        {
            // Sort vertices by x-coordinate
            polygon.Sort((a, b) => a.X.CompareTo(b.X));

            // Initialize data structures
            Stack<Vertex> stack = new Stack<Vertex>();
            List<List<Vertex>> monotoneChains = new List<List<Vertex>>();

            // Traverse vertices
            foreach (Vertex vertex in polygon)
            {
                while (stack.Count >= 2 && !vertexOnSameChain(stack.Peek(), stack.ElementAt(1), vertex))
                {
                    monotoneChains.Add(new List<Vertex>(stack));
                    stack.Clear();
                    stack.Push(vertex);
                }

                stack.Push(vertex);
            }

            // Add the last monotone chain
            monotoneChains.Add(new List<Vertex>(stack));

            return monotoneChains;
        }

        private static bool vertexOnSameChain(Vertex v1, Vertex v2, Vertex v3)
        {
            // Calculate the cross product of vectors (v2 - v1) and (v3 - v2)
            double crossProduct = (v2.X - v1.X) * (v3.Y - v2.Y) - (v2.Y - v1.Y) * (v3.X - v2.X);

            // If the cross product is non-negative, the vertices make a left turn (on the same chain)
            // If the cross product is non-positive, the vertices make a right turn (on different chains)
            return crossProduct >= 0;
        }

        public override string ToString()
        {
            return "Monotone Partitioning";
        }
    }
}
*/