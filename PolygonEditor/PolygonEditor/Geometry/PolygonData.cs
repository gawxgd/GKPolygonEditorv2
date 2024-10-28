using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolygonEditor.Geometry
{
    public class PolygonData
    {
        public List<VertexData> Vertices { get; set; }
        public List<EdgeData> Edges { get; set; }
    }

    public class VertexData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string ContinuityType { get; set; }
        public int? InEdgeIndex { get; set; } 
        public int? OutEdgeIndex { get; set; }
    }

    public class EdgeData
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public bool IsBezier { get; set; }
        public PointData ControlPoint1 { get; set; }
        public PointData ControlPoint2 { get; set; }
        public string ConstraintType { get; set; }
    }

    public class PointData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public PointData(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}

