using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolygonEditor.Geometry;
namespace PolygonEditor.Continuity
{
    public abstract class VertexContinuity
    {
        public abstract bool CheckIfHasContinuity(Vertex vertex);
        public abstract bool PreserveContinuity(Vertex vertex);
    }
}
