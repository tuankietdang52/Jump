using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
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
    public class MagicCircle : Entity
    {
        public readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle magiccircle = new Rectangle();
        public Entity? boss { get; set; }

        public double maxheight { get; set; }
        public double maxwidth { get; set; }

        public double angle = 0;

        public bool IsReady = false;

        public MagicCircle()
        {
            this.player = player;
            this.playground = playground;
            this.main = main;

            height = 10;
            width = 10;
        }

        public override Rect getHitbox()
        {
            return new Rect(0, 0, 0, 0);
        }

        public virtual void Morph() { }

        public void RotateCircle()
        {
            Canvas.SetZIndex(this.entity, 1);
            Canvas.SetZIndex(this.boss!.entity, 2);
            if (angle == 360) angle = 0;

            angle += 5;
            this.entity!.RenderTransformOrigin = new Point(0.5, 0.5);
            this.entity.RenderTransform = new RotateTransform(angle);
        }

        public virtual void Disappear() { }
    }
}
