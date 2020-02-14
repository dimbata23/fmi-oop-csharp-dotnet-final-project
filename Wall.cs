using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace fmi_oop_csharp_dotnet_final_project
{
    class Wall
    {
        private const int DEFAULT_SIZE = 24;
        private const int SEPARATION = DEFAULT_SIZE * 4;

        #region Members
        private double width;
        private double height;
        private readonly Line line;
        private readonly SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(150, 150, 150));
        private readonly Random rand;
        #endregion

        #region Properties
        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Y
        {
            get { return line.Y1; }
        }

        public double X
        {
            get { return line.X1; }
        }
        #endregion

        #region Constructors
        public Wall(Canvas cnv, int randomSeed)
        {
            rand = new Random(randomSeed);

            line = new Line
            {
                X1 = rand.Next((int)(cnv.ActualWidth * 0.1), (int)(cnv.ActualWidth * 0.85)),
                Y1 = rand.Next((int)(cnv.ActualHeight * 0.1), (int)(cnv.ActualHeight * 0.85)),
                Stroke = brush,
                StrokeThickness = DEFAULT_SIZE
            };

            line.X1 = RoundToCertainInt(line.X1, SEPARATION);
            line.Y1 = RoundToCertainInt(line.Y1, SEPARATION);

            if (rand.Next(0, 2) == 0)
            {
                width = DEFAULT_SIZE;
                height = SEPARATION + rand.Next((int)(cnv.ActualHeight * .9 - Y));
                height = RoundToCertainInt(height, SEPARATION);
                line.X2 = X;
                line.Y2 = Y + height;
            }
            else
            {
                width = SEPARATION + rand.Next((int)(cnv.ActualWidth * .9 - X));
                width = RoundToCertainInt(width, SEPARATION);
                height = DEFAULT_SIZE;
                line.X2 = X + width;
                line.Y2 = Y;
            }
        }

        public void Draw(Canvas canvas)
        {
            canvas.Children.Add(line);
        }

        private double RoundToCertainInt(double value, int roundTo)
            => Math.Round(value / roundTo) * roundTo;
        #endregion

    }
}
