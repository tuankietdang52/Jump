using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
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
    public class MagicCircleShoot : MagicCircle
    {
        public bool IsShoot = false;

        public MagicCircleShoot(PlayerCharacter player, Canvas playground, MainWindow main)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;

            pathimgentity = pathpic + "magiccircleshoot.png";

            maxheight = 180;
            maxwidth = 180;

            entity = magiccircle;

            SetPosCircleShoot();
        }

        public void SetPosCircleShoot()
        {
            Random randleft = new Random();
            left = randleft.Next(750, 851);

            Random randtop = new Random();
            top = randtop.Next(50, 190);

            SetEntity();
        }

        public void SetMagicMissile(ref MagicMissile magicmissile)
        {
            magicmissile.boss = this.boss;

            magicmissile.pathimgentity = pathpic + "magicmissile2.png";
            magicmissile.SetEntity();
            magicmissile.soundpath = pathsound + "magicshot.mp3";
        }

        public async void Shoot()
        {
            var posleft = Canvas.GetLeft(this.entity) + 100;
            var postop = Canvas.GetTop(this.entity) + 90;

            MagicMissile magicmissile = new MagicMissile(posleft, postop, player!, playground!, main!);

            SetMagicMissile(ref magicmissile);

            main!.entities.Add(magicmissile);
            playground!.Children.Add(magicmissile.entity);

            IsShoot = true;
            await magicmissile.Action();
        }

        public override void Morph()
        {
            entity!.Height += 20;
            entity!.Width += 20;
        }

        public override void Disappear()
        {
            entity!.Height -= 10;
            entity!.Width -= 10;

            if (entity.Height <= 0 && entity.Width <= 0) IsDead = true;
        }

        public async override Task Action()
        {
            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);

            string sound = pathsound + "magiccircleshoot.mp3";

            Playsound(sound, 0.5);

            while (!IsDead)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main!.IsQuit) break;
                if (boss!.IsDead) break;

                RotateCircle();
                await Task.Delay(50);

                if (entity!.Height <= maxheight && !IsReady) Morph();
                else IsReady = true;

                if (!IsReady) continue;

                if (!IsShoot) Shoot();

                Disappear();
            }

            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}
