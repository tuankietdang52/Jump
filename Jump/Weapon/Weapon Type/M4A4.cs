﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump.Weapon.Weapon_Type
{
    public class M4A4 : Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public M4A4()
        {
            cost = 3100;
        }

        public M4A4(PlayerCharacter player, MainWindow main)
        {
            this.player = player;
            this.main = main;

            damage = 45;
            bulletlimit = 30;
            magazinebulletlimit = 90;

            reloadtime = 1800;

            bulletAmount = bulletlimit;
            magazinebullet = magazinebulletlimit;

            bulletspeed = 100;
        }

        public override void getPathPlayer()
        {
            player!.stand = pathpic + "m4a4stand.png";
            player!.standshoot = pathpic + "m4a4shoot.png";
            player!.crouchshoot = pathpic + "m4a4crouchshoot.png";
            player!.jumpgun = pathpic + "m4a4jump.png";
            player!.jumpshoot = pathpic + "m4a4jumpshoot.png";
        }

        public override void getSound()
        {
            gunsoundpath = pathsound + "m4a4sound.mp3";
            reloadsoundpath = pathsound + "m4a4reload.mp3";
        }

        public override string getPathCost()
        {
            return pathpic + "m4a4cost.png";
        }

        public override string getPathGun()
        {
            return pathpic + "m4a4.png";
        }

        public override string getPathBuySound()
        {
            return pathsound + "m4a4buy.mp3";
        }
    }
}
