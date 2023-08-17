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
        public EnemyBullet(string bulletimg, PlayerCharacter player, Canvas playground, int speed)
        {
            pathimgentity = bulletimg;
            this.player = player;
            this.playground = playground;
            this.main = main;
            movementspeed = speed;
        }


        public void SetBulletElement(int bulletheight, int bulletwidth, double left, double top)
        {
            height = bulletheight;
            width = bulletwidth;

            this.left = left;
            this.top = top;

            entity = enemybullet;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height - 5);
            return hitbox;
        }
    }
}
