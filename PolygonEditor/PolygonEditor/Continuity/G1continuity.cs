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
                    nextEdge.ControlPoint1 = Algorithm.ProjectVertex(controlPoint, nonBezierVertex, bezierVertex);
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
            if(prevEdge.isBezier && nextEdge.isBezier)
            {
                // Step 1: Calculate the direction vector between nextEdge.ControlPoint1 and prevEdge.ControlPoint2
                double directionX = nextEdge.ControlPoint1.X - prevEdge.ControlPoint2.X;
                double directionY = nextEdge.ControlPoint1.Y - prevEdge.ControlPoint2.Y;

                // Step 2: Normalize the direction vector
                double length = Math.Sqrt(directionX * directionX + directionY * directionY);
                directionX /= length;
                directionY /= length;

                // Step 3: Calculate half the distance between the two control points
                double halfDistance = length / 2;

                // Step 4: Reposition prevEdge.ControlPoint2 and nextEdge.ControlPoint1

                // Move prevEdge.ControlPoint2 towards the vertex along the negative direction
                var pos = new System.Drawing.Point((int)(vertex.X - directionX * halfDistance), (int)(vertex.Y - directionY * halfDistance));
                prevEdge.ControlPoint2.point = pos;

                var pos2 = new System.Drawing.Point((int)(vertex.X + directionX * halfDistance), (int)(vertex.Y + directionY * halfDistance));
                // Move nextEdge.ControlPoint1 away from the vertex along the positive direction
                nextEdge.ControlPoint1.point = pos2;
                

                return true;
            }
            return false;
        }

    }
}
