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
            bullet.Margin = new Thickness(0, 0, 500, posout + 30);
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
            double bulletpos = bullet.Margin.Right;
            while (bullet.Margin.Right > -1000)
            {
                bullet.Margin = new Thickness(0, 0, bulletpos, posout + 30);
                TimeSpan bulletmove = TimeSpan.FromSeconds(0.007);
                await Task.Delay(bulletmove);
                bulletpos -= speed;
                if (HitEntity(bulletpos))
                {
                    return;
                }
            }
        }
        public bool HitEntity(double bulletpos)
        {
            if (entities!.Count == 0)
            {
                return false;
            }
            for (int entityindex = 0; entityindex < entities.Count; entityindex++)
            {
                if (bullet.Margin.Bottom >= entities[entityindex].getHitboxHeight() - 60 && bullet.Margin.Bottom <= entities[entityindex].getHitboxHeight() + 60)
                {
                    if (bulletpos <= entities[entityindex].getHitbox())
                    {
                        if (entities[entityindex].IsHarmless) return false;
                        entities[entityindex].getHit = true;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
