using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
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
    public class Mac10Bullet : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";

        public Rectangle mac10bullet = new Rectangle();

        public Mac10Bullet(double left, double top, PlayerCharacter player, Canvas playground, MainWindow main)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;

            height = 30;
            width = 30;

            movementspeed = 70;

            this.left = left;
            this.top = top;

            pathimgentity = pathpic + "mac10bullet.png";

            entity = mac10bullet;

            IsBullet = true;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height - 5);
            return hitbox;
        }

        public override async Task Action()
        {
            double pos = Canvas.GetLeft(entity);
            while (pos > 0)
            {
                if (player!.IsDead) return;

                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer())
                {
                    if (!player.IsDead) break;

                    playground!.Children.Remove(entity);
                    return;
                }
            }
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}
