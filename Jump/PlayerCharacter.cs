using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
        public int playerhitboxright { get; }
        private double playerhitboxjump;
        public bool IsDead = false;
        public bool crouch { get; set; }
        public bool jump { get; set; }
        public MediaPlayer soundgun = new MediaPlayer();
        public MainWindow? main { get; set; }
        

        public PlayerCharacter()
        {
            playershape.Height = 137;
            playershape.Width = 86;
            playershape.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "player.png")),
                Stretch = Stretch.Fill,
            };
            playershape.Margin = new Thickness(0, 0, 700, 80);
            playerhitboxright = 700;
        }

        public double getHeight()
        {
            playerhitboxjump = playershape.Margin.Bottom;
            return playerhitboxjump;
        }

        public void Default()
        {
            if (IsDead) return;
            string pathimgcharstand = pathpic + "player.png";
            string pathimgcharcrouch = pathpic + "playercrouch.png";
            playershape.Height = 137;
            playershape.Width = 86;
            if (crouch)
            {
                playershape.Height = 90;
                playershape.Width = 76;
                ChangeSprite(pathimgcharcrouch);
                return;
            }
            ChangeSprite(pathimgcharstand);
        }

        private void setDefaultDead()
        {
            playershape.Margin = new Thickness(0, 0, 700, 0);
        }

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
            playershape.Height = 137;
            playershape.Width = 86;

            ChangeSprite(pathpic + "jump.png");

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
            playershape.Height = 90;
            playershape.Width = 76;
            playershape.Margin = new Thickness(0, 0, 700, 30);
            ChangeSprite(pathpic + "playercrouch.png");
            await Task.Delay(100);
            if (IsDead) setDefaultDead(); 
        }

        private void ChangeSprite(string path)
        {
            playershape.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(path)),
                Stretch = Stretch.Fill,
            };
        }

        private void PlayShootSound()
        {
            soundgun.Open(new(pathsound + "DesertEagle.mp3"));
            soundgun.Volume = 1;
            soundgun.Play();
        }

        private void ChangeToCrouchShootSprite()
        {
            string crouchshoot = pathpic + "eagleprinstreamcrouch.png";
            playershape.Height = 110;
            playershape.Width = 96;

            ChangeSprite(crouchshoot);
        }

        private void ChangeToJumpShoot()
        {
            string jumpshoot = pathpic + "jumpshoot.png";

            ChangeSprite(jumpshoot);
        }

        private void DefaultShoot()
        {
            string shoot = pathpic + "eagleprinstream.png";

            ChangeSprite(shoot);
        }

        public async void Shoot(Bullet bullet, Grid playground)
        {
            playershape.Height = 141;
            playershape.Width = 151;
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

            PlayShootSound();

            await bullet.Move();

            main!.IsShoot = false; 

            playground.Children.Remove(bullet.bullet);
            await Task.Delay(200);
            Default();
        }

        public void Die()
        {
            playershape.Height = 86;
            playershape.Width = 137;
            crouch = false;

            ChangeSprite(pathpic + "dead.png");

        }
    }
}
