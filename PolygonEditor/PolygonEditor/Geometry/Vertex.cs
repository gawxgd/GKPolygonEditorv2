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
                Width = size,  
                Height = size, 
                Fill = color
            };
            Canvas.SetLeft(vertex, position.X - 7);  
            Canvas.SetTop(vertex, position.Y - 7);   
            drawingCanvas.Children.Add(vertex);
            DrawContinuityLabel(position, size, drawingCanvas);
        }
        private void DrawContinuityLabel(Vertex position, int size, Canvas drawingCanvas)
        {
            if (continuityType is not NoneContinuity)
            {
                
                string labelText = continuityType.GetType().Name switch
                {
                    "G1continuity" => "G1",
                    "G0continuity" => "G0",
                    "C1continuity" => "C1",
                    _ => ""
                };

                TextBlock label = new TextBlock
                {
                    Text = labelText,
                    Foreground = Brushes.Black,
                    FontSize = 12,
                    Background = Brushes.White 
                };

                Canvas.SetLeft(label, position.X + size / 2);  
                Canvas.SetTop(label, position.Y + size / 2);   
                drawingCanvas.Children.Add(label);
            }
        }
    }
}
