using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Jump.Weapon.Weapon_Type
{
    public class R8 : Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public MediaPlayer r8preshootsound = new MediaPlayer();

        public R8()
        {
            cost = 600;
        }

        public R8(PlayerCharacter player, MainWindow main)
        {
            this.player = player;
            this.main = main;

            damage = 100;
            bulletlimit = 8;
            magazinebulletlimit = 8;

            reloadtime = 1100;

            bulletAmount = bulletlimit;
            magazinebullet = magazinebulletlimit;

            bulletspeed = 50;
        }

        public override void getPathPlayer()
        {
            player!.stand = pathpic + "r8stand.png";
            player!.standshoot = pathpic + "r8shoot.png";
            player!.crouchshoot = pathpic + "r8crouchshoot.png";
            player!.jumpgun = pathpic + "r8jump.png";
            player!.jumpshoot = pathpic + "r8jumpshoot.png";
        }

        public override void getSound()
        {
            gunsoundpath = pathsound + "r8sound.mp3";
            reloadsoundpath = pathsound + "r8reload.mp3";
        }

        public override string getPathGun()
        {
            return pathpic + "r8.png";
        }

        public override string getPathBuySound()
        {
            return pathsound + "r8buy.mp3";
        }

        public override string getPathCost()
        {
            return pathpic + "r8cost.png";
        }


        public override void OtherGunSound()
        {
            r8preshootsound.Open(new(pathsound + "r8preshoot.mp3"));
            r8preshootsound.Volume = 1;
            r8preshootsound.Play();
        }
    }
}
