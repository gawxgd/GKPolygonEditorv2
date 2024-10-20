using PolygonEditor.Constraints;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PolygonEditor.Geometry
{
    public class Polygon
    {
        public List<Vertex> vertices;

        public List<Edge> edges;
        private Canvas drawingCanvas;

        public Vertex? selectedVertex;
        public Edge? selectedEdge;
        public Vertex? movingVertex;

        public Vertex? movingContolPoint;
        public Edge? movingControlPointEdge;
        public Polygon(List<System.Drawing.Point> points, Canvas drawingCanvas)
        {
            this.vertices = new List<Vertex>();
            this.edges = new List<Edge>();
            this.drawingCanvas = drawingCanvas;
            foreach(var point in points)
            {
                var vertex = new Vertex(point);
                vertices.Add(vertex);   
            }
            for(int i = 0; i < vertices.Count; i++)
            {
                Vertex startVertex = vertices[i];
                Vertex endVertex = vertices[(i + 1) % vertices.Count];

                Edge edge = new Edge(startVertex, endVertex);
                edges.Add(edge);

                startVertex.OutEdge = edge;
                endVertex.InEdge = edge;
            }
            //foreach(var vertex in vertices)
            //{
            //    Debug.WriteLine($"{vertex.point} {vertex.InEdge.Start.point} {vertex.InEdge.End.point} {vertex.OutEdge.Start.point} {vertex.OutEdge.End.point}");
            //}
            //foreach (var edge in edges)
            //{
            //    Debug.WriteLine($"{edge.Start.point} {edge.End.point}");
            //}
            //foreach (var edge in edges)
            //{
            //    Debug.WriteLine($"{edge.Start.InEdge.Start.point} {edge.Start.InEdge.End.point}");
            //}
        }
        public Vertex? ChangeVertexPosition(Vertex oldPosition, Vertex newPosition)
        {
            int oldIndex = vertices.IndexOf(oldPosition);
            if(oldIndex != -1)
            {
               if(vertices.Any(vertices => vertices.point == newPosition.point))
                {
                    return oldPosition;
                }

                var edgeIn = oldPosition.InEdge;
                var edgeOut = oldPosition.OutEdge;
                var oldEdgeInIndex = edges.IndexOf(edgeIn);
                var oldEdgeOutIndex = edges.IndexOf(edgeOut);

                if (edgeIn != null)
                {
                    edgeIn.End.point = newPosition.point;
                }
                if (edgeOut != null)
                {
                    edgeOut.Start.point = newPosition.point;
                }
                edges[oldEdgeInIndex] = edgeIn;
                edges[oldEdgeOutIndex] = edgeOut;
                vertices[oldIndex].point = newPosition.point;

                return vertices[oldIndex];
            }
            else
            {
                MessageBox.Show("error vertex position");
                return null;
            }
        }
        public void DrawPolygon()
        {
            drawingCanvas.Children.Clear();
            for (int i = 0; i < vertices.Count; i++)
            {
                int nextIndex = (i + 1) % vertices.Count;
                Vertex start = vertices[i];
                Vertex end = vertices[nextIndex];
                Edge edge = new Edge(start, end);
                int edgeIndex = edges.IndexOf(edge);
                Edge currentEdge = edges[edgeIndex];
                if (currentEdge.isBezier)
                {
                    if (selectedEdge != null && selectedEdge.Equals(new Edge(start, end)))
                    {
                        Algorithm.DrawBezier(currentEdge, Brushes.Red, drawingCanvas);
                    }
                    else
                    {
                        Algorithm.DrawBezier(currentEdge, Brushes.Blue, drawingCanvas);
                    }
                }
                else
                {
                    if (selectedEdge != null && selectedEdge.Equals(new Edge(start, end)))
                    {
                        Algorithm.DrawBresenhamLine(start, end, Brushes.Red, drawingCanvas);
                    }
                    else
                    {
                        Algorithm.DrawBresenhamLine(start, end, Brushes.Black, drawingCanvas);
                    }
                }
                if(selectedVertex != null && selectedVertex.Equals(start))
                {
                    start.DrawVertex(start, Brushes.Red, drawingCanvas);
                }
                else
                {
                    start.DrawVertex(start, Brushes.Black, drawingCanvas);
                }
            }
            foreach (Edge edge in edges)
            {
                if (edge.Constraints.CheckIfEdgeHasConstraints())
                {
                    switch (edge.Constraints)
                    {
                        case HorizontalEdgeConstraints _:
                            DrawConstraintIcon(edge.Start, edge.End);
                            break;
                        case VerticalEdgeConstraints _:
                            DrawConstraintIcon(edge.Start, edge.End, true);
                            break;
                        case DistanceConstraint _:
                            DrawLengthLabel(edge);
                            break;
                    }
                }
            }
        }
        public void Clear()
        {
            drawingCanvas.Children.Clear();
            edges.Clear();
            vertices.Clear();
            selectedEdge = null;
            movingVertex = null;
            selectedVertex = null;
        }
        public void AddVertex()
        {
            if (selectedEdge != null)
            {
                Vertex start = selectedEdge.Start;
                Vertex end = selectedEdge.End;
                int startIndex = vertices.IndexOf(start);
                int endIndex = vertices.IndexOf(end);
                int index = vertices.IndexOf(start);
                int edgeIndex = edges.IndexOf(selectedEdge);
                edges.RemoveAt(edgeIndex);

                Vertex newVertex = new Vertex((start.X + end.X) / 2, (start.Y + end.Y) / 2);
                var inEdge = new Edge(start, newVertex);
                var outEdge = new Edge(newVertex, end);
                edges.Add(inEdge);
                edges.Add(outEdge);
                newVertex.InEdge = inEdge;
                newVertex.OutEdge = outEdge;
                vertices[startIndex].OutEdge = inEdge;
                vertices[endIndex].InEdge = outEdge;
                vertices.Insert(index + 1, newVertex);
                selectedEdge = null;
                DrawPolygon();
            }
        }
        public void RemoveVertex()
        {
            if(selectedVertex != null)
            {
                Vertex start = selectedVertex.InEdge.Start;
                Vertex end = selectedVertex.OutEdge.End;
                int startIndex = vertices.IndexOf(start);
                int endIndex = vertices.IndexOf(end);
                int index = vertices.IndexOf(selectedVertex);
                Edge inEdge = selectedVertex.InEdge;
                Edge outEdge = selectedVertex.OutEdge;
                edges.Remove(inEdge);
                edges.Remove(outEdge);
                Edge newEdge = new Edge(start, end);
                edges.Add(newEdge);
                vertices[startIndex].OutEdge = newEdge;
                vertices[endIndex].InEdge = newEdge;
                vertices.RemoveAt(index);
                DrawPolygon();

            }
        }
        private void DrawConstraintIcon(Vertex start, Vertex end, bool rotateArrow = false)
        {
            // Calculate the middle point of the line
            double middleX = (start.X + end.X) / 2;
            double middleY = (start.Y + end.Y) / 2;

            // Create the arrow icon image
            Image icon = new Image
            {
                Width = 40,  // Set the appropriate icon size
                Height = 40,
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/arrow.png"))

            };

            // Apply rotation if needed
            if (rotateArrow)
            {
                // Rotate the arrow by 90 degrees
                icon.RenderTransform = new RotateTransform(90);
                icon.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);  // Rotate around the center of the icon
                Canvas.SetLeft(icon, middleX + 10);
                Canvas.SetTop(icon, middleY - icon.Height / 2); //
            }
            else
            {
                // Position the icon below the line (shift down by 10 pixels)
                Canvas.SetLeft(icon, middleX - icon.Width / 2);
                Canvas.SetTop(icon, middleY + 10); // 10 pixels below the line
            }
            // Add the icon to the canvas
            drawingCanvas.Children.Add(icon);
        }
        public Polygon DeepCopy()
        {
            // Create new lists for the copied vertices and edges
            List<Vertex> newVertices = new List<Vertex>();
            List<Edge> newEdges = new List<Edge>();

            // Create a copy of each vertex
            foreach (var vertex in vertices)
            {
                var newVertex = new Vertex(vertex.X, vertex.Y);
                newVertices.Add(newVertex);
            }

            // Create a copy of each edge using the new vertices
            for (int i = 0; i < newVertices.Count; i++)
            {
                Vertex startVertex = newVertices[i];
                Vertex endVertex = newVertices[(i + 1) % newVertices.Count];

                Edge edge = new Edge(startVertex, endVertex);
                newEdges.Add(edge);

                startVertex.OutEdge = edge;
                endVertex.InEdge = edge;
            }

            // Create a new Polygon instance with the copied vertices and edges
            Polygon copiedPolygon = new Polygon(new List<System.Drawing.Point>(), drawingCanvas)
            {
                vertices = newVertices,
                edges = newEdges
            };

            return copiedPolygon;
        }
        public void DrawLengthLabel(Edge edge)
        {
            var start = edge.Start.point;
            var end = edge.End.point;

            // Calculate the midpoint
            var midX = (start.X + end.X) / 2;
            var midY = (start.Y + end.Y) / 2;

            // Create a TextBlock to display the length
            TextBlock lengthLabel = new TextBlock
            {
                Text = edge.Length.ToString(),  // Display the length
                Foreground = Brushes.Black,
                FontSize = 12
            };

            // Set position of the TextBlock
            Canvas.SetLeft(lengthLabel, midX);
            Canvas.SetTop(lengthLabel, midY);

            // Add the TextBlock to the canvas
            drawingCanvas.Children.Add(lengthLabel);
        }
        
    }
   
}
