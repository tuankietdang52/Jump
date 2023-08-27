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

        public int basehealth = 3;

        public PirateCaptain()
        {
            height = 169;
            width = 134;

            pathimgentity = pathpic + "piratecaptain.png";

            left = 700;
            top = 195;

            basehealthmob = 3;

            entity = piratecap;

            SetEntity();
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
                if (main!.IsPause)
                {
                    shoottime.Stop();
                    await Task.Delay(1);
                    continue;
                }
                else shoottime.Start();

                if (player!.IsDead || main.IsQuit) return;

                SetHealth(ref health);

                if (CheckHitTime(health)) return;

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
    }
}
