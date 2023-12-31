﻿using Jump.Weapon.Weapon_Type;
using System;
using System.Collections.Generic;
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
using Jump.EnemyEntity;

namespace Jump
{
    public class PlayerCharacter
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle playershape = new Rectangle();
        public MainWindow? main { get; set; }

        public MediaPlayer voicedead = new MediaPlayer();
        public MediaPlayer winvoice = new MediaPlayer();
        public MediaPlayer voice = new MediaPlayer();

        public Gun gun = new Gun();

        public List<string> inventory = new List<string>();

        public Rect playerhitbox;

        public int indexgun = 0;
        public double damage;

        public bool IsVietCongKilled = false;
        public bool IsDefaultDead = false;
        public bool IsHaveArmor = false;
        public bool IsHaveSniper = false;
        public bool IsDead = false;
        public bool IsR8 = false;

        public bool IsJump = false;
        public bool IsHoldCtrlLeft = false;
        public bool IsCrouch = false;
        public bool IsReload = false;
        public bool IsShoot = false;

        public int playerhitboxright { get; }
        public bool crouch { get; set; }
        public bool jump { get; set; }
        public string? stand { get; set; }
        public string? standshoot { get; set; }
        public string? jumpgun { get; set; }
        public string? jumpshoot { get; set; }
        public string? crouchshoot { get; set; }
        public string? currentgun { get; set; }

        public PlayerCharacter()
        {
            setElement(137, 86);

            ChangeGun("de");
            inventory.Add("de");

            playershape.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(stand!)),
                Stretch = Stretch.Fill,
            };

            setDefault();
        }

        public void Restart()
        {
            setDefault();

            if (IsDead || main!.IsQuit || main.IsReplay)
            {
                IsHaveArmor = false;
                IsHaveSniper = false;
                ChangeGun("de");
            }
            IsDead = false;
            IsVietCongKilled = false;

            IsJump = false;
            IsShoot = false;
            IsCrouch = false;
            IsReload = false;

            Default();
        }

        public void RestartInventory()
        {
            indexgun = 0;

            inventory.Clear();
            inventory.Add("de");
            gun.getPathPlayer();
        }

        // GET SET //

        public void setDefault()
        {
            Canvas.SetLeft(playershape, 130);
            Canvas.SetTop(playershape, 215);
        }

        public void setDefaultDead()
        {
            Canvas.SetLeft(playershape, 130);
            Canvas.SetTop(playershape, 270);
        }

        // SOUND AND VOICE //

        private void PlayDeadVoice(string path)
        {
            voicedead.Open(new(path));
            voicedead.Volume = 1;
            voicedead.Play();
        }

        public void GetVoiceDead()
        {
            Random voicedead = new Random();
            int voicedeadindex = voicedead.Next(1, 3);
            string deadvoice = pathsound + "voicedead" + voicedeadindex + ".mp3";
            PlayDeadVoice(deadvoice);
        }

        private void PlayWinVoice(string path)
        {
            winvoice.Open(new(path));
            winvoice.Volume = 1;
            winvoice.Play();
        }

        public void GetVoiceWin()
        {
            Random voicewin = new Random();
            int voicewinindex = voicewin.Next(1, 6);
            string winvoice = pathsound + "voicewin" + voicewinindex + ".mp3";
            PlayWinVoice(winvoice);
        }

        public void VoicePlay(string path)
        {
            voice.Open(new(path));
            voice.Volume = 1;
            voice.Play();
        }

        public void VoiceStart()
        {
            Random voicestart = new Random();
            int voicestartindex = voicestart.Next(1, 10);
            string pathvoicestart = pathsound + "voicestart" + voicestartindex + ".mp3";

            VoicePlay(pathvoicestart);
        }

        // SET PLAYER //

        public void setElement(int height, int width)
        {
            playershape.Height = height;
            playershape.Width = width;
        }

        private void ChangeSprite(string path)
        {
            playershape.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(path)),
                Stretch = Stretch.Fill,
            };
        }

        public void Default()
        {
            if (IsDead) return;

            var top = Canvas.GetTop(playershape);
            string pathimgcharcrouch = pathpic + "playercrouch.png";

            setElement(137, 86);

            if (top >= 250)
            {
                setElement(90, 76);
                ChangeSprite(pathimgcharcrouch);
                return;
            }

            ChangeSprite(stand!);
        }

        // WEAPON //

        public void ChangeGun(string gunname)
        {
            switch (gunname)
            {
                case "de":
                    DesertEagle de = new DesertEagle(this, main!);
                    gun = de;
                    indexgun = 0;
                    break;

                case "m4a4":
                    M4A4 m4a4 = new M4A4(this, main!);
                    gun = m4a4;
                    indexgun = 1;
                    break;

                case "awp":
                    AWP awp = new AWP(this, main!);
                    gun = awp;
                    indexgun = 2;
                    IsHaveSniper = true;
                    break;

                case "m4a1s":
                    M4A1S m4a1s = new M4A1S(this, main!);
                    gun = m4a1s;
                    indexgun = 3;
                    break;

                case "ak47":
                    AK47 ak47 = new AK47(this, main!);
                    gun = ak47;
                    indexgun = 4;
                    break;

                case "ssg08":
                    SSG08 ssg08 = new SSG08(this, main!);
                    gun = ssg08;
                    indexgun = 5;
                    IsHaveSniper = true;
                    break;

                case "r8":
                    R8 r8 = new R8(this, main!);
                    gun = r8;
                    indexgun = 6;
                    IsR8 = true;
                    break;
            }

            gun.getPathPlayer();
            gun.getSound();
            currentgun = gunname;
            if (indexgun != 2 && indexgun != 5) IsHaveSniper = false;
            if (indexgun != 6) IsR8 = false;
        }



        // HANDLE ACTION //

        public void HandleJump()
        {
            if (IsJump) return;

            IsJump = true;

            setDefault();
            jump = true;

            Jump();
        }

        public void HandleCrouch()
        {
            IsHoldCtrlLeft = true;

            if (IsJump) return;

            setDefault();

            IsCrouch = true;
            crouch = true;
            Crouch();
        }

        private void CreateBullet()
        {
            Bullet bullet = new Bullet()
            {
                entities = this.main!.entities,
                player = this,
                main = this.main!,
                posout = Canvas.GetTop(playershape),
            };

            bullet.setPosition();

            main.Playground.Children.Add(bullet.bullet);
            Shoot(bullet, main.Playground);
        }

        public void HandleShoot()
        {
            if (IsReload) return;

            if (gun.bulletAmount <= 0) return;

            if (IsShoot) return;

            IsShoot = true;
            gun.bulletAmount--;

            main!.AmountBullet();

            CreateBullet();
        }

        public async void HandleR8Shoot()
        {
            if (IsReload) return;

            if (gun.bulletAmount <= 0) return;

            if (IsShoot) return;

            IsShoot = true;

            gun.OtherGunSound();
            await Task.Delay(300);

            gun.bulletAmount--;

            main!.AmountBullet();

            CreateBullet();
        }

        public async void HandleSniperShoot()
        {
            if (IsReload) return;

            if (gun.bulletAmount <= 0) return;

            if (IsShoot) return;

            IsShoot = true;
            gun.bulletAmount--;

            main!.AmountBullet();

            CreateBullet();

            await Task.Delay(500);
            gun.OtherGunSound();

            await Task.Delay(gun.waittime);
            IsShoot = false;
        }

        // ACTION //

        public async Task DropPlayer()
        {
            setElement(137, 86);
            ChangeSprite(jumpgun!);

            double movedown = Canvas.GetTop(playershape);
            while (movedown < 245)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                Canvas.SetTop(playershape, movedown);
                TimeSpan dropdown = TimeSpan.FromSeconds(0.05);
                await Task.Delay(dropdown);
                movedown += 50;
            }
            IsJump = false;
        }

        public async void Jump()
        {
            jump = false;

            setElement(137, 86);

            ChangeSprite(jumpgun!);

            double moveup = Canvas.GetTop(playershape);
            while (moveup > 60)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                Canvas.SetTop(playershape, moveup);
                TimeSpan jumpup = TimeSpan.FromSeconds(0.05);
                await Task.Delay(jumpup);
                moveup -= 50;
            }

            Thread.Sleep(1);
            await DropPlayer();

            if (IsDead)
            {
                setDefaultDead();
                KillerType(main!.killer);
            }

            else if (!main!.IsHoldCtrlLeft)
            {
                setDefault();
                Default();
            }
            else
            {
                Crouch();
            }
        }

        public async void Crouch()
        {
            setElement(90, 76);

            Canvas.SetTop(playershape, 260);

            ChangeSprite(pathpic + "playercrouch.png");

            await Task.Delay(100);
            if (IsDead) setDefaultDead(); 
        }


        private void ChangeToCrouchShootSprite()
        {
            setElement(110, 96);

            Canvas.SetTop(playershape, 250);

            ChangeSprite(crouchshoot!);
        }

        private void ChangeToJumpShoot()
        {
            ChangeSprite(jumpshoot!);
        }

        private void DefaultShoot()
        {
            ChangeSprite(standshoot!);
        }

        public async void Shoot(Bullet bullet, Canvas playground)
        {
            var top = Canvas.GetTop(playershape);
            setElement(141, 151);

            if (IsJump)
            {
                ChangeToJumpShoot();
            }
            else if (top >= 250)
            {
                ChangeToCrouchShootSprite();
            }
            else
            {
                DefaultShoot();
            }

            gun.PlaySoundShoot();

            await bullet.Move();

            if (IsR8) main!.ReleaseJ();
            if (!IsHaveSniper) IsShoot = false; 

            playground.Children.Remove(bullet.bullet);
            await Task.Delay(200);
        }

        // DEAD //

        public void DeadByVietCong()
        {
            string deadpath = pathsound + "vietcongxien.mp3";
            PlayDeadVoice(deadpath);

            //Thread.Sleep(600);

            setElement(86, 137);
            crouch = false;

            ChangeSprite(pathpic + "deadbyvietcong.png");
        }

        public void KillerType(Entity entity)
        {
            switch (entity)
            {
                case Bush:
                    if (IsVietCongKilled)
                    {
                        DeadByVietCong();
                    }
                    else DefaultDead();
                    break;

                default:
                    DefaultDead();
                    return;
            }
        }

        public void Die(Entity entity)
        {
            KillerType(entity);
        }
        
        public void DefaultDead()
        {
            IsDefaultDead = true;
            setElement(86, 137);
            crouch = false;

            ChangeSprite(pathpic + "dead.png");
        }
    }
}
