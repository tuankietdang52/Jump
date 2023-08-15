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
using System.Threading;
using System.Numerics;
using System.Threading.Tasks.Sources;
using System.Windows.Automation;
using System.Windows.Media.Animation;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Security.Cryptography;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.Reflection;
using System.Windows.Automation.Provider;
using System.Security.Policy;

namespace Jump
{
    public partial class MainWindow : Window
    {
        public int phase = 1;
        public int score = 0;
        public int timechange = 40;
        public int changetime = 0;
        public int hiscore;
        public int bulletAmount;
        public int bulletlimit;
        public int magazinebullet;
        public int magazinebulletlimit;
        public int mapindex = 1;
        public int limitchangetime = 5;
        public double volumeadjust = 0.2;
        public int money = 0;

        public bool IsJump = false;
        public bool IsShoot = false;
        public bool IsCrouch = false;
        public bool IsReload = false;
        public bool IsBreakHiScore = false;
        public bool IsReplay = false;
        public bool IsHoldCtrlLeft = false;
        public bool IsChangeMap = false;
        public bool NotChangeMap = false;
        public bool InShop = false;

        private readonly string pathsave = $"{Directory.GetCurrentDirectory()}\\HiScore.txt";
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public string? backgroundpath;
        public string? undergroundpath;
        public string? themepath;

        public Entity killer = new Entity();
        public Stopwatch timetochangemov = new Stopwatch();
        public PlayerCharacter player = new PlayerCharacter();
        public MediaPlayer theme = new MediaPlayer();
        public MediaPlayer voice = new MediaPlayer();
        public ListEntity changeentity = new ListEntity();
        public Gun gun = new Gun();
        public List<Entity> entities = new List<Entity>();
        public Phase setphase = new Phase();
        public ListGun listgun = new ListGun();
        public Item item = new Item();
        public MainWindow()
        {
            InitializeComponent();

            ChangeGameVisibility(Visibility.Hidden);

            try
            {
                CreateGameDisplay();
                MainMenu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void ChangeGameVisibility(Visibility visibility)
        {
            Score.Visibility = visibility;
            HiScore.Visibility = visibility;
            BulletAmount.Visibility = visibility;
            Map.Visibility = visibility;
            Ground.Visibility = visibility;
            Money.Visibility = visibility;
        }

        // PRE START GAME //

        public void MainMenu()
        {
            string pathmainmenu = pathsound + "Melody of Guidance.mp3";

            PlayTheme(pathmainmenu, 1);

            Ground.Visibility = Visibility.Hidden;

            Rectangle Title = new()
            {
                Name = "Title",
                Width = 700,
                Height = 200,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "Title.png")),
                    Stretch = Stretch.Fill,
                },
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
            };

            RegisterName(Title.Name, Title);
            Playground.Children.Add(Title);

            CreateStartButton();
        }

        private void CreateStartButton()
        {
            TextBlock starttxt = new()
            {
                FontSize = 25,
                Text = "START",

                Foreground = Brushes.LightSkyBlue,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid startstack = new()
            {
                Height = 50,
                Width = 300,
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "start.jpg")),
                    Stretch = Stretch.Fill,
                }
            };

            Button start = new()
            {
                Name = "Start",
                Height = 50,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = startstack
            };


            start.Click += PreStartGame;

            RegisterName(start.Name, start);

            startstack.Children.Add(starttxt);


            Playground.Children.Add(start);
        }

        private void PreStartGame(object sender, RoutedEventArgs e)
        {
            Rectangle title = (Rectangle)Playground.FindName("Title");
            Button start = (Button)Playground.FindName("Start");

            Playground.Children.Remove(title);
            Playground.Children.Remove(start);

            UnregisterName("Title");
            UnregisterName("Start");

            theme.Stop();

            ChangeGameVisibility(Visibility.Visible);

            SetPlayer();

            setphase.main = (MainWindow)Main;

            RestartEntitySpeed();
            ChangeMapName("Galaxy", Brushes.Purple);

            try
            {
                GameStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        public void SetPlayer()
        {
            player.playershape.Name = "Player";
            player.main = (MainWindow?)Main;
            gun.player = this.player;

            RegisterName(player.playershape.Name, player.playershape);

            Playground.Children.Add(player.playershape);

            getAmountBullet();
        }

        private void CreateGameDisplay()
        {
            backgroundpath = pathpic + "galaxy.jpg";
            undergroundpath = pathpic + "moon.png";
            ChangeBackground();

            theme.MediaEnded += Looptheme!;
        }


        // BACKGROUND AND MAP //

        public void ChangeBackground()
        {
            Playground.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new(backgroundpath!)),
                Stretch = Stretch.Fill,
            };
            Ground.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(undergroundpath!)),
            };
        }

        public async Task BlackScreenChanging()
        {
            player.playershape.Visibility = Visibility.Hidden;
            while (BlackBackground.Fill.Opacity < 1)
            {
                BlackBackground.Fill.Opacity += 0.3;
                theme.Volume -= 0.3;
                await Task.Delay(100);
            }

            ChangeBackground();
            theme.Stop();

            await Task.Delay(1000);

            while (BlackBackground.Fill.Opacity > 0)
            {
                BlackBackground.Fill.Opacity -= 0.3;
                await Task.Delay(100);
            }
            BlackBackground.Fill.Opacity = 0;

            player.playershape.Visibility = Visibility.Visible;
        }

        public async Task ChangeMap()
        {
            if (!GetPathMapandTheme() && !player.IsDead)
            {
                NotChangeMap = true;
                return;
            }

            await BlackScreenChanging();

            IsChangeMap = true;

        }

        public void ChangeMapName(string mapname, SolidColorBrush color)
        {
            Map.Text = "Map: " + mapname;
            Map.Foreground = color;
        }

        public bool GetPathMapandTheme()
        {
            switch (mapindex)
            {
                case 2:
                    backgroundpath = pathpic + "jungle.jpg";
                    undergroundpath = pathpic + "dirt.jpg";
                    themepath = pathsound + "Touhou 7 Stage 2.mp3";
                    volumeadjust = 0.6;
                    ChangeMapName("Jungle", Brushes.LightGreen);
                    return true;

                default:
                    mapindex--;
                    return false;
            }
        }

        // MONEY //

        public void ShowMoney()
        {
            Money.Text = "Money: " + money;
        }

        public void GetMoney(int amountmoney)
        {
            money += amountmoney;
            ShowMoney();
        }

        public void GetMoneyType(Entity entity)
        {
            switch (entity)
            {
                case Demon:
                    GetMoney(100);
                    break;
                case Bush:
                    if (!entity.IsHarmless) GetMoney(150);
                    break;
                default:
                    GetMoney(50);
                    break;
            }
        }

        // SCORE //

        public void getHiScore()
        {
            using var load = new StreamReader(pathsave);
            hiscore = int.Parse(load.ReadToEnd());
            HiScore.Text = "Highest Score: " + hiscore;
            load.Close();
        }

        public void ScoreUp(int amountscore)
        {
            score += amountscore;
            Score.Text = "Score: " + score;

            if (score < hiscore) return;

            if (!IsBreakHiScore)
            {
                string pathbreakHiscore = pathsound + "breakhiscore.mp3";
                VoicePlay(pathbreakHiscore);
            }
            IsBreakHiScore = true;

            using var rewrite = new StreamWriter(pathsave, false);
            hiscore = score;
            HiScore.Text = "Highest Score: " + hiscore;

            rewrite.Write(hiscore);
            rewrite.Close();
        }

        public void ScoreUpType(Entity entity)
        {
            switch (entity)
            {
                case Demon:
                    ScoreUp(4);
                    break;
                case Bush:
                    if (!entity.IsHarmless) ScoreUp(3);
                    break;
                default:
                    ScoreUp(2); 
                    break;
            }

        }

        // SOUND AND MUSIC //

        private void PlayTheme(string path, double volume)
        {
            theme.Open(new (path));
            theme.Volume = volume;
            theme.Play();
        }

        private void VoiceStart()
        {
            Random voicestart = new Random();
            int voicestartindex = voicestart.Next(1, 10);
            string pathvoicestart = pathsound + "voicestart" + voicestartindex + ".mp3";

            VoicePlay(pathvoicestart);
        }

        private void VoicePlay(string path)
        {
            voice.Open(new(path));
            voice.Volume = 1;
            voice.Play();
        }

        private void Looptheme(object sender, EventArgs e)
        {
            if (!player.IsDead)
            {
                theme.Stop();
                theme.Play();
            }
        }

        // GAME START //
        
        public void FirstStartGame()
        {
            Main.KeyDown += KeyCommand;
            Main.KeyUp += ReleaseKey;


            themepath = pathsound + "Luminous Memory.mp3";

            PlayTheme(themepath, volumeadjust);
        }

        public async void GameStart()
        {
            if (IsReplay)
            {
                DeleteReplayElement();
                ChangeMapName("Galaxy", Brushes.Purple);
            }

            VoiceStart();

            await Task.Delay(2000);

            if (!IsChangeMap) FirstStartGame();
            else ChangeElementMap();

            await Task.Delay(500);

            getHiScore();
            ShowMoney();
            AmountBullet();

            player.IsDead = false;

            timetochangemov.Restart();
            timetochangemov.Stop();

            GC.Collect();

            while (!player.IsDead)
            {
                InShop = false;
                if (bulletAmount <= 0)
                {
                    await Reload();
                    AmountBullet();
                }

                setphase.phase = phase;
                timetochangemov.Start();

                //await Task.Delay(1);

                await setphase.ChangePhase();

                int elapsedtime = timetochangemov.Elapsed.Seconds;

                if (elapsedtime >= timechange)
                {
                    timetochangemov.Restart();
                    changetime++;
                }

                if (changetime > limitchangetime)
                {
                    changetime = limitchangetime;
                    continue;
                }

                if (changetime % 3 == 0 && changetime != 0)
                {
                    mapindex++;
                    changetime++;

                    ClearEntity();
                    
                    try
                    {
                        ToShop();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        this.Close();
                    }

                    return;
                }

            }

            GameOver();
        }

        public void ChangeElementMap()
        {
            if (NotChangeMap) return;

            timetochangemov.Stop();

            IsChangeMap = false;
            phase++;

            timetochangemov.Restart();

            PlayTheme(themepath!, volumeadjust);
        }

        // GAME OVER DISPLAY //

        private void CreateDeadTitle()
        {
            Rectangle DeadTitle = new()
            {
                Name = "DeadTitle",
                Width = 900,
                Height = 400,
                Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "die.png")),
                    Stretch = Stretch.Fill,
                },
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };

            RegisterName(DeadTitle.Name, DeadTitle);

            Playground.Children.Add(DeadTitle);
        }
        
        private void CreateButtonReplay()
        {
            Button replay = new()
            {
                Name = "Replay",
                Height = 50,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            Grid replaystack = new()
            {
                Height = 50,
                Width = 300,
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "start.jpg")),
                    Stretch = Stretch.Fill,
                }
            };

            TextBlock replaytxt = new()
            {
                FontSize = 25,
                Text = "REPLAY",
                Foreground = Brushes.LightSkyBlue,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            RegisterName(replay.Name, replay);

            replaystack.Children.Add(replaytxt);

            replay.Content = replaystack;
            replay.Click += HandleReplay;

            Playground.Children.Add(replay);
        }

        // REPLAY AND GAME OVER //

        public async void GameOver()
        {
            IsReplay = true;

            theme.Stop();

            Dead();

            string themedead = pathsound + "Bachram.mp3";
            PlayTheme(themedead, 1);

            await Task.Delay(10000);

            if (!IsReplay) return;

            string themereplay = pathsound + "Melody of Guidance.mp3";
            PlayTheme(themereplay, 1);
        }

        public void RestartEntitySpeed()
        {
            setphase.speedfish = 0;
            setphase.speeddemon = 0;
        }
        
        public void Dead()
        {
            player.Die(killer);

            Random voicedead = new Random();
            int voicedeadindex = voicedead.Next(1, 3);
            string deadvoice = pathsound + "voicedead" + voicedeadindex + ".mp3";

            VoicePlay(deadvoice);

            CreateDeadTitle();

            CreateButtonReplay();

            Main.KeyDown -= KeyCommand;
            Main.KeyUp -= ReleaseKey;

            Main.KeyDown += ReplayKey;

            player.playershape.Margin = new Thickness(0, 0, 700, 20);
        }

        public void DeleteReplayElement()
        {
            Button replay = (Button)Playground.FindName("Replay");
            Rectangle DeadTitle = (Rectangle)Playground.FindName("DeadTitle");

            Playground.Children.Remove(replay);
            Playground.Children.Remove(DeadTitle);

            UnregisterName("Replay");
            UnregisterName("DeadTitle");

            IsReplay = false;
        }

        public void ClearEntity()
        {
            if (entities.Count == 0) return;
            for (int entityindex = 0; entityindex < entities.Count; entityindex++)
            {
                entities[entityindex].soundplay.Stop();
                Playground.Children.Remove(entities[entityindex].entity);
            }
            entities.Clear();
        }

        public void RestartPlayer()
        {
            player.IsDead = false;
            player.IsVietCongKilled = false;
            player.indexgun = 0;
            player.playershape.Margin = new Thickness(0, 0, 700, 80);
            player.inventory.Clear();
            player.inventory.Add("de");
            gun.getPathGun(player.indexgun);
            player.Default();
        }

        public void RestartElement()
        {
            Main.KeyDown -= ReplayKey;

            timetochangemov.Restart();

            phase = 1;
            changetime = 0;
            score = 0;
            mapindex = 1;
            volumeadjust = 0.2;
            money = 0;

            IsJump = false;
            IsShoot = false;
            IsCrouch = false;
            IsReload = false;
            IsBreakHiScore = false;
            IsChangeMap = false;
            NotChangeMap = false;

            Score.Text = "Score: 0";

            ClearEntity();
            RestartPlayer();

            getAmountBullet();
            AmountBullet();
            ShowMoney();

            CreateGameDisplay();

            theme.Stop();
        }

        public void HandleReplay(object sender, RoutedEventArgs e)
        {
            Replay();
        }

        public void Replay()
        {
            ClearEntity();
            RestartElement();
            RestartEntitySpeed();
            GameStart();
        }

        // ENTITY //

        public async Task SpawnEntity(int phase, Entity newentity)
        {
            Entity entity;

            entity = newentity;

            entity.player = player;
            entity.playground = Playground;
            entity.main = (MainWindow)Main;

            Playground.Children.Add(entity.entity);
            entities.Add(entity);

            await entity.Move();

            CheckEntity(entity);
        }

        private void CheckEntity(Entity entity)
        {
            if (entity.getHit)
            {
                ScoreUpType(entity);
                GetMoneyType(entity);
                Playground.Children.Remove(entity.entity);
            }
            else
            {
                if (!player.IsDead && !InShop)
                {
                    Playground.Children.Remove(entity.entity);
                    if (!entity.IsHarmless) ScoreUp(1);
                }
                else
                {
                    killer = entity;
                    return;
                }
            }

            entities.Remove(entity);
        }

        // BULLET AND MAGAZINE //

        public void getAmountBullet()
        {
            gun.getBullet(player.indexgun, ref bulletlimit, ref magazinebulletlimit);
            bulletAmount = bulletlimit;
            magazinebullet = magazinebulletlimit;
        }

        public void AmountBullet()
        {
            BulletAmount.Text = "Bullet: " + bulletAmount + "/" + magazinebullet;
        }

        public async void SpawnMag()
        {
            Magazine mag = new Magazine();
            mag.player = player;

            Playground.Children.Add(mag.magazine);

            await mag.Move();

            if (mag.IsTaken)
            {
                magazinebullet += magazinebulletlimit;
                CheckMagazine();
                AmountBullet();
            }

            Playground.Children.Remove(mag.magazine);
        }

        public void CheckMagazine()
        {
            if (magazinebullet < magazinebulletlimit) return;

            magazinebullet = magazinebulletlimit;
        }

        public async Task Reload()
        {
            if (magazinebullet <= 0) return;

            player.PlayReloadSound();

            await Task.Delay(1600);

            if (magazinebullet < bulletlimit)
            {
                bulletAmount = magazinebullet;
                magazinebullet = 0;

                IsReload = false;

                return;
            }

            magazinebullet -= (bulletlimit - bulletAmount);
            bulletAmount = bulletlimit;

            IsReload = false;
        }

        // KEY COMMAND //

        private void ReplayKey(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if (key == Key.R)
            {
                Replay();
            }
        }

        private void KeyCommand(object sender, KeyEventArgs e)
        {
            if (player.IsDead) return;

            Key key = e.Key;

            switch (key)
            {
                case Key.Space:
                    HandleJump();
                    break;
                case Key.LeftCtrl:
                    HandleCrouch();
                    break;
                case Key.J:
                    HandleShoot();
                    break;
                case Key.R:
                    HandleReload();
                    break;
                default:
                    return;
            }
           
        }

        private void ReleaseKey(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            switch (key)
            {
                case Key.LeftCtrl:
                    ReleaseCtrlLeft();
                    break;

                case Key.J:
                    ReleaseJ();
                    break;

                default:
                    return;
            }
            
        }

        // HANDLE ACTION //

        private async void HandleReload()
        {
            if (bulletAmount == bulletlimit || IsReload) return;

            IsReload = true;

            await Reload();
            AmountBullet();
        }

        private void HandleJump()
        {
            if (IsJump) return;

            IsJump = true;

            player.playershape.Height = 137;
            player.playershape.Width = 86;
            player.jump = true;

            player.Jump(this);
        }

        private void HandleCrouch()
        {
            IsHoldCtrlLeft = true;

            if (IsJump) return;

            IsCrouch = true;

            if (player.playershape.Margin.Bottom <= 86)
            {
                player.crouch = true;
                player.Crouch();
            }
        }

        private void HandleShoot()
        {
            if (IsReload) return;

            if (bulletAmount <= 0) return;

            if (IsShoot) return;
            
            IsShoot = true;
            bulletAmount--;

            AmountBullet();

            Bullet bullet = new Bullet()
            {
                entities = this.entities,
                player = this.player,
                posout = player.playershape.Margin.Bottom,
            };

            Playground.Children.Add(bullet.bullet);
            player.Shoot(bullet, Playground);
        }

        private void ReleaseCtrlLeft()
        {
            player.crouch = false;
            IsHoldCtrlLeft = false;

            if (IsJump) return;

            player.Default();

            if (player.IsDead) return;

            player.playershape.Margin = new Thickness(0, 0, 700, 80);
        }

        private void ReleaseJ()
        {
            player.Default();

            if (player.IsDead) return;
        }

        // OUT SHOP //

        public void CreateButtonPlay()
        {
            Button play = new Button()
            {
                Height = 100,
                Width = 103,
                Margin = new Thickness(0, 530, 1000, 0),
                Content = new Image
                {
                    Source = new BitmapImage(new(pathpic + "go.png")),
                    VerticalAlignment = VerticalAlignment.Center,
                    Stretch = Stretch.Fill,
                },
                Name = "ContinuetoGame",
            };

            RegisterName(play.Name, play);

            play.Click += ContinueToGame;

            Playground.Children.Add(play);
        }

        public async void ContinueToGame(object sender, RoutedEventArgs e)
        {
            DeleteShopnInvenElement();

            await ChangeMap();
            IsChangeMap = true;

            Main.KeyDown += KeyCommand;
            Main.KeyUp += ReleaseKey;

            ChangeGameVisibility(Visibility.Visible);

            player.crouch = false;
            player.playershape.Margin = new Thickness(0, 0, 700, 80);
            player.Default();
            player.playershape.Visibility = Visibility.Visible;

            try
            {
                GameStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        public void DeleteShopnInvenElement()
        {
            TabControl shopninven = (TabControl)Playground.FindName("ShopandInventory");
            Button play = (Button)Playground.FindName("ContinuetoGame");
            TextBlock mymoney = (TextBlock)Playground.FindName("MyMoney");

            UnregisterName(shopninven.Name);
            UnregisterName(play.Name);
            UnregisterName(mymoney.Name);
            UnregisterName("Inventory");

            Playground.Children.Remove(shopninven);
            Playground.Children.Remove(play);
            Playground.Children.Remove(mymoney);
        }

        // SHOP AND INVENTORY //

        public void ShowMyMoney()
        {
            TextBlock mymoney = (TextBlock)Playground.FindName("MyMoney");

            mymoney.Text = Convert.ToString("$" + money);
        }

        public void MyMoney()
        {
            TextBlock mymoney = new TextBlock()
            {
                Height = 55,
                Width = 204,
                Foreground = Brushes.Green,
                FontWeight = FontWeights.Bold,
                FontSize = 30,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(788, 0, 0, 550),
                Name = "MyMoney"
            };

            RegisterName(mymoney.Name, mymoney);

            ShowMyMoney();
            
            Playground.Children.Add(mymoney);
        }

        public async void ToShop()
        {
            InShop = true;

            if (player.IsDead) player.IsDead = false;
            Main.KeyDown -= KeyCommand;
            Main.KeyUp -= ReleaseKey;

            player.playershape.Visibility = Visibility.Hidden;
            ChangeGameVisibility(Visibility.Hidden);

            backgroundpath = pathpic + "shopshiba.jpg";
            await BlackScreenChanging();

            CreateShopDisplay();
            CreateButtonPlay();
            MyMoney();

            themepath = pathsound + "vendetta.mp3";
            PlayTheme(themepath, 0.4);
        } 

        public void CreateShopDisplay()
        {
            TabControl shopninven = new TabControl()
            {
                TabStripPlacement = Dock.Left,
                Background = Brushes.Black,
                Name = "ShopandInventory",
            };

            RegisterName(shopninven.Name, shopninven);

            shopninven.SelectionChanged += UpdateInventory;

            CreateShopnInvenButton(shopninven);

            Playground.Children.Add(shopninven);
        }

        public void CreateShopnInvenButton(TabControl shopninven)
        {
            TabItem shop = new TabItem()
            {
                Height = 100,
                Width = 100,
                Background = Brushes.Black,
                Content = CreateWeaponStalls(),
                Header = CreateShopButton(),
            };


            TabItem inventory = new TabItem()
            {
                Height = 100,
                Width = 100,
                Background = Brushes.Black,
                Content = CreateInventory(),
                Header = CreateInventoryButton(),
            };

            shopninven.Items.Add(shop);
            shopninven.Items.Add(inventory);
        }

        public Image CreateShopButton()
        {      
            Image img = new Image()
            {
                Source = new BitmapImage(new(pathpic + "shop.jpg")),
                Stretch = Stretch.Fill,
            };

            return img;
        }

        public Image CreateInventoryButton()
        {
            Image img = new Image()
            {
                Source = new BitmapImage(new(pathpic + "inventory.jpg")),
                Stretch = Stretch.Fill,
            };

            return img;
        }

        // SHOP //
        public StackPanel CreateWeaponStalls()
        {
            StackPanel weaponstalls = new StackPanel()
            {
                Width = 900,
                Height = 650,
                Background = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new(pathpic + "shopbackground.png")),
                    Stretch = Stretch.Fill,
                },
            };

            var pistol = CreatePistolStalls();
            var rifle = CreateRifleStalls();

            weaponstalls.Children.Add(pistol);
            weaponstalls.Children.Add(rifle);

            ItemPistolStalls(pistol);
            ItemRifleStalls(rifle);

            return weaponstalls;
        }

        public StackPanel CreatePistolStalls()
        {
            StackPanel pistol = new StackPanel()
            {
                Width = 900,
                Height = 325,
                Orientation = Orientation.Horizontal,
                Background = Brushes.Transparent,
            };
            return pistol;
        }

        public StackPanel CreateRifleStalls()
        {
            StackPanel rifle = new StackPanel()
            {
                Width = 900,
                Height = 325,
                Orientation = Orientation.Horizontal,
                Background = Brushes.Transparent,
            };
            return rifle;
        }

        public void ItemPistolStalls(StackPanel pistolstalls)
        {
            

        }

        public void ItemRifleStalls(StackPanel riflestalls)
        {
            AddItemShop("m4a4", riflestalls);
        }

        public void AddItemShop(string gunname, StackPanel stalls)
        {
            Item newitem = new Item()
            {
                name = gunname,
                main = this,
                player = this.player,
            };

            stalls.Children.Add(newitem.Additem(gunname));
        }


            // INVENTORY //
        
        public StackPanel CreateInventory()
        {
            StackPanel inventory = new StackPanel()
            {
                Width = 900,
                Height = 650,
                Orientation = Orientation.Horizontal,
                Background = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new(pathpic + "inventorybackground.jpg")),
                    Stretch = Stretch.Fill,
                },
                Name = "Inventory"
            };

            RegisterName(inventory.Name, inventory);

            return inventory;
        }

        public void AddItemInventory()
        {
            StackPanel inventory = (StackPanel)Playground.FindName("Inventory");

            inventory.Children.Clear();

            foreach (var item in player.inventory)
            {
                Item newitem = new Item()
                {
                    IsBuy = true,
                    name = item,
                    player = this.player,
                    main = this
                };

                inventory.Children.Add(newitem.Additem(item));
            }
        }

        public void UpdateInventory(object sender, SelectionChangedEventArgs e)
        {
            TabControl shopninven = (TabControl)sender;

            if (shopninven.SelectedIndex == 1)
            {
                AddItemInventory();
            }
        }
    }
}