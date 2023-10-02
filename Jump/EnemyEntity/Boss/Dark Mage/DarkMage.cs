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

namespace Jump.EnemyEntity
{ 
    public class DarkMage : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle darkmage = new Rectangle();

        public Stopwatch timemove = new Stopwatch();
        public Stopwatch skilltime = new Stopwatch();

        public List<DarkMageClone> clone = new List<DarkMageClone>(3);

        public bool IsGetPos = false;
        public bool IsShootDouble = false;
        public bool[] IsTrigger = new bool[6];
        public bool IsInvisible = false;
        public bool IsCreateClone = false;
        public bool IsCreateMagicMissileClone = false;
        public bool IsClone = false;
        public bool SkillCloneWait = false;

        public int actions = 0;
        public int missilecount = 0;
        public int timetoskill = 1;
        public int skillphase = 1;
        public int timetomove = 2;

        public double pointleft;
        public double pointtop;

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
                    return skillrand.Next(0, 12);

                default:
                    return 0;
            }
        }

        public void RandomSkill()
        {
            if (IsBuff) return;
            if (IsCreateClone) return;

            int skillindex = SkillPhase();

            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);

            switch (skillindex)
            {
                case 0: case 1: case 2:
                    if (actions == 0) Shoot(pos, postop);
                    break;

                case 3: case 4: case 5:
                    if (actions == 0) ShootDouble(pos, postop);
                    break;

                case 6:
                    if (actions == 0) SpawnMagicBall();
                    break;

                case 7: case 8: case 9:
                    if (actions == 0) RandomCircle();
                    break;

                case 10: case 11:
                   if (actions != 0 || SkillCloneWait) return;
                   HandleClone();
                   timetoskill = 0;
                   return;

                default:
                    return;
            }
            SkillCloneWait = false;
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

            MagicMissile magicmissile = new MagicMissile(posleft, postop, player!, playground!, main!, this);

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
            actions++;
            MagicCircleShoot magiccircle = new MagicCircleShoot(player!, playground!, main!, this);

            magiccircle.boss = this;

            main!.entities.Add(magiccircle);
            playground!.Children.Add(magiccircle.entity);

            await magiccircle.Action();
            actions--;
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

        public void HandleClone()
        {
            IsInvisible = true;
            IsCreateClone = true;
            IsHarmless = true;
            timetomove = 1;

            newpathimg = pathpic + "darkmageskillclone.png";

            SetAnimation(120, 100);

            Clone();
            CreateMagicCircleClone(this);
        }

        public async void CreateMagicCircleClone(DarkMage owner)
        {
            MagicCircleClone magiccircle = new MagicCircleClone(player!, playground!, main!, owner);

            main!.entities.Add(magiccircle);
            playground!.Children.Add(magiccircle.entity);

            await magiccircle.Action();
        }

        public async void Clone()
        {
            for (int cloneindex = 0; cloneindex < 3; cloneindex++)
            {
                clone.Add(CreateClone(cloneindex));
                CloneAction(clone[cloneindex]);
            }

            newpathimg = pathpic + "darkmageskillclone.png";
            SetAnimation(120, 100);

            await Task.Delay(700);
            IsInvisible = false;
        }

        public async void CloneAction(DarkMageClone clone)
        {
            CreateMagicCircleClone(clone);
            await clone.Action();
        }

        public DarkMageClone CreateClone(int index)
        {
            DarkMageClone clone = new DarkMageClone(player!, playground!, main!, this);

            clone.cloneindex = index;

            main!.entities.Add(clone);
            playground!.Children.Add(clone.entity);

            return clone;
        }

        public async void CreateMagicMissileClone()
        {
            MagicMissileClone magicmissileclone = new MagicMissileClone(player!, playground!, main!, this);

            magicmissileclone.boss = this;

            main!.entities.Add(magicmissileclone);
            playground!.Children.Add(magicmissileclone.entity);

            await magicmissileclone.Action();
        }

        public void CheckClone()
        {
            if (clone.Count != 0) return;

            IsCreateClone = false;
            IsHarmless = false;
            SkillCloneWait = true;
            entity!.Fill.Opacity = 1;

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

        public void CheckPhaseBossSecondHealthBar()
        {
            switch (secondhealthbar!.Width)
            {
                case > 500:
                    break;

                case >= 200:
                    if (IsTrigger[0]) break;
                    IsTrigger[0] = true;
                    armor = 70;
                    break;

                case >= 50:
                    if (IsTrigger[1]) break;
                    IsTrigger[1] = true;
                    timetoskill = 0;
                    skillphase = 2;
                    HandleClone();
                    break;

                default:
                    break;
            }
        }

        public void CheckPhaseBoss()
        {
            switch (healthbar!.Width)
            {
                case >= 900:
                    if (IsTrigger[2]) break;
                    armor = 75;
                    IsTrigger[2] = true;
                    break;

                case >= 600:
                    if (IsTrigger[3]) break;
                    armor = 80;
                    IsTrigger[3] = true;
                    break;

                case >= 350:
                    if (IsTrigger[4]) break;
                    armor = 85;
                    IsTrigger[4] = true;
                    Healing();
                    break;

                case >= 200:
                    if (IsTrigger[5]) break;
                    armor = 90;
                    IsTrigger[5] = true;
                    break;

                default:
                    armor = 90;
                    break;
            }
        }

        public void DecreaseBossHealth()
        {
            getHit = false;

            if (IsBuff) return;
            if (IsInvisible) return;

            if (secondhealthbar!.Width <= 0)
            {
                healthbar!.Width -= GetDamage(healthbar);
            }
            else
            {
                secondhealthbar.Width -= GetDamage(secondhealthbar);
            }
        }

        public void Move(ref double pos, ref double postop)
        {
            if (timemove.Elapsed.Seconds > timetomove)
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
        }

        public void UseSkill()
        {
            if (skilltime.Elapsed.Seconds <= timetoskill) return;
            if (actions == 0)
            {
                RandomSkill();
                skilltime.Restart();
            }
        }

        public void CloneMagic()
        {
            CheckClone();
            if (skilltime.Elapsed.Milliseconds <= 450) return;
            Random rand = new Random();
            int issshootindex = rand.Next(0, 4);

            if (issshootindex < 2) CreateMagicMissileClone();
            skilltime.Restart();
        }

        public void CheckPhase()
        {
            if (secondhealthbar!.Width > 0) CheckPhaseBossSecondHealthBar();
            else CheckPhaseBoss();
        }

        public bool CheckDead()
        {
            if (healthbar!.Width <= 0)
            {
                ElementDead();
                return true;
            }
            return false;
        }

        public async void CreateMagicCircleDead()
        {
            MagicCircleDead magiccircledead = new MagicCircleDead(player!, playground!, main!, this);

            playground!.Children.Add(magiccircledead.entity);
            main!.entities.Add(magiccircledead);

            await magiccircledead.Action();
        }

        public async Task Dead()
        {
            double pos = Canvas.GetTop(this.entity);
            double opacity = this.entity!.Fill.Opacity;
            while (opacity >= 0)
            {
                pos += 10;
                TimeSpan move = TimeSpan.FromSeconds(0.1);
                await Task.Delay(move);

                DeadAnimation(opacity);
                opacity -= 0.1;
                if (pos <= 230) Canvas.SetTop(this.entity, pos);
            }
            main!.IsWin = true;
            main!.ClearEntity();
        }

        public void ElementDead()
        {
            getHit = true;
            IsDead = true;
            player!.IsDead = false;
            playground!.Children.Remove(healthbar);
        }

        public void DeadAnimation(double opacity)
        {
            newpathimg = pathpic + "darkmagedead.png";
            SetAnimation(120, 100);
            this.entity!.Fill.Opacity = opacity;
        }

        public override async Task Action()
        {
            var pos = Canvas.GetLeft(this.entity);
            var postop = Canvas.GetTop(this.entity);

            CreateHealthBar(1000);
            CreateSecondHealthbar(1000);

            timemove.Start();
            skilltime.Start();

            while (!IsDead)
            {
                if (main!.IsPause)
                {
                    StopTime();
                    await Task.Delay(1);
                    continue;
                }
                else ResumeTime();

                if (player!.IsDead || main!.IsQuit) return;

                if (getHit && !IsHarmless)
                {
                    DecreaseBossHealth();

                    if (CheckDead()) break;
                }

                CheckPhase();

                await Task.Delay(1);

                Move(ref pos, ref postop);

                if (!IsCreateClone) UseSkill();
                else CloneMagic();
            }

            CreateMagicCircleDead();
            await Dead();
        }
    }
}
