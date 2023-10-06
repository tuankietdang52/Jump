using Jump.View;
using Jump.Weapon.Weapon_Type;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump
{
    public class Inventory
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public string? name { get; set; }

        public bool IsBuy = false;
        public bool IsUse = false;

        public int cost { get; set; }
        public string? pathcost { get; set; }
        public string? pathgunimg { get; set; }
        public string? pathbuysound { get; set; }

        public MainWindow? main { get; set; }
        public PlayerCharacter? player { get; set; }
        public Gun gun = new Gun();
        public MediaPlayer buysound = new MediaPlayer();
        public ShopnInven? shopninven { get; set; }


        public void getGunType()
        {
            switch (name)
            {
                case "de":
                    DesertEagle de = new DesertEagle();
                    gun = de;
                    break;

                case "m4a4":
                    M4A4 m4a4 = new M4A4();
                    gun = m4a4;
                    break;

                case "awp":
                    AWP awp = new AWP();
                    gun = awp;
                    break;

                case "m4a1s":
                    M4A1S m4a1s = new M4A1S();
                    gun = m4a1s;
                    break;

                case "ak47":
                    AK47 ak47 = new AK47();
                    gun = ak47;
                    break;

                case "ssg08":
                    SSG08 ssg08 = new SSG08();
                    gun = ssg08;
                    break;

                case "r8":
                    R8 r8 = new R8();
                    gun = r8;
                    break;
            }

            getElementGun();
        }

        public void getElementGun()
        {
            gun.player = this.player;
            cost = gun.cost;
            pathgunimg = gun.getPathGun();
            pathcost = gun.getPathCost();
            pathbuysound = gun.getPathBuySound();
        }

        public bool AlreadyHaveGun()
        {
            foreach (var item in player!.inventory)
            {
                if (name == item) return true;
            }
            return false;
        }

        public void getPathButtonImg(ref string buttonimg)
        {
            if (!IsBuy)
            {
                if (AlreadyHaveGun()) buttonimg = pathpic + "alreadybuy.png";
                else buttonimg = pathpic + "buy.png";
            }

            else buttonimg = CheckGunUsed();
        }

        public void PlaySound()
        {
            buysound.Open(new(pathbuysound!));
            buysound.Volume = 1;
            buysound.Play();
        }

        public Rectangle ImginShop(string pathgunimg)
        {
            Rectangle img = new Rectangle()
            {
                Width = 250,
                Height = 210,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathgunimg)),
                }
            };
            return img;
        }

        public Rectangle ImginInventory(string pathgunimg)
        {
            Rectangle img = new Rectangle()
            {
                Width = 200,
                Height = 210,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathgunimg)),
                }
            };
            return img;
        }

        public Rectangle CreateImage(string pathgunimg)
        {
            Rectangle img;

            if (main!.InShop) img = ImginShop(pathgunimg);
            else img = ImginInventory(pathgunimg);

            return img;
        }

        public Button ButtonBuy(string buttonimg)
        {
            Button buy = new Button()
            {
                Width = 250,
                Height = 100,
                Content = new Image
                {
                    Source = new BitmapImage(new(buttonimg)),
                    VerticalAlignment = VerticalAlignment.Center,
                    Stretch = Stretch.Fill,
                },
            };
            return buy;
        }

        public Button ButtonUse(string buttonimg)
        {
            Button use = new Button()
            {
                Width = 200,
                Height = 100,
                Content = new Image
                {
                    Source = new BitmapImage(new(buttonimg)),
                    VerticalAlignment = VerticalAlignment.Center,
                    Stretch = Stretch.Fill,
                },
            };
            return use;
        }

        public Button CreateButton(string buttonimg)
        {
            Button button;
            if (main!.InShop) button = ButtonBuy(buttonimg);
            else button = ButtonUse(buttonimg);

            if (!IsBuy)
            {
                if (!AlreadyHaveGun())
                {
                    button.Click += Buy;
                    button.ToolTip = ShowCost();
                }
            }
            else button.Click += UseGun;

            return button;
        }

        public Image ShowCost()
        {
            Image cost = new Image()
            {
                Height = 50,
                Width = 100,
                Source = new BitmapImage(new (pathcost!)),
            };
            return cost;
        }

        public StackPanel Additem()
        {
            string buttonimg = "";

            StackPanel item = new StackPanel()
            {
                Width = 275,
                Height = 325,
            };

            var img = CreateImage(pathgunimg!);

            getPathButtonImg(ref buttonimg);

            var button = CreateButton(buttonimg);

            item.Children.Add(img);
            item.Children.Add(button);

            return item;
        }

        public bool CheckCost()
        {
            if (main!.money < cost) return false;
                
            main.money -= cost;
            shopninven!.ShowMyMoney();
            return true;
        }

        public void Buy(object sender, RoutedEventArgs e)
        {
            if (!CheckCost()) return;

            Button? buy = sender as Button;

            GetItem();

            PlaySound();

            buy!.Content = new Image
            {
                Source = new BitmapImage(new(pathpic + "alreadybuy.png")),
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Fill,
            };

            buy!.Click -= Buy;
            buy.ToolTip = null;
        }

        public void GetItem()
        {
            player!.inventory.Add(name!);
        }

        public void UseGun(object sender, RoutedEventArgs e)
        {
            Button use = (Button)sender;
            gun.player = this.player;

            player!.ChangeGun(name!);

            if (!IsUse) PlaySound();

            shopninven!.AddItemInventory();
        }

        public string CheckGunUsed()
        {
            if (player!.currentgun == name)
            {
                IsUse = true;
                return pathpic + "inuse.png";
            }
            else
            {
                IsUse = false;
                return pathpic + "replace.png";
            }
        }
    }
}
