using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump.Weapon.Weapon_Type
{
    public class AK47 : Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        
        public AK47()
        {
            cost = 2700;
        }
        public AK47(PlayerCharacter player, MainWindow main)
        {
            this.player = player;
            this.main = main;

            damage = 65;
            bulletlimit = 30;
            magazinebulletlimit = 90;

            reloadtime = 1700;

            bulletAmount = bulletlimit;
            magazinebullet = magazinebulletlimit;

            bulletspeed = 70;
        }

        public override void getPathPlayer()
        {
            player!.stand = pathpic + "ak47stand.png";
            player!.standshoot = pathpic + "ak47shoot.png";
            player!.crouchshoot = pathpic + "ak47crouchshoot.png";
            player!.jumpgun = pathpic + "ak47jump.png";
            player!.jumpshoot = pathpic + "ak47jumpshoot.png";
        }

        public override void getSound()
        {
            gunsoundpath = pathsound + "ak47sound.mp3";
            reloadsoundpath = pathsound + "ak47reload.mp3";
        }

        public override string getPathCost()
        {
            return pathpic + "ak47cost.png";
        }

        public override string getPathGun()
        {
            return pathpic + "ak47.png";
        }

        public override string getPathBuySound()
        {
            return pathsound + "ak47buy.mp3";
        }
    }
}
