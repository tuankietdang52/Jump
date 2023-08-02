using System;
using System.Collections.Generic;
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
    public class Bush : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public Rectangle bush = new Rectangle();
        public bool IsShoot = false;
        public Bush()
        {
            height = 80;
            width = 80;
            bulletheight = 10;
            bulletwidth = 30;

            thickness = new Thickness(0, 0, -1000, 0);
            entity = bush;

            pathimgentity = pathpic + "bush.png";
            newpathimg = pathpic + "vietcong.png";
            bulletpath = pathpic + "kar98bullet.png";

            Random IsEnemyRandom = new Random();
            int IsEnemyRandomindex = IsEnemyRandom.Next(2);
            if (IsEnemyRandomindex == 0) IsHarmless = true;
            else IsHarmless = false;

            SetEntity();
        }

        public override async Task Move()
        {
            double pos = entity!.Margin.Right;
            while (pos < 1000)
            {

                if (player!.IsDead) return;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (IsHarmless) continue;

                Showup(pos);

                if (CheckHitPlayer(pos)) return;

                if (getHit)
                {
                    soundplay.Stop();
                    return;
                }
            }
        }

        public async void CreateEnemyBullet(double pos)
        {
            string pathsoundeffect = pathsound + "kar98.mp3";
            string bulletpath = pathpic + "kar98bullet.png";

            EnemyBullet enemyBullet = new EnemyBullet(bulletpath!, player!, playground!, bulletspeed);
            enemyBullet.SetBulletElement(10, 30, pos, 150);
            playground!.Children.Add(enemyBullet.entity);

            Playsound(pathsoundeffect);
            await enemyBullet.BulletMove();
        }

        public void Showup(double pos)
        {
            if (pos >= 200 && IsShoot)
            {
                Thickness newmargin = entity!.Margin;
                newmargin.Bottom = 0;
                thickness = newmargin;

                SetEntity();
            }

            else if (pos >= -500 && !IsShoot)
            {
                Thickness newmargin = entity!.Margin;
                newmargin.Bottom = 80;
                newthickness = newmargin;
                IsShoot = true;

                SetNewEntity(177, 120);
                CreateEnemyBullet(pos + 200);
            }
        }
    }
}
