using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump.EnemyEntity
{
    public class CTBullet : Entity
    {

        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public Rectangle ctbullet = new Rectangle();

        public CTBullet(PlayerCharacter player, Canvas playground, MainWindow main)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;

            height = 10;
            width = 30;

            top = 250;
            left = 1200;

            movementspeed = 150;

            pathimgentity = pathpic + "bullet.png";

            entity = ctbullet;

            IsBullet = true;

            SetEntity();
            RotateHorizontal();
        }

        public void RotateHorizontal()
        {
            double angle = 180;

            this.entity!.RenderTransformOrigin = new Point(0.5, 0.5);
            this.entity.RenderTransform = new RotateTransform(angle);
        }

        public async override Task Action()
        {
            string soundpath = pathsound + "awpsound.mp3";
            Playsound(soundpath, 1);

            double pos = Canvas.GetLeft(this.entity);
            while (pos > -10)
            {
                TimeSpan move = TimeSpan.FromSeconds(0.05);
                await Task.Delay(move);

                ChangePositionMove(ref pos);

                if (CheckHitPlayer()) break;
            }


            string sasvoice = pathsound + "sasvoice.mp3";
            Playsound(sasvoice, 1);

            playground!.Children.Remove(this.entity);
            main!.entities.Remove(this);
            main.ResultScene();
        }
    }
}
