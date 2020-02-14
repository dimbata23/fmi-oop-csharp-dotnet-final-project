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
            game = new Game(Cnv_GameCanvas);
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
            game.ScoreChanged += UpdateTitle;
            game.Difficulty = (uint)Sld_Difficulty.Value;
            game.Start();
        }

        private void Btn_ChangeDifficultyClick(object sender, RoutedEventArgs e)
        {
            Grd_MainMenu.Visibility = Visibility.Hidden;
            Grd_DifficultyMenu.Visibility = Visibility.Visible;
        }

        private void Btn_BackClick(object sender, RoutedEventArgs e)
        {
            Grd_DifficultyMenu.Visibility = Visibility.Hidden;
            Grd_MainMenu.Visibility = Visibility.Visible;
        }

        private void Btn_IncreaseDifficultyClick(object sender, RoutedEventArgs e)
            => --Sld_Difficulty.Value;

        private void Btn_DecreaseDifficultyClick(object sender, RoutedEventArgs e)
            => --Sld_Difficulty.Value;

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Wnd_MainWindow.DragMove();
        }

        private void UpdateTitle(object sender, string e)
        {
            Lbl_Title.Content = e;
        }
    }
}
