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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump
{
    public class Mine : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public Rectangle mine = new Rectangle();

        public Mine()
        {
            height = 30;
            width = 50;
            thickness = new Thickness(0, 0, -1000, -30);

            pathimgentity = pathpic + "mine.png";
            newpathimg = pathpic + "explode.png";

            entity = mine;

            SetEntity();
        }

        public override async Task Move()
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
                    newthickness = entity.Margin;
                    Explode();
                    return;
                }
            }
        }

        public void Explode()
        {
            string soundpath = pathsound + "explodesound.mp3";
            SetNewEntity(300, 300);
            Playsound(soundpath);
        }
    }
}
