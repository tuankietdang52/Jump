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
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump
{
    public class Apostate : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public Rectangle apostate = new Rectangle();

        public int hittime = 0;
        public int health = 2;

        public Apostate()
        {
            height = 204;
            width = 189;

            left = 830;
            top = 150;

            newleft = left;
            newtop = top;

            pathimgentity = pathpic + "apostate.png";
            newpathimg = pathpic + "apostatespawnskill.png";

            entity = apostate;

            SetEntity();
        }

        public override Rect getHitbox()
        {
            Rect hitbox = new Rect(Canvas.GetLeft(entity), Canvas.GetTop(entity), width - 30, height);
            return hitbox;
        }

        public Entity RandomEntity(int entityindex)
        {
            switch (entityindex)
            {
                case 0:
                    GoldFish goldfish = new GoldFish();
                    return goldfish;

                case 1:
                    Demon demon = new Demon();
                    return demon;

                case 2:
                    Mine mine = new Mine();
                    return mine;

                case 3:
                    Bush bush = new Bush();
                    bush.bulletspeed = 70;
                    return bush;

                default:
                    return null!;
            }
        }

        public async void SpawnPrevEntity(double pos)
        {
            Random entityrand = new Random();
            int entittyindex = entityrand.Next(0, 4);

            Entity preventity = RandomEntity(entittyindex);
            preventity.movementspeed = 40;

            Canvas.SetLeft(preventity.entity, pos - 30);

            await main!.SpawnEntity(preventity);
        }

        public bool DeadCheck(int health)
        {
            if (hittime >= health)
            {
                getHit = true;
                IsDead = true;
                return true;
            }
            else if (getHit)
            {
                getHit = false;
                hittime++;
            }
            return false;
        }

        public override async Task Action()
        {
            main!.IsHaveAspotate = true;
            var pos = Canvas.GetLeft(this.entity);

            Stopwatch spawntime = new Stopwatch();

            spawntime.Start();
            while (!IsDead)
            {
                if (player!.IsDead) break;
                await Task.Delay(1);

                if (DeadCheck(health)) break;

                if (spawntime.Elapsed.Seconds == 3)
                {
                    spawntime.Restart();
                    SetNewEntity(204, 189);
                    Playsound(pathsound + "apostateskill.mp3", 100);

                    SpawnPrevEntity(pos);

                    await Task.Delay(1000);
                    SetEntity();
                }
            }
            main!.IsHaveAspotate = false;
        }
    }
}
