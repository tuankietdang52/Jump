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
    public class Armor : Item
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public Rectangle armor = new Rectangle();
        public Canvas? playground { get; set; }

        public Armor()
        {
            topnum = new double[2] { 140, 275 };

            height = 50;
            width = 50;

            speed = 50;

            left = 1000;

            imgpath = pathpic + "armor.png";

            item = armor;

            RandomTop();
            SetItem();
        }

        public void ShowArmor()
        {
            Rectangle armoricon = new Rectangle()
            {
                Height = 40,
                Width = 40,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "armoricon.png")),
                },
                Name = "ArmorIcon",
            };
            Canvas.SetLeft(armoricon, 55);
            Canvas.SetTop(armoricon, 385);

            playground!.RegisterName(armoricon.Name, armoricon);

            playground.Children.Add(armoricon);
        }
    }
}
