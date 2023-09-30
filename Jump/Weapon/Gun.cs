using Jump.Weapon.DesertEagle;
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
    public class Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathgun = $"{Directory.GetCurrentDirectory()}\\ListGun.txt";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public PlayerCharacter? player { get; set; }
        public MainWindow? main { get; set; }
        public List<string> listgun = new List<string>();

        public string? gunsoundpath;
        public string? reloadsoundpath;

        public double damage;

        public int bulletspeed;
        public int bulletAmount;
        public int bulletlimit;
        public int magazinebullet;
        public int magazinebulletlimit;

        public Gun()
        {
            listgun.Add("de");
            listgun.Add("m4a4");
            listgun.Add("awp");
        }

        public void setOwnerGun(PlayerCharacter player, MainWindow main)
        {
            this.player = player;
            this.main = main;
        }

        public virtual void getPathPlayer() { }
        public virtual void getSound() { }

        public void PlaySoundShoot()
        {
            MediaPlayer gunsound = new MediaPlayer();
            gunsound.Open(new (gunsoundpath!));
            gunsound.Volume = 1;
            gunsound.Play();
        }

        public void PlayReloadSound()
        {
            MediaPlayer gunsound = new MediaPlayer();
            gunsound.Open(new(reloadsoundpath!));
            gunsound.Volume = 1;
            gunsound.Play();
        }

        public async Task Reload()
        {
            if (magazinebullet <= 0)
            {
                player!.IsReload = false;
                return;
            }

            PlayReloadSound();

            await Task.Delay(1600);

            if (magazinebullet < bulletlimit)
            {
                OutofMag();
                return;
            }

            magazinebullet -= (bulletlimit - bulletAmount);
            bulletAmount = bulletlimit;

            player!.IsReload = false;
        }

        private void OutofMag()
        {
            bulletAmount = magazinebullet;
            magazinebullet = 0;

            player!.IsReload = false;
        }

        public async void HandleReload()
        {
            if (bulletAmount == bulletlimit || player!.IsReload) return;

            player.IsReload = true;

            await Reload();
            main!.AmountBullet();
            
        }
    }
}
