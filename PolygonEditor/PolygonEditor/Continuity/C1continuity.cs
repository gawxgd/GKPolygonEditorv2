using PolygonEditor.Constraints;
using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonEditor.Continuity
{
    public class C1continuity : VertexContinuity
    {
        public override bool CheckIfContinuityIsSatisfied(Vertex vertex, Edge edge)
        {
            Vertex control;
            Vertex nonBezier;
            if (vertex.Equals(edge.Start))
            {
                control = edge.ControlPoint1;
                nonBezier = vertex.InEdge.GetOtherEnd(vertex);
            }
            else
            {
                control = edge.ControlPoint2;
                nonBezier = vertex.OutEdge.GetOtherEnd(vertex);
            }
            var controlBezierDistance = Algorithm.CalculateDistance(control, vertex);
            var bezierNonBezierDistance = Algorithm.CalculateDistance(nonBezier, vertex);
            bool isProperDistance = controlBezierDistance * 3 == bezierNonBezierDistance;
            return Algorithm.CheckIfColinear(vertex, nonBezier, control) && isProperDistance;
        }

        public override bool CheckIfHasContinuity(Vertex vertex)
        {
            return true;
        }
        public override bool PreserveContinuity(Vertex vertex, Polygon polygon, bool isMovingControlPoint = false)
        {
            Edge prevEdge = vertex.InEdge;
            Edge nextEdge = vertex.OutEdge;
            if (isMovingControlPoint)
            { 
                if (prevEdge != null && nextEdge != null && nextEdge.isBezier && !prevEdge.isBezier)
                {
                    Vertex nonBezierVertex = prevEdge.Start;
                    Vertex bezierVertex = vertex;
                    Vertex controlPoint = nextEdge.ControlPoint1;
                    
                    double bezierToNonBezierDistance = Algorithm.CalculateDistance(bezierVertex, controlPoint);
                    double newDistance = bezierToNonBezierDistance * 3;
                    var newPos = Algorithm.CalculateG1(controlPoint, bezierVertex, nonBezierVertex);
                    newPos = Algorithm.SetVertexDistance(bezierVertex, newPos, newDistance);
                    polygon.ChangeVertexPosition(prevEdge.Start, newPos);
                    return true;
                }
                if (prevEdge != null && nextEdge != null && prevEdge.isBezier && !nextEdge.isBezier)
                {
                    Vertex nonBezierVertex = nextEdge.End;
                    Vertex bezierVertex = vertex;
                    Vertex controlPoint = prevEdge.ControlPoint2;

                    double bezierToNonBezierDistance = Algorithm.CalculateDistance(bezierVertex, controlPoint);
                    double newDistance = bezierToNonBezierDistance * 3;
                    var newPos = Algorithm.CalculateG1(controlPoint, bezierVertex, nonBezierVertex);
                    newPos = Algorithm.SetVertexDistance(bezierVertex, newPos, newDistance);

                    polygon.ChangeVertexPosition(nextEdge.End, newPos);
                    return true;

                }
            }
            if (prevEdge != null && nextEdge != null && nextEdge.isBezier && !prevEdge.isBezier)
            {
                Vertex nonBezierVertex = prevEdge.Start;
                Vertex bezierVertex = vertex;
                Vertex controlPoint = nextEdge.ControlPoint1;

                double bezierToNonBezierDistance = Algorithm.CalculateDistance(bezierVertex, nonBezierVertex);
                double newDistance = bezierToNonBezierDistance / 3;

                if (prevEdge.Constraints is VerticalEdgeConstraints || prevEdge.Constraints is HorizontalEdgeConstraints)
                {
                    if (prevEdge.Constraints is HorizontalEdgeConstraints)
                    {
                        nextEdge.ControlPoint1 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                        nextEdge.ControlPoint1 = new Vertex(nextEdge.ControlPoint1.X, bezierVertex.Y);
                        nextEdge.ControlPoint1 = Algorithm.SetVertexDistance(bezierVertex, nextEdge.ControlPoint1, newDistance);
                    }
                    else
                    {
                        nextEdge.ControlPoint1 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                        nextEdge.ControlPoint1 = new Vertex(bezierVertex.X, nextEdge.ControlPoint1.Y);
                        nextEdge.ControlPoint1 = Algorithm.SetVertexDistance(bezierVertex, nextEdge.ControlPoint1, newDistance);
                    }
                }
                else
                {
                    var newControlPointPosition = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                    nextEdge.ControlPoint1 = Algorithm.SetVertexDistance(bezierVertex, newControlPointPosition, newDistance);
                }

                return true;
            }
            if (prevEdge != null && nextEdge != null && prevEdge.isBezier && !nextEdge.isBezier)
            {
                Vertex nonBezierVertex = nextEdge.End;
                Vertex bezierVertex = vertex;
                Vertex controlPoint = prevEdge.ControlPoint2;

                double bezierToNonBezierDistance = Algorithm.CalculateDistance(bezierVertex, nonBezierVertex);
                double newDistance = bezierToNonBezierDistance / 3;

                if (nextEdge.Constraints is VerticalEdgeConstraints || nextEdge.Constraints is HorizontalEdgeConstraints)
                {
                    if (nextEdge.Constraints is HorizontalEdgeConstraints)
                    {
                        prevEdge.ControlPoint2 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                        prevEdge.ControlPoint2 = new Vertex(prevEdge.ControlPoint2.X, bezierVertex.Y);
                        prevEdge.ControlPoint2 = Algorithm.SetVertexDistance(bezierVertex, prevEdge.ControlPoint2, newDistance);
                    }
                    else
                    {
                        prevEdge.ControlPoint2 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                        prevEdge.ControlPoint2 = new Vertex(bezierVertex.X, prevEdge.ControlPoint2.Y);
                        prevEdge.ControlPoint2 = Algorithm.SetVertexDistance(bezierVertex, prevEdge.ControlPoint2, newDistance);
                    }
                }
                else
                {
                   Vertex newControlPointPosition = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                   prevEdge.ControlPoint2 = Algorithm.SetVertexDistance(bezierVertex, newControlPointPosition, newDistance);
                }
                return true;

            }
            if (prevEdge.isBezier && nextEdge.isBezier)
            {
                double directionX = nextEdge.ControlPoint1.X - prevEdge.ControlPoint2.X;
                double directionY = nextEdge.ControlPoint1.Y - prevEdge.ControlPoint2.Y;

                double length = Math.Sqrt(directionX * directionX + directionY * directionY);
                directionX /= length;
                directionY /= length;

                double halfDistance = length / 2;


                var pos = new System.Drawing.Point((int)(vertex.X - directionX * halfDistance), (int)(vertex.Y - directionY * halfDistance));
                prevEdge.ControlPoint2.point = pos;

                var pos2 = new System.Drawing.Point((int)(vertex.X + directionX * halfDistance), (int)(vertex.Y + directionY * halfDistance));
                nextEdge.ControlPoint1.point = pos2;

                return true;
            }
            return false;
        }
    }


    
}
