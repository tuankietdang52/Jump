using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump
{
    public class Kamikaze : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        
        public Rectangle kamikaze = new Rectangle();

        public bool up = false;
        public bool down = true;

        public int wingindex = 0;

        public Kamikaze()
        {
            width = 50;
            height = 50;

            left = 1000;
            top = 50;

            pathimgentity = pathpic + "kamikaze.png";

            entity = kamikaze;

            SetEntity();
        }

        public void CheckFly(double posheight)
        {
            if (posheight >= 290)
            {
                up = true;
                down = false;
            }
            if (posheight <= 50)
            {
                down = true;
                up = false;
            }
        }

        public void ChangePositionHeight(ref double posheight)
        {
            Canvas.SetTop(this.entity, posheight);

            if (down && !up) posheight += 30;
            if (up && !down)
            {
                posheight -= 30;
            } 
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height);
            return hitbox;
        }

        public void Fly()
        {
            newleft = Canvas.GetLeft(this.entity);
            newtop = Canvas.GetTop(this.entity);

            if (wingindex == 0)
            {
                newpathimg = pathpic + "kamikaze1.png";
                wingindex = 1;
            }
            else
            {
                newpathimg = pathpic + "kamikaze.png";
                wingindex = 0;
            }

            SetNewEntity(50, 50);
        }

        public override async Task Action()
        {
            string soundpath = pathsound + "kamikazesound.mp3";
            Playsound(soundpath, 100);

            var pos = Canvas.GetLeft(this.entity);
            var posheight = Canvas.GetTop(this.entity);
        
            while (pos > -30)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main.IsQuit) return;

                if (CheckHitPlayer())
                {
                    if (!player!.IsDead) break;

                    newtop = 210;
                    newpathimg = pathpic + "cannonexplode.png";
                    Explode(pos);
                    return;
                }

                if (getHit)
                {
                    IsDead = true;
                    return;
                }

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);
        
                Fly();

                ChangePositionMove(ref pos);
                CheckFly(posheight);
                ChangePositionHeight(ref posheight);
        
            }

            if (!player!.IsDead && !IsDead) main!.ScoreUp(1);
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}
