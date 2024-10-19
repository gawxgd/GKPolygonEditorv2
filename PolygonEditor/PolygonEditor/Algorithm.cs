using PolygonEditor.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
namespace PolygonEditor
{
    public static class Algorithm
    {
        public static void DrawBresenhamLine(Vertex start, Vertex end, Brush color, Canvas drawingCanvas)
        {
            if(start.point == end.point)
            {
                return;
            }
            int x0 = (int)start.X, y0 = (int)start.Y;
            int x1 = (int)end.X, y1 = (int)end.Y;
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                var pixel = new Rectangle { Width = 1, Height = 1, Fill = color };
                Canvas.SetLeft(pixel, x0);
                Canvas.SetTop(pixel, y0);
                drawingCanvas.Children.Add(pixel);

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
    }
}
