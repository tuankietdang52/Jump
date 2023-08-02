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

namespace Jump
{
    public class EnemyBullet : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public Rectangle enemybullet = new Rectangle();
        public EnemyBullet(string bulletimg, PlayerCharacter player, Grid playground, int speed)
        {
            pathimgentity = bulletimg;
            this.player = player;
            this.playground = playground;
            this.main = main;
            movementspeed = speed;
        }


        public void SetBulletElement(int bulletheight, int bulletwidth, double right, double bottom)
        {
            height = bulletheight;
            width = bulletwidth;
            thickness = new Thickness(0, 0, right, bottom);
            entity = enemybullet;

            SetEntity();
        }
    }
}
