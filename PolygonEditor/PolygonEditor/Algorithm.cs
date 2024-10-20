using PolygonEditor.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public static bool IsPointNearLine(Vertex start, Vertex end, System.Windows.Point point, double threshold = 5)
        {
            double lineLength = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)); // euclidian norm line length

            if (lineLength < 0.1) // Avoid division by near-zero line length
                return false;

            double distance = Math.Abs((end.Y - start.Y) * point.X - (end.X - start.X) * point.Y + end.X * start.Y - end.Y * start.X)
                              / lineLength; // edge point distance using perpendicular formula

            if (distance > threshold) // Check if the point is within the threshold distance from the line
                return false;

            // Check if the projection of point onto the line segment lies within the segment
            double dotProduct = ((point.X - start.X) * (end.X - start.X)) + ((point.Y - start.Y) * (end.Y - start.Y)); // projection of the point edge start vector on the edge
            double projectedLengthSquared = dotProduct * dotProduct / (lineLength * lineLength);

            return projectedLengthSquared <= lineLength * lineLength;
        }
        public static bool IsPointNearBezierCurve(Edge edge, System.Windows.Point mousePosition, double threshold = 10)
        {
            var p0 = edge.Start;
            var p1 = edge.ControlPoint1;
            var p2 = edge.ControlPoint2;
            var p3 = edge.End;
            int steps = 20; // Number of steps to evaluate points along the Bezier curve
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;

                // Calculate the Bezier point for this t
                double x = Math.Pow(1 - t, 3) * p0.X +
                           3 * Math.Pow(1 - t, 2) * t * p1.X +
                           3 * (1 - t) * Math.Pow(t, 2) * p2.X +
                           Math.Pow(t, 3) * p3.X;

                double y = Math.Pow(1 - t, 3) * p0.Y +
                           3 * Math.Pow(1 - t, 2) * t * p1.Y +
                           3 * (1 - t) * Math.Pow(t, 2) * p2.Y +
                           Math.Pow(t, 3) * p3.Y;

                // Check if the mouse position is within the threshold distance of the point
                if (Math.Sqrt(Math.Pow(mousePosition.X - x, 2) + Math.Pow(mousePosition.Y - y, 2)) < threshold)
                {
                    return true; // Point is near the Bezier curve
                }
            }
            return false; // Point is not near the Bezier curve
        }
        public static (Vertex, Vertex) CalculateControlPointPosition(Edge edge)
        {
            double midX = (edge.Start.X + edge.End.X) / 2;
            double midY = (edge.Start.Y + edge.End.Y) / 2;

            // Vector from start to end
            double deltaX = edge.End.X - edge.Start.X;
            double deltaY = edge.End.Y - edge.Start.Y;

            // Perpendicular vector (scaled for better control point placement)
            double perpX = -deltaY * 0.3; // Adjust 0.3 to change curvature intensity
            double perpY = deltaX * 0.3;

            // Place control points perpendicular to the line at the midpoint
            return (new Vertex(new System.Drawing.Point((int)(midX + perpX), (int)(midY + perpY))), new Vertex(new System.Drawing.Point((int)(midX - perpX), (int)(midY - perpY))));
        }
        public static (Vertex, Vertex) CalculateControlPointRelativePosition(Edge edge, Vertex oldPosition, Vertex newPosition)
        {
            // Determine if the moving vertex is the start or end vertex of the edge
            bool isMovingStartVertex = edge.Start.Equals(oldPosition);
            bool isMovingEndVertex = edge.End.Equals(oldPosition);

            System.Windows.Point nPosition = new System.Windows.Point(newPosition.X, newPosition.Y);
            System.Windows.Point oPosition = new System.Windows.Point(oldPosition.X, oldPosition.Y);
            Vector offset = nPosition - oPosition;

            // Adjust control points relative to the moving vertex
            Vertex newControlPoint1 = edge.ControlPoint1;
            Vertex newControlPoint2 = edge.ControlPoint2;

            if (isMovingStartVertex)
            {
                // Adjust ControlPoint1 relative to StartPoint (if the StartPoint is moving)
                newControlPoint1 = new Vertex(edge.ControlPoint1.point.X + (int)offset.X, edge.ControlPoint1.point.Y + (int)offset.Y);
            }

            if (isMovingEndVertex)
            {
                // Adjust ControlPoint2 relative to EndPoint (if the EndPoint is moving)
                newControlPoint2 = new Vertex(edge.ControlPoint2.point.X + (int)offset.X, edge.ControlPoint2.point.Y + (int)offset.Y);
            }

            return (newControlPoint1, newControlPoint2);
        }
        public static void ChangeControlPointCositionWithoutContinuity(Geometry.Polygon polygon, System.Drawing.Point newPosition)
        {
            Vertex oldPosition = new Vertex(polygon.movingVertex.point);
            foreach (Edge edge in polygon.edges)
            {
                if (edge.isBezier)
                {
                    Debug.WriteLine(edge.ControlPoint1.point);
                    var controlPoints = Algorithm.CalculateControlPointRelativePosition(edge, oldPosition, new Vertex(newPosition));
                    int index = polygon.edges.IndexOf(edge);
                    polygon.edges[index].ControlPoint1 = controlPoints.Item1;
                    polygon.edges[index].ControlPoint2 = controlPoints.Item2;
                    Debug.WriteLine(controlPoints.Item1.point);

                }
            }
        }


    }
}
