using PolygonEditor.Geometry;
using System;
using System.CodeDom;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PolygonEditor.Geometry;
namespace PolygonEditor.Constraints
{
    public abstract class EdgeConstraints
    {
        public abstract bool CheckIfEdgeHasConstraints();
        public abstract bool CheckIfConstraintsAreSatisfied(Edge edge);
        public abstract bool PreserveConstraint(Edge edge, Vertex current, Geometry.Polygon polygon);
        public virtual void RemoveConstraints(Edge edge)
        {
            edge.Constraints = new NoneConstraints();
        }

        public override string ToString()
        {
            return "edge constraints";
        }

    }
}
