using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace fmi_oop_csharp_dotnet_final_project
{
    class Snake
    {
        #region Constants
        private const int DEFAULT_HEAD_SIZE = 32;
        private const int DEFAULT_SIZE = 24;
        private const int DEFAULT_LENGTH = 5;
        private const double DEFAULT_SPEED = 100;
        private const double MAX_ROTATE_ANGLE_RAD = Math.PI / 6;
        private const double MAX_ROTATE_ANGLE_DEG = MAX_ROTATE_ANGLE_RAD * 180 / Math.PI;
        private const double DETECTION_FACTOR = 0.5; 
        #endregion

        #region Members
        // Head of the snake
        private readonly Ellipse head;

        // List of points where ellipses will be drawn
        private LinkedList<Point> body;

        // Color of the snake
        private SolidColorBrush brush = new SolidColorBrush(Colors.Green);

        private double sizeFactor;

        private double speedFactor;
        #endregion

        #region Properties
        public LinkedList<Point> Body { get => body; }
        public double Size { get => DEFAULT_SIZE * sizeFactor; }
        public double HeadSize { get => DEFAULT_HEAD_SIZE * sizeFactor; }
        public Color Color
        {
            get => brush.Color;
            set => brush.Color = value;
        }
        public double SizeFactor
        {
            get => sizeFactor;
            set
            {
                if (value >= 0)
                    sizeFactor = value;
            }
        }
        public double SpeedFactor
        {
            get => speedFactor;
            set
            {
                if (value > 0)
                    speedFactor = value;
            }
        }
        #endregion

        #region Constructors
        public Snake(double x, double y, double sizeFactor = 1, double speedFactor = 1)
        {
            body = new LinkedList<Point>();
            SizeFactor = sizeFactor;
            SpeedFactor = speedFactor;

            // Craete the default snake points
            for (int i = 0; i < DEFAULT_LENGTH; i++)
                body.AddLast(new Point(x + i * Size, y));

            head = new Ellipse();
            head.Width = HeadSize;
            head.Height = HeadSize;
            head.Fill = brush;
        }
        #endregion

        #region Methods
        // Adds the snake to the given canvas
        public void Draw(Canvas canvas)
        {
            // Create a polyline object and add all the points
            Polyline polyline = new Polyline();
            foreach (var point in body)
                polyline.Points.Add(point);

            polyline.StrokeThickness = Size;
            polyline.Stroke = brush;
            polyline.StrokeEndLineCap = PenLineCap.Triangle;
            canvas.Children.Add(polyline);
            canvas.Children.Add(head);
            Canvas.SetLeft(head, body.First.Value.X - HeadSize / 2);
            Canvas.SetTop(head, body.First.Value.Y - HeadSize / 2);
        }

        // Move towards a given point accomodating the max steering angle
        public void Move(Point point, double deltaTime)
        {
            Vector snakeToMouseVector = Point.Subtract(point, Body.First.Value);
            if (snakeToMouseVector.Length > Size * 0.5)
            {
                Vector snakeVector = Point.Subtract(Body.First.Value, Body.First.Next.Value);
                var angle = Vector.AngleBetween(snakeVector, snakeToMouseVector);
                // Checks the angle between the snake's direction and the direction to the mouse
                if (angle > MAX_ROTATE_ANGLE_DEG || angle < -MAX_ROTATE_ANGLE_DEG)
                {
                    // If it's more than the MAX
                    // Calculate a new vector corresponding to the direction where the snake should go instead
                    double rotateAngle = Math.Sign(angle) * MAX_ROTATE_ANGLE_RAD;
                    Vector v2 = new Vector(snakeVector.X * Math.Cos(rotateAngle) - snakeVector.Y * Math.Sin(rotateAngle),
                                           snakeVector.X * Math.Sin(rotateAngle) + snakeVector.Y * Math.Cos(rotateAngle));
                    v2.Normalize();
                    v2.X *= DEFAULT_SPEED * speedFactor;
                    v2.Y *= DEFAULT_SPEED * speedFactor;

                    // Move with the calculated vector
                    MoveTowards(new Point(Body.First.Value.X + v2.X, Body.First.Value.Y + v2.Y), deltaTime);
                }
                else
                {
                    // If the angle is in the range, just move the snake towards the mouse
                    MoveTowards(point, deltaTime);
                }
            }
        }

        private void MoveTowards(Point point, double deltaTime)
        {
            // Move each point towards the next, starting from the last
            for (var curr = body.Last; curr.Previous != null; curr = curr.Previous)
            {
                curr.Value =
                    new Point(
                        curr.Value.X + (curr.Previous.Value.X - curr.Value.X) * (DEFAULT_SPEED * speedFactor * deltaTime / Size),
                        curr.Value.Y + (curr.Previous.Value.Y - curr.Value.Y) * (DEFAULT_SPEED * speedFactor * deltaTime / Size)
                    );
            }

            // Move the first point of the body (the head) towards the given position
            var head = body.First;
            double distanceToPoint = Point.Subtract(point, head.Value).Length;
            head.Value =
                new Point(
                    head.Value.X + (point.X - head.Value.X) * (DEFAULT_SPEED * speedFactor * deltaTime / distanceToPoint),
                    head.Value.Y + (point.Y - head.Value.Y) * (DEFAULT_SPEED * speedFactor * deltaTime / distanceToPoint)
                );
        }

        // Makes the snake longer
        public void Grow(int factor = 1)
        {
            for (int i = 0; i < factor; i++)
                body.AddLast(body.Last.Value);
        }

        // Detects wheter the snake has collided with itself
        public bool DetectSelfCollision()
        {
            double dist;
            for (var curr = Body.First.Next.Next.Next; curr != null; curr = curr.Next)
            {
                dist = Point.Subtract(Body.First.Value, curr.Value).Length;
                if (dist < (HeadSize * DETECTION_FACTOR + Size * DETECTION_FACTOR))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
