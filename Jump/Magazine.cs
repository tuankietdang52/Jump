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
    public class Magazine : Item
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public Rectangle magazine = new Rectangle();
        public Magazine()
        {
            topnum = new double[2] { 150, 285 };

            height = 40;
            width = 40;

            speed = 50;

            left = 1000;

            imgpath = pathpic + "magazine.png";

            item = magazine;

            RandomTop();
            SetItem();
        }
    }
}
