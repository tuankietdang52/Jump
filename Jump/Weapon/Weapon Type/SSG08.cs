using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Jump.Weapon.Weapon_Type
{
    public class SSG08 : Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public MediaPlayer awpwaitsound = new MediaPlayer();

        public SSG08()
        {
            cost = 1700;
        }

        public SSG08(PlayerCharacter player, MainWindow main)
        {
            this.player = player;
            this.main = main;

            damage = 40;
            bulletlimit = 10;
            magazinebulletlimit = 90;

            bulletAmount = bulletlimit;
            magazinebullet = magazinebulletlimit;

            waittime = 500;
            reloadtime = 2600;

            bulletspeed = 100;
        }

        public override void getPathPlayer()
        {
            player!.stand = pathpic + "ssg08stand.png";
            player!.standshoot = pathpic + "ssg08shoot.png";
            player!.crouchshoot = pathpic + "ssg08crouchshoot.png";
            player!.jumpgun = pathpic + "ssg08jump.png";
            player!.jumpshoot = pathpic + "ssg08jumpshoot.png";
        }

        public override void getSound()
        {
            gunsoundpath = pathsound + "ssg08sound.mp3";
            reloadsoundpath = pathsound + "ssg08reload.mp3";
        }

        public override string getPathCost()
        {
            return pathpic + "ssg08cost.png";
        }

        public override string getPathGun()
        {
            return pathpic + "ssg08.png";
        }

        public override string getPathBuySound()
        {
            return pathsound + "ssg08buy.mp3";
        }

        public override void OtherGunSound()
        {
            awpwaitsound.Open(new(pathsound + "ssg08buy.mp3"));
            awpwaitsound.Volume = 1;
            awpwaitsound.Play();
        }
    }
}