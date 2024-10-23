using PolygonEditor.Constraints;
using PolygonEditor.Geometry;

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
            if(prevEdge == null || nextEdge == null)
                return false;
            if(isMovingControlPoint)
            { //inop
                if (nextEdge.isBezier && !prevEdge.isBezier)
                {
                    Vertex nonBezierVertex = prevEdge.Start;
                    Vertex bezierVertex = vertex;
                    Vertex controlPoint = nextEdge.ControlPoint1;

                    polygon.ChangeVertexPosition(prevEdge.Start,Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint));
                    return true;
                }
                if (prevEdge.isBezier && !nextEdge.isBezier)
                {
                    Vertex nonBezierVertex = nextEdge.End;
                    Vertex bezierVertex = vertex;
                    Vertex controlPoint = prevEdge.ControlPoint2;

                    polygon.ChangeVertexPosition(nextEdge.End, Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint));
                    return true;

                }
            }
            if (nextEdge.isBezier && !prevEdge.isBezier)
            {
                Vertex nonBezierVertex = prevEdge.Start;
                Vertex bezierVertex = vertex;
                Vertex controlPoint = nextEdge.ControlPoint1;
                if (prevEdge.Constraints is VerticalEdgeConstraints || prevEdge.Constraints is HorizontalEdgeConstraints)
                {
                    if (prevEdge.Constraints is HorizontalEdgeConstraints)
                    {
                        nextEdge.ControlPoint1 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                        nextEdge.ControlPoint1 = new Vertex(nextEdge.ControlPoint1.X, bezierVertex.Y);
                    }
                    else
                    {
                        nextEdge.ControlPoint1 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                        nextEdge.ControlPoint1 = new Vertex(bezierVertex.X, nextEdge.ControlPoint1.Y);
                    }
                }
                else
                {

                    nextEdge.ControlPoint1 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                }

                return true;
            }
            if (prevEdge.isBezier && !nextEdge.isBezier)
            {
                Vertex nonBezierVertex = nextEdge.End;
                Vertex bezierVertex = vertex;
                Vertex controlPoint = prevEdge.ControlPoint2;
                if (nextEdge.Constraints is VerticalEdgeConstraints || nextEdge.Constraints is HorizontalEdgeConstraints)
                {
                    if (nextEdge.Constraints is HorizontalEdgeConstraints)
                    {
                        prevEdge.ControlPoint2 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                        prevEdge.ControlPoint2 = new Vertex(prevEdge.ControlPoint2.X, bezierVertex.Y);
                    }
                    else
                    {
                        prevEdge.ControlPoint2 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                        prevEdge.ControlPoint2 = new Vertex(bezierVertex.X, prevEdge.ControlPoint2.Y);
                    }
                }
                else
                {
                    prevEdge.ControlPoint2 = Algorithm.CalculateG1(nonBezierVertex, bezierVertex, controlPoint);
                }
                return true;

            }
            if(prevEdge.isBezier && nextEdge.isBezier)
            {
                double directionX = nextEdge.ControlPoint1.X - vertex.X;
                double directionY = nextEdge.ControlPoint1.Y - vertex.Y;

                double length = Math.Sqrt(directionX * directionX + directionY * directionY);
                directionX /= length;
                directionY /= length;

                double distancePrev = Math.Sqrt(
                    (prevEdge.ControlPoint2.X - vertex.X) * (prevEdge.ControlPoint2.X - vertex.X) +
                    (prevEdge.ControlPoint2.Y - vertex.Y) * (prevEdge.ControlPoint2.Y - vertex.Y)
                );

                double newPrevX = vertex.X - directionX * distancePrev;
                double newPrevY = vertex.Y - directionY * distancePrev;

                prevEdge.ControlPoint2 = new Vertex((int)newPrevX, (int)newPrevY);

                return true;
            }
            return false;
        }

    }
}
