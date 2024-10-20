using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonEditor.Continuity
{
    public class G1continuity : VertexContinuity
    {
        public override bool CheckIfHasContinuity(Vertex vertex)
        {
            return true;
        }
        
        public override bool PreserveContinuity(Vertex vertex, Polygon polygon, bool isMovingControlPoint = false)
        {
            Edge prevEdge = vertex.InEdge;
            Edge nextEdge = vertex.OutEdge;
            if(isMovingControlPoint)
            {
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

                nextEdge.ControlPoint1 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                return true;
            }
            if (prevEdge != null && nextEdge != null && prevEdge.isBezier && !nextEdge.isBezier)
            {
                Vertex nonBezierVertex = nextEdge.End;
                Vertex bezierVertex = vertex;
                Vertex controlPoint = prevEdge.ControlPoint2;

                prevEdge.ControlPoint2 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                return true;

            }
            return false;
        }

    }
}
