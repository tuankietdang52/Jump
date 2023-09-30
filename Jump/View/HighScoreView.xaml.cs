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

namespace Jump.View
{
    public partial class HighScoreView : UserControl
    {
        public MainWindow? main { get; set; }

        public ScoreBar scorebar = new ScoreBar();
        public HighScore highscore = new HighScore();
        public MainMenu? mainmenu { get; set; }
        public HighScoreView(MainMenu mainmenu)
        {
            this.mainmenu = mainmenu;

            InitializeComponent();
            UpdateScore();
            CreateButton();
        }

        public void CreateButton()
        {
            ButtonQuit();
        }

        public void ButtonQuit()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("QUIT", ref Quit, 173, 65);
            Quit.Click += HandleQuit;
        }

        public void HandleQuit(object sender, RoutedEventArgs e)
        {
            mainmenu!.mainmenu.Children.Remove(this);
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
