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
        public override bool CheckIfConstraintsAreSatisfied(Edge edge)
        {
            return true; 
        }

        public override bool CheckIfEdgeHasConstraints()
        {
            return false;
        }

        public override bool PreserveConstraint(Edge edge, Vertex current, Polygon polygon)
        {
            return true;
        }

        public override void RemoveConstraints(Edge edge)
        {
        }

        public override string ToString()
        {
            return "No constraints";
        }
    }
}
