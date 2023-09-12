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
    public class MagicBall : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle magicball = new Rectangle();
        public Entity? boss { get; set; }

        public double angle = 360;

        public MagicBall(PlayerCharacter player, Canvas playground, MainWindow main)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;

            height = 200;
            width = 200;

            pathimgentity = pathpic + "magicball.png";

            left = 1000;
            top = 120;

            movementspeed = 45;
            basehealthmob = 2;

            entity = magicball;

            SetEntity(); 
        }


        public void SpinAround()
        {
            if (angle == 0) angle = 360;

            angle -= 5;
            entity!.LayoutTransform = new RotateTransform(angle);
        }

        public void KnockUp(ref double postop)
        {
            postop -= 15;
            Canvas.SetTop(this.entity, postop);
        }

        public async override Task Action()
        {
            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);
            int health = basehealthmob;

            string soundpath = pathsound + "magicballsound.mp3";
            Playsound(soundpath, 0.4);

            while (pos > -100)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main!.IsQuit) break;
                if (boss!.IsDead) break;

                if (CheckHitTime(health)) break;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);
                SpinAround();

                if (pos >= 50 && pos <= 300) KnockUp(ref postop);

                if (CheckHitPlayer()) break;
            }
            playground!.Children.Remove(this.entity);
            main!.entities.Remove(this);
        }
    }
}
