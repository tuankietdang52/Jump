using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump
{
    public class BigGoldFish : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        
        public Rectangle biggoldfish = new Rectangle();
        public Random actionrand = new Random();

        public double armor = 20;

        public bool AlreadyRotate = false;

        public int actionindex;

        public BigGoldFish()
        {
            height = 300;
            width = 200;


            left = 780;
            top = 60;

            movementspeed = 100;

            pathimgentity = pathpic + "goldfishking.png";

            entity = biggoldfish;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox;
            if (!AlreadyRotate) hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), entity!.Width - 30, entity.Height);
            else hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity) - 150, entity!.Width, entity.Height - 60);
            return hitbox;
        }

        public void setDefault()
        {
            AlreadyRotate = false;

            entity!.Height = 300;
            entity.Width = 200;

            Canvas.SetLeft(entity, 780);
            Canvas.SetTop(entity, 60);
        }

        public async void UseSkill(int actionindex, double pos)
        {
            switch (actionindex)
            {
                case 0: case 1: case 2: case 3:
                    await Task.Delay(1);
                    return;

                case 4: case 5: case 6: case 7: case 8:
                    if (!AlreadyRotate) await Rotate();
                    return;

                case 9: case 10:
                    SpawnGoldFish(pos);
                    return;

                default:
                    await Task.Delay(1);
                    return;
            }
        }

        public double GetDamage()
        {
            double damage = player!.damage -  (player!.damage * (armor / 100));
            if (damage >= healthbar!.Width)
            {
                damage = healthbar!.Width;
            }
            return damage;
        }

        public void Dead()
        {
            getHit = true;
            IsDead = true;
            main!.ClearEntity();
            player!.IsDead = false;
            playground!.Children.Remove(healthbar);
        }

        public override async Task Action()
        {
            CreateHealthBar(700);

            double pos = Canvas.GetLeft(this.entity);

            Stopwatch skilltime = new Stopwatch();

            skilltime.Start();

            while (!IsDead)
            {
                if (main!.IsPause)
                {
                    skilltime.Stop();
                    await Task.Delay(1);
                    continue;
                }
                else skilltime.Start();

                if (player!.IsDead && main!.IsQuit) return;

                if (getHit)
                {
                    healthbar!.Width -= GetDamage();
                    getHit = false;
                    if (healthbar.Width <= 0)
                    {
                        Dead();
                        return;
                    }
                }

                if (CheckHitPlayer()) return;

                await Task.Delay(1);

                if (skilltime.Elapsed.Seconds == 1)
                {
                    skilltime.Restart();
                    GetRandomSkill(pos);
                }
                else continue;
            }
        }

        public void GetRandomSkill(double pos)
        {
            actionindex = actionrand.Next(0, 11);
            UseSkill(actionindex, pos);
        }

        public void SetGoldFish(ref GoldFish goldfish)
        {
            goldfish.movementspeed = 50;

            goldfish.player = this.player;
            goldfish.playground = this.playground;
            goldfish.main = this.main;
        }

        public async void SpawnGoldFish(double left)
        {
            GoldFish goldfish = new GoldFish();

            SetGoldFish(ref goldfish);

            main!.entities.Add(goldfish);
            playground!.Children.Add(goldfish.entity);

            await goldfish.SpawnAction(this);
        }

        public async Task Push()
        {
            double pos = Canvas.GetLeft(entity);

            while (pos > -10)
            {
                if (player!.IsDead) return;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer()) return;
            }

            await Back();
        }

        public void MoveBack(ref double pos)
        {
            Canvas.SetLeft(entity, pos);
            pos += movementspeed;
        }

        public async Task Back()
        {
            double pos = Canvas.GetLeft(entity);
            while (pos <= 680)
            {
                if (player!.IsDead) return;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                MoveBack(ref pos);

                if (CheckHitPlayer()) return;
            }

            for (double angle = 270; angle >= 0; angle -= 10)
            {
                entity!.RenderTransform = new RotateTransform(angle);
                await Task.Delay(10);
            }

            setDefault();
        }

        public async Task Rotate()
        {
            AlreadyRotate = true;
            for (double angle = 0; angle <= 270; angle += 10)
            {
                entity!.RenderTransform = new RotateTransform(angle);
                await Task.Delay(10);
            }

            entity!.Height = 200;
            entity.Width = 300;


            Canvas.SetLeft(entity, 680);
            Canvas.SetTop(entity, 250);

            await Push();
            
        }
    }
}
