using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump.EnemyEntity
{
    public class Kar98Bullet : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public Rectangle kar98bullet = new Rectangle();
        public Kar98Bullet(double left, PlayerCharacter player, Canvas playground, MainWindow main, Bush owner)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;
            this.owner = owner;

            height = 10;
            width = 30;

            top = 250;
            this.left = left;

            pathimgentity = pathpic + "kar98bullet.png";

            entity = kar98bullet;

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
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main.IsQuit) return;

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
