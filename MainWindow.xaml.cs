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

        // The main game object
        private Game game;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Exit the app when the Quit button is clicked
        private void Btn_QuitClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        // Starting a new game when the New Game button is clicked
        private void Btn_NewGameClick(object sender, RoutedEventArgs e)
        {
            // Hide the main menu and show the game screen maximized
            Grd_MenuScreen.Visibility = Visibility.Hidden;
            Grd_GameScreen.Visibility = Visibility.Visible;
            Wnd_MainWindow.WindowState = WindowState.Maximized;

            // Create a new game object with the given difficulty
            game = new Game(Cnv_GameCanvas, (uint)Sld_Difficulty.Value);

            // Subsscribe to the ScoreChanged event
            game.ScoreChanged += UpdateTitle;

            // Start the game
            game.Start();
        }

        // When clicked the change game difficulty
        private void Btn_ChangeDifficultyClick(object sender, RoutedEventArgs e)
        {
            // Hide main menu and show difficulty menu
            Grd_MainMenu.Visibility = Visibility.Hidden;
            Grd_DifficultyMenu.Visibility = Visibility.Visible;
        }

        // Wheck clicked the back button
        private void Btn_BackClick(object sender, RoutedEventArgs e)
        {
            // Hide difficulty menu and show main menu
            Grd_DifficultyMenu.Visibility = Visibility.Hidden;
            Grd_MainMenu.Visibility = Visibility.Visible;
        }

        private void Btn_IncreaseDifficultyClick(object sender, RoutedEventArgs e)
            => ++Sld_Difficulty.Value;

        private void Btn_DecreaseDifficultyClick(object sender, RoutedEventArgs e)
            => --Sld_Difficulty.Value;

        // Change the titlebar's text
        private void UpdateTitle(object sender, string e)
        {
            Lbl_Title.Content = "Difficulty: " + game.Difficulty + " / 10\t\t" + e;
        }
    }
}
