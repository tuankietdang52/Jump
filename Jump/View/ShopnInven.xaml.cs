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

namespace Jump.View
{
    public partial class ShopnInven : UserControl
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public MainWindow? main { get; set; }
        public ShopnInven() { }
        public ShopnInven(MainWindow main)
        {
            InitializeComponent();
            this.main = main;

            SetShopnInven();
            ShowMyMoney();
        }
        public void HandleContinue(object sender, RoutedEventArgs e)
        {
            main!.ContinueToGame(this);
        }

        public void SetShopnInven()
        {
            ItemRifleStalls();
            ItemRifleStalls2();
        }

        public void ShowMyMoney()
        {
            mymoney.Text = Convert.ToString("$" + main!.money);
        }

        public void ItemRifleStalls()
        {
            AddItemShop("r8", riflestall);
            AddItemShop("ssg08", riflestall);
            AddItemShop("m4a1s", riflestall);
        }

        public void ItemRifleStalls2()
        {
            AddItemShop("ak47", riflestall2);
            AddItemShop("m4a4", riflestall2);
            AddItemShop("awp", riflestall2);
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

            newitem.getGunType();

            stalls.Children.Add(newitem.Additem());
        }

        // INVENTORY //

        public void AddItemInventory()
        {
            inventoryitem.Items.Clear();

            StackPanel itemstalls = NewItemStalls();

            foreach (var item in this.main!.player.inventory)
            {
                if (itemstalls.Children.Count == 3)
                {
                    itemstalls = NewItemStalls();
                }

                Inventory newitem = new Inventory()
                {
                    IsBuy = true,
                    name = item,
                    player = this.main!.player,
                    main = this.main,
                    shopninven = this,
                };

                newitem.getGunType();

                itemstalls.Children.Add(newitem.Additem());
            }
        }

        public StackPanel NewItemStalls()
        {
            StackPanel itemstalls = new StackPanel()
            {
                Height = 325,
                Width = 900,
                Orientation = Orientation.Horizontal,
            };

            inventoryitem.Items.Add(itemstalls);

            return itemstalls;
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