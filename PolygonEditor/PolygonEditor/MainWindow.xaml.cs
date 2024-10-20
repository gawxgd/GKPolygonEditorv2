﻿using System.Text;
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

namespace PolygonEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PolygonEditor.Geometry.Polygon? polygon;

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
                    polygon.ChangeVertexPosition(polygon.selectedEdge.Start, new Vertex(polygon.selectedEdge.Start.X, polygon.selectedEdge.End.Y));
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

            // Draw lines between points if more than one vertex
            if (tempPoints.Count > 1)
            {
                Algorithm.DrawBresenhamLine(new Vertex(tempPoints[tempPoints.Count - 2]), newVertex, Brushes.Black, DrawingCanvas);
            }
        }
        private void normal_MouseLeftButtonDown(Vertex mousePosition)
        {
            polygon.movingVertex = null;
            // Check for vertex selection first
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                // distance beetwean vertex and mouse
                if (Math.Sqrt(Math.Pow(polygon.vertices[i].X - mousePosition.X, 2) + Math.Pow(polygon.vertices[i].Y - mousePosition.Y, 2)) < 10)
                {
                    polygon.movingVertex = polygon.vertices[i];
                    polygon.DrawPolygon();
                    return; // Exit early if a vertex is selected
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
                        return; // Exit early if a vertex is selected
                    }
                    else if (Algorithm.CalculateDistance(e.ControlPoint2, mousePosition) < 10)
                    {
                        polygon.movingContolPoint = e.ControlPoint2;
                        polygon.movingControlPointEdge = e;
                        polygon.DrawPolygon();
                        return; // Exit early if a vertex is selected
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
            else if (isDraggingPolygon) // Start dragging the polygon
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

            // Check for vertex selection first
            for (int i = 0; i < polygon.vertices.Count; i++)
            {
                if (Math.Sqrt(Math.Pow(polygon.vertices[i].X - mousePosition.X, 2) + Math.Pow(polygon.vertices[i].Y - mousePosition.Y, 2)) < 10)
                {
                    polygon.selectedVertex = polygon.vertices[i];
                    polygon.selectedEdge = null;
                    polygon.DrawPolygon();
                    return; // Exit early if a vertex is selected
                }
            }

            polygon.selectedEdge = null;

            // Loop through the existing edge list to select the edge
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
                // Check if the click is near the edge
                else if (Algorithm.IsPointNearLine(edge.Start, edge.End, mousePosition))
                {
                    polygon.selectedEdge = edge;  // Select the existing edge from the list
                    polygon.DrawPolygon();
                    return; // Exit after selecting the edge
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

        private void NewPolygon_Click(object sender, RoutedEventArgs e)
        {

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
            isDraggingPolygon = !isDraggingPolygon;
            MessageBox.Show(isDraggingPolygon ? "Polygon dragging enabled" : "Polygon dragging disabled");
        }

        
        private bool PreserveConstraintsLoop(Vertex vertex, Func<Vertex, Edge?> direction)
        {

            Vertex current = vertex;
            bool isAllEdgesPositionConstraint = polygon.edges.All(edge => edge.Constraints is HorizontalEdgeConstraints || edge.Constraints is VerticalEdgeConstraints);
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
                    if (current.continuityType.CheckIfHasContinuity(current))
                    {
                        current.continuityType.PreserveContinuity(current,polygon);
                    }
                    break;
                }
                else // !edge.IsBezier
                {
                    if (edge.Constraints.CheckIfEdgeHasConstraints())
                    {
                        if (otherEnd.Equals(vertex)) return !isAllEdgesPositionConstraint;

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
            if (!PreserveConstraintsLoop(vertex, v => v.InEdge))
            {
                return false;
            }
            if (!PreserveConstraintsLoop(vertex, v => v.OutEdge))
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
                MessageBox.Show("making deep copy");
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
            if(polygon.vertices.All(vertex => vertex.continuityType.CheckIfHasContinuity(vertex) == false))
            {
                // only if we want the control point to move to save the edge curve
                //Algorithm.ChangeControlPointCositionWithoutContinuity(polygon, newPosition);
            }
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
                if (sv.continuityType.CheckIfHasContinuity(sv) && polygon.movingContolPoint.Equals(polygon.movingControlPointEdge.ControlPoint1))
                {
                    if (polygon.movingControlPointEdge.Start.InEdge.Constraints is HorizontalEdgeConstraints
                        || polygon.movingControlPointEdge.Start.InEdge.Constraints is VerticalEdgeConstraints)
                    {
                        var newPost = Algorithm.ProjectVertex(new Vertex(drawingPoint), polygon.movingControlPointEdge.Start, polygon.movingControlPointEdge.Start.InEdge.Start);
                        polygon.movingContolPoint.point = newPost.point;
                    }
                    else
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        polygon.ChangeVertexPosition(polygon.movingControlPointEdge.Start.InEdge.Start,
                            Algorithm.CalculateG1(polygon.movingContolPoint,
                            polygon.movingControlPointEdge.Start,
                            polygon.movingControlPointEdge.Start.InEdge.Start));
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.Start, v => v.InEdge);
                        PreserveConstraintsLoop(polygon.movingControlPointEdge.Start.InEdge.Start, v => v.InEdge);
                    }

                }
                else if (ev.continuityType.CheckIfHasContinuity(ev))
                {
                    if (polygon.movingControlPointEdge.End.OutEdge.Constraints is HorizontalEdgeConstraints
                        || polygon.movingControlPointEdge.End.OutEdge.Constraints is VerticalEdgeConstraints)
                    {
                        var newPost = Algorithm.ProjectVertex(new Vertex(drawingPoint), polygon.movingControlPointEdge.End, polygon.movingControlPointEdge.End.OutEdge.End);
                        polygon.movingContolPoint.point = newPost.point;
                    }
                    else
                    {
                        polygon.movingContolPoint.point = drawingPoint;
                        polygon.ChangeVertexPosition(polygon.movingControlPointEdge.End.OutEdge.End,
                             Algorithm.CalculateG1(polygon.movingContolPoint,
                             polygon.movingControlPointEdge.End,
                             polygon.movingControlPointEdge.End.OutEdge.End));
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
                    
                    // Prompt the user for a distance value
                    string input = Microsoft.VisualBasic.Interaction.InputBox("Enter Distance:", "Set Distance", "0");

                    if (float.TryParse(input, out float distance))
                    {
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
               if(polygon.selectedEdge.Start.InEdge.isBezier || polygon.selectedEdge.End.OutEdge.isBezier)
                {
                    MessageBox.Show("neighbor is already bezier"); return;
                }
               if(polygon.selectedEdge.isBezier == false)
                {
                    polygon.selectedEdge.SetBezier(polygon);
                }
               else
                {
                    polygon.selectedEdge.RemoveBezier();
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
            }
        }
    }
}

