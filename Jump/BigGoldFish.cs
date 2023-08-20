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
        public bool IsRotate = false;
        public bool IsSpawnGoldFish = true;
        public bool AlreadyRotate = false;

        public int actionindex;

        public BigGoldFish()
        {
            height = 300;
            width = 200;


            left = 780;
            top = 60;

            movementspeed = 100;

            pathimgentity = pathpic + "goldfish.png";

            entity = biggoldfish;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox;
            if (!IsRotate) hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), entity!.Width - 30, entity.Height);
            else hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity) - 150, entity!.Width, entity.Height - 60);
            return hitbox;
        }

        public void setDefault()
        {
            IsRotate = false;
            IsSpawnGoldFish = false;
            AlreadyRotate = false;

            entity!.Height = 300;
            entity.Width = 200;

            Canvas.SetLeft(entity, 780);
            Canvas.SetTop(entity, 60);
        }

        public void getSkill(int actionindex)
        {
            switch (actionindex)
            {
                case 0: case 1: case 2: case 3:
                    break;
                case 4: case 5: case 6: case 7: case 8:
                    IsRotate = true;
                    return;
                case 9: case 10:
                    IsSpawnGoldFish = true;
                    return;
                default:
                    return;
            }
            IsRotate = false;
            IsSpawnGoldFish = false;
        }

        public override async Task Action()
        {
            CreateHealthBar(700);

            double pos = Canvas.GetLeft(this.entity);
            Random actionrand = new Random();

            Stopwatch skilltime = new Stopwatch();

            skilltime.Start();

            await Task.Delay(1000);

            string theme = pathsound + "unwelcome school.mp3";
            main!.PlayTheme(theme, 0.1);

            while (!IsDead)
            {
                if (player!.IsDead) return;

                if (getHit)
                {
                    healthbar!.Width -= 70;
                    getHit = false;
                    if (healthbar.Width <= 0)
                    {
                        getHit = true;
                        IsDead = true;
                        playground!.Children.Remove(healthbar);
                    }
                }

                if (CheckHitPlayer()) return;

                await Task.Delay(1);

                if (skilltime.Elapsed.Seconds == 1)
                {
                    skilltime.Restart();
                    actionindex = actionrand.Next(0, 11);
                    getSkill(actionindex);
                }
                else continue;

                if (IsRotate && !AlreadyRotate) await Rotate();

                if (IsSpawnGoldFish) SpawnGoldFish(pos);
            }
        }

        public async void SpawnGoldFish(double left)
        {
            GoldFish goldfish = new GoldFish();

            goldfish.movementspeed = 50;

            goldfish.player = this.player;
            goldfish.playground = this.playground;
            goldfish.main = this.main;

            goldfish.IsSpawn = true;

            main!.entities.Add(goldfish);
            playground!.Children.Add(goldfish.entity);

            await goldfish.Action();
            IsSpawnGoldFish = false;
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
