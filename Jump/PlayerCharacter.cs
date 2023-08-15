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

        public int indexgun = 0;

        public bool IsVietCongKilled = false;

        public int playerhitboxright { get; }
        private double playerhitboxjump;
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

            playershape.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(stand!)),
                Stretch = Stretch.Fill,
            };
            playershape.Margin = new Thickness(0, 0, 700, 80);
            playerhitboxright = 700;

        }

        // GET SET //

        public double getHeight()
        {
            playerhitboxjump = playershape.Margin.Bottom;
            return playerhitboxjump;
        }

        private void setDefaultDead()
        {
            playershape.Margin = new Thickness(0, 0, 700, 0);
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

            string pathimgcharcrouch = pathpic + "playercrouch.png";

            setElement(137, 86);

            if (crouch)
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
            double movedown = playershape.Margin.Bottom;
            while (playershape.Margin.Bottom > 80)
            {
                playershape.Margin = new Thickness(0, 0, 700, movedown);
                TimeSpan dropdown = TimeSpan.FromSeconds(0.05);
                await Task.Delay(dropdown);
                movedown -= 100;
            }
        }

        public async void Jump(MainWindow mainWindow)
        {
            jump = false;

            setElement(137, 86);

            ChangeSprite(jumpgun!);

            double moveup = playershape.Margin.Bottom;
            while (moveup < 450)
            {
                playershape.Margin = new Thickness(0, 0, 700, moveup);
                TimeSpan jumpup = TimeSpan.FromSeconds(0.05);
                await Task.Delay(jumpup);
                moveup += 100;
            }

            Thread.Sleep(1);
            await DropPlayer();
            mainWindow.IsJump = false;

            Default();
            if (IsDead) setDefaultDead();

            else if (!mainWindow.IsHoldCtrlLeft)
            {
                playershape.Margin = new Thickness(0, 0, 700, 80);
            }
        }

        public async void Crouch()
        {
            setElement(90, 76);

            playershape.Margin = new Thickness(0, 0, 700, 30);

            ChangeSprite(pathpic + "playercrouch.png");

            await Task.Delay(100);
            if (IsDead) setDefaultDead(); 
        }


        private void ChangeToCrouchShootSprite()
        {
            setElement(110, 96);

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

        public async void Shoot(Bullet bullet, Grid playground)
        {
            setElement(141, 151);

            if (playershape.Margin.Bottom > 86)
            {
                ChangeToJumpShoot();
            }
            else if (crouch && !jump)
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
            //Default();
        }

        // DEAD //

        public void DeadByVietCong()
        {
            string deadpath = pathsound + "vietcongxien.mp3";
            PlayDeadVoice(deadpath);

            Thread.Sleep(600);

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
            setElement(86, 137);
            crouch = false;

            ChangeSprite(pathpic + "dead.png");
        }
    }
}
