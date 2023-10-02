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
using static System.Formats.Asn1.AsnWriter;

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

        public Border CreateScoreBar(int index, double width)
        {
            Border scorebar = new Border();
            scorebar.BorderBrush = Brushes.Red;
            scorebar.BorderThickness = new Thickness(5);

            Canvas scorebarcontent = new Canvas()
            {
                Width = width,
                Height = 40,
            };


            var name = CreateName(index, width);
            var score = CreateScore(index, width);

            scorebarcontent.Children.Add(name);
            scorebarcontent.Children.Add(score);

            scorebar.Child = scorebarcontent;

            return scorebar;
        }

        private TextBlock CreateName(int index, double width)
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

        private TextBlock CreateScore(int index, double width)
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

            Canvas.SetLeft(score, width - 100);

            return score;
        }
    }
}
