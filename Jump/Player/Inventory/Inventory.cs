using Jump.ShopnInvenView;
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

        public MainWindow? main { get; set; }
        public PlayerCharacter? player { get; set; }
        public Gun gun = new Gun();
        public MediaPlayer buysound = new MediaPlayer();
        public ShopnInven? shopninven { get; set; }

        public bool AlreadyHaveGun(string gunname)
        {
            foreach (var item in player!.inventory)
            {
                if (gunname == item) return true;
            }
            return false;
        }

        public void getPathButtonImg(ref string buttonimg, string gunname)
        {
            if (!IsBuy)
            {
                if (AlreadyHaveGun(gunname)) buttonimg = pathpic + "alreadybuy.png";
                else buttonimg = pathpic + "buy.png";
            }

            else buttonimg = CheckGunUsed(gunname);
        }

        public void PlaySound(string gunname)
        {
            buysound.Open(new(pathsound + gunname + "buy.mp3"));
            buysound.Volume = 1;
            buysound.Play();
        }

        public Rectangle ImginShop(string pathgunimg)
        {
            Rectangle img = new Rectangle()
            {
                Width = 300,
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
                Width = 300,
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

        public Button CreateButton(string buttonimg, string gunname)
        {
            Button button;
            if (main!.InShop) button = ButtonBuy(buttonimg);
            else button = ButtonUse(buttonimg);

            if (!IsBuy)
            {
                if (!AlreadyHaveGun(gunname))
                {
                    button.Click += Buy;
                    button.ToolTip = ShowCost(gunname);
                }
            }
            else button.Click += UseGun;

            return button;
        }

        public Image ShowCost(string gunname)
        {
            Image cost = new Image()
            {
                Height = 50,
                Width = 100,
                Source = new BitmapImage(new (getPathCost(gunname))),
            };
            return cost;
        }

        public string getPathCost(string gunname)
        {
            switch (gunname)
            {
                case "m4a4":
                    return pathpic + "m4a4cost.png";

                case "awp":
                    return pathpic + "awpcost.png";

                default:
                    return "";
            }
        }

        public StackPanel Additem(string gunname)
        {
            string buttonimg = "";
            string pathgunimg = pathpic + gunname + ".png";

            StackPanel item = new StackPanel()
            {
                Width = 325,
                Height = 325,
            };

            var img = CreateImage(pathgunimg);

            getPathButtonImg(ref buttonimg, gunname);

            var button = CreateButton(buttonimg, gunname);

            item.Children.Add(img);
            item.Children.Add(button);

            return item;
        }

        public bool CheckCost(string gunname)
        {
            switch (name)
            {
                case "m4a4":
                    cost = 3100;
                    break;

                case "awp":
                    cost = 4750;
                    break;

                default:
                    cost = 0;
                    break;
            }

            if (main!.money < cost) return false;
            else
            {
                main.money -= cost;
                shopninven!.ShowMyMoney();
                return true;
            }
        }

        public void Buy(object sender, RoutedEventArgs e)
        {
            if (!CheckCost(name!)) return;

            Button? buy = sender as Button;
            gun.player = this.player;

            GetItem();

            PlaySound(name!);

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

            switch (name)
            {
                case "de":
                    gun.ChangeGun("de");
                    player!.indexgun = 0;
                    break;
                case "m4a4":
                    gun.ChangeGun("m4a4");
                    player!.indexgun = 1;
                    break;
                case "awp":
                    gun.ChangeGun("awp");
                    player!.indexgun = 2;
                    break;

                default:
                    return;
            }

            if (!IsUse) PlaySound(name);

            main!.getAmountBullet();
            shopninven!.AddItemInventory();
        }

        public string CheckGunUsed(string gunname)
        {
            if (gun.listgun[player!.indexgun] == gunname)
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
