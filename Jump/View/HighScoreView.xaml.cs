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
    public partial class HighScoreView : UserControl
    {
        public MainWindow? main { get; set; }

        public ScoreBar scorebar = new ScoreBar();
        public HighScore highscore = new HighScore();

        public bool IsSaveScore = false;
        public int score { get; set; }
        public MainMenu? mainmenu { get; set; }

        public HighScoreView(MainMenu mainmenu, MainWindow main)
        {
            this.mainmenu = mainmenu;
            this.main = main;

            InitializeComponent();
            UpdateScore();
            CreateButtoninMainMenu();
        }

        public HighScoreView(MainWindow main, int score)
        {
            this.main = main;
            this.score = score;

            InitializeComponent();
            UpdateScore();
            CreateButtonwhenWin();
        }

        public void CreateButtoninMainMenu()
        {
            ButtonQuit();
            HideTypeScore();
        }

        public void CreateButtonwhenWin()
        {
            ButtonQuit();
            CreateButtonSave();
            GetYourScore();
        }

        public void HideTypeScore()
        {
            ScoreType.Visibility = Visibility.Collapsed;
            Save.Visibility = Visibility.Collapsed;
            SaveScoreTxt.Visibility = Visibility.Collapsed;
            scoretxt.Visibility = Visibility.Collapsed;
            yourscoretxt.Visibility = Visibility.Collapsed;
        }

        public void ButtonQuit()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("QUIT", ref Quit, 173, 65);
            if (!main!.IsWin) Quit.Click += HandleQuittoStart;
            else Quit.Click += HandleQuitinWin;
        }

        public void HandleQuittoStart(object sender, RoutedEventArgs e)
        {
            mainmenu!.mainmenu.Children.Remove(this);
        }

        public void HandleQuitinWin(object sender, RoutedEventArgs e)
        {
            main!.IsWin = false;
            main.Quit(this);
        }

        public void CreateButtonSave()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("SAVE", ref Save, 95, 50);
            Save.Click += HandleSave;
        }

        public void GetYourScore()
        {
            yourscoretxt.Text = Convert.ToString(score);
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
            if (!main!.IsWin) main!.KeyDown += main.ReplayKey;
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
            HighScoreList.Items.Clear();
            scorebar.GetScore();
            for (int index = 0; index < scorebar.listhighscore.Count; index++)
            {
                HighScoreList.Items.Add(scorebar.CreateScoreBar(index, HighScoreList.Width - 10));
            }
        }
    }
}
