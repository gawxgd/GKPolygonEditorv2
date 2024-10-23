using PolygonEditor.Continuity;
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
            if (start.point == end.point)
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

            Algorithm.DrawBezierCurve(edge.Start, edge.ControlPoint1, edge.ControlPoint2, edge.End, color, drawingCanvas);

            edge.ControlPoint1.DrawVertex(edge.ControlPoint1, Brushes.Blue, drawingCanvas, 10);
            edge.ControlPoint2.DrawVertex(edge.ControlPoint2, Brushes.Blue, drawingCanvas, 10);
        }
        private static void DrawBezierCurve(Vertex p0, Vertex p1, Vertex p2, Vertex p3, Brush color, Canvas drawingCanvas)
        {
            double length = CalculateDistance(p0, p1) + CalculateDistance(p1, p2) + CalculateDistance(p2, p3);

            int minSteps = 20;      

            int steps = minSteps + (int)(length * 2);

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

                var pixel = new System.Windows.Shapes.Rectangle { Width = 1, Height = 1, Fill = color };
                Canvas.SetLeft(pixel, x);
                Canvas.SetTop(pixel, y);
                drawingCanvas.Children.Add(pixel);
            }
        }
        public static bool IsPointNearLine(Vertex start, Vertex end, System.Windows.Point point, double threshold = 5)
        {
            double lineLength = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)); 

            if (lineLength < 0.1) 
                return false;

            double distance = Math.Abs((end.Y - start.Y) * point.X - (end.X - start.X) * point.Y + end.X * start.Y - end.Y * start.X)
                              / lineLength; 

            if (distance > threshold) 
                return false;

            double dotProduct = ((point.X - start.X) * (end.X - start.X)) + ((point.Y - start.Y) * (end.Y - start.Y)); 
            double projectedLengthSquared = dotProduct * dotProduct / (lineLength * lineLength);

            return projectedLengthSquared <= lineLength * lineLength;
        }
        public static bool IsPointNearBezierCurve(Edge edge, System.Windows.Point mousePosition, double threshold = 10)
        {
            var p0 = edge.Start;
            var p1 = edge.ControlPoint1;
            var p2 = edge.ControlPoint2;
            var p3 = edge.End;
            int steps = 20; 
            for (int i = 0; i <= steps; i++)
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

                if (Math.Sqrt(Math.Pow(mousePosition.X - x, 2) + Math.Pow(mousePosition.Y - y, 2)) < threshold)
                {
                    return true; 
                }
            }
            return false; 
        }
        public static (Vertex, Vertex) CalculateControlPointPosition(Edge edge)
        {
            double midX = (edge.Start.X + edge.End.X) / 2;
            double midY = (edge.Start.Y + edge.End.Y) / 2;

            double deltaX = edge.End.X - edge.Start.X;
            double deltaY = edge.End.Y - edge.Start.Y;

            double perpX = -deltaY * 0.3; 
            double perpY = deltaX * 0.3;

            return (new Vertex(new System.Drawing.Point((int)(midX + perpX), (int)(midY + perpY))), new Vertex(new System.Drawing.Point((int)(midX - perpX), (int)(midY - perpY))));
        }
        public static (Vertex, Vertex) CalculateControlPointRelativePosition(Edge edge, Vertex oldPosition, Vertex newPosition)
        {
            bool isMovingStartVertex = edge.Start.Equals(oldPosition);
            bool isMovingEndVertex = edge.End.Equals(oldPosition);

            System.Windows.Point nPosition = new System.Windows.Point(newPosition.X, newPosition.Y);
            System.Windows.Point oPosition = new System.Windows.Point(oldPosition.X, oldPosition.Y);
            Vector offset = nPosition - oPosition;

            Vertex newControlPoint1 = edge.ControlPoint1;
            Vertex newControlPoint2 = edge.ControlPoint2;

            if (isMovingStartVertex)
            {
                newControlPoint1 = new Vertex(edge.ControlPoint1.point.X + (int)offset.X, edge.ControlPoint1.point.Y + (int)offset.Y);
            }

            if (isMovingEndVertex)
            {
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
        public static (Vertex old, Vertex newV) PreserveControlPoint(Edge e)
        {
            Vertex controlPoint = e.ControlPoint1; 

            Vector bezierToControlPoint = new Vector(
                controlPoint.point.X - e.Start.X,
                controlPoint.point.Y - e.Start.Y
            );

            bezierToControlPoint.Normalize();

            double distance = Math.Sqrt(Math.Pow(controlPoint.point.X - e.Start.X, 2) +
                                        Math.Pow(controlPoint.point.Y - e.Start.Y, 2));

            Vertex newNonBezierVertex = new Vertex(
                (int)(e.Start.X - bezierToControlPoint.X * distance),
                (int)(e.Start.Y - bezierToControlPoint.Y * distance)
            );

            return (e.Start, newNonBezierVertex); 
        }
        public static Vertex CalculateG1(Vertex v1, Vertex v2, Vertex v3)
        {
            Vector v = new Vector(v2.X - v1.X, v2.Y - v1.Y);
            double distance = CalculateDistance(v3, v2);
            v.Normalize();
            return new Vertex(
                    (int)(v2.X + v.X * distance),
                    (int)(v2.Y + v.Y * distance));
        }
        public static Vertex CalculateG1WithDistance(Vertex v1, Vertex v2, Vertex v3, double distance)
        {
            Vector v3ToV2 = new Vector(v2.X - v3.X, v2.Y - v3.Y);

            v3ToV2.Normalize();

            Vector scaledDirection = new Vector(v3ToV2.X * distance, v3ToV2.Y * distance);

            return new Vertex(
                v1.X + scaledDirection.X,
                v1.Y + scaledDirection.Y
            );
        }


        public static Vertex ProjectVertexControl(Vertex point, Vertex linePoint1, Vertex linePoint2)
        {
            if(linePoint1.continuityType is G1continuity)
            {
                var dist1 = CalculateDistance(linePoint1, linePoint2);
                var dist2 = CalculateDistance(linePoint1, point);
                if (dist2 == 0)
                    return linePoint2;
                var dist = dist1 / dist2;
                return ProjectVertex(linePoint2, linePoint1, point,dist);
            }
            else if(linePoint1.continuityType is C1continuity)
            {
                return ProjectVertex(linePoint2, linePoint1, point,3);
            }
            return point;
        }
        public static Vertex ProjectVertex(Vertex p1, Vertex point, Vertex p2,double distance )
        {
            var vector = new Vector(point.point.X - p2.point.X, point.point.Y - p2.point.Y);
            return new Vertex(point.point.X + distance * vector.X, point.point.Y + distance * vector.Y);
        }
        public static bool CheckIfColinear(Vertex point1, Vertex point2, Vertex point3)
        {
            double crossProduct = (point2.X - point1.X) * (point3.Y - point1.Y)
                                - (point2.Y - point1.Y) * (point3.X - point1.X);

            return Math.Abs(crossProduct) < 1e-10;
        }
        public static Vertex SetVertexDistance(Vertex bezierVertex, Vertex nonBezierVertex, double newDistance)
        {
            double dx = nonBezierVertex.X - bezierVertex.X;
            double dy = nonBezierVertex.Y - bezierVertex.Y;

            double currentDistance = Math.Sqrt(dx * dx + dy * dy);

            if (currentDistance < 1e-10)
            {
                return new Vertex(nonBezierVertex.X, nonBezierVertex.Y);
            }

            double unitX = dx / currentDistance;
            double unitY = dy / currentDistance;

            double newX = bezierVertex.X + unitX * newDistance;
            double newY = bezierVertex.Y + unitY * newDistance;

            return new Vertex(newX, newY);
        }
        public static (System.Drawing.Point, System.Drawing.Point) MoveControlPointBetweenBezierEdgesC1(Vertex movingControlPoint, Edge prevEdge, Edge nextEdge)
        {
            double directionX = nextEdge.ControlPoint1.X - prevEdge.ControlPoint2.X;
            double directionY = nextEdge.ControlPoint1.Y - prevEdge.ControlPoint2.Y;
            var bezierVertex = prevEdge.End;
            double length = Math.Sqrt(directionX * directionX + directionY * directionY);
            if (length != 0)
            {
                directionX /= length;
                directionY /= length;
            }

            double halfDistance = length / 2;

            var newControlPoint2Pos = new System.Drawing.Point(
                (int)(bezierVertex.X - directionX * halfDistance),
                (int)(bezierVertex.Y - directionY * halfDistance));

            var newControlPoint1Pos = new System.Drawing.Point(
                (int)(bezierVertex.X + directionX * halfDistance),
                (int)(bezierVertex.Y + directionY * halfDistance));

            return (newControlPoint1Pos, newControlPoint2Pos);
        }
        public static (System.Drawing.Point, System.Drawing.Point) MoveControlPointBetweenBezierEdgesG1control2(Vertex movingControlPoint, Edge prevEdge, Edge nextEdge)
        {
            var bezierVertex = prevEdge.End;

            double directionX = nextEdge.ControlPoint1.X - bezierVertex.X;
            double directionY = nextEdge.ControlPoint1.Y - bezierVertex.Y;

            double length = Math.Sqrt(directionX * directionX + directionY * directionY);
            if (length != 0)
            {
                directionX /= length;
                directionY /= length;
            }

            double distancePrev = Math.Sqrt(
                (prevEdge.ControlPoint2.X - bezierVertex.X) * (prevEdge.ControlPoint2.X - bezierVertex.X) +
                (prevEdge.ControlPoint2.Y - bezierVertex.Y) * (prevEdge.ControlPoint2.Y - bezierVertex.Y)
            );

            var newControlPoint2Pos = new System.Drawing.Point(
                (int)(bezierVertex.X - directionX * distancePrev),
                (int)(bezierVertex.Y - directionY * distancePrev));

            var newControlPoint1Pos = nextEdge.ControlPoint1.point;

            return (newControlPoint1Pos, newControlPoint2Pos);
        }
        public static(System.Drawing.Point,System.Drawing.Point) MoveControlPointBetweenBezierEdgesG1control1(Vertex movingControlPoint,Edge prevEdge, Edge nextEdge)
        {
            var bezierVertex = prevEdge.End;

            double directionX = prevEdge.ControlPoint2.X - bezierVertex.X;
            double directionY = prevEdge.ControlPoint2.Y - bezierVertex.Y;

            double length = Math.Sqrt(directionX * directionX + directionY * directionY);
            if (length != 0)
            {
                directionX /= length;
                directionY /= length;
            }

            double distanceNext = Math.Sqrt(
                (nextEdge.ControlPoint1.X - bezierVertex.X) * (nextEdge.ControlPoint1.X - bezierVertex.X) +
                (nextEdge.ControlPoint1.Y - bezierVertex.Y) * (nextEdge.ControlPoint1.Y - bezierVertex.Y)
            );

            var newControlPoint1Pos = new System.Drawing.Point(
                (int)(bezierVertex.X - directionX * distanceNext),
                (int)(bezierVertex.Y - directionY * distanceNext));

            var newControlPoint2Pos = prevEdge.ControlPoint2.point;

            return (newControlPoint1Pos, newControlPoint2Pos);

        }
        public static System.Drawing.Point ProjectC1(System.Drawing.Point p1, System.Drawing.Point p2, float distProp = 3.0f)
        {
            var p1Vector = new System.Numerics.Vector2(p1.X, p1.Y);
            var p2Vector = new System.Numerics.Vector2(p2.X, p2.Y);

            var newP = p1Vector + (p1Vector - p2Vector) / distProp;

            if (Math.Abs(newP.X) > int.MaxValue / 2 ||
                Math.Abs(newP.Y) > int.MaxValue / 2)
            {
                newP = p1Vector;
            }

            return new System.Drawing.Point((int)newP.X, (int)newP.Y);
        }
    }
}
