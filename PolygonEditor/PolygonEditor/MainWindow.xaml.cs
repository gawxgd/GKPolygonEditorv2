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
        private bool IsPointNearLine(Vertex start, Vertex end, System.Windows.Point point, double threshold = 5)
        {
            double lineLength = Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2)); // euclidian norm line length

            if (lineLength < 0.1) // Avoid division by near-zero line length
                return false;

            double distance = Math.Abs((end.Y - start.Y) * point.X - (end.X - start.X) * point.Y + end.X * start.Y - end.Y * start.X)
                              / lineLength; // edge point distance using perpendicular formula

            if (distance > threshold) // Check if the point is within the threshold distance from the line
                return false;

            // Check if the projection of point onto the line segment lies within the segment
            double dotProduct = ((point.X - start.X) * (end.X - start.X)) + ((point.Y - start.Y) * (end.Y - start.Y)); // projection of the point edge start vector on the edge
            double projectedLengthSquared = dotProduct * dotProduct / (lineLength * lineLength);

            return projectedLengthSquared <= lineLength * lineLength;
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
                // Check if the click is near the edge
                if (IsPointNearLine(edge.Start, edge.End, mousePosition))
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

                if (polygon.selectedEdge != null)
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

        private void ToggleBezier_Click(object sender, RoutedEventArgs e)
        {

        }
        private bool PreserveConstraintsLoop(Vertex vertex, Func<Vertex, Edge?> direction)
        {

            Vertex current = vertex;

            while (true)
            {
                var edge = direction(current);

                if (edge == null) return false;

                var other = edge.GetOtherEnd(current);

                //if (edge.IsBezier)
                //{
                //    if (current.ConstraintType != VertexConstraintType.None &&
                //        current.ConstraintType != VertexConstraintType.G0 &&
                //        !current.CheckConstraint())
                //    {
                //        current.PreserveConstraint();
                //    }

                //    break;
                //}
                //else // !edge.IsBezier
                //{
                if (edge.Constraints.CheckIfEdgeHasConstraints())
                {
                    if (other == vertex) return false;

                    edge.Constraints.PreserveConstraint(edge,current,polygon);
                    current = other;
                }
                //else if (edge.IsBezierNeighbour(other))
                //{
                //    current = other;
                //}
                else
                {
                    break;
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
            if (polygon.edges.All(edge => edge.Constraints.CheckIfEdgeHasConstraints() == false))
            {
                var ver = polygon.ChangeVertexPosition(polygon.movingVertex, new Vertex(newPosition));
                if(ver != null)
                {
                    polygon.movingVertex = ver; 
                }
                polygon.DrawPolygon();
                return true;
            }
            MoveVertexWithEdgeConstraints(newPosition);
            polygon.DrawPolygon();
            return true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingPolygon && initialMousePosition != null && e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point currentMousePosition = e.GetPosition(DrawingCanvas);
                Vector offset = currentMousePosition - initialMousePosition.Value;
                foreach (var vertex in polygon.vertices)
                {
                    polygon.ChangeVertexPosition(vertex, new Vertex(vertex.point.X + (int)offset.X, vertex.point.Y + (int)offset.Y));
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
        }
        

    }
}

