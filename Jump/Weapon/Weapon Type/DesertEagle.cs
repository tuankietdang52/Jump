﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump.Weapon.Weapon_Type
{
    public class DesertEagle : Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public DesertEagle() { }

        public DesertEagle(PlayerCharacter player, MainWindow main)
        {
            this.player = player;
            this.main = main;

            damage = 50;
            bulletlimit = 7;
            magazinebulletlimit = 35;

            reloadtime = 1500;

            bulletAmount = bulletlimit;
            magazinebullet = magazinebulletlimit;

            bulletspeed = 50;
        }

        public override void getPathPlayer()
        {
            player!.stand = pathpic  + "destand.png";
            player!.standshoot = pathpic + "deshoot.png";
            player!.crouchshoot = pathpic + "decrouchshoot.png";
            player!.jumpgun = pathpic + "dejump.png";
            player!.jumpshoot = pathpic + "dejumpshoot.png";
        }

        public override void getSound()
        {
            gunsoundpath = pathsound + "desound.mp3";
            reloadsoundpath = pathsound + "dereload.mp3";
        }

        public override string getPathGun()
        {
            return pathpic + "de.png";
        }

        public override string getPathBuySound()
        {
            return pathsound + "debuy.mp3";
        }
    }
}
