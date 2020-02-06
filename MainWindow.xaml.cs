using System;
using System.Windows;
using System.Windows.Input;

namespace fmi_oop_csharp_dotnet_final_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Game game;

        public MainWindow()
        {
            InitializeComponent();
            game = null;
        }

        private void Btn_QuitClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Btn_NewGameClick(object sender, RoutedEventArgs e)
        {
            Grd_MenuScreen.Visibility = Visibility.Hidden;
            Grd_GameScreen.Visibility = Visibility.Visible;
            Wnd_MainWindow.WindowState = WindowState.Maximized;
            game = new Game(Cnv_GameCanvas);
            game.Start();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Wnd_MainWindow.DragMove();
        }
    }
}
