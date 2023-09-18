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
        public MediaPlayer theme = new MediaPlayer();

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
            SetName();
        }

        public void SetName()
        {
            this.Name = "DeadWindow";
            main!.RegisterName(this.Name, this);
        }

        public async void ShowWindow()
        {
            PlayTheme("Melody of Guidance.mp3");
            Dead.Opacity = 0;
            while (Dead.Opacity < 1)
            {
                Dead.Opacity += 0.1;
                await Task.Delay(80);
            }
        }

        public void PlayTheme(string themedead)
        {
            theme.Open(new(pathsound + themedead));
            theme.Volume = 1;
            theme.Play();
        }

        public void SetWindow()
        {
            CreateButtonQuit();
            CreateButtonReplay();
            GetNameMap();
            GetCurrentScore();
            GetKillerImg();
            GetGun();
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

        public void GetGun()
        {
            string namegun = "de";
            switch (main!.player.indexgun)
            {
                case 0:
                    namegun = "de";
                    break;

                case 1:
                    namegun = "m4a4";
                    break;

                case 2:
                    namegun = "awp";
                    break;

                default:
                    break;

            }
            SetGun(namegun);
        }

        public void SetGun(string namegun)
        {
            namegun = pathpic + namegun + ".png";
            Gun.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(namegun)),
                Stretch = Stretch.Fill,
            };
        }

        public void CreateButtonQuit()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("QUIT", ref Quit, 135, 50);
        }

        public void CreateButtonReplay()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("REPLAY", ref Replay, 135, 50);
            Replay.Click += HandleReplay;
        }

        public void HandleReplay(object sender, RoutedEventArgs e)
        {
            main!.Replay(this);
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
