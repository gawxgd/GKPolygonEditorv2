using PolygonEditor.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace PolygonEditor
{
    public static class Algorithm
    {
        public static void DrawBresenhamLine(Vertex start, Vertex end, Brush color, Canvas drawingCanvas)
        {
            if(start.point == end.point)
            {
                return;
            }
            int x0 = (int)start.X, y0 = (int)start.Y;
            int x1 = (int)end.X, y1 = (int)end.Y;
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                var pixel = new System.Windows.Shapes.Rectangle { Width = 1, Height = 1, Fill = color };
                Canvas.SetLeft(pixel, x0);
                Canvas.SetTop(pixel, y0);
                drawingCanvas.Children.Add(pixel);

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
        public static double CalculateDistance(Vertex p1, Vertex p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
        public static void DrawDashedLine(Vertex start, Vertex end, Canvas canvas)
        {
            Line dashedLine = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = Brushes.Gray,
                StrokeDashArray = new DoubleCollection { 2, 2 } // Linia przerywana
            };
            canvas.Children.Add(dashedLine);
        }
        public static void DrawBezier(Edge edge, Brush color, Canvas drawingCanvas)
        {
            Algorithm.DrawDashedLine(edge.Start, edge.ControlPoint1, drawingCanvas);
            Algorithm.DrawDashedLine(edge.ControlPoint1, edge.ControlPoint2, drawingCanvas);
            Algorithm.DrawDashedLine(edge.ControlPoint2, edge.End, drawingCanvas);

            // Rysowanie samej krzywej Beziera
            Algorithm.DrawBezierCurve(edge.Start, edge.ControlPoint1, edge.ControlPoint2, edge.End,color, drawingCanvas);

            // Rysowanie punktów kontrolnych (małe okręgi)
            edge.ControlPoint1.DrawVertex(edge.ControlPoint1, Brushes.Blue, drawingCanvas,10);
            edge.ControlPoint2.DrawVertex(edge.ControlPoint2, Brushes.Blue, drawingCanvas,10);
        }
        private static void DrawBezierCurve(Vertex p0, Vertex p1, Vertex p2, Vertex p3,Brush color, Canvas drawingCanvas)
        {
            // Oblicz długość krawędzi (odległość od p0 do p3)
            double length = CalculateDistance(p0, p1) + CalculateDistance(p1,p2) + CalculateDistance(p2,p3);

            // Ustal minimalną liczbę kroków oraz czynnik wpływający na liczbę kroków
            int minSteps = 20;      // Minimalna liczba kroków, nawet dla bardzo krótkich krawędzi

            // Oblicz dynamiczną liczbę kroków na podstawie długości
            int steps = minSteps + (int)(length * 2);

            // Algorytm iteracyjny rysowania krzywej Beziera z dynamiczną liczbą kroków
            for (int i = 0; i < steps; i++)
            {
                double t = (double)i / steps;

                double x = Math.Pow(1 - t, 3) * p0.X +
                           3 * Math.Pow(1 - t, 2) * t * p1.X +
                           3 * (1 - t) * Math.Pow(t, 2) * p2.X +
                           Math.Pow(t, 3) * p3.X;

                double y = Math.Pow(1 - t, 3) * p0.Y +
                           3 * Math.Pow(1 - t, 2) * t * p1.Y +
                           3 * (1 - t) * Math.Pow(t, 2) * p2.Y +
                           Math.Pow(t, 3) * p3.Y;

                // Rysowanie punktów na krzywej
                var pixel = new System.Windows.Shapes.Rectangle { Width = 1, Height = 1, Fill = color };
                Canvas.SetLeft(pixel, x);
                Canvas.SetTop(pixel, y);
                drawingCanvas.Children.Add(pixel);
            }
        }


    }
}
