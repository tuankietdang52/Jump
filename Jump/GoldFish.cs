using Microsoft.VisualBasic.FileIO;
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
    public class GoldFish : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public Rectangle goldFish = new Rectangle();
        public GoldFish()
        {
            height = 60;
            width = 60;

            left = 1000;
            top = 285;

            pathimgentity = pathpic + "goldfish.png";

            entity = goldFish;

            SetEntity();
        }

        public async Task SpawnAction(Entity boss)
        {
            Canvas.SetLeft(this.entity, 800);
            double pos = Canvas.GetLeft(entity);
            while (pos > -30)
            {
                if (player!.IsDead) return;
                if (boss.IsDead) IsDead = true;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (getHit)
                {
                    playground!.Children.Remove(this.entity);
                    main!.entities.Remove(this);
                    break;
                }
                if (CheckHitPlayer()) break;
            }
            playground!.Children.Remove(this.entity);
            main!.entities.Remove(this);
        }
    }
}
