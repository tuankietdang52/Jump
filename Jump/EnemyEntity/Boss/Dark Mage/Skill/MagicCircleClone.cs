using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
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

namespace Jump.EnemyEntity
{
    public class MagicCircleClone : MagicCircle
    {
        public DarkMage cloneowner = new DarkMage();
        public MagicCircleClone(PlayerCharacter player, Canvas playground, MainWindow main, DarkMage owner)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;
            cloneowner = owner;
            this.boss = owner;

            pathimgentity = pathpic + "magiccircleclone.png";

            entity = magiccircle;

            SetPosCircleClone();
            this.entity!.Fill.Opacity = 0;
        }

        public void SetPosCircleClone()
        {
            height = 150;
            width = 150;

            left = Canvas.GetLeft(cloneowner!.entity);
            top = Canvas.GetTop(cloneowner!.entity);

            SetEntity();
        }

        public void FollowMage()
        {
            left = Canvas.GetLeft(cloneowner!.entity) - 28;
            top = Canvas.GetTop(cloneowner!.entity) - 10;

            Canvas.SetLeft(this.entity, left);
            Canvas.SetTop(this.entity, top);
        }

        public override void Morph()
        {
            this.entity!.Fill.Opacity += 0.1;

            if (this.entity!.Fill.Opacity >= 1)
            {
                IsReady = true;
            }
        }

        public override void Disappear()
        {
            this.entity!.Fill.Opacity -= 0.1;

            if (this.entity!.Fill.Opacity <= 0)
            {
                IsDead = true;
            }
        }

        public bool CheckExpressionDisappear()
        {
            if (cloneowner.IsClone && cloneowner.IsDead) return true;
            
            else if (!cloneowner.IsCreateClone && !cloneowner.IsClone) return true;

            return false;
        }

        public async override Task Action()
        {
            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);

            while (!IsDead)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }
                if (player!.IsDead || main!.IsQuit) break;

                await Task.Delay(50);
                RotateCircle();

                if (CheckExpressionDisappear())
                {
                    Disappear();
                    continue;
                }

                FollowMage();

                if (cloneowner.IsCreateMagicMissileClone && !IsReady) Morph();
            }

            if (!IsDead) return;
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}