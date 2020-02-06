using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace fmi_oop_csharp_dotnet_final_project
{
    class Food
    {
        private const int SIZE = 24;

        #region Members
        private Point position;
        private Ellipse ellipse;
        #endregion

        #region Properties
        public Point Position
        {
            get => position;
            set => position = new Point(value.X, value.Y);
        }
        #endregion

        #region Constructors
        public Food() : this(new Point(0, 0))
        { }

        public Food(Point pos)
        {
            Position = pos;
            ellipse = new Ellipse();
            ellipse.Fill = new SolidColorBrush(Colors.Red);
            ellipse.Width = SIZE;
            ellipse.Height = SIZE;
        }
        #endregion

        #region Methods
        public void Draw(Canvas canvas)
        {
            canvas.Children.Add(ellipse);
            Canvas.SetTop(ellipse, position.Y);
            Canvas.SetLeft(ellipse, position.X);
        }
        #endregion
    }
}
