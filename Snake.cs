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
        private const double DEFAULT_SPEED = 100;

        #region Members
        // Head of the snake
        private Ellipse head;

        // List of points where ellipses will be drawn
        private LinkedList<Point> body;

        // Color of the snake
        private SolidColorBrush color = new SolidColorBrush(Colors.Green);
        #endregion

        #region Constructors
        public Snake(double x, double y)
        {
            body = new LinkedList<Point>();

            for (int i = 0; i < DEFAULT_LENGTH; i++)
                body.AddLast(new Point(x + i * SIZE, y));

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
            Canvas.SetLeft(head, body.First.Value.X - HEAD_SIZE / 2);
            Canvas.SetTop(head, body.First.Value.Y - HEAD_SIZE / 2);
        }

        public void moveTowards(Point point, double deltaTime)
        {
            for (var curr = body.Last; curr.Previous != null; curr = curr.Previous)
            {
                curr.Value =
                    new Point(
                        curr.Value.X + (curr.Previous.Value.X - curr.Value.X) * (DEFAULT_SPEED * deltaTime / SIZE),
                        curr.Value.Y + (curr.Previous.Value.Y - curr.Value.Y) * (DEFAULT_SPEED * deltaTime / SIZE)
                    );
            }

            var head = body.First;
            var distanceToPoint = Point.Subtract(point, head.Value).Length;
            head.Value =
                new Point(
                    head.Value.X + (point.X - head.Value.X) * (DEFAULT_SPEED * deltaTime / distanceToPoint),
                    head.Value.Y + (point.Y - head.Value.Y) * (DEFAULT_SPEED * deltaTime / distanceToPoint)
                );
        }
        #endregion
    }
}
