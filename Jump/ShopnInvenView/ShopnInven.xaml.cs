using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace Jump.ShopnInvenView
{
    public partial class ShopnInven : UserControl
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public MainWindow? main { get; set; }
        public ShopnInven(MainWindow main)
        {
            InitializeComponent();
            this.main = main;

            SetImg();
            SetShopnInven();
            ShowMyMoney();
        }
        public void HandleContinue(object sender, RoutedEventArgs e)
        {
            main!.ContinueToGame(this);
        }

        public void SetImg()
        {
            ButtonPlayImg();
            ButtonShopImg();
            ButtonInventoryImg();
            ShopBackground();
            InventoryBackground();
        }

        public void SetShopnInven()
        {
            ItemEquipmentStalls();
            ItemRifleStalls();
        }

        public void ButtonPlayImg()
        {
            play.Content = new Image
            {
                Source = new BitmapImage(new(pathpic + "go.png")),
            };
        }

        public void ButtonShopImg()
        {
            shop.Header = new Image
            {
                Source = new BitmapImage(new(pathpic + "shop.jpg")),
                Stretch = Stretch.Fill,
            };
        }

        public void ButtonInventoryImg()
        {
            inventory.Header = new Image
            {
                Source = new BitmapImage(new(pathpic + "inventory.jpg")),
                Stretch = Stretch.Fill,
            };
        }

        public void ShopBackground()
        {
            weaponstalls.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "shopbackground.png")),
                Stretch = Stretch.Fill,
            };
        }

        public void InventoryBackground()
        {
            inventoryitem.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "inventorybackground.jpg")),
                Stretch = Stretch.Fill,
            };
        }

        public void ShowMyMoney()
        {
            mymoney.Text = Convert.ToString("$" + main!.money);
        }

        public void ItemEquipmentStalls()
        {


        }

        public void ItemRifleStalls()
        {
            AddItemShop("m4a4", rifle);
            AddItemShop("awp", rifle);
        }

        public void AddItemShop(string gunname, StackPanel stalls)
        {
            Inventory newitem = new Inventory()
            {
                name = gunname,
                main = this.main,
                player = this.main!.player,
                shopninven = this,
            };

            stalls.Children.Add(newitem.Additem(gunname));
        }

        // INVENTORY //

        public void AddItemInventory()
        {
            inventoryitem.Children.Clear();

            foreach (var item in this.main!.player.inventory)
            {
                Inventory newitem = new Inventory()
                {
                    IsBuy = true,
                    name = item,
                    player = this.main!.player,
                    main = this.main,
                    shopninven = this,
                };

                inventoryitem.Children.Add(newitem.Additem(item));
            }
        }

        public void UpdateInventory(object sender, SelectionChangedEventArgs e)
        {
            TabControl shopninven = (TabControl)sender;

            if (shopninven.SelectedIndex == 1)
            {
                main!.InInventory = true;
                main.InShop = false;
                AddItemInventory();
            }
            else
            {
                main!.InShop = true;
                main.InInventory = false;
            }
        }
    }
}