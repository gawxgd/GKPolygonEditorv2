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
namespace PolygonEditor.Geometry
{
    public class Vertex
    {
        public Point point { get; set; }
        public Edge? InEdge { get; set; }
        public Edge? OutEdge { get; set; }

        public VertexContinuity continuityType;
        public Vertex(int x, int y)
        {
            point = new Point(x, y);
            continuityType = new NoneContinuity();

        }
        public Vertex(double x, double y)
        {
            point = new Point((int)x, (int)y);

        }
        public Vertex(Point point)
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
        }
    }
}
