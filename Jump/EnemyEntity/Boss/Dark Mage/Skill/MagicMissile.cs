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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump.EnemyEntity
{
    public class MagicMissile : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle magicmissile = new Rectangle();
        public Entity? boss { get; set; }

        public Rect pointplayer;
        public Rect missilepoint;

        public bool IsBottom = false;
        public bool IsTop = false;
        public bool IsShootDouble = false;

        public string? soundpath { get; set; }

        public int angleindex;

        public double maxheight = 170;

        public double angle;

        public double speedtop;

        public MagicMissile(double left, double top, PlayerCharacter player, Canvas playground, MainWindow main)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;

            height = 30;
            width = 50;

            IsBullet = true;

            pathimgentity = pathpic + "magicmissile.png";

            this.left = left;
            this.top = top;

            movementspeed = 50;

            entity = magicmissile;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height - 20);
            return hitbox;
        }

        public void SetMagicMissileElement(int missilecount)
        {
            IsShootDouble = true;
            angleindex = missilecount;
            speedtop = 20;
            if (missilecount == 1) movementspeed = 35;
        }

        public void GetTopSpeed()
        {
            switch (top)
            {
                case < 100:
                    speedtop = 15;
                    return;
                case < 160:
                    speedtop = 10;
                    return;
                default:
                    speedtop = 5;
                    return;
            }
        }

        public void GetAngleIndex(double pos, double postop)
        {
            Random anglerand = new Random();
            if (!IsShootDouble)
            {
                angleindex = anglerand.Next(0, 2);
                GetTopSpeed();
            }

            GetPoint(pos, postop);
            GetAngle(angleindex);

            switch (angleindex)
            {
                case 0:
                    SetAngleBot();
                    return;

                case 1:
                    SetAngleTop();
                    return;

                default:
                    IsBottom = false;
                    IsTop = false;
                    return;
            }
        }

        public void GetPoint(double pos, double postop)
        {
            var playerleft = Canvas.GetLeft(player!.playershape);
            var playertop = Canvas.GetTop(player!.playershape);

            pointplayer = new Rect(playerleft, playertop, player.playershape.ActualWidth, player.playershape.ActualHeight);

            missilepoint = new Rect(pos, postop, this.entity!.ActualWidth, this.entity.ActualHeight);
        }

        public Vector2 VectorFromMtoP()
        {

            float pointX = (float)(pointplayer.X - missilepoint.X);
            float pointY = (float)(missilepoint.Y - missilepoint.Y);

            return new Vector2(pointX, pointY);
        }

        public Vector2 VectorFromMtoPosBottom()
        {
            float pointX = (float)((pointplayer.X + 200) - missilepoint.X);
            float pointY = (float)((pointplayer.Y) - missilepoint.Y);

            return new Vector2(pointX, pointY);
        }

        public Vector2 VectorFromMtoPosTop()
        {
            var playertop = Canvas.GetTop(player!.playershape) - 150;
            Point Pointtop = new Point(pointplayer.X, playertop);

            float pointX = (float)(Pointtop.X - missilepoint.X);
            float pointY = (float)(Pointtop.Y - missilepoint.Y);

            return new Vector2(pointX, pointY);
        }

        public double GetCrossProduct(Vector2 MtoP, Vector2 MtoPos)
        {
            var vector1 = Math.Pow(MtoP.X, 2) + Math.Pow(MtoP.Y, 2);
            var vector2 = Math.Pow(MtoPos.X, 2) + Math.Pow(MtoPos.Y, 2);

            var result = Math.Sqrt(vector1) * Math.Sqrt(vector2);
            return result;
        }

        public void GetAngle(int angleindex)
        {
            Vector2 MtoP = VectorFromMtoP();
            Vector2 MtoPos;

            if (angleindex == 0) MtoPos = VectorFromMtoPosBottom();
            else MtoPos = VectorFromMtoPosTop();

            var dotproduct = Vector2.Dot(MtoP, MtoPos);
            var crossproduct = GetCrossProduct(MtoP, MtoPos);

            angle = Math.Acos((double)dotproduct / crossproduct);

            angle *= 180 / Math.PI;
        }

        public void SetAngleBot()
        {
            IsBottom = true;

            entity!.RenderTransform = new RotateTransform(-angle);
        }

        public void SetAngleTop()
        {
            IsTop = true;

            entity!.RenderTransform = new RotateTransform(angle);
        }

        public void ChangePosHeightBottom(ref double postop)
        {
            if (postop <= 290)
            {
                postop += speedtop;
                Canvas.SetTop(this.entity, postop);
            }
        }

        public void ChangePosHeightTop(ref double postop)
        {
            if (postop >= maxheight)
            {
                postop -= speedtop;
                Canvas.SetTop(this.entity, postop);
            }
        }

        public override async Task Action()
        {
            double pos = Canvas.GetLeft(entity);
            double postop = Canvas.GetTop(entity);

            GetAngleIndex(pos, postop);

            Playsound(soundpath!, 0.2);

            while (pos > 0)
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
                if (IsBottom) ChangePosHeightBottom(ref postop);
                if (IsTop) ChangePosHeightTop(ref postop);

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
