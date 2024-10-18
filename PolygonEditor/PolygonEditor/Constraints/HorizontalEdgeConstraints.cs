using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Constraints
{
    public class HorizontalEdgeConstraints : EdgeConstraints
    {
        public override bool CheckIfEdgeHasConstraints()
        {
            return true;
        }

        public override bool PreserveConstraint(Edge edge, Vertex current,Polygon polygon)
        {
            var secondVertex = edge.GetOtherEnd(current);
            if (secondVertex == null) return false;
            polygon.ChangeVertexPosition(secondVertex, new Vertex(secondVertex.X, current.Y));
            return true;
        }

        public override void RemoveConstraints(Edge edge)
        {
            base.RemoveConstraints(edge);
        }

        public override string ToString()
        {
            return "horizontal constraints";
        }
    }
}
