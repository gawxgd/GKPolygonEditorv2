using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonEditor.Constraints
{
    public class DistanceConstraint : EdgeConstraints
    {
        public override bool CheckIfConstraintsAreSatisfied(Edge edge)
        {
            var vertex1 = edge.Start;
            var vertex2 = edge.End;

            if (Algorithm.CalculateDistance(vertex1,vertex2) == edge.Length)
            {
                return true;
            }
            else
            {
                return false; 
            }
        }

        public override bool CheckIfEdgeHasConstraints()
        {
            return true;
        }

        public override bool PreserveConstraint(Edge edge, Vertex current, Polygon polygon)
        {
            var secondVertex = edge.GetOtherEnd(current);
            if (secondVertex == null) return false;
            if (edge.Length == null)
            {
                MessageBox.Show(" edge length is null");
                return false;
            }
            var secondVertexVector = new Vector2(secondVertex.X, secondVertex.Y);
            var currentVertexVector = new Vector2(current.X, current.Y);
            var dir = secondVertexVector - currentVertexVector;
            var a = currentVertexVector + Vector2.Normalize(dir) * edge.Length;
           
            polygon.ChangeVertexPosition(secondVertex, new Vertex((int)a.Value.X, (int)a.Value.Y));
            return true;
        }
        public override void RemoveConstraints(Edge edge)
        {
            base.RemoveConstraints(edge);
        }

        public override string ToString()
        {
            return "distance constraints";
        }
    }
}
