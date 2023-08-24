using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Jump
{
    public class CannonBall : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle cannonball = new Rectangle();

        public CannonBall(double left, PlayerCharacter player, Canvas playground, MainWindow main)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;

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
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height - 5);
            return hitbox;
        }

        public override async Task Action()
        {
            double pos = Canvas.GetLeft(entity);
            while (pos > -30)
            {
                if (player!.IsDead) return;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer())
                {
                    if (!player!.IsDead) break;

                    Explode(pos);
                    return;
                }
            }
            main!.ScoreUp(1);
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }

        public void Explode(double pos)
        {
            string soundpath = pathsound + "cannonexplodesound.mp3";

            Canvas.SetTop(this.entity, top);

            SetNewEntity(200, 200);
            Canvas.SetLeft(this.entity, pos - 50);
            Playsound(soundpath, 1);
        }
    }
}
