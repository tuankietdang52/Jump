using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading;
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

namespace Jump.Sql
{
    public class ScoreBar
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";

        public HighScore highscore = new HighScore();
        public List<HighScore.HighScoreOwner> listhighscore = new List<HighScore.HighScoreOwner>();
        public ScoreBar()
        {
            GetScore();
        }

        public void GetScore()
        {
            highscore.GetScore(ref listhighscore);
        }

        public Canvas CreateScoreBar(int index)
        {
            Canvas scorebar = new Canvas()
            {
                Width = 250,
                Height = 40,
                Margin = new Thickness(0, 0, 0, 0),
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "ScoreBg.png")),
                },
            };

            var name = CreateName(index);
            var score = CreateScore(index);

            scorebar.Children.Add(name);
            scorebar.Children.Add(score);
            return scorebar;
        }

        private TextBlock CreateName(int index)
        {
            TextBlock name = new TextBlock()
            {
                Text = listhighscore[index].name,
                Height = 50,
                Width = 160,
                FontSize = 30,
                Foreground = Brushes.LightSkyBlue,
                FontWeight = FontWeights.Bold,
            };

            return name;
        }

        private TextBlock CreateScore(int index)
        {
            TextBlock score = new TextBlock()
            {
                Text = Convert.ToString(listhighscore[index].score),
                Height = 50,
                Width = 90,
                FontSize = 30,
                TextAlignment = TextAlignment.Right,
                Foreground = Brushes.LightSkyBlue,
                FontWeight = FontWeights.Bold,
            };

            Canvas.SetLeft(score, 150);

            return score;
        }
    }
}
