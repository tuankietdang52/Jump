using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump
{
    public class MagicMissileClone : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle magicmissileclone = new Rectangle();
        public Entity? boss { get; set; }


        public MagicMissileClone(PlayerCharacter player, Canvas playground, MainWindow main)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;

            height = 30;
            width = 50;

            IsBullet = true;

            pathimgentity = pathpic + "magicmissile4.png";

            left = 1000;
            setTop();

            movementspeed = 50;

            entity = magicmissileclone;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height - 20);
            return hitbox;
        }

        public void setTop()
        {
            Random toprand = new Random();
            top = toprand.Next(50, 300);
        }

        public override async Task Action()
        {
            double pos = Canvas.GetLeft(entity);
            while (pos > -30)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main!.IsQuit) break;
                if (boss!.IsDead) break;

                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer())
                {
                    if (!player.IsDead) break;
                
                    playground!.Children.Remove(entity);
                    return;
                }
            }
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}
