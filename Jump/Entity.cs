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
    public class Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public Thickness newthickness { get; set; }

        public PlayerCharacter? player { get; set; }
        public Rectangle? entity { get; set; }
        public MediaPlayer soundplay = new MediaPlayer();
        public Grid? playground { get; set; }
        public MainWindow? main { get; set; }
        public Thickness thickness { get; set; }

        public string? bulletpath { get; set; }
        public string? newpathimg { get; set; }
        public int phase { get; set; }
        public double hitboxheight { get; set; }
        public bool getHit = false;
        public int movementspeed { get; set; }
        public string? pathimgentity { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int bulletheight { get; set; }
        public int bulletwidth { get; set; }

        public bool IsDemon = false;
        public bool IsHarmless = false;

        public int turn = 0;
        public int playerhitbox = 80;
        public int bulletspeed = 100;

        public Entity() { }

        // SET ENTITY DISPLAY //

        public void SetEntity()
        {
            entity!.Height = height;
            entity!.Width = width;
            entity.Margin = thickness;
            entity.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathimgentity!)),
                Stretch = Stretch.Fill,
            };
        }

        public void SetNewEntity(int newheight, int newwidth)
        {
            entity!.Height = newheight;
            entity!.Width = newwidth;
            entity.Margin = newthickness;
            entity.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new (newpathimg!)),
                Stretch = Stretch.Fill,
            };
        }

        // CHILDREN ENTITY SPECIFIC FUNCTION //
        public virtual void DemonTurn() { }

        // GET SET //

        public double getHitboxHeight()
        {
            return entity!.Margin.Bottom;
        }
        
        public double getHitbox()
        {
            return entity!.Margin.Right;
        }
        
        // SOUND EFFECT //

        public void Playsound(string path)
        {
            soundplay.Open(new (path));
            soundplay.Play();
        }


        // HIT PLAYER //
        public bool CheckHitPlayer(double pos)
        {
            if (HitPlayer(pos))
            {
                player!.IsDead = true;
                return true;
            }
            return false;
        }

        public bool HitPlayer(double entityhitbox)
        {
            if (entity!.Margin.Bottom <= player!.getHeight() + player!.playershape.Height && entity!.Margin.Bottom >= player!.getHeight() - 200)
            {
                if (entityhitbox >= player!.playerhitboxright && entityhitbox <= 800)
                {
                    return true;
                }
            }
            return false;
        }


        // MOVE METHOD //

        public async Task MoveType(Entity entitytype)
        {
            switch (entitytype)
            {
                case GoldFish:
                    await Move();
                    break;
                case Demon:
                    await Move();
                    break;
                case Bush:
                    await Move();
                    break;
                case Mine:
                    await Move();
                    break;
                default:
                    return;
            }
        }
        
        public void ChangePositionMove(ref double pos)
        {
            entity!.Margin = new Thickness(0, 0, pos, getHitboxHeight());
            pos += movementspeed;
        }

        public virtual async Task Move()
        {
            double pos = entity!.Margin.Right;
            while (pos < 1000)
            {
                if (player!.IsDead) return;
                if (IsDemon) DemonTurn();

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer(pos)) return;
                if (getHit) return;
            }
        }

        public async Task BulletMove()
        {
            double pos = entity!.Margin.Right;
            while (pos < 1000)
            {
                if (player!.IsDead) return;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer(pos))
                {
                    playground!.Children.Remove(entity);
                    return;
                }
            }
            playground!.Children.Remove(entity);
        }
    }
}