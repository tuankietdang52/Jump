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

namespace Jump
{
    public class MagicCircleHeal : MagicCircle
    {
        public List<HealingOrb> healingorb = new List<HealingOrb>();
        public bool IsDisappear = false;
        public MagicCircleHeal(PlayerCharacter player, Canvas playground, MainWindow main, Entity boss)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;
            this.boss = boss;

            pathimgentity = pathpic + "magiccircleheal.png";

            entity = magiccircle;

            SetPosCircleHeal();
        }

        public void SetPosCircleHeal()
        {
            height = 300;
            width = 300;

            left = Canvas.GetLeft(this.boss!.entity);
            top = Canvas.GetTop(this.boss!.entity);

            SetEntity();
        }

        public HealingOrb CreateHealingOrb(int index)
        {
            HealingOrb healingorb = new HealingOrb(player!, playground!, main!, boss!, this);

            healingorb.entity!.Visibility = Visibility.Hidden;
            healingorb.orbindex = index;

            main!.entities.Add(healingorb);
            playground!.Children.Add(healingorb.entity);
            return healingorb;
        }

        public void CreateThreeHealingOrb()
        {
            for (int index = 0; index < 3; index++)
            {
                healingorb.Add(CreateHealingOrb(index));
            }
        }

        public void Healing()
        {
            if (boss!.secondhealthbar!.Width >= 1000) return;
            if (boss!.healthbar!.Width <= 1000)
            {
                boss.healthbar!.Width += 4;
            }
            else
            {
                boss.secondhealthbar!.Width += 2;
            }
        }

        public void OrbMove()
        {
            if (healingorb.Count == 0)
            {
                IsDisappear = true;
                return;
            }

            for (int index = 0; index < healingorb.Count; index++)
            {
                healingorb[index].entity!.Visibility = Visibility.Visible;
                healingorb[index].MovingOrb(ref index);
            }
        }

        public void FollowMage()
        {
            left = Canvas.GetLeft(this.boss!.entity) - 105;
            top = Canvas.GetTop(this.boss!.entity) - 80;

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
                boss!.IsBuff = false;
            }
        }

        public async override Task Action()
        {
            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);

            this.entity!.Fill.Opacity = 0;

            CreateThreeHealingOrb();

            string sound = pathsound + "magiccircleheal.mp3";

            Playsound(sound, 0.5);

            while (!IsDead)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                if (player!.IsDead || main!.IsQuit) break;
                if (boss!.IsDead) break;

                RotateCircle();
                FollowMage();
                await Task.Delay(50);

                if (this.entity!.Fill.Opacity < 1 && !IsReady) Morph();
                else IsReady = true;

                if (!IsReady) continue;

                OrbMove();

                if (!IsDisappear) Healing();
                else Disappear();
            }

            if (!IsDead) return;
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}
