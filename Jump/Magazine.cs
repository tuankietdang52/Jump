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
        public double posheight;
        public Magazine()
        {
            double[] heightnum = new double[2] { 150, 285 };
            Random posheightrand = new Random();

            int posheightindex = posheightrand.Next(2);
            posheight = heightnum[posheightindex];

            magazine.Height = 40;
            magazine.Width = 40;

            Canvas.SetLeft(magazine, 1000);
            Canvas.SetTop(magazine, posheight);

            magazine.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "magazine.png")),
                Stretch = Stretch.Fill,
            };
        }
        public async Task Move()
        {
            double posmag = Canvas.GetLeft(magazine);
            while (posmag > -30)
            {
                Canvas.SetLeft(magazine, posmag);

                TimeSpan magmove = TimeSpan.FromSeconds(0.05);
                await Task.Delay(magmove);

                posmag -= 50;

                if (HitPlayer())
                {
                    IsTaken = true;
                    return;
                }
            }
        }
        public bool HitPlayer()
        {
            var playershape = player!.playershape;
            var playerleft = Canvas.GetLeft(playershape);
            var playertop = Canvas.GetTop(playershape);
            
            var magleft = Canvas.GetLeft(magazine);
            var magtop = Canvas.GetTop(magazine);

            player.playerhitbox = new Rect(playerleft, playertop, playershape.Width - 50, playershape.Height);
            Rect maghitbox = new Rect(magleft, magtop, magazine.Width, magazine.Height);

            if (player.playerhitbox.IntersectsWith(maghitbox)) return true;

            return false;
        }
    }
}
