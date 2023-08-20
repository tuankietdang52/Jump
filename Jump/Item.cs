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
    public class Item
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";

        public bool IsTaken = false;

        public double top;

        public double[]? topnum { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int speed { get; set; }

        public double left { get; set; }
        public string? imgpath { get; set; }

        public Rectangle? item { get; set; }
        public PlayerCharacter? player { get; set; }

        public Item() { }

        public void RandomTop()
        {
            Random toprand = new Random();
            int topindex = toprand.Next(2);

            top = topnum![topindex];
        }

        public void SetItem()
        {
            item!.Height = height;
            item!.Width = width;

            Canvas.SetLeft(item, left);
            Canvas.SetTop(item, top);

            item.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(imgpath!)),
                Stretch = Stretch.Fill,
            };
        }

        public virtual async Task Move()
        {
            double posmag = Canvas.GetLeft(item);
            while (posmag > -30)
            {
                Canvas.SetLeft(item, posmag);

                TimeSpan magmove = TimeSpan.FromSeconds(0.05);
                await Task.Delay(magmove);

                posmag -= speed;

                if (HitPlayer())
                {
                    IsTaken = true;
                    return;
                }
            }
        }

        public bool HitPlayer()
        {
            if (player!.IsDead) return false;

            var playershape = player!.playershape;
            var playerleft = Canvas.GetLeft(playershape);
            var playertop = Canvas.GetTop(playershape);

            var magleft = Canvas.GetLeft(item);
            var magtop = Canvas.GetTop(item);

            player.playerhitbox = new Rect(playerleft, playertop, playershape.Width - 50, playershape.Height);
            Rect maghitbox = new Rect(magleft, magtop, item!.Width, item.Height);

            if (player.playerhitbox.IntersectsWith(maghitbox)) return true;

            return false;
        }

    }
}
