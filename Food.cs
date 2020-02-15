using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace fmi_oop_csharp_dotnet_final_project
{
    class Food
    {
        private const int DEFAULT_SIZE = 24;
        private const int DEFAULT_MIN_LIFETIME = 10; // seconds
        private const int DEFAULT_MAX_LIFETIME = 30; // seconds

        #region Members
        // The position of the food
        private Point position;

        // The ecllipse which will be drawn
        private Ellipse ellipse;

        private double sizeFactor;
        private readonly Random rand;
        private bool isDead;
        #endregion

        #region Properties
        public Point Position
        {
            get => position;
            set => position = new Point(value.X, value.Y);
        }

        public double Size { get => DEFAULT_SIZE * sizeFactor; }

        public double SizeFactor
        {
            get => sizeFactor;
            set
            {
                if (value > 0)
                    sizeFactor = value;
            }
        }

        public bool IsDead
        {
            get => isDead;
        }
        #endregion

        #region Constructors
        public Food(Canvas cnv, int randomSeed, double difficultyFactor, double sizeFactor = 1) : this(new Point(0, 0), randomSeed, difficultyFactor, sizeFactor)
        {
            // Create the food at a random position
            Position = new Point(rand.Next((int)(cnv.ActualWidth - Size + 1)), rand.Next((int)(cnv.ActualHeight - Size + 1)));
        }

        public Food(Point pos, int randomSeed, double difficultyFactor, double sizeFactor = 1)
        {
            isDead = false;
            rand = new Random(randomSeed);
            SizeFactor = sizeFactor;
            Position = pos;
            ellipse = new Ellipse();
            
            // Colorize the food with a random red and green colors (making them yellow-ish red)
            byte red = (byte)rand.Next((int)(0.5 * 256), 256);
            byte green = 0;
            if (rand.Next(0,2) == 0)
                green = (byte)rand.Next((int)(0.5 * 256), red + 1);
            ellipse.Fill = new SolidColorBrush(Color.FromRgb(red, green, 0));
            ellipse.Width = Size;
            ellipse.Height = Size;

            // Mark the food as dead after a random period of time
            Task.Delay(TimeSpan.FromSeconds(
                rand.Next((int)(DEFAULT_MIN_LIFETIME * 0.5 + DEFAULT_MIN_LIFETIME * (1 - difficultyFactor)), 
                          (int)(1 + DEFAULT_MAX_LIFETIME * 0.5 + DEFAULT_MAX_LIFETIME * (1 - difficultyFactor)))
                )).ContinueWith(t => { isDead = true;  });
        }
        #endregion

        #region Methods
        // Add the food to the given canvas
        public void Draw(Canvas canvas)
        {
            canvas.Children.Add(ellipse);
            Canvas.SetTop(ellipse, position.Y);
            Canvas.SetLeft(ellipse, position.X);
        }
        #endregion
    }
}
