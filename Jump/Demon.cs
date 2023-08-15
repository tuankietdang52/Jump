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

namespace Jump
{
    public class Demon : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public Rectangle demon = new Rectangle();
        public MediaPlayer mortissound = new MediaPlayer();
        public Demon()
        {
            height = 70;
            width = 50;
            thickness = new Thickness(0, 0, -1000, 200);
            pathimgentity = new(pathpic + "demonleft.png");
            mortissound.Open(new(pathsound + "mortis.mp3"));
            entity = demon;
            IsDemon = true;
            SetEntity();
        }

        public override void DemonTurn()
        {
            mortissound.Volume = 1;
            mortissound.Play();

            if (turn == 0)
            {
                entity!.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "demonleft.png")),
                    Stretch = Stretch.Fill,
                };
                turn = 1;
            }
            else
            {
                entity!.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "demonright.png")),
                    Stretch = Stretch.Fill,
                };
                turn = 0;
            }
        }
    }
}
