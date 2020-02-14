using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace fmi_oop_csharp_dotnet_final_project
{
    class Game
    {
        #region Constants
        private const int MILISECONDS_IN_SECOND = 1000;
        private const int DEFAULT_FPS = 60;
        private const int GROWTH_RATE = 3;
        private const int DEFAULT_STARTING_FOODS = 8;
        private const int DEFAULT_STARTING_WALLS = 12;
        private const uint DEFAULT_DIFFICULTY = 5;
        private const uint MIN_DIFFICULTY = 0;
        private const uint MAX_DIFFICULTY = 10;
        #endregion

        public event EventHandler<string> ScoreChanged;

        #region Members
        // Whether the game is running or not
        private bool isRunning;

        // Update interval of the game in miliseconds
        private int updateInterval;

        // Canvas where the game will be drawn
        private Canvas canvas;

        // Thread that's running the game loop
        private Thread gameLoopThread;

        // The snake object
        private Snake snake;

        // The walls in the game
        private readonly LinkedList<Wall> walls;

        // The foods in the game
        private LinkedList<Food> foods;

        // The player's score
        private uint score;

        // Random number generator
        private readonly Random rand;

        // The game's difficulty
        private uint difficulty;
        #endregion

        #region Properties
        public bool IsRunning
        {
            get { lock (this) return isRunning; }
            private set { lock (this) isRunning = value; }
        }

        public Canvas Canvas
        {
            get { lock (this) return canvas; }
            set
            {
                if (value != null)
                    lock (this)
                        canvas = value;
            }
        }

        public int UpdateInterval
        {
            get => updateInterval;
            set
            {
                if (value > 0)
                    updateInterval = value;
            }
        }

        public int FPS
        {
            get => MILISECONDS_IN_SECOND / UpdateInterval;
            set
            {
                if (value > 0)
                    updateInterval = MILISECONDS_IN_SECOND / value;
            }
        }

        public uint Score
        {
            get => score;
            set
            {
                score = value;
                ScoreChanged?.Invoke(this, "Score: " + score);
            }
        }

        public uint Difficulty
        {
            get => difficulty;
            set => difficulty = value;
        }
        #endregion

        #region Constructors
        public Game(Canvas cnv, int fps = DEFAULT_FPS)
        {
            Difficulty = DEFAULT_DIFFICULTY;
            rand = new Random();
            IsRunning = false;
            Canvas = cnv;
            FPS = fps;
            snake = new Snake(canvas.ActualWidth * 0.5, canvas.ActualHeight * 0.5);
            foods = new LinkedList<Food>();
            for (int i = 0; i < DEFAULT_STARTING_FOODS; i++)
                foods.AddLast(new Food(canvas, rand.Next(), (double)difficulty / MAX_DIFFICULTY));

            walls = new LinkedList<Wall>();
            for (int i = 0; i < DEFAULT_STARTING_WALLS; i++)
            {
                walls.AddLast(new Wall(canvas, rand.Next()));

                // Add walls but not in the middle
                while (RectanglesCollide(walls.Last.Value.X, walls.Last.Value.Y, walls.Last.Value.Width, walls.Last.Value.Height,
                                         canvas.ActualWidth * 0.3, canvas.ActualHeight * 0.3, canvas.ActualWidth * 0.4, canvas.ActualHeight * 0.4))
                {
                    walls.Last.Value = new Wall(canvas, rand.Next());
                }
            }
        }
        #endregion

        #region Methods
        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                gameLoopThread = new Thread(GameLoop);
                gameLoopThread.Start();
                Score = 0;
            }
        }

        public void Stop()
        {
            lock (this)
                IsRunning = false;
        }

        private void GameLoop()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            double deltaTime = 0;
            while (IsRunning)
            {
                deltaTime = timer.Elapsed.TotalMilliseconds;
                if (deltaTime < updateInterval)
                    Thread.Sleep((int)(updateInterval - deltaTime));
                timer.Restart();

                Application.Current.Dispatcher.Invoke(Update);
                if (IsRunning)
                    Application.Current.Dispatcher.Invoke(Draw);
            }
        }

        private void Update()
        {
            // Remove dead food
            for (var currNode = foods.First; currNode != null; currNode = currNode.Next)
            {
                if (currNode.Value.IsDead)
                    currNode.Value = new Food(canvas, rand.Next(), (double)difficulty / MAX_DIFFICULTY);
            }

            snake.Move(Mouse.GetPosition(canvas), updateInterval * 0.001);

            if (snake.DetectSelfCollision())
                GameOver();

            // Collision with walls
            foreach (Wall wall in walls)
            {
                if (CircleAndRectangleCollide(snake.Body.First.Value.X + snake.HeadSize * 0.5, snake.Body.First.Value.Y + snake.HeadSize * 0.5, snake.HeadSize * 0.5,
                                              wall.X, wall.Y, wall.Width, wall.Height))
                {
                    GameOver();
                }
            }

            // Eaten food detection
            for (var currFoodNode = foods.First; currFoodNode != null; currFoodNode = currFoodNode.Next)
            {
                double dist = Point.Subtract(snake.Body.First.Value,
                                             new Point(currFoodNode.Value.Position.X + currFoodNode.Value.Size * 0.5,
                                                       currFoodNode.Value.Position.Y + currFoodNode.Value.Size * 0.5)
                                            ).Length;
                if (dist < (snake.HeadSize * 0.5 + currFoodNode.Value.Size * 0.5))
                {
                    currFoodNode.Value = new Food(canvas, rand.Next(), (double)difficulty / MAX_DIFFICULTY);
                    ++Score;
                    snake.Grow(GROWTH_RATE);
                }
            }
        }

        private void Draw()
        {
            canvas.Children.Clear();

            foreach (Food food in foods)
                food.Draw(canvas);

            foreach (Wall wall in walls)
                wall.Draw(canvas);

            snake.Draw(canvas);
        }

        private void GameOver()
        {
            TextBlock Txt_gameOver = new TextBlock();
            Txt_gameOver.Width = canvas.ActualWidth;
            Txt_gameOver.Text = "Game Over!";
            Txt_gameOver.TextAlignment = TextAlignment.Center;
            Txt_gameOver.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Txt_gameOver.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            Canvas.SetTop(Txt_gameOver, canvas.ActualHeight * 0.45);
            canvas.Children.Add(Txt_gameOver);
            Stop();
        }

        private bool RectanglesCollide(double x1, double y1, double width1, double height1, double x2, double y2, double width2, double height2)
        {
            return x1 < x2 + width2 &&
                   x1 + width1 > x2 &&
                   y1 < y2 + height2 &&
                   y1 + height1 > y2;
        }

        private bool CircleAndRectangleCollide(double circleX, double circleY, double circleRadius, double rectX, double rectY, double rectW, double rectH)
        {
            double distX = Math.Abs(circleX - rectX - rectW * 0.5);
            double distY = Math.Abs(circleY - rectY - rectH * 0.5);

            if (distX > (rectW * 0.5 + circleRadius) ||
                distY > (rectH * 0.5 + circleRadius))
            {
                return false;
            }

            if (distX < rectW * 0.5 || distY < rectH * 0.5)
                return true;

            double dx = distX - rectW * 0.5;
            double dy = distY - rectH * 0.5;
            return dx * dx + dy * dy < circleRadius * circleRadius;
        }
        #endregion

    }
}
