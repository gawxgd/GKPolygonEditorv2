using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Constraints
{
    public class NoneConstraints : EdgeConstraints
    {
        public override bool CheckIfEdgeHasConstraints()
        {
            return false; // No constraints exist
        }

        public override bool PreserveConstraint(Edge edge, Vertex current, Polygon polygon)
        {
            return true;
        }

        // No constraints to remove
        public override void RemoveConstraints(Edge edge)
        {
            // Nothing to remove, no-op
        }

        public override string ToString()
        {
            return "No constraints";
        }
    }
}
