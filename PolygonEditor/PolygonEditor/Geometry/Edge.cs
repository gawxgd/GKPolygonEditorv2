using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolygonEditor.Constraints;
namespace PolygonEditor.Geometry
{
    public class Edge
    {
        public Vertex Start { get; set; }
        public Vertex End { get; set; }
        public EdgeConstraints Constraints { get; set; }
        public float? Length;
        public bool isBezier = false;
        public Vertex ControlPoint1 { get; set; }  
        public Vertex ControlPoint2 { get; set; }  
        public Edge(Vertex start, Vertex end, EdgeConstraints constraints = null )
        {
            Start = start;
            End = end;
            Constraints = constraints ?? new NoneConstraints();
        }
        public Vertex GetOtherEnd(Vertex vertex)
        {
            return vertex.Equals(Start) ? End : Start;
        }
        public bool Equals(Edge? other)
        {
            if (other is null) return false;
            return (Start.Equals(other.Start) && End.Equals(other.End)) ||
                   (Start.Equals(other.End) && End.Equals(other.Start));
        }
        public bool CheckIfHasBezierSegmentNeighbor(Vertex o)
        {
            if(o.Equals(Start))
            {
                return o.InEdge.isBezier;
            }
            else if(o.Equals(End))
            {
                return o.OutEdge.isBezier;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Edge);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }
    }
}
