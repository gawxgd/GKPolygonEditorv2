using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PolygonEditor.Geometry;
using System.Drawing;
using System.Windows.Media;
using PolygonEditor.Constraints;
using System.Diagnostics;
using System.Windows.Shell;
using PolygonEditor.Continuity;
using Microsoft.Win32;

namespace PolygonEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PolygonEditor.Geometry.Polygon? polygon;
        private bool CustomAlgorithm = true;

        private bool isDrawingMode = false;
        private bool isDraggingPolygon = false;
        private System.Windows.Point? initialMousePosition = null;


        private List<System.Drawing.Point> tempPoints = new List<System.Drawing.Point>();
        public MainWindow()
        {
            InitializeComponent();
            NewPolygon();
        }
        private void NewPolygon()
        {
            var vertex1 = new System.Drawing.Point(100, 100);
            var vertex2 = new System.Drawing.Point(300, 100);
            var vertex3 = new System.Drawing.Point(300, 300);
            var vertex4 = new System.Drawing.Point(100, 300);
            var points = new List<System.Drawing.Point>();
            points.Add(vertex1);
            points.Add(vertex2);
            points.Add(vertex3);
            points.Add(vertex4);
            polygon = new PolygonEditor.Geometry.Polygon(points, DrawingCanvas);
            polygon.edges[3].Constraints = new VerticalEdgeConstraints();
            polygon.edges[1].SetBezier(polygon);
            polygon.edges[2].SetBezier(polygon);
            polygon.vertices[2].continuityType = new C1continuity();
            polygon.vertices[3].continuityType = new C1continuity();
            polygon.DrawPolygon();
        }

        private void MakeHorizontal_Click(object sender, RoutedEventArgs e)
        {
            if(polygon.selectedEdge != null)
            {
                if (polygon.selectedEdge.Constraints.CheckIfEdgeHasConstraints() == false)
                {
                    var start = polygon.selectedEdge.Start;
                    var end = polygon.selectedEdge.End;
                    Edge startEdge = start.GetOtherEdge(polygon.selectedEdge);
                    Edge endEdge = end.GetOtherEdge(polygon.selectedEdge);
                    if (startEdge == null || endEdge == null)
                    {
                        foreach (var edge in polygon.edges)
                        {
                            Debug.WriteLine($"{edge.Start.InEdge.Start.point} {edge.Start.InEdge.End.point}");
                        }
                    }
                    if (startEdge.Constraints is HorizontalEdgeConstraints || endEdge.Constraints is HorizontalEdgeConstraints)
                    {
                        MessageBox.Show("The neighboring edges already have horizontal constraints.");
                        return;
                    }
                    polygon.selectedEdge.Constraints = new HorizontalEdgeConstraints();
                    var vertex = polygon.ChangeVertexPosition(polygon.selectedEdge.End, new Vertex(polygon.selectedEdge.End.X, polygon.selectedEdge.Start.Y));
                    EnsureConstraints(polygon.selectedEdge.Start);

                    polygon.DrawPolygon();

                }
                else
                    MessageBox.Show($"The edge already have verticalEdgeConstraints ");
            }
        }
        
        private void MakeVertical_Click(object sender, RoutedEventArgs e)
        {
            if (polygon.selectedEdge != null)
            {
                if (polygon.selectedEdge.Constraints.CheckIfEdgeHasConstraints() == false)
                {
                    var start = polygon.selectedEdge.Start;
                    var end = polygon.selectedEdge.End;
                    Edge startEdge = start.GetOtherEdge(polygon.selectedEdge);
                    Edge endEdge = end.GetOtherEdge(polygon.selectedEdge);
                    if (startEdge == null || endEdge == null)
                    {
                        foreach (var edge in polygon.edges)
                        {
                            Debug.WriteLine($"{edge.Start.InEdge.Start.point} {edge.Start.InEdge.End.point}");
                        }
                    }
                    if (startEdge.Constraints is VerticalEdgeConstraints || endEdge.Constraints is VerticalEdgeConstraints)
                    {
                        MessageBox.Show("The neighboring edges already have horizontal constraints.");
                        return;
                    }
                    polygon.selectedEdge.Constraints = new VerticalEdgeConstraints();
                    polygon.ChangeVertexPosition(polygon.selectedEdge.Start, new Vertex(polygon.selectedEdge.End.X, polygon.selectedEdge.Start.Y));
                    EnsureConstraints(polygon.selectedEdge.Start);
                    polygon.DrawPolygon();

                }
                else
                    MessageBox.Show($"The edge already have verticalEdgeConstraints ");
            }
        }

        private void RemoveConstraint_Click(object sender, RoutedEventArgs e)
        {
            if(polygon.selectedEdge != null)
            {
                polygon.selectedEdge.Constraints.RemoveConstraints(polygon.selectedEdge);
                polygon.DrawPolygon();
            }
        }

        private void AddVertex_Click(object sender, RoutedEventArgs e)
        {
            polygon.AddVertex();
        }
        private void drawingMode_MouseLeftButtonDown(System.Drawing.Point mousePosition)
        {
            tempPoints.Add(mousePosition);
            var newVertex = new Vertex(mousePosition);
            newVertex.DrawVertex(newVertex, Brushes.Black, DrawingCanvas);

            if (tempPoints.Count > 1)
            {
                Algorithm.DrawBresenhamLine(new Vertex(tempPoints[tempPoints.Count - 2]), newVertex, Brushes.Black, DrawingCanvas);
            }
        }
        private void normal_MouseLeftButtonDown(Vertex mousePosition)
        {
            polygon.movingVertex = null;
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                if (Math.Sqrt(Math.Pow(polygon.vertices[i].X - mousePosition.X, 2) + Math.Pow(polygon.vertices[i].Y - mousePosition.Y, 2)) < 10)
                {
                    polygon.movingVertex = polygon.vertices[i];
                    polygon.DrawPolygon();
                    return; 
                }
            }
            foreach(Edge e in polygon.edges)
            {
                if (e.ControlPoint1 != null && e.ControlPoint2 != null)
                {
                    if (Algorithm.CalculateDistance(e.ControlPoint1, mousePosition) < 10)
                    {
                        polygon.movingContolPoint = e.ControlPoint1;
                        polygon.movingControlPointEdge = e;
                        polygon.DrawPolygon();
                        return; 
                    }
                    else if (Algorithm.CalculateDistance(e.ControlPoint2, mousePosition) < 10)
                    {
                        polygon.movingContolPoint = e.ControlPoint2;
                        polygon.movingControlPointEdge = e;
                        polygon.DrawPolygon();
                        return; 
                    }
                }
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mousePosition = e.GetPosition(DrawingCanvas);
            System.Drawing.Point drawingPoint = new System.Drawing.Point((int)mousePosition.X, (int)mousePosition.Y);
            if (isDrawingMode)
            {
                drawingMode_MouseLeftButtonDown(drawingPoint);
            }
            else if (isDraggingPolygon) 
            {
                initialMousePosition = mousePosition;
            }
            else
            {
                normal_MouseLeftButtonDown(new Vertex(drawingPoint));
            }
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDraggingPolygon)
            {
                initialMousePosition = null;
            }
        }
        private void drawingMode_MouseRightButtonDown()
        {
            isDrawingMode = false;

            if (tempPoints.Count >= 3)
            {
                polygon = new PolygonEditor.Geometry.Polygon(tempPoints, DrawingCanvas);
                polygon.DrawPolygon();
            }
            else
            {
                MessageBox.Show("A polygon must have at least 3 vertices.");
            }

            tempPoints.Clear();
        }
        private void normal_MouseRightButtonDown(System.Windows.Point mousePosition)
        {
            polygon.selectedVertex = null;

            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                if (Math.Sqrt(Math.Pow(polygon.vertices[i].X - mousePosition.X, 2) + Math.Pow(polygon.vertices[i].Y - mousePosition.Y, 2)) < 10)
                {
                    polygon.selectedVertex = polygon.vertices[i];
                    polygon.selectedEdge = null;
                    polygon.DrawPolygon();
                    return; 
                }
            }

            polygon.selectedEdge = null;

            foreach (var edge in polygon.edges)
            {
                if (edge.isBezier)
                {
                    if (Algorithm.IsPointNearBezierCurve(edge, mousePosition))
                    {
                        polygon.selectedEdge = edge;
                        polygon.DrawPolygon();
                        return;
                    }
                }
                
                else if (Algorithm.IsPointNearLine(edge.Start, edge.End, mousePosition))
                {
                    polygon.selectedEdge = edge;  
                    polygon.DrawPolygon();
                    return; 
                }
            }
        }
        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingMode)
            {
                drawingMode_MouseRightButtonDown();
            }
            else
            {
                System.Windows.Point mousePosition = e.GetPosition(DrawingCanvas);
                normal_MouseRightButtonDown(mousePosition);
                if(polygon.selectedVertex != null)
                {
                    ContextMenu vertexMenu = (ContextMenu)this.Resources["VertexContextMenu"];
                    vertexMenu.PlacementTarget = DrawingCanvas;
                    vertexMenu.IsOpen = true;
                }
                else if (polygon.selectedEdge != null)
                {
                    ContextMenu contextMenu = (ContextMenu)this.Resources["EdgeContextMenu"];
                    contextMenu.PlacementTarget = DrawingCanvas;
                    contextMenu.IsOpen = true;
                }
            }
        }
        private void DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            isDrawingMode = true;
            tempPoints.Clear();
            if (polygon != null)
                polygon.Clear();
            MessageBox.Show("Click on the canvas to add vertices. Right-click to finish drawing.");
        }
        private void RemoveVertex_Click(object sender, RoutedEventArgs e)
        {
            polygon.RemoveVertex();
        }

        private void TogglePolygonDragging_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            isDraggingPolygon = !isDraggingPolygon;
            if (isDraggingPolygon)
            {
                button.Content = "Polygon dragging enabled";
            }
            else
            {
                button.Content = "Polygon dragging disabled";
            }
            MessageBox.Show(isDraggingPolygon ? "Polygon dragging enabled" : "Polygon dragging disabled");
        }

        
        private bool PreserveConstraintsLoop(Vertex vertex, Func<Vertex, Edge?> direction)
        {

            Vertex current = vertex;
            while (true)
            {
                var edge = direction(current);
                if (edge == null)
                {
                    MessageBox.Show("error null edge");
                    return false;
                }
                var otherEnd = edge.GetOtherEnd(current);
                if (edge.isBezier)
                {
                    if (current.continuityType.CheckIfContinuityIsSatisfied(vertex,edge) == false)
                    {
                        current.continuityType.PreserveContinuity(current,polygon);
                    }
                    break;
                }
                else 
                {
                    if (edge.Constraints.CheckIfConstraintsAreSatisfied(edge) == false)
                    {
                        if (otherEnd.Equals(vertex)) 
                            return false;

                        edge.Constraints.PreserveConstraint(edge, current, polygon);
                        current = otherEnd;
                    }
                    else if (edge.CheckIfHasBezierSegmentNeighbor(otherEnd))
                    {
                        current = otherEnd;
                    }
                    else
                    {
                        break;
                    }
                }

            }
            return true;
        }
        private bool EnsureConstraints(Vertex vertex)
        {
            if (PreserveConstraintsLoop(vertex, v => v.InEdge) == false)
            {
                return false;
            }
            if (PreserveConstraintsLoop(vertex, v => v.OutEdge) == false)
            {
                return false;
            }
            return true;
        }
        private bool MoveVertexWithEdgeConstraints(System.Drawing.Point nPosition)
        {
            PolygonEditor.Geometry.Polygon backupPolygon = polygon.DeepCopy();
            var ver = polygon.ChangeVertexPosition(polygon.movingVertex, new Vertex(nPosition));
            if (ver != null)
            {
                polygon.movingVertex = ver;
            }
            if (!EnsureConstraints(polygon.movingVertex))
            {
                MessageBox.Show("Cannot preserve constraints");
                polygon = backupPolygon.DeepCopy();
                return false;
            }
            return false;
        }
        private bool MoveVertex(System.Drawing.Point newPosition)
        {
            if (polygon.movingVertex == null)
                return false;
            if (polygon.edges == null || polygon.edges.Count == 0)
                return false;
            if (polygon.edges.All(edge => edge.Constraints.CheckIfEdgeHasConstraints() == false) && polygon.vertices.All(vertex => vertex.continuityType.CheckIfHasContinuity(vertex) == false))
            {   // only if we want the control point to move to save the edge curve
                //Algorithm.ChangeControlPointCositionWithoutContinuity(polygon, newPosition);
                var ver = polygon.ChangeVertexPosition(polygon.movingVertex, new Vertex(newPosition));
                if(ver != null)
                {
                    polygon.movingVertex = ver; 
                }
               
                polygon.DrawPolygon();
                return true;
            }
            //if(polygon.vertices.All(vertex => vertex.continuityType.CheckIfHasContinuity(vertex) == false))
            //{
            //    // only if we want the control point to move to save the edge curve
            //    //Algorithm.ChangeControlPointCositionWithoutContinuity(polygon, newPosition);
            //}
            MoveVertexWithEdgeConstraints(newPosition);
            polygon.DrawPolygon();
            return true;
        }    
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawingMode)
                return;
            if (isDraggingPolygon && initialMousePosition != null && e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point currentMousePosition = e.GetPosition(DrawingCanvas);
                Vector offset = currentMousePosition - initialMousePosition.Value;
                foreach (var vertex in polygon.vertices)
                {
                    polygon.ChangeVertexPosition(vertex, new Vertex(vertex.point.X + (int)offset.X, vertex.point.Y + (int)offset.Y));
                }
                foreach (var edge in polygon.edges)
                {
                    if (edge.isBezier)
                    {
                        edge.ControlPoint1.point = new System.Drawing.Point(edge.ControlPoint1.point.X + (int)offset.X, edge.ControlPoint1.point.Y + (int)offset.Y);
                        edge.ControlPoint2.point = new System.Drawing.Point(edge.ControlPoint2.point.X + (int)offset.X, edge.ControlPoint2.point.Y + (int)offset.Y);
                    }
                }
                initialMousePosition = currentMousePosition;
                polygon.DrawPolygon();
            }
            else if (polygon.movingVertex != null && e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point nPosition = e.GetPosition(DrawingCanvas);
                System.Drawing.Point drawingPoint = new System.Drawing.Point((int)nPosition.X, (int)nPosition.Y);

                MoveVertex(drawingPoint);

                polygon.DrawPolygon();
            }
            else if (polygon.movingContolPoint != null && e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point nPosition = e.GetPosition(DrawingCanvas);
                System.Drawing.Point drawingPoint = new System.Drawing.Point((int)nPosition.X, (int)nPosition.Y);
                var sv = polygon.movingControlPointEdge.Start;
                var ev = polygon.movingControlPointEdge.End;

                PolygonEditor.Geometry.Polygon backupPolygon = polygon.DeepCopy();
                if ((sv.continuityType.CheckIfHasContinuity(sv) || sv.continuityType is G0continuity) && polygon.movingContolPoint.Equals(polygon.movingControlPointEdge.ControlPoint1))
                {
                    if(sv.InEdge.isBezier && sv.OutEdge.isBezier)
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        if (sv.continuityType is C1continuity)
                        {
                            var points = Algorithm.MoveControlPointBetweenBezierEdgesC1(polygon.movingContolPoint, sv.InEdge, sv.OutEdge);
                            sv.InEdge.ControlPoint2.point = points.Item2;
                            sv.OutEdge.ControlPoint1.point = points.Item1;
                        }
                        else if(sv.continuityType is G1continuity)
                        {
                            var points = Algorithm.MoveControlPointBetweenBezierEdgesG1control2(polygon.movingContolPoint, sv.InEdge, sv.OutEdge);
                            sv.InEdge.ControlPoint2.point = points.Item2;
                            sv.OutEdge.ControlPoint1.point = points.Item1;
                        }
                        else if(sv.continuityType is G0continuity)
                        {
                            polygon.movingContolPoint.point = drawingPoint;
                        }

                        polygon.DrawPolygon();
                        return;
                    }
                    if (sv.continuityType is G0continuity)
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        polygon.DrawPolygon();
                        return;
                    }
                    if (polygon.movingControlPointEdge.Start.InEdge.Constraints is HorizontalEdgeConstraints
                        || polygon.movingControlPointEdge.Start.InEdge.Constraints is VerticalEdgeConstraints)
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        if (sv.InEdge.Constraints is HorizontalEdgeConstraints)
                        {
                            polygon.ChangeVertexPosition(polygon.movingControlPointEdge.Start, new Vertex(sv.X, drawingPoint.Y));
                        }
                        else
                        {
                            polygon.ChangeVertexPosition(polygon.movingControlPointEdge.Start, new Vertex(drawingPoint.X, sv.Y));
                        }
                        var newPost = Algorithm.ProjectVertexControl(new Vertex(drawingPoint), 
                            polygon.movingControlPointEdge.Start, polygon.movingControlPointEdge.Start.InEdge.Start);
                        polygon.ChangeVertexPosition(polygon.movingControlPointEdge.Start.InEdge.Start, newPost);

                       
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.Start, v => v.InEdge);
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.Start.InEdge.Start, v => v.InEdge);
                        
                        
                    }
                    else
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        if (sv.continuityType is G1continuity)
                        {
                            polygon.ChangeVertexPosition(polygon.movingControlPointEdge.Start.InEdge.Start,
                            Algorithm.CalculateG1(polygon.movingContolPoint,
                            polygon.movingControlPointEdge.Start,
                            polygon.movingControlPointEdge.Start.InEdge.Start));
                          
                        }
                        else if (sv.continuityType is C1continuity)
                        {
                            if (polygon.movingControlPointEdge.Start.InEdge.Constraints is DistanceConstraint)
                            {
                                polygon.ChangeVertexPosition(sv.InEdge.Start, Algorithm.CalculateG1(polygon.movingContolPoint, sv, sv.InEdge.Start));
                                var distance = Algorithm.CalculateDistance(sv, sv.InEdge.Start);
                                polygon.movingContolPoint.point = Algorithm.SetVertexDistance(sv, polygon.movingContolPoint, distance / 3).point;
                            }
                            else
                            {
                                polygon.movingControlPointEdge.Start.continuityType.PreserveContinuity(polygon.movingControlPointEdge.Start, polygon, true);
                            }
                        }
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.Start, v => v.InEdge);
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.Start.InEdge.Start, v => v.InEdge);
                    }

                }
                else if (ev.continuityType.CheckIfHasContinuity(ev) || ev.continuityType is G0continuity)
                {
                    if (ev.InEdge.isBezier && ev.OutEdge.isBezier)
                    {
                        if (ev.continuityType is C1continuity)
                        {
                            polygon.movingContolPoint.point = drawingPoint;
                            var points = Algorithm.MoveControlPointBetweenBezierEdgesC1(polygon.movingContolPoint, ev.InEdge, ev.OutEdge);
                            ev.InEdge.ControlPoint2.point = points.Item2;
                            ev.OutEdge.ControlPoint1.point = points.Item1;
                        }
                        else if(ev.continuityType is G1continuity)
                        {
                            polygon.movingContolPoint.point = drawingPoint;
                            var points = Algorithm.MoveControlPointBetweenBezierEdgesG1control1(polygon.movingContolPoint, ev.InEdge, ev.OutEdge);
                            ev.InEdge.ControlPoint2.point = points.Item2;
                            ev.OutEdge.ControlPoint1.point = points.Item1;
                        }
                        else if (ev.continuityType is G0continuity)
                        {
                            polygon.movingContolPoint.point = drawingPoint;
                        }

                        polygon.DrawPolygon();
                        return;
                    }
                    if(ev.continuityType is G0continuity)
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        polygon.DrawPolygon();
                        return;
                    }
                    if (polygon.movingControlPointEdge.End.OutEdge.Constraints is HorizontalEdgeConstraints
                        || polygon.movingControlPointEdge.End.OutEdge.Constraints is VerticalEdgeConstraints)
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        if (ev.OutEdge.Constraints is HorizontalEdgeConstraints)
                        {
                            polygon.ChangeVertexPosition(polygon.movingControlPointEdge.End, new Vertex(ev.X, drawingPoint.Y));
                        }
                        else
                        {
                            polygon.ChangeVertexPosition(polygon.movingControlPointEdge.End, new Vertex(drawingPoint.X, ev.Y));
                        }
                        var newPost = Algorithm.ProjectVertexControl(new Vertex(drawingPoint),
                            polygon.movingControlPointEdge.End, polygon.movingControlPointEdge.End.OutEdge.End);
                        polygon.ChangeVertexPosition(polygon.movingControlPointEdge.End.OutEdge.End, newPost);
                        
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.End, v => v.OutEdge);
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.End.OutEdge.End, v => v.OutEdge);
                        
                    }
                    else
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        if (ev.continuityType is G1continuity)
                        {
                            polygon.ChangeVertexPosition(polygon.movingControlPointEdge.End.OutEdge.End,
                                 Algorithm.CalculateG1(polygon.movingContolPoint,
                                 polygon.movingControlPointEdge.End,
                                 polygon.movingControlPointEdge.End.OutEdge.End));
                        }else if(ev.continuityType is C1continuity)
                        {
                            if (polygon.movingControlPointEdge.End.OutEdge.Constraints is DistanceConstraint)
                            {
                                polygon.ChangeVertexPosition(ev.OutEdge.End, Algorithm.CalculateG1(polygon.movingContolPoint, ev, ev.OutEdge.End));
                                var distance = Algorithm.CalculateDistance(ev, ev.OutEdge.End);
                                polygon.movingContolPoint.point = Algorithm.SetVertexDistance(ev, polygon.movingContolPoint, distance / 3).point;
                            }
                            else
                            {
                                polygon.movingControlPointEdge.End.continuityType.PreserveContinuity(polygon.movingControlPointEdge.End, polygon, true);
                            }
                        }
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.End, v => v.OutEdge);
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.End.OutEdge.End, v => v.OutEdge);
                    }

                }
                else
                {
                    polygon.movingContolPoint.point = drawingPoint;
                }
                polygon.DrawPolygon();
            }
        }
        private void SetDistance_Click(object sender, RoutedEventArgs e)
        {
            if (polygon.selectedEdge != null)
            {
                if (polygon.selectedEdge.Constraints.CheckIfEdgeHasConstraints() == false)
                {
                    var start = polygon.selectedEdge.Start;
                    var end = polygon.selectedEdge.End;
                    Edge startEdge = start.GetOtherEdge(polygon.selectedEdge);
                    Edge endEdge = end.GetOtherEdge(polygon.selectedEdge);
                    
                    string input = Microsoft.VisualBasic.Interaction.InputBox("Enter Distance:", "Set Distance", "0");

                    if (float.TryParse(input, out float distance))
                    {
                        if (distance <= 0 || distance >= 5000)
                        {
                            MessageBox.Show("Invalid distance value. Please enter a numeric value between 0 and 5000.");
                        }
                        if (polygon.selectedEdge != null)
                        {
                            polygon.selectedEdge.Length = distance;
                            polygon.selectedEdge.Constraints = new DistanceConstraint();
                            polygon.selectedEdge.Constraints.PreserveConstraint(polygon.selectedEdge, polygon.selectedEdge.Start, polygon);
                            EnsureConstraints(polygon.selectedEdge.Start);
                            polygon.DrawPolygon();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid distance value. Please enter a numeric value.");
                    }

                }
                else
                    MessageBox.Show($"The edge already have verticalEdgeConstraints ");
            }
            
        }

        private void ChangeBezier_Click(object sender, RoutedEventArgs e)
        {
            if(polygon.selectedEdge != null)
            {

               if(polygon.selectedEdge.isBezier == false)
                {
                    polygon.selectedEdge.SetBezier(polygon);
                }
               else
                {
                    polygon.selectedEdge.RemoveBezier();
                    polygon.selectedEdge.Start.DrawVertex(polygon.selectedEdge.Start, Brushes.Black, DrawingCanvas);
                    polygon.selectedEdge.End.DrawVertex(polygon.selectedEdge.End, Brushes.Black, DrawingCanvas);
                }

            }
            polygon.DrawPolygon();
        }
        private void SetG0Continuity_Click(object sender, RoutedEventArgs e)
        {
            if (polygon.selectedVertex != null)
            {
                if (!polygon.selectedVertex.InEdge.isBezier && !polygon.selectedVertex.OutEdge.isBezier)
                {
                    MessageBox.Show("vertex is not in bezier segment");
                    return;
                }
                polygon.selectedVertex.continuityType = new G0continuity();
                polygon.DrawPolygon();

            }
        }

        private void SetG1Continuity_Click(object sender, RoutedEventArgs e)
        {
            if (polygon.selectedVertex != null)
            {
                if (!polygon.selectedVertex.InEdge.isBezier && !polygon.selectedVertex.OutEdge.isBezier)
                {
                    MessageBox.Show("vertex is not in bezier segment");
                    return;
                }
                polygon.selectedVertex.continuityType = new G1continuity();
                polygon.selectedVertex.continuityType.PreserveContinuity(polygon.selectedVertex,polygon);
                polygon.DrawPolygon();
            }
        }

        private void SetC1Continuity_Click(object sender, RoutedEventArgs e)
        {
            if (polygon.selectedVertex != null)
            {
                if (!polygon.selectedVertex.InEdge.isBezier && !polygon.selectedVertex.OutEdge.isBezier)
                {
                    MessageBox.Show("vertex is not in bezier segment");
                    return;
                }
                polygon.selectedVertex.continuityType = new C1continuity();
                polygon.selectedVertex.continuityType.PreserveContinuity(polygon.selectedVertex, polygon);
                polygon.DrawPolygon();
            }
        }

        private void ChangeAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            polygon.isCustom = !polygon.isCustom;
            var button = sender as Button;
            if(polygon.isCustom)
            {
                button.Content = "Custom Algorithm";
            }
            else
            {
                button.Content = "Library Algorithm";
            }
            polygon.DrawPolygon();
        }
    }
}

