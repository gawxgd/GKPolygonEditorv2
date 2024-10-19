using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Continuity
{
    public class NoneContinuity : VertexContinuity
    {
        public override bool CheckIfHasContinuity(Vertex vertex)
        {
            return false;
        }

        public override bool PreserveContinuity(Vertex vertex)
        {
            return true;
        }
    }
}
