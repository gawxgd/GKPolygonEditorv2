using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Continuity
{
    internal class G0continuity : VertexContinuity
    {
        public override bool CheckIfHasContinuity(Vertex vertex)
        {
            return false;
        }

        public override bool PreserveContinuity(Vertex vertex,Polygon polygon, bool isMovingControlPoint = false)
        {
            return true;
        }
    }
}
