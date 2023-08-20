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

namespace Jump
{
    public class PlayerCharacter
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle playershape = new Rectangle();
        public MainWindow? main { get; set; }
        public MediaPlayer soundreload = new MediaPlayer();
        public MediaPlayer voicedead = new MediaPlayer();

        private Gun gun = new Gun();

        public List<string> inventory = new List<string>();

        public Rect playerhitbox;

        public int indexgun = 0;

        public bool IsVietCongKilled = false;
        public bool IsDefaultDead = false;
        public bool IsHaveArmor = false;

        public int playerhitboxright { get; }
        public bool IsDead = false;
        public bool crouch { get; set; }
        public bool jump { get; set; }

        public string? reloadsoundpath;
        public string? stand { get; set; }
        public string? standshoot { get; set; }
        public string? jumpgun { get; set; }
        public string? jumpshoot { get; set; }
        public string? crouchshoot { get; set; }

        public PlayerCharacter()
        {
            setElement(137, 86);

            gun.player = this;

            gun.ChangeGun("de");
            inventory.Add("de");
            gun.getPathGun(indexgun);

            playershape.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(stand!)),
                Stretch = Stretch.Fill,
            };

            setDefault();
        }

        // GET SET //

        public double getHeight()
        {
            return 0.2;
        }

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

        public void PlayDeadVoice(string path)
        {
            voicedead.Open(new(path));
            voicedead.Volume = 1;
            voicedead.Play();
        }

        public void PlayReloadSound()
        {
            gun.getReloadsound(ref reloadsoundpath!, indexgun);
            soundreload.Open(new (reloadsoundpath));
            soundreload.Volume = 100;
            soundreload.Play();
        }

        public void setElement(int height, int width)
        {
            playershape.Height = height;
            playershape.Width = width;
        }

        // SET PLAYER //

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

        // ACTION //

        public async Task DropPlayer()
        {
            setElement(137, 86);
            ChangeSprite(jumpgun!);

            double movedown = Canvas.GetTop(playershape);
            while (movedown < 245)
            {
                Canvas.SetTop(playershape, movedown);
                TimeSpan dropdown = TimeSpan.FromSeconds(0.05);
                await Task.Delay(dropdown);
                movedown += 50;
            }
            main!.IsJump = false;
        }

        public async void Jump()
        {
            jump = false;

            setElement(137, 86);

            ChangeSprite(jumpgun!);

            double moveup = Canvas.GetTop(playershape);
            while (moveup > 60)
            {
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

            if (main!.IsJump)
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

            bullet.PlayShootSound();

            await bullet.Move();

            main!.IsShoot = false; 

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
