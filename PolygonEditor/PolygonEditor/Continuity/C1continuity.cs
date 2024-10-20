using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonEditor.Continuity
{
    public class C1continuity : VertexContinuity
    {
        public override bool CheckIfHasContinuity(Vertex vertex)
        {
            return true;
        }

       public override bool PreserveContinuity(Vertex vertex)
        {
            return true;
        }
    }
}
