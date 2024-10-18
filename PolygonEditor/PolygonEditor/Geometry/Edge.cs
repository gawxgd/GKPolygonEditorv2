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
