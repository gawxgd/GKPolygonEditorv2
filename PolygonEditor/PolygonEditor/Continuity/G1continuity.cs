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
        public override bool PreserveContinuity(Vertex vertex)
        {
            Edge prevEdge = vertex.InEdge;
            Edge nextEdge = vertex.OutEdge;

            // Ensure the previous edge is a straight line and the next edge is a Bezier curve
            if (prevEdge != null && nextEdge != null && nextEdge.isBezier && !prevEdge.isBezier)
            {
                // Get the non-Bezier vertex (from the straight line) and Bezier control point
                Vertex nonBezierVertex = prevEdge.Start; // Assume previous edge is a straight line
                Vertex bezierVertex = vertex;
                Vertex controlPoint = nextEdge.ControlPoint1; // Assume using ControlPoint1 for continuity

                // Calculate the vector from the non-Bezier vertex to the Bezier vertex
                Vector nonBezierToBezier = new Vector(bezierVertex.point.X - nonBezierVertex.point.X,
                                                      bezierVertex.point.Y - nonBezierVertex.point.Y);

                // Calculate the distance from Bezier vertex to the control point (to preserve its distance)
                double distance = Math.Sqrt(Math.Pow(controlPoint.point.X - bezierVertex.point.X, 2) +
                                            Math.Pow(controlPoint.point.Y - bezierVertex.point.Y, 2));

                // Normalize the vector to get the direction
                nonBezierToBezier.Normalize();

                // Adjust the control point so that it's collinear with the non-Bezier and Bezier vertex
                nextEdge.ControlPoint1 = new Vertex(
                    (int)(bezierVertex.point.X + nonBezierToBezier.X * distance),
                    (int)(bezierVertex.point.Y + nonBezierToBezier.Y * distance)
                );

                return true;  // Continuity preserved
            }
            if (prevEdge != null && nextEdge != null && prevEdge.isBezier && !nextEdge.isBezier)
            {
                Vertex nonBezierVertex = nextEdge.End;
                Vertex bezierVertex = vertex;
                Vertex controlPoint = prevEdge.ControlPoint2;
                Vector nonBezierToBezier = new Vector(bezierVertex.point.X - nonBezierVertex.point.X,
                                                      bezierVertex.point.Y - nonBezierVertex.point.Y);

                // Calculate the distance from Bezier vertex to the control point (to preserve its distance)
                double distance = Math.Sqrt(Math.Pow(controlPoint.point.X - bezierVertex.point.X, 2) +
                                            Math.Pow(controlPoint.point.Y - bezierVertex.point.Y, 2));

                // Normalize the vector to get the direction
                nonBezierToBezier.Normalize();

                // Adjust the control point so that it's collinear with the non-Bezier and Bezier vertex
                prevEdge.ControlPoint2 = new Vertex(
                    (int)(bezierVertex.point.X + nonBezierToBezier.X * distance),
                    (int)(bezierVertex.point.Y + nonBezierToBezier.Y * distance)
                );

            }

                return false;  // Continuity couldn't be preserved
        }

    }
}
