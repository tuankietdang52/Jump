using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Jump.EnemyEntity
{
    public class MagicCircleDead : MagicCircle
    {
        public MagicCircleDead(PlayerCharacter player, Canvas playground, MainWindow main, DarkMage owner)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;
            this.owner = owner;
            this.boss = owner;

            pathimgentity = pathpic + "deadcircle.png";

            entity = magiccircle;

            SetPosCircleClone();
            this.entity!.Fill.Opacity = 0;
        }

        public void SetPosCircleClone()
        {
            height = 200;
            width = 200;

            left = Canvas.GetLeft(owner!.entity);
            top = Canvas.GetTop(owner!.entity);

            SetEntity();
        }

        public void FollowMage()
        {
            left = Canvas.GetLeft(owner!.entity) - 45;
            top = Canvas.GetTop(owner!.entity) - 20;

            Canvas.SetLeft(this.entity, left);
            Canvas.SetTop(this.entity, top);
        }

        public override void Disappear()
        {
            this.entity!.Fill.Opacity -= 0.1;

            if (this.entity!.Fill.Opacity <= 0)
            {
                IsDead = true;
            }
        }

        public override void Morph()
        {
            this.entity!.Fill.Opacity += 0.1;

            if (this.entity!.Fill.Opacity >= 1)
            {
                IsReady = true;
            }
        }

        public async override Task Action()
        {
            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);
            Morph();

            while (!IsDead)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main!.IsQuit) break;
                if (main!.IsWin)
                {
                    Disappear();
                    continue;
                }

                await Task.Delay(50);

                RotateCircle();
                FollowMage();

                if (!IsReady) Morph();
            }

            if (!IsDead) return;
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}
