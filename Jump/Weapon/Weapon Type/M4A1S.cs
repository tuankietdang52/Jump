using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Jump.Weapon.Weapon_Type
{
    public class M4A1S : Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public M4A1S()
        {
            cost = 2900;
        }

        public M4A1S(PlayerCharacter player, MainWindow main)
        {
            this.player = player;
            this.main = main;

            damage = 35;

            reloadtime = 1800;

            bulletlimit = 25;
            magazinebulletlimit = 75;

            bulletAmount = bulletlimit;
            magazinebullet = magazinebulletlimit;

            bulletspeed = 100;
        }

        public override void getPathPlayer()
        {
            player!.stand = pathpic + "m4a1sstand.png";
            player!.standshoot = pathpic + "m4a1sshoot.png";
            player!.crouchshoot = pathpic + "m4a1scrouchshoot.png";
            player!.jumpgun = pathpic + "m4a1sjump.png";
            player!.jumpshoot = pathpic + "m4a1sjumpshoot.png";
        }

        public override void getSound()
        {
            gunsoundpath = pathsound + "m4a1ssound.mp3";
            reloadsoundpath = pathsound + "m4a1sreload.mp3";
        }

        public override string getPathCost()
        {
            return pathpic + "m4a1scost.png";
        }

        public override string getPathGun()
        {
            return pathpic + "m4a1s.png";
        }

        public override string getPathBuySound()
        {
            return pathsound + "m4a1sbuy.mp3";
        }
    }
}
