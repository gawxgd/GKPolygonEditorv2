using PolygonEditor.Constraints;
using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonEditor.Continuity
{
    public class G1continuity : VertexContinuity
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
            return Algorithm.CheckIfColinear(vertex, nonBezier, control);
        }

        public override bool CheckIfHasContinuity(Vertex vertex)
        {
            return true;
        }
        
        public override bool PreserveContinuity(Vertex vertex, Polygon polygon, bool isMovingControlPoint = false)
        {
            Edge prevEdge = vertex.InEdge;
            Edge nextEdge = vertex.OutEdge;
            if(isMovingControlPoint)
            { //inop
                if (prevEdge != null && nextEdge != null && nextEdge.isBezier && !prevEdge.isBezier)
                {
                    Vertex nonBezierVertex = prevEdge.Start;
                    Vertex bezierVertex = vertex;
                    Vertex controlPoint = nextEdge.ControlPoint1;

                    polygon.ChangeVertexPosition(prevEdge.Start,Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint));
                    return true;
                }
                if (prevEdge != null && nextEdge != null && prevEdge.isBezier && !nextEdge.isBezier)
                {
                    Vertex nonBezierVertex = nextEdge.End;
                    Vertex bezierVertex = vertex;
                    Vertex controlPoint = prevEdge.ControlPoint2;

                    polygon.ChangeVertexPosition(nextEdge.End, Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint));
                    return true;

                }
            }
            if (prevEdge != null && nextEdge != null && nextEdge.isBezier && !prevEdge.isBezier)
            {
                Vertex nonBezierVertex = prevEdge.Start;
                Vertex bezierVertex = vertex;
                Vertex controlPoint = nextEdge.ControlPoint1;
                if (prevEdge.Constraints is VerticalEdgeConstraints || prevEdge.Constraints is HorizontalEdgeConstraints)
                {
                    nextEdge.ControlPoint1 = Algorithm.ProjectVertex(controlPoint, nonBezierVertex, bezierVertex);
                }
                else
                {

                    nextEdge.ControlPoint1 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                }

                return true;
            }
            if (prevEdge != null && nextEdge != null && prevEdge.isBezier && !nextEdge.isBezier)
            {
                Vertex nonBezierVertex = nextEdge.End;
                Vertex bezierVertex = vertex;
                Vertex controlPoint = prevEdge.ControlPoint2;
                if (nextEdge.Constraints is VerticalEdgeConstraints || nextEdge.Constraints is HorizontalEdgeConstraints)
                {
                    // warning experimental
                    if(nextEdge.Constraints is HorizontalEdgeConstraints)
                    {
                        prevEdge.ControlPoint2 = Algorithm.ProjectVertex(controlPoint, new Vertex(nonBezierVertex.X,bezierVertex.Y), 
                            bezierVertex);
                    }
                    else if(nextEdge.Constraints is VerticalEdgeConstraints)
                    {
                        prevEdge.ControlPoint2 = Algorithm.ProjectVertex(controlPoint, new Vertex(bezierVertex.X,nonBezierVertex.Y),
                            bezierVertex);
                    }
                }
                else
                {
                    prevEdge.ControlPoint2 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                }
                return true;

            }
            return false;
        }

    }
}
