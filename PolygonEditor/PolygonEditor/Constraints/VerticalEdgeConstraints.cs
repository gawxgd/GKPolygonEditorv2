using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonEditor.Constraints
{
    public class VerticalEdgeConstraints : EdgeConstraints
    {
        public override bool CheckIfEdgeHasConstraints()
        {
            return true;
        }
        public override bool PreserveConstraint(Edge edge, Vertex current, Polygon polygon)
        {
            var secondVertex = edge.GetOtherEnd(current);
            if (secondVertex == null)
            {
                MessageBox.Show("secondVertex null");
                return false;
            }
            polygon.ChangeVertexPosition(secondVertex, new Vertex(current.X, secondVertex.Y));
            return true;
        }
        public override void RemoveConstraints(Edge edge)
        {
            base.RemoveConstraints(edge);
        }
        public override string ToString()
        {
            return "vertical constraints";
        }
    }
}
