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
    public class GoldFish : Entity
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        public Rectangle goldFish = new Rectangle();
        public GoldFish()
        {
            height = 60;
            width = 60;
            thickness = new Thickness(0, 0, -1000, 10);
            pathimgentity = pathpic + "goldfish.png";

            entity = goldFish;

            SetEntity();
        }
    }
}
