using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace Jump.EnemyEntity
{
    public class Bush : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public Rectangle bush = new Rectangle();
        public bool IsShoot = false;
        public bool DoneShoot = false;
        public Bush()
        {
            height = 80;
            width = 80;
            bulletheight = 10;
            bulletwidth = 30;

            left = 1000;
            top = 275;

            newtop = 190;

            pathimgentity = pathpic + "bush.png";
            newpathimg = pathpic + "vietcong.png";
            bulletpath = pathpic + "kar98bullet.png";

            entity = bush;

            Random IsEnemyRandom = new Random();
            int IsEnemyRandomindex = IsEnemyRandom.Next(2);

            if (IsEnemyRandomindex == 0) IsHarmless = true;
            else IsHarmless = false;

            SetEntity();
        }

        public override async Task Action()
        {
            double pos = Canvas.GetLeft(this.entity);
            while (pos > -30)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main!.IsQuit) return;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (IsHarmless) continue;

                Shoot(pos);

                if (getHit)
                {
                    IsDead = true;
                    soundplay.Stop();
                    return;
                }

                if (CheckHitPlayer())
                {
                    if (!player.IsDead) return;
                    player.IsVietCongKilled = true;
                    await RenderPicture();
                    return;
                }

            }
        }

        public void Shoot(double pos)
        {
            if (pos <= 500) GetDown(pos);
            else if (pos <= 800) Showup(pos);
        }

        public async Task RenderPicture()
        {
            for (int i = 1; i <= 3; i++)
            {
                newpathimg = pathpic + "vietcongkill" + Convert.ToString(i) + ".png";
                SetNewEntity(177, 120);
                await Task.Delay(200);
            }
        }

        public async void CreateKar98Bullet(double left)
        {
            string pathsoundeffect = pathsound + "kar98.mp3";

            Kar98Bullet kar98bullet = new Kar98Bullet(left, player!, playground!, main!);

            kar98bullet.movementspeed = bulletspeed;

            main!.entities.Add(kar98bullet);
            playground!.Children.Add(kar98bullet.entity);

            Playsound(pathsoundeffect, 0.4);
            await kar98bullet.Action();
        }

        public void Showup(double pos)
        {
            if (!IsShoot)
            {
                IsShoot = true;

                SetNewEntity(177, 120);
                CreateKar98Bullet(pos - 30);
            }
        }

        public void GetDown(double pos)
        {
            if (IsShoot)
            {
                left = Canvas.GetLeft(this.entity);
                SetEntity();
                DoneShoot = true;
            }
        }
    }
}
