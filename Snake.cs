using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace fmi_oop_csharp_dotnet_final_project
{
    class Snake
    {
        private const int HEAD_SIZE = 32;
        private const int SIZE = 24;
        private const int DEFAULT_LENGTH = 5;
        private const double DEFAULT_SPEED = 2;

        #region Members
        // Head of the snake
        private Ellipse head;

        // List of points where ellipses will be drawn
        private List<Point> body;

        // Color of the snake
        private SolidColorBrush color = new SolidColorBrush(Colors.Green);
        #endregion

        #region Constructors
        public Snake(double x, double y)
        {
            body = new List<Point>();

            for (int i = 0; i < DEFAULT_LENGTH; i++)
                body.Add(new Point(x + i * SIZE, y));

            head = new Ellipse();
            head.Width = HEAD_SIZE;
            head.Height = HEAD_SIZE;
            head.Fill = color;
        }
        #endregion

        #region Methods
        public void Draw(Canvas canvas)
        {
            Polyline polyline = new Polyline();
            foreach (var point in body)
                polyline.Points.Add(point);

            polyline.StrokeThickness = SIZE;
            polyline.Stroke = color;
            polyline.StrokeEndLineCap = PenLineCap.Triangle;
            canvas.Children.Add(polyline);
            canvas.Children.Add(head);
            Canvas.SetLeft(head, body[0].X - HEAD_SIZE / 2);
            Canvas.SetTop(head, body[0].Y - HEAD_SIZE / 2);
        }

        public void moveTowards(Point point)
        {
            for (int i = body.Count - 1; i > 0; i--)
                body[i] = new Point(body[i].X + (body[i - 1].X - body[i].X) * (DEFAULT_SPEED / SIZE), body[i].Y + (body[i - 1].Y - body[i].Y) * (DEFAULT_SPEED / SIZE));

            var distanceToPoint = Point.Subtract(point, body[0]).Length;
            body[0] = new Point(body[0].X + (point.X - body[0].X) * (DEFAULT_SPEED / distanceToPoint), body[0].Y + (point.Y - body[0].Y) * (DEFAULT_SPEED / distanceToPoint));
        }
        #endregion
    }
}
