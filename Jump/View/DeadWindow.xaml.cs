using Jump.Sql;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Jump.View
{
    public partial class DeadWindow : UserControl
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public MainWindow? main { get; set; }
        public ScoreBar scorebar = new ScoreBar();

        public int mapindex { get; set; }
        public int score { get; set; }

        public DeadWindow() { }
        public DeadWindow(MainWindow main, int mapindex, int score)
        {
            this.main = main;
            this.mapindex = mapindex;
            this.score = score;

            InitializeComponent();
            UpdateScore();
            SetWindow();
            ShowWindow();
        }

        public async void ShowWindow()
        {
            Dead.Opacity = 0;
            while (Dead.Opacity < 1)
            {
                Dead.Opacity += 0.1;
                await Task.Delay(80);
            }
        }

        public void SetWindow()
        {
            CreateButtonQuit();
            CreateButtonReplay();
            GetNameMap();
            GetCurrentScore();
            GetKillerImg();
        }

        public void GetCurrentScore()
        {
            Score.Text = "Score: " + score;
        }

        public void GetKillerImg()
        {
            string path = main!.killer.pathimgentity!;
            KillerImg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(path)),
                Stretch = Stretch.Fill,
            };
        }

        public void CreateButtonQuit()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("QUIT", ref Quit, 135, 50);
        }

        public void GetNameMap()
        {
            string mapname;
            switch (mapindex)
            {
                case < 3:
                    mapname = "Galaxy";
                    UpdateMap(mapname, Brushes.Purple);
                    break;

                case 3:
                    mapname = "Luxury Castle";
                    UpdateMap(mapname, Brushes.Yellow);
                    break;

                case < 6:
                    mapname = "Jungle";
                    UpdateMap(mapname, Brushes.LightGreen);
                    break;

                case 6:
                    mapname = "Shrine";
                    UpdateMap(mapname, Brushes.Red);
                    break;

                case < 9:
                    mapname = "Village";
                    UpdateMap(mapname, Brushes.Green);
                    break;

                case 9:
                    mapname = "Astrolab";
                    UpdateMap(mapname, Brushes.Violet);
                    break;
            }
        }

        public void UpdateMap(string mapname, Brush color)
        {
            Map.Text = "Map: " + mapname;
            Map.Foreground = color;
        }

        public void CreateButtonReplay()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("REPLAY", ref Replay, 135, 50);
        }

        public void UpdateScore()
        {
            ListScore.Items.Clear();
            for (int index = 0; index < scorebar.listhighscore.Count; index++)
            {
                ListScore.Items.Add(scorebar.CreateScoreBar(index));
            }
        }
    }
}
