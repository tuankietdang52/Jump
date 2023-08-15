using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump
{
    public class Gun
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathgun = $"{Directory.GetCurrentDirectory()}\\ListGun.txt";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public PlayerCharacter? player { get; set; }
        public List<string> listgun = new List<string>();

        public Gun()
        {
            using var read = new StreamReader(pathgun);
            string line;
            while (true)
            {
                if ((line = read.ReadLine()!) == null) break;
                listgun.Add(line);
            }
        }

        public void ChangeGun(string name)
        {
            switch (name)
            {
                case "de":
                    getPathGun(0);
                    break;
                case "m4a4":
                    getPathGun(1);
                    break;
                default:
                    return;
            }
        }

        public void getPathGun(int indexgun)
        {
            player!.stand = pathpic + listgun[indexgun] + "stand.png";
            player!.standshoot = pathpic + listgun[indexgun] + "shoot.png";
            player!.crouchshoot = pathpic + listgun[indexgun] + "crouchshoot.png";
            player!.jumpgun = pathpic + listgun[indexgun] + "jump.png";
            player!.jumpshoot = pathpic + listgun[indexgun] + "jumpshoot.png";
        }

        public void getGunsound(ref string gunsoundpath, int indexgun)
        {
            gunsoundpath = pathsound + listgun[indexgun] + "sound.mp3";
        }

        public void getReloadsound(ref string reloadsoundpath, int indexgun)
        {
            reloadsoundpath = pathsound + listgun[indexgun] + "reload.mp3";
        }

        public void getBullet(int indexgun, ref int bulletlimit, ref int magazinebulletlimit)
        {
            switch (indexgun)
            {
                case 0:
                    bulletlimit = 7;
                    magazinebulletlimit = 35;
                    break;
                case 1:
                    bulletlimit = 30;
                    magazinebulletlimit = 90;
                    break;
                default:
                    return;
            }
        }

        public int getBulletspeed(int indexgun)
        {
            switch (indexgun)
            {
                case 0:
                    return 100;
                case 1:
                    return 200;
                default:
                    return 0;
            }
        }
    }
}
