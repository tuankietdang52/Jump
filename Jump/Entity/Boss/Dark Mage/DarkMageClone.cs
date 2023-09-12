using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
    public class DarkMageClone : DarkMage
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public int cloneindex { get; set; }

        public DarkMage boss { get; set; }

        public DarkMageClone(PlayerCharacter player, Canvas playground, MainWindow main, DarkMage boss)
        {
            this.player = player;
            this.playground = playground;
            this.main = main;
            this.boss = boss;

            IsClone = true;

            GetPosToMove(ref pointleft, ref pointtop);

            Canvas.SetLeft(this.entity, pointleft);
            Canvas.SetTop(this.entity, pointtop);

            SetClone();
            this.entity!.Fill.Opacity = 0;
        }

        public void SetClone()
        {
            entity!.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "darkmageskillclone.png")),
                Stretch = Stretch.Fill,
            };
        }

        public void Invisible(Entity entity)
        {
            if (entity.entity!.Fill.Opacity <= 0) return;
            entity.entity!.Fill.Opacity -= 0.2;
        }

        public void Visible(Entity entity)
        {
            if (entity.entity!.Fill.Opacity >= 1)
            {
                boss.IsCreateMagicMissileClone = true;
                this.IsCreateMagicMissileClone = true;
                return;
            }
            entity.entity!.Fill.Opacity += 0.2;
        }

        public bool CheckVisibleStatus()
        {
            if (boss.IsInvisible)
            {
                if (cloneindex == 0) Invisible(boss);
                return false;
            }
            else
            {
                if (cloneindex == 0) Visible(boss);
                Visible(this);
                return true;
            }
        }

        public async override Task Action()
        {
            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);

            while (!IsDead)
            {
                if (main!.IsPause)
                {
                    StopTime();
                    await Task.Delay(1);
                    continue;
                }
                else ResumeTime();

                if (player!.IsDead || main!.IsQuit) break;
                if (boss!.IsDead) break;

                await Task.Delay(1);

                if (!CheckVisibleStatus()) continue;

                Move(ref pos, ref postop);
                boss!.entity!.Fill.Opacity = 1;

                if (getHit)
                {
                    if (boss.IsInvisible) return;
                    IsDead = true;
                    break;
                }
            }

            if (!IsDead) return;
            boss.clone.Remove(this);
            main!.entities.Remove(this);
            playground!.Children.Remove(entity);
        }
    }
}
