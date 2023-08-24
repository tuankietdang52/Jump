﻿using System;
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
    public class Warrior : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle warrior = new Rectangle();
        public Rectangle? dash;

        public bool IsDash = false;
        public string? slashsound;

        public Warrior()
        {
            Random leftrand = new Random();


            height = 80;
            width = 100;

            left = leftrand.Next(400, 900);
            top = 260;

            newleft = left;
            newtop = top;

            slashsound = pathsound + "warriorslash.mp3";
            pathimgentity = pathpic + "warrior.png";
            newpathimg = pathpic + "warriorhold.png";

            entity = warrior;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height);
            return hitbox;
        }

        public void CreateDashEffect(double pos)
        {
            dash = new Rectangle()
            {
                Height = 80,
                Width = 100,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new (pathpic + "warriordash.png"))
                }
            };

            Canvas.SetTop(dash, 260);
            Canvas.SetLeft(dash, pos + 80);
            playground!.Children.Add(dash);
        }

        public override async Task Action()
        {
            double pos = Canvas.GetLeft(this.entity);

            Stopwatch timetodash = new Stopwatch();

            timetodash.Start();
            while (pos > -30)
            {
                if (player!.IsDead) return;

                await Task.Delay(1);
                if (getHit)
                {
                    IsDead = true;
                    break;
                }

                if (timetodash.Elapsed.Seconds == 1)
                {
                    SetNewEntity(80, 100);
                }
                if (timetodash.Elapsed.Seconds < 2) continue;

                Playsound(slashsound!, 1);
                pathimgentity = pathpic + "warriorslash.png";
                SetEntity();

                if (!IsDash) CreateDashEffect(pos);
                IsDash = true;

                TimeSpan move = TimeSpan.FromSeconds(0.01);
                await Task.Delay(move);

                Dash(ref pos);

                if (CheckHitPlayer())
                {
                    if (!player!.IsDead) break;

                    playground!.Children.Remove(dash);
                    return;
                }
            }
            playground!.Children.Remove(dash);
        }

        public void Dash(ref double pos)
        {
            Canvas.SetLeft(dash, pos + 80);
            Canvas.SetTop(dash, 260);

            Canvas.SetLeft(this.entity, pos);
            pos -= 100;
        }
    }
}