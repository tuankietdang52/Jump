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
using static System.Net.Mime.MediaTypeNames;

namespace Jump
{
    public class DarkMage : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle darkmage = new Rectangle();

        public Stopwatch timemove = new Stopwatch();
        public Stopwatch skilltime = new Stopwatch();

        public bool IsGetPos = false;
        public bool IsShootDouble = false;
        public bool[] IsTrigger = new bool[10];

        public int actions = 0;
        public int missilecount = 0;
        public int timetoskill = 1;
        public int skillphase = 1;

        public DarkMage()
        {
            height = 120;
            width = 100;

            pathimgentity = pathpic + "darkmage.png";

            left = 850;
            top = 230;

            armor = 60;

            entity = darkmage;

            SetEntity();
        }

        public void CreateSecondHealthbar(double health)
        {
            ScaleTransform flip = new ScaleTransform();
            flip.ScaleX = -1;

            secondhealthbar = new Rectangle()
            {
                Width = health,
                Height = 50,

                Fill = Brushes.LightBlue,

                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = flip,
            };

            Canvas.SetRight(secondhealthbar, 0);
            Canvas.SetTop(secondhealthbar, 0);

            playground!.Children.Add(secondhealthbar);
        }

        public void SetDefault()
        {
            newleft = Canvas.GetLeft(this.entity);
            newtop = Canvas.GetTop(this.entity);

            newpathimg = pathpic + "darkmage.png";

            SetNewEntity(120, 100);
        }

        public void SetAnimation(double height, double width)
        {
            newleft = Canvas.GetLeft(this.entity);
            newtop = Canvas.GetTop(this.entity);

            SetNewEntity(height, width);
        }

        public void GetPosToMove(ref double pointleft, ref double pointtop)
        {
            IsGetPos = true;
            Random randleft = new Random();
            pointleft = randleft.Next(650, 851);

            Random randtop = new Random();
            pointtop = randtop.Next(60, 230);
        }

        public void MoveLeft(ref double pos, double pointleft)
        {
            if (pos < pointleft)
            {
                if (pointleft - pos <= 5)
                {
                    pos = pointleft;
                    return;
                }
                pos += 10;
            }
            else
            {
                if (pos - pointleft <= 5)
                {
                    pos = pointleft;
                    return;
                }
                pos -= 10;
            }
            Canvas.SetLeft(this.entity, pos);
        }

        public void MoveTop(ref double postop, double pointtop)
        {
            if (postop < pointtop)
            {
                if (pointtop - postop <= 5)
                {
                    postop = pointtop;
                    return;
                }
                postop += 5;
            }
            else
            {
                if (postop - pointtop <= 5)
                {
                    postop = pointtop;
                    return;
                }
                postop -= 5;
            }
            Canvas.SetTop(this.entity, postop);
        }

        public int SkillPhase()
        {
            Random skillrand = new Random();
            switch (skillphase)
            {
                case 1:
                    return skillrand.Next(0, 7);

                case 2:
                    return skillrand.Next(0, 13);

                default:
                    return 0;
            }
        }

        public void RandomSkill()
        {
            int skillindex = SkillPhase();

            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);

            if (IsBuff) return;

            switch (skillindex)
            {
                case 0: case 1: case 2:
                    if (actions == 0) Shoot(pos, postop);
                    return;

                case 3: case 4: case 5:
                    if (actions == 0) ShootDouble(pos, postop);
                    return;

                case 6:
                    if (actions == 0) SpawnMagicBall();
                    return;

                case 7: case 8: case 9:
                    RandomCircle();
                    return;

                case 10: case 11:
                   if (actions == 0) HandleHealing();
                   return;

                default:
                    return;
            }
        }

        public void RandomCircle()
        {
            Random circlerand = new Random();
            int circleindex = circlerand.Next(0, 5);

            switch (circleindex)
            {
                case 0: case 1:
                    CreateMagicCircleShoot();
                    break;
                default:
                    return;
            }
        }

        public async void Shoot(double pos, double postop)
        {
            actions++;

            newpathimg = pathpic + "darkmageskill1.png";

            SetAnimation(120, 100);

            while (missilecount != 2)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }
                CreateMagicMissile();
                missilecount++;
                await Task.Delay(1000);
            }

            newpathimg = pathpic + "darkmage.png";

            SetDefault();

            missilecount = 0;
            actions--;
        }

        public async void ShootDouble(double pos, double postop)
        {
            actions++;

            IsShootDouble = true;

            newpathimg = pathpic + "darkmageskill1.png";

            SetAnimation(120, 100);

            missilecount = 0;

            while (missilecount != 2)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }
                CreateMagicMissile();
            }

            await Task.Delay(1000);

            newpathimg = pathpic + "darkmage.png";

            SetDefault();

            IsShootDouble = false;
            missilecount = 0;
            actions--;
        }

        public async void CreateMagicMissile()
        {
            var posleft = Canvas.GetLeft(this.entity) - 30;
            var postop = Canvas.GetTop(this.entity) - 10;

            MagicMissile magicmissile = new MagicMissile(posleft, postop, player!, playground!, main!);

            magicmissile.boss = this;
            magicmissile.soundpath = pathsound + "magicshot.mp3";

            if (IsShootDouble)
            {
                magicmissile.SetMagicMissileElement(missilecount);
                missilecount++;
            }

            main!.entities.Add(magicmissile);
            playground!.Children.Add(magicmissile.entity);

            await magicmissile.Action();
        }

        public void SpawnMagicBall()
        {
            actions++;

            newpathimg = pathpic + "darkmageskill2.png";

            SetAnimation(120, 100);

            CreateMagicBall();
        }

        public async void CreateMagicBall()
        {
            MagicBall magicball = new MagicBall(player!, playground!, main!);

            magicball.boss = this;

            main!.entities.Add(magicball);
            playground!.Children.Add(magicball.entity);

            await magicball.Action();

            await Task.Delay(500);
            actions--;
            SetDefault();
        }

        public async void CreateMagicCircleShoot()
        {
            MagicCircleShoot magiccircle = new MagicCircleShoot(player!, playground!, main!);

            magiccircle.boss = this;

            main!.entities.Add(magiccircle);
            playground!.Children.Add(magiccircle.entity);

            await magiccircle.Action();
        }

        public void HandleHealing()
        {
            Random healrand = new Random();
            int healindex = healrand.Next(0, 15);

            if (healindex > 4) return;

            Healing();
        }

        public void Healing()
        {
            newpathimg = pathpic + "darkmageheal.png";

            SetAnimation(120, 100);

            CreateMagicCircleHeal();
        }

        public async void CreateMagicCircleHeal()
        {
            MagicCircleHeal magiccircle = new MagicCircleHeal(player!, playground!, main!, this);

            main!.entities.Add(magiccircle);
            playground!.Children.Add(magiccircle.entity);

            IsBuff = true;

            await magiccircle.Action();

            newpathimg = pathpic + "darkmage.png";

            SetDefault();
        }

        public void StopTime()
        {
            skilltime.Stop();
            timemove.Stop();
        }

        public void ResumeTime()
        {
            skilltime.Start();
            timemove.Start();
        }

        public void Dead()
        {
            getHit = true;
            IsDead = true;
            main!.ClearEntity();
            player!.IsDead = false;
            playground!.Children.Remove(healthbar);
        }

        public void CheckPhaseBossSecondHealthBar()
        {
            switch (secondhealthbar!.Width)
            {
                case > 500:
                    break;

                case >= 200:
                    if (IsTrigger[0]) break;
                    armor = 70;
                    IsTrigger[0] = true;
                    skillphase = 2;
                    Healing();
                    break;

                case >= 50:
                    if (IsTrigger[1]) break;
                    timetoskill = 0;
                    armor = 75;
                    IsTrigger[1] = true;
                    break;

                default:
                    break;
            }
        }

        public void CheckPhaseBoss()
        {
            switch (healthbar!.Width)
            {
                case > 900:
                    break;

                case > 800:
                    if (IsTrigger[2]) break;
                    armor = 80;
                    IsTrigger[2] = true;
                    break;
            }
        }

        public void CheckHealth()
        {
            getHit = false;

            if (IsBuff) return;

            if (secondhealthbar!.Width <= 0)
            {
                healthbar!.Width -= GetDamage(healthbar);
            }
            else
            {
                secondhealthbar.Width -= GetDamage(secondhealthbar);
            }
        }

        public override async Task Action()
        {
            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);

            CreateHealthBar(1000);
            CreateSecondHealthbar(1000);

            timemove.Start();
            skilltime.Start();

            double pointleft = 0;
            double pointtop = 0;

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


                if (getHit)
                {
                    CheckHealth();

                    if (healthbar!.Width <= 0)
                    {
                        Dead();
                        return;
                    }
                }

                if (secondhealthbar!.Width >= 0) CheckPhaseBossSecondHealthBar();
                else CheckPhaseBoss();

                await Task.Delay(1);

                if (timemove.Elapsed.Seconds > 2)
                {
                    if (!IsGetPos) GetPosToMove(ref pointleft, ref pointtop);
                    MoveLeft(ref pos, pointleft);
                    MoveTop(ref postop, pointtop);
                
                    if (pos == pointleft && postop == pointtop)
                    {
                        timemove.Restart();
                        IsGetPos = false;
                    }
                }

                if (skilltime.Elapsed.Seconds > timetoskill)
                {
                    if (actions == 0)
                    {
                        RandomSkill();
                        skilltime.Restart();
                    }
                }
            }
        }
    }
}
