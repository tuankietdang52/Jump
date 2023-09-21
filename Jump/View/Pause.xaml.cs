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
    public partial class Pause : UserControl
    {
        public MainWindow? main { get; set; }

        public Pause() { }
        public Pause(MainWindow main)
        {
            this.main = main;
            InitializeComponent();
            SetButton();
            SetName();
        }

        public void SetName()
        {
            this.Name = "Pause";
            main!.RegisterName(this.Name, this);
        }

        public void SetButton()
        {
            SetButtonResume();
            SetButtonQuit();
        }

        public void SetButtonResume()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("RESUME", ref Resume, 152, 44);
            Resume.Click += HandleResume;
        }

        public void HandleResume(object sender, RoutedEventArgs e)
        {
            main!.Resume(this);
        }

        public void SetButtonQuit()
        {
            CustomButton custom = new CustomButton();
            custom.CreateButton("QUIT", ref Quit, 152, 44);
            Quit.Click += HandleQuit;
        }

        public void HandleQuit(object sender, RoutedEventArgs e)
        {
            main!.Quit(this);
        }
    }
}
