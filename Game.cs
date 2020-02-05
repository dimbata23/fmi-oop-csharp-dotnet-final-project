using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace fmi_oop_csharp_dotnet_final_project
{
    class Game
    {
        private const int MILISECONDS_IN_SECOND = 1000;

        #region Members
        // Whether the game is running or not
        private bool isRunning;

        // Update interval of the game in nanoseconds
        private int updateInterval;

        // Canvas where the game will be drawn
        private Canvas canvas;

        // Thread that's running the game loop
        private Thread gameLoopThread;

        // The snake object
        private Snake snake;
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
        #endregion

        #region Constructors
        public Game(Canvas cnv, int fps = 60)
        {
            IsRunning = false;
            Canvas = cnv;
            FPS = fps;
            snake = new Snake(cnv.ActualWidth / 2, cnv.ActualHeight / 2);
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
                Application.Current.Dispatcher.Invoke(Draw);
            }
        }

        private void Update()
        {

        }

        private void Draw()
        {
            canvas.Children.Clear();
            snake.Draw(canvas);
        }
        #endregion

    }
}
