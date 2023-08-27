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

            left = 1000;
            top = 320;

            newtop = 180;

            pathimgentity = pathpic + "mine.png";
            newpathimg = pathpic + "explode.png";

            entity = mine;

            SetEntity();
        }

        public override async Task Action()
        {
            double pos = Canvas.GetLeft(this.entity);
            while (pos > 0)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main.IsQuit) return;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer())
                {
                    if (!player!.IsDead) return;

                    Explode(pos);
                    return;
                }
            }
        }
    }
}
