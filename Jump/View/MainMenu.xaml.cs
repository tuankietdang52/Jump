using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Jump.View
{
    public partial class MainMenu : UserControl
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public MainWindow? main { get; set; }
        
        public MainMenu() { }
        public MainMenu(MainWindow main)
        {
            this.main = main;
            InitializeComponent();
            CreateButton();
        }

        public void CreateButton()
        {
            ButtonStart();
        }

        public void ButtonStart()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("START", ref Start);
        }

        public void HandlePreStartGame(object sender, RoutedEventArgs e)
        {
            main!.PreStartGame();
            main.Playground.Children.Remove(this);
        }
    }
}
