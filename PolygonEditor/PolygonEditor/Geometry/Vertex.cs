using System;
using System.CodeDom;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PolygonEditor.Geometry;
using System.Drawing;
using PolygonEditor.Continuity;
using System.Windows;
namespace PolygonEditor.Geometry
{
    public class Vertex
    {
        public System.Drawing.Point point { get; set; }
        public Edge? InEdge { get; set; }
        public Edge? OutEdge { get; set; }

        public VertexContinuity continuityType = new NoneContinuity();
        public Vertex(int x, int y)
        {
            point = new System.Drawing.Point(x, y);

        }
        public Vertex(double x, double y)
        {
            point = new System.Drawing.Point((int)x, (int)y);

        }
        public Vertex(System.Drawing.Point point)
        {
            this.point = point;

        }
        public void SetDefaultContinuity()
        {
            continuityType = new G1continuity();
        }

        public Edge? GetOtherEdge(Edge edge)
        {
            return edge.Equals(InEdge) ? OutEdge : InEdge;
        }
        public int X => point.X;
        public int Y => point.Y;
        public bool Equals(Vertex? other)
        {
            if (other is null) return false;
            return X == other.X && Y == other.Y;
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as Vertex);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        public void DrawVertex(Vertex position, Brush color, Canvas drawingCanvas, int size = 14)
        {
            Ellipse vertex = new Ellipse
            {
                Width = size,  // Zwiększamy szerokość
                Height = size, // Zwiększamy wysokość
                Fill = color
            };
            Canvas.SetLeft(vertex, position.X - 7);  // Aktualizujemy pozycję, aby elipsa była wycentrowana
            Canvas.SetTop(vertex, position.Y - 7);   // Aktualizujemy pozycję, aby elipsa była wycentrowana
            drawingCanvas.Children.Add(vertex);
            DrawContinuityLabel(position, size, drawingCanvas); 
        }
        private void DrawContinuityLabel(Vertex position, int size, Canvas drawingCanvas)
        {
            if (continuityType is not NoneContinuity)
            {
                // Determine label text based on the type of continuity
                string labelText = continuityType.GetType().Name switch
                {
                    "G1continuity" => "G1",
                    "G0continuity" => "G0",
                    _ => ""
                };

                // Create a TextBlock to display the label
                TextBlock label = new TextBlock
                {
                    Text = labelText,
                    Foreground = Brushes.Black,
                    FontSize = 12,
                    Background = Brushes.White // Optional: To make the text more readable
                };

                // Set the position of the label slightly offset from the vertex
                Canvas.SetLeft(label, position.X + size / 2);  // Offset to the right of the vertex
                Canvas.SetTop(label, position.Y + size / 2);   // Offset below the vertex
                drawingCanvas.Children.Add(label);
            }
        }
    }
}
