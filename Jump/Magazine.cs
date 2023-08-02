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
    public class Magazine
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public Rectangle magazine = new Rectangle();
        public bool IsTaken = false;
        public PlayerCharacter? player { get; set; }
        public int posheight;
        public Magazine()
        {
            int[] heightnum = new int[2] { 0, 300 };
            Random posheightrand = new Random();
            int posheightindex = posheightrand.Next(2);
            posheight = heightnum[posheightindex];
            magazine.Height = 40;
            magazine.Width = 40;
            magazine.Margin = new Thickness(0, 0, -1000, posheight);
            magazine.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "magazine.png")),
                Stretch = Stretch.Fill,
            };
        }
        public async Task Move()
        {
            double posmag = magazine.Margin.Right;
            while (magazine.Margin.Right < 900)
            {
                magazine.Margin = new Thickness(0, 0, posmag, posheight);
                TimeSpan magmove = TimeSpan.FromSeconds(0.05);
                await Task.Delay(magmove);
                posmag += 100;
                if (HitPlayer(posmag))
                {
                    IsTaken = true;
                    return;
                }
            }
        }
        public bool HitPlayer(double maghitbox)
        {
            if (magazine.Margin.Bottom <= player!.getHeight() + player!.playershape.Height && magazine.Margin.Bottom >= player!.getHeight() - 150)
            {
                if (maghitbox >= player!.playerhitboxright && maghitbox <= 800)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
