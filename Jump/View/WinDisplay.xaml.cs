using Jump.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Formats.Asn1.AsnWriter;

namespace Jump.View
{
    public partial class WinDisplay : UserControl
    {
        public MainWindow? main { get; set; }


        public int score { get; set; }

        public WinDisplay() { }

        public WinDisplay(MainWindow main)
        {
            this.main = main;

            InitializeComponent();
            ShowWindow();
            GetScore();
            CreateButtonHighScore();
        }

        public void CreateButtonHighScore()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("HIGHSCORE", ref HighScore, 216, 70);

            HighScore.Click += HandleToHighScore;
        }

        public void GetScore()
        {
            score = main!.score;
            Score.Text = Convert.ToString(score);
        }

        public async void HandleToHighScore(object sender, RoutedEventArgs e)
        {
            HighScoreView highscoreview = new HighScoreView(main!);
            highscoreview.score = score;

            Canvas.SetLeft(highscoreview, 1000);
            Canvas.SetLeft(win, 0);
            
            main!.Playground.Children.Add(highscoreview);
            await WinDisplayDissapear(highscoreview);
        }

        public async Task WinDisplayDissapear(HighScoreView highscoreview)
        {
            double poswin = Canvas.GetLeft(win);
            double poshighscore = Canvas.GetLeft(highscoreview);
            while (poswin > -1000)
            {
                poswin -= 100;
                poshighscore -= 100;

                TimeSpan move = TimeSpan.FromSeconds(0.03);
                await Task.Delay(move);

                main!.MoveWinDisplay(this, poswin);
                Canvas.SetLeft(highscoreview, poshighscore);
            }

            main!.Playground.Children.Remove(this);
        }

        public void ShowWindow()
        {
            WhiteScreen();
        }

        public async void WhiteScreen()
        {
            windisplay.Opacity = 0;
            while (whitescreen.Fill.Opacity < 1)
            {
                whitescreen.Fill.Opacity += 0.1;
                await Task.Delay(80);
            }

            await Task.Delay(1000);
            ShowWinDisplay();
        }

        public async void ShowWinDisplay()
        {
            while (windisplay.Opacity < 1)
            {
                windisplay.Opacity += 0.1;
                await Task.Delay(80);
            }
            whitescreen.Fill.Opacity = 0;
        }
    }
}