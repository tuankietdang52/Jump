using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump.Weapon.Weapon_Type
{
    public class AWP : Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public AWP(PlayerCharacter player, MainWindow main)
        {
            this.player = player;
            this.main = main;

            damage = 80;
            bulletlimit = 5;
            magazinebulletlimit = 30;

            bulletAmount = bulletlimit;
            magazinebullet = magazinebulletlimit;

            bulletspeed = 100;
        }

        public override void getPathPlayer()
        {
            player!.stand = pathpic + "awpstand.png";
            player!.standshoot = pathpic + "awpshoot.png";
            player!.crouchshoot = pathpic + "awpcrouchshoot.png";
            player!.jumpgun = pathpic + "awpjump.png";
            player!.jumpshoot = pathpic + "awpjumpshoot.png";
        }

        public override void getSound()
        {
            gunsoundpath = pathsound + "awpsound.mp3";
            reloadsoundpath = pathsound + "awpreload.mp3";
        }
    }
}
