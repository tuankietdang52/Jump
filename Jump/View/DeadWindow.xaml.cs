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
        public HighScore highscore = new HighScore();

        public bool IsSaveScore = false;

        public int mapindex { get; set; }
        public int score { get; set; }

        public DeadWindow()
        {
            InitializeComponent();
            UpdateScore();
        }
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
            CreateButtonSave();
            GetNameMap();
            GetCurrentScore();
            GetKillerImg();
            SetGun();
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

        public void SetGun()
        {
            string namegun = main!.player.gun.getPathGun();
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
            Quit.Click += HandleQuit;
        }

        public void HandleQuit(object sender, RoutedEventArgs e)
        {
            main!.Quit(this);
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

        public void CreateButtonSave()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("SAVE", ref Save, 95, 50);
            Save.Click += HandleSave;
        }

        public void NonEditScoreType()
        {
            ScoreType.IsReadOnly = true;
            SaveScoreTxt.Text = "Score Saved";
            SaveScoreTxt.Foreground = Brushes.LightGreen;
        }

        private void TypeText(object sender, KeyboardFocusChangedEventArgs e)
        {
            main!.KeySettingOff();
            main.KeyDown -= main.ReplayKey;
        }

        private void DoneType(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);
            main!.KeyDown += main.ReplayKey;
        }

        public void HandleSave(object sender, RoutedEventArgs e)
        {
            if (IsSaveScore) return;
            if (ScoreType.Text.Length >= 8)
            {
                MessageBox.Show("adu man?");
                return;
            }

            IsSaveScore = true;
            NonEditScoreType();

            highscore.InsertScore(ScoreType.Text, score);
            UpdateScore();
        }

        public void UpdateScore()
        {
            ListScore.Items.Clear();
            scorebar.GetScore();
            for (int index = 0; index < scorebar.listhighscore.Count; index++)
            {
                ListScore.Items.Add(scorebar.CreateScoreBar(index, 250));
            }
        }
    }
}
