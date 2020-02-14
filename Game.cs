using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace fmi_oop_csharp_dotnet_final_project
{
    class Game
    {
        private const int MILISECONDS_IN_SECOND = 1000;
        private const int DEFAULT_FPS = 60;
        private const int GROWTH_RATE = 3;

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

        // TODO: Add obsticles and walls

        // The food object
        private readonly Food food; // TODO: Change to List of Food to create more than one food

        // Random number generator
        private readonly Random rand;

        // The player's score
        private uint score; 
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
        #endregion

        #region Constructors
        public Game(Canvas cnv, int fps = DEFAULT_FPS)
        {
            IsRunning = false;
            Canvas = cnv;
            FPS = fps;
            snake = new Snake(canvas.ActualWidth / 2, canvas.ActualHeight / 2);
            rand = new Random();
            food = new Food();
            food.Position = new Point(rand.Next() % (canvas.ActualWidth - food.Size), rand.Next() % (canvas.ActualHeight - food.Size));
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
            snake.Move(Mouse.GetPosition(canvas), updateInterval * 0.001);

            if (snake.DetectSelfCollision())
                GameOver();

            // Eaten food detection
            double dist = Point.Subtract(snake.Body.First.Value, new Point(food.Position.X + food.Size/2, food.Position.Y + food.Size/2)).Length;
            if (dist < (snake.HeadSize / 2 + food.Size / 2))
            {
                food.Position = new Point(rand.Next() % (canvas.ActualWidth - food.Size), rand.Next() % (canvas.ActualHeight - food.Size));
                ++Score;
                snake.Grow(GROWTH_RATE);
            }
        }

        private void Draw()
        {
            canvas.Children.Clear();
            food.Draw(canvas);
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

        #endregion

    }
}
