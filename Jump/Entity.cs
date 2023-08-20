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

        public PlayerCharacter? player { get; set; }
        public Rectangle? entity { get; set; }
        public MediaPlayer soundplay = new MediaPlayer();
        public Canvas? playground { get; set; }
        public MainWindow? main { get; set; }
        public Rectangle? healthbar = null;

        public Rect entityhitbox;

        public string? bulletpath { get; set; }
        public string? newpathimg { get; set; }
        public int phase { get; set; }
        public double hitboxheight { get; set; }
        public bool getHit = false;
        public int movementspeed { get; set; }
        public string? pathimgentity { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public double newleft { get; set; }
        public double newtop { get; set; }
        public int bulletheight { get; set; }
        public int bulletwidth { get; set; }

        public bool IsDemon = false;
        public bool IsHarmless = false;
        public bool IsBullet = false;
        public bool IsDead = false;
        public bool IsSpawn = false;

        public int turn = 0;
        public int bulletspeed = 100;

        public Entity() { }

        // SET ENTITY DISPLAY //

        public void SetEntity()
        {
            entity!.Height = height;
            entity!.Width = width;
            entity.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathimgentity!)),
                Stretch = Stretch.Fill,
            };

            Canvas.SetLeft(entity, left);
            Canvas.SetTop(entity, top);
        }

        public void SetNewEntity(int newheight, int newwidth)
        {
            entity!.Height = newheight;
            entity!.Width = newwidth;
            entity.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new (newpathimg!)),
                Stretch = Stretch.Fill,
            };
            Canvas.SetTop(entity, newtop);
        }

        // CHILDREN ENTITY SPECIFIC FUNCTION //

        public virtual Rect getHitbox()
        {
            return new Rect(0, 0, 0, 0);
        }

        public void CreateHealthBar(double health)
        {
            ScaleTransform flip = new ScaleTransform();
            flip.ScaleX = -1;

            healthbar = new Rectangle()
            {
                Width = health,
                Height = 50,

                Fill = Brushes.Red,

                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = flip,
            };

            Canvas.SetRight(healthbar, 0);
            Canvas.SetTop(healthbar, 0);

            playground!.Children.Add(healthbar);
        }

        public virtual void DemonTurn() { }

        // GET SET //

        public double getHitboxHeight()
        {
            return entity!.Margin.Bottom;
        }
        
        // SOUND EFFECT //

        public void Playsound(string path)
        {
            soundplay.Open(new (path));
            soundplay.Volume = 1;
            soundplay.Play();
        }


        // HIT PLAYER //

        public bool CheckHitPlayer()
        {
            if (HitPlayer())
            {
                if (CheckArmor()) return true;

                player!.IsDead = true;
                return true;
            }
            return false;
        }

        public bool CheckArmor()
        {
            if (player!.IsHaveArmor)
            {
                player.IsHaveArmor = false;
                main!.BreakArmor();
                return true;
            }
            return false;
        }

        public bool HitPlayer()
        {
            var playershape = player!.playershape;
            var playerleft = Canvas.GetLeft(playershape);
            var playertop = Canvas.GetTop(playershape);

            player.playerhitbox = new Rect(playerleft , playertop, playershape.Width - 50, playershape.Height);
            entityhitbox = getHitbox();

            if (player.playerhitbox.IntersectsWith(entityhitbox)) return true;

            return false;
        }

        // GET HIT //

        public virtual bool CheckGetHit()
        {
            if (!player!.IsDead)
            {
                return true;
            }
            return false;
        }


        // MOVE METHOD //
        
        public void ChangePositionMove(ref double pos)
        {
            Canvas.SetLeft(entity, pos);
            pos -= movementspeed;
        }

        public virtual async Task Action()
        {
            double pos = Canvas.GetLeft(entity);
            while (pos > -30)
            {
                if (player!.IsDead) return;
                if (IsDemon) DemonTurn();

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer()) return;
                if (getHit)
                {
                    if (IsSpawn)
                    {
                        playground!.Children.Remove(this.entity);
                        main!.entities.Remove(this);
                    }
                    return;
                }
            }
            if (IsSpawn)
            {
                playground!.Children.Remove(this.entity);
                main!.entities.Remove(this);
            }
        }
    }
}