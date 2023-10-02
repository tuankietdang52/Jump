using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Jump.EnemyEntity
{
    public class CannonBall : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle cannonball = new Rectangle();

        public CannonBall(double left, PlayerCharacter player, Canvas playground, MainWindow main, PirateCaptain owner)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;
            this.owner = owner;

            height = 60;
            width = 60;

            top = 290;
            newtop = 210;

            this.left = left;

            pathimgentity = pathpic + "cannonball.png";
            newpathimg = pathpic + "cannonexplode.png";

            entity = cannonball;

            IsBullet = true;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 40, height - 5);
            return hitbox;
        }

        public override async Task Action()
        {
            double pos = Canvas.GetLeft(entity);
            while (pos > -30)
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
                    if (!player!.IsDead) break;

                    string soundpath = pathsound + "cannonexplodesound.mp3";
                    Explode(pos, soundpath);
                    return;
                }
            }
            if (!player!.IsDead) main!.ScoreUp(1);
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}
