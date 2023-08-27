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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump
{
    public class Bullet
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";

        public double posout { get; set; }
        public double speed { get; set; }

        public string? gunsoundpath;

        public List<Entity>? entities { get; set; }
        public Rectangle bullet = new Rectangle();
        public MediaPlayer gunsound = new MediaPlayer();
        public Gun gun = new Gun();
        public MainWindow? main { get; set; }
        public PlayerCharacter? player { get; set; }

        public Bullet()
        {
            bullet.Height = 10;
            bullet.Width = 30;
            bullet.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "bullet.png")),
                Stretch = Stretch.Fill,
            };
        }

        public void setPosition()
        {
            var left = Canvas.GetLeft(player!.playershape);

            Canvas.SetLeft(bullet, left + 80);

            var top = Canvas.GetTop(player!.playershape);
            
            if (top >= 250) Canvas.SetTop(bullet, posout + 30);
            else Canvas.SetTop(bullet, posout + 50);
        }

        public void PlayShootSound()
        {
            gun.getGunsound(ref gunsoundpath!, player!.indexgun);
            gunsound.Open(new (gunsoundpath!));
            gunsound.Volume = 1;
            gunsound.Play();
        }

        public async Task Move()
        {
            speed = gun.getBulletspeed(player!.indexgun);
            double bulletpos = Canvas.GetLeft(bullet);

            while (bulletpos < 1000)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player.IsDead || main.IsQuit) return;

                Canvas.SetLeft(bullet, bulletpos);
                TimeSpan bulletmove = TimeSpan.FromSeconds(0.007);
                await Task.Delay(bulletmove);
                bulletpos += speed;

                if (HitEntity())
                {
                    if (!player.IsHaveAwp) return;
                }
            }
        }

        public bool HitEntity()
        {
            if (entities!.Count == 0)
            {
                return false;
            }

            var left = Canvas.GetLeft(bullet);
            var top = Canvas.GetTop(bullet);
            Rect bullethitbox = new Rect(left, top, bullet.Width, bullet.Height);

            foreach (var entity in entities)
            {
                if (entity.IsHarmless) continue;
                if (entity.IsBullet) continue;

                var entityhitbox = entity.getHitbox();
                var entitytop = entityhitbox.Top;
                var entityleft = entityhitbox.Left;
                var entityheight = entityhitbox.Height;


                if (bullethitbox.IntersectsWith(entityhitbox))
                {
                    entity.getHit = true;
                    return true;
                }
                else if (left >= entityleft)
                {
                    if (top >= entitytop && top <= entitytop + entityheight)
                    {
                        entity.getHit = true;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
