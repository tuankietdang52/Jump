using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
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
    public class PirateCaptain : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle piratecap = new Rectangle();

        public int hittime = 0;
        public int basehealth = 3;

        public bool IsBuff = false;

        public PirateCaptain()
        {
            height = 169;
            width = 134;

            pathimgentity = pathpic + "piratecaptain.png";

            left = 700;
            top = 195;

            entity = piratecap;

            SetEntity();
        }

        public void SetHealth(ref int health)
        {
            if (main!.IsHaveAspotate && !IsBuff)
            {
                IsBuff = true;
                health *= 2;
            }
            else if (!main.IsHaveAspotate)
            {
                IsBuff = false;
                health = basehealth;
            }
        }

        public bool DeadCheck(int health)
        {
            if (hittime >= health)
            {
                getHit = true;
                IsDead = true;
                return true;
            }
            else if (getHit)
            {
                getHit = false;
                hittime++;
            }
            return false;
        }

        public override async Task Action()
        {
            main!.IsSpawnPirate = true;
            double pos = Canvas.GetLeft(this.entity);

            Stopwatch shoottime = new Stopwatch();

            shoottime.Start();
            int health = basehealth;

            while (!IsDead)
            {
                if (player!.IsDead) return;
                
                SetHealth(ref health);

                if (DeadCheck(health)) return;

                await Task.Delay(1);

                if (shoottime.Elapsed.Seconds == 2)
                {
                    shoottime.Restart();
                    CreateCannonBullet(pos - 30);
                }
            }
            main!.IsSpawnPirate = false;
        }

        public async void CreateCannonBullet(double left)
        {
            string pathsoundeffect = pathsound + "cannonshot.mp3";

            CannonBall cannonball = new CannonBall(left, player!, playground!, main!);

            cannonball.movementspeed = bulletspeed;

            main!.entities.Add(cannonball);
            playground!.Children.Add(cannonball.entity);

            Playsound(pathsoundeffect, 1);
            await cannonball.Action();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height);
            return hitbox;
        }
    }
}
