using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
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

namespace Jump
{
    public class Samurai : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle samurai = new Rectangle();
        public MediaPlayer slashsound = new MediaPlayer();

        public Random actionrand = new Random();
        public TimeSpan soundpos;

        public int actions = 0;

        public bool IsShoot = false;
        public bool resumesound = false;

        public int actionindex;
        public int slashindex = 0;

        public Samurai()
        {
            height = 232;
            width = 182;

            movementspeed = 100;

            left = 780;
            top = 120;

            armor = 40;

            pathimgentity = pathpic + "samurai.png";

            entity = samurai;

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

        public void CheckSecondHealthbar()
        {
            if (secondhealthbar!.Width == 0)
            {
                playground!.Children.Remove(secondhealthbar);
                secondhealthbar = null;
            }
        }

        public void PlaySwordSound(string path)
        {
            slashsound.Open(new(path));
            slashsound.Volume = 1;
            slashsound.Play();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height);
            return hitbox;
        }

        public void Dead()
        {
            getHit = true;
            IsDead = true;
            main!.ClearEntity();
            player!.IsDead = false;
            playground!.Children.Remove(healthbar);
        }

        public void ChangeAnimation(string namepath)
        {
            newleft = Canvas.GetLeft(this.entity);
            newtop = Canvas.GetTop(this.entity);

            newpathimg = namepath;

            this.left = left;
            this.top = top;

            SetNewEntity(entity!.Height, entity.Width);
        }

        public void SetCrouch()
        {
            string crouchshoot = pathpic + "samuraicrouchshoot.png";
            Canvas.SetTop(this.entity, 182);
            entity!.Height = 182;
            ChangeAnimation(crouchshoot);
        }

        public void SetDefault()
        {
            Canvas.SetLeft(this.entity, 780);
            newtop = 120;

            newpathimg = pathpic + "samurai.png";

            SetNewEntity(232, 182);
        }

        public void GetRandomSkill(double pos)
        {
            actionindex = actionrand.Next(0, 14);
            UseSkill(actionindex, pos);
        }

        public async void UseSkill(int actionindex, double pos)
        {
            if (actions != 0) return;
            switch (actionindex)
            {
                case 0: case 1:
                    if (healthbar!.Width <= 350) await CrouchShoot();
                    else await Task.Delay(1);
                    return;

                case 2: case 3:case 4: case 5:
                    await StandShoot();
                    return;

                case 6: case 7: case 8: 
                    await CrouchShoot();
                    return;

                case 9: case 10: case 11: case 12: case 13:
                    if (!IsShoot) Slash();
                    return;

                default:
                    await Task.Delay(1);
                    return;
            }
        }

        public async void CreateBullet(double left, double top)
        {
            Mac10Bullet mac10Bullet = new Mac10Bullet(left, top, player!, playground!, main!);

            mac10Bullet.boss = this;

            main!.entities.Add(mac10Bullet);
            playground!.Children.Add(mac10Bullet.entity);

            await mac10Bullet.Action();
        }

        public override void SoundEnd(object sender, EventArgs e)
        {
            IsShoot = false;
        }

        public async Task StandShoot()
        {
            IsShoot = true;
            actions++;

            string mac10sound = pathsound + "mac10sound.mp3";

            Playsound(mac10sound, 0.2);

            ChangeAnimation(pathpic + "samuraishoot.png");

            while (IsShoot && !IsDead)
            {
                if (main!.IsPause)
                {
                    soundpos = GetPositonSound();
                    resumesound = true;
                    await Task.Delay(1);
                    continue;
                }
                else if (resumesound)
                {
                    ResumeSound(soundpos);
                    resumesound = false;
                }

                await Task.Delay(100);

                var left = Canvas.GetLeft(this.entity);
                var top = Canvas.GetTop(this.entity);

                CreateBullet(left, top + 110);
            }

            ChangeAnimation(pathpic + "samurai.png");
            await Task.Delay(500);
            actions--;
        }

        public async Task CrouchShoot()
        {
            actions++;

            SetCrouch();

            string mac10sound = pathsound + "mac10sound.mp3";

            Playsound(mac10sound, 0.2);

            int bulletcount = 0;

            while (bulletcount <= 1)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                await Task.Delay(50);

                var left = Canvas.GetLeft(this.entity);
                var top = Canvas.GetTop(this.entity);

                CreateBullet(left, top + 80);
                bulletcount++;
            }

            soundplay.Stop();

            SetDefault();
            actions--;
        }

        public async void Slash()
        {
            if (IsShoot) return;

            Random slashpos = new Random();
            slashindex = slashpos.Next(0, 6);

            actions++;

            string readysound = pathsound + "holdsword.mp3";
            PlaySwordSound(readysound);

            switch (slashindex)
            {
                case 0: case 1: case 2: case 3:
                    slashindex = 0;
                    await SlashTop();
                    return;

                case 4: case 5:
                    slashindex = 1;
                    await SlashBottom();
                    return;
            }
        }

        public void Dash(ref double pos)
        {
            Canvas.SetLeft(this.entity, pos);
            pos -= 100;
        }

        public void DashBack(ref double pos)
        {
            Canvas.SetLeft(this.entity, pos);
            pos += 100;
        }

        public async Task SlashTop()
        {
            string ready = pathpic + "samuraireadystand.png";
            ChangeAnimation(ready);

            await Task.Delay(500);

            var pos = Canvas.GetLeft(this.entity);

            string dash = pathpic + "samuraidash.png";
            ChangeAnimation(dash);

            while (pos >= 150)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                TimeSpan move = TimeSpan.FromSeconds(0.01);
                await Task.Delay(move);
                Dash(ref pos);
            }

            string slashtop = pathpic + "samuraislashtop.png";
            Canvas.SetTop(this.entity, 150);
            ChangeAnimation(slashtop);

            Check();
        }

        public async Task SlashBottom()
        {
            string ready = pathpic + "samuraireadystandcrouch.png";
            Canvas.SetTop(this.entity, 150);
            ChangeAnimation(ready);

            await Task.Delay(500);

            var pos = Canvas.GetLeft(this.entity);

            string dash = pathpic + "samuraidash.png";
            ChangeAnimation(dash);

            while (pos >= 150)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                TimeSpan move = TimeSpan.FromSeconds(0.04);
                await Task.Delay(move);
                Dash(ref pos);
            }

            string slashtop = pathpic + "samuraislashbot.png";
            Canvas.SetTop(this.entity, 150);
            ChangeAnimation(slashtop);

            Check();
        }

        public async Task Back()
        {
            var pos = Canvas.GetLeft(this.entity);
            while (pos <= 780)
            {
                if (main!.IsPause)
                {
                    await Task.Delay(1);
                    continue;
                }

                TimeSpan move = TimeSpan.FromSeconds(0.01);
                await Task.Delay(move);
                DashBack(ref pos);
            }

            SetDefault();
            actions--;
        }

        public bool CheckPlayer()
        {
            if (CheckArmor()) return false;

            player!.IsDead = true;
            return true;
        }

        public async void Check()
        {
            string soundpath;
            var top = Canvas.GetTop(player!.playershape);
            switch (slashindex)
            {
                case 0:
                    if (top >= 250) break;

                    if (CheckPlayer())
                    {
                        soundpath = pathsound + "slashhit.mp3";
                        PlaySwordSound(soundpath);
                        return;
                    }
                    break;

                case 1:
                    if (top <= 195) break;

                    if (CheckPlayer())
                    {
                        soundpath = pathsound + "slashhit.mp3";
                        PlaySwordSound(soundpath);
                        return;
                    }
                    break;

                default:
                    break;
            }

            soundpath = pathsound + "slash.mp3";
            PlaySwordSound(soundpath);

            await Task.Delay(500);

            ChangeAnimation(pathpic + "samuraiback.png");
            await Back();
        }

        public void CheckHealth()
        {
            if (secondhealthbar == null) healthbar!.Width -= GetDamage(healthbar);
            else
            {
                secondhealthbar.Width -= GetDamage(secondhealthbar);
                CheckSecondHealthbar();
            }

            getHit = false;
        }

        public override async Task Action()
        {
            CreateHealthBar(850);
            CreateSecondHealthbar(425);

            double pos = Canvas.GetLeft(this.entity);

            Stopwatch skilltime = new Stopwatch();

            skilltime.Start();

            while (!IsDead)
            {
                if (main!.IsPause)
                {
                    skilltime.Stop();
                    await Task.Delay(1);
                    continue;
                }
                else skilltime.Start();

                if (player!.IsDead || main.IsQuit) return;

                if (getHit)
                {
                    CheckHealth();
                    if (healthbar!.Width <= 0)
                    {
                        Dead();
                        return;
                    }
                }

                if (skilltime.Elapsed.Seconds == 1)
                {
                    skilltime.Restart();
                    if (!IsShoot) GetRandomSkill(pos);
                }

                await Task.Delay(1);
            }
        }

    }
}
