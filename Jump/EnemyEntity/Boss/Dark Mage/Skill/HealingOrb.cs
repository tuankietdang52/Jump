using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
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

namespace Jump.EnemyEntity
{
    public class HealingOrb : Entity
    {
        public MagicCircleHeal circle { get; set; }
        public Entity boss { get; set; }

        public Rectangle healingorb = new Rectangle();

        public double angle = 0;
        public int orbindex { get; set; }

        public bool IsGetAngle = false;
        public bool IsShoot = false;

        public readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public HealingOrb(PlayerCharacter player, Canvas playground, MainWindow main, Entity boss, MagicCircleHeal circle)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;
            this.boss = boss;
            this.circle = circle;

            height = 50;
            width = 50;

            pathimgentity = pathpic + "healingorb.png";

            SetOrb();
        }

        public void SetOrb()
        {
            left = Canvas.GetLeft(this.circle.entity);
            top = Canvas.GetTop(this.circle.entity);

            entity = healingorb;

            SetEntity();
        }

        public double GetAngle()
        {
            switch (orbindex)
            {
                case 0:
                    return 3 * Math.PI / 4;

                case 1:
                    return Math.PI / 4;

                case 2:
                    return 3 * Math.PI / 2;

                case 3:
                    return 0;

                default:
                    return 0;
            }
        }

        public void MovingOrb(ref int index)
        {
            var center = new Rect(Canvas.GetLeft(this.boss.entity) + 30, Canvas.GetTop(this.boss.entity) + 50, this.boss.entity!.ActualWidth, this.boss.entity.ActualHeight);

            if (!IsGetAngle)
            {
                angle = GetAngle();
                IsGetAngle = true;
            }
            else angle += 0.2;
            if (angle == 2 * Math.PI) angle = 0;


            left = center.X + Math.Cos(angle) * 150;
            top = center.Y + Math.Sin(angle) * 150;

            Canvas.SetLeft(this.entity, left);
            Canvas.SetTop(this.entity, top);

            CheckOrb(ref index);

            if (left <= center.X - 50 && top <= center.Y - 50)
            {
                if (!IsShoot) Shoot();
                IsShoot = true;
            }
            else if (left <= center.X + 150)
            {
                IsShoot = false;
            }
        }

        public void RemoveOrb()
        {
            circle.healingorb.Remove(this);
            main!.entities.Remove(this);
            playground!.Children.Remove(this.entity);
        }

        public void CheckOrb(ref int index)
        {
            if (this.getHit)
            {
                RemoveOrb();
                index--;
            }
        }

        public void SetMagicMissile(ref MagicMissile magicmissile)
        {
            magicmissile.boss = this.boss;

            magicmissile.pathimgentity = pathpic + "magicmissile3.png";
            magicmissile.SetEntity();
            magicmissile.movementspeed = 30;

            magicmissile.soundpath = pathsound + "magicshot2.mp3";
        }

        public async void Shoot()
        {
            var posleft = Canvas.GetLeft(this.entity) + 25;
            var postop = Canvas.GetTop(this.entity) - 10;

            MagicMissile magicmissile = new MagicMissile(posleft, postop, player!, playground!, main!);

            SetMagicMissile(ref magicmissile);

            main!.entities.Add(magicmissile);
            playground!.Children.Add(magicmissile.entity);

            await magicmissile.Action();
        }
    }
}
