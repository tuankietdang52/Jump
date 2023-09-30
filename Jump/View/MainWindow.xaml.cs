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
using System.Windows.Media.Converters;
using Jump.Sql;
using Jump.View;
using Jump.EnemyEntity;

namespace Jump
{
    public partial class MainWindow : Window
    {
        public int phase = 1;
        public int mapindex = 1;
        public int changetime = 3;

        public int timechange = 30;
        public int limitchangetime = 9;

        public int money = 20000;
        public int score = 200;
        public double volumeadjust = 0.2;

        public bool IsBreakHiScore = false;
        public bool IsReplay = false;
        public bool IsHoldCtrlLeft = false;
        public bool IsChangeMap = false;
        public bool NotChangeMap = false;
        public bool IsSpawnPirate = false;
        public bool IsHaveAspotate = false;
        public bool IsHaveBoss = false;
        public bool IsPause = false;
        public bool IsQuit = false;
        public bool InShopnInven = false;
        public bool InShop = false;
        public bool InInventory = false;
        public bool IsClickReplay = false;
        public bool IsWin = false;

        private readonly string pathsave = $"{Directory.GetCurrentDirectory()}\\HiScore.txt";
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public string? backgroundpath;
        public string? undergroundpath;
        public string? themepath;

        public PlayerCharacter player = new PlayerCharacter();
        public Gun gun = new Gun();

        public Entity killer = new Entity();
        public Inventory item = new Inventory();

        public Stopwatch timetochangemov = new Stopwatch();
        public Phase setphase = new Phase();

        public MediaPlayer theme = new MediaPlayer();
        public MediaPlayer voice = new MediaPlayer();

        public ListEntity changeentity = new ListEntity();
        public List<Entity> entities = new List<Entity>();
        public List<Item> items = new List<Item>();
        public List<HighScore.HighScoreOwner> listhighscore = new List<HighScore.HighScoreOwner>();
        
        public Rectangle? armoricon;

        public HighScore highscore = new HighScore();
        public CustomButton custom = new CustomButton();

        public MainWindow()
        {
            InitializeComponent();

            ChangeGameVisibility(Visibility.Hidden);

            CreateGameDisplay();
            MainMenu();
        }

        public void ChangeGameVisibility(Visibility visibility)
        {
            Score.Visibility = visibility;
            BulletAmount.Visibility = visibility;
            Map.Visibility = visibility;
            Ground.Visibility = visibility;
            Money.Visibility = visibility;
        }

        // PRE START GAME //

        public void MainMenu()
        {
            string pathmainmenu = pathsound + "Main Theme.mp3";

            PlayTheme(pathmainmenu, 1);

            MainMenu mainmenu = new MainMenu(this);
            Playground.Children.Add(mainmenu);
        }

        public async void PreStartGame()
        {
            if (!IsQuit) SetPlayer();
            else ShowPlayer();

            GetPathMapandTheme();
            await BlackScreenChanging();

            ChangeGameVisibility(Visibility.Visible);

            setphase.main = this;

            RestartEntitySpeed();

            GameStart();
        }

        public void SetPlayer()
        {
            player.main = this;
            player.playershape.Name = "Player";
            player.gun.setOwnerGun(player, this);
            gun.player = this.player;

            RegisterName(player.playershape.Name, player.playershape);

            Playground.Children.Add(player.playershape);
            AmountBullet();
        }

        public void ShowPlayer()
        {
            player.playershape.Visibility = Visibility.Visible;
        }

        private void CreateGameDisplay()
        {
            GetPathMapandTheme();
            backgroundpath = pathpic + "mainbackground.png";
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
            Map.Text = mapname;
            Map.Foreground = color;
        }

        public bool GetBossMapandTheme()
        {
            switch (mapindex)
            {
                case 1:
                    backgroundpath = pathpic + "bossmap1.jpg";
                    undergroundpath = pathpic + "luxury.png";
                    themepath = pathsound + "Abstruse Dilemma.mp3";
                    volumeadjust = 0.1;
                    ChangeMapName("BOSS: GoldFish King", Brushes.Yellow);
                    return true;

                case 2:
                    backgroundpath = pathpic + "bossmap2.jpg";
                    undergroundpath = pathpic + "shrine.png";
                    themepath = pathsound + "Cloud Heaven.mp3";
                    volumeadjust = 0.2;
                    ChangeMapName("BOSS: Shogun", Brushes.Red);
                    return true;

                case 3:
                    backgroundpath = pathpic + "Astrolab.jpg";
                    undergroundpath = pathpic + "astrolabground.png";
                    themepath = pathsound + "Astrolab.mp3";
                    volumeadjust = 0.5;
                    ChangeMapName("BOSS: Dark Mage", Brushes.Violet);
                    return true;

                default:
                    return false;
            }
        }

        public bool GetPathMapandTheme()
        {
            if (changetime % 3 == 0 && changetime != 0)
            {
                if (GetBossMapandTheme()) return true;

                phase++;

                return false;
            }

            switch (mapindex)
            {
                case 1:
                    backgroundpath = pathpic + "galaxy.jpg";
                    undergroundpath = pathpic + "moon.png";
                    themepath = pathsound + "Luminous Memory.mp3";
                    volumeadjust = 0.1;
                    ChangeMapName("Map: Galaxy", Brushes.Purple);
                    return true;

                case 2:
                    backgroundpath = pathpic + "jungle.jpg";
                    undergroundpath = pathpic + "dirt.jpg";
                    themepath = pathsound + "Touhou 7.2.mp3";
                    volumeadjust = 0.3;
                    ChangeMapName("Map: Jungle", Brushes.LightGreen);
                    return true;

                case 3:
                    backgroundpath = pathpic + "village.png";
                    undergroundpath = pathpic + "villageground.png";
                    themepath = pathsound + "The Village.mp3";
                    volumeadjust = 0.4;
                    ChangeMapName("Map: Village", Brushes.Green);
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
                case BigGoldFish:
                    GetMoney(1000); 
                    break;
                case Bush:
                    if (!entity.IsHarmless) GetMoney(150);
                    break;
                case Samurai:
                    GetMoney(2000);
                    break;
                case PirateCaptain:
                    GetMoney(200);
                    break;
                case Apostate:
                    GetMoney(100);
                    break;
                case Kamikaze:
                    GetMoney(150);
                    break;
                default:
                    GetMoney(50);
                    break;
            }
        }

        // SCORE //

        public void ScoreUp(int amountscore)
        {
            if (IsQuit || IsReplay) return;

            score += amountscore;
            Score.Text = "Score: " + score;
        }

        public void ScoreUpType(Entity entity)
        {
            switch (entity)
            {
                case Demon:
                    ScoreUp(4);
                    break;
                case BigGoldFish:
                    ScoreUp(10); 
                    break;
                case Bush:
                    if (!entity.IsHarmless) ScoreUp(3);
                    break;
                case Samurai:
                    ScoreUp(20);
                    break;
                case PirateCaptain:
                    ScoreUp(5);
                    break;
                case Apostate:
                    ScoreUp(4);
                    break;
                case Kamikaze:
                    ScoreUp(3);
                    break;
                default:
                    ScoreUp(2); 
                    break;
            }
        }

        // THEME //

        public void PlayTheme(string path, double volume)
        {
            theme.Open(new (path));
            theme.Volume = volume;
            theme.Play();
        }

        private void Looptheme(object sender, EventArgs e)
        {
            if (!player.IsDead)
            {
                theme.Stop();
                theme.Play();
            }
        }

        // GAME SETTING //

        public void KeyCommandManage(string status)
        {
            switch (status)
            {
                case "on":
                    Main.KeyDown += KeyCommand;
                    Main.KeyUp += ReleaseKey;
                    break;

                case "off":
                    Main.KeyDown -= KeyCommand;
                    Main.KeyUp -= ReleaseKey;
                    break;
            }
        }

        public void KeySettingOff()
        {
            Main.KeyDown -= PauseKey;
            Main.KeyDown -= ReplayKey;
        }

        // GAME START //
        
        public void FirstStartGame()
        {
            KeySettingOff();
            KeyCommandManage("on");

            PlayTheme(themepath!, volumeadjust);
        }

        public async void GameStart()
        {
            IsQuit = false;
            IsReplay = false;

            await Task.Delay(1000);

            player.VoiceStart();
            player.setDefault();
            player.Default();

            await Task.Delay(2000);

            PlayTheme(themepath!, volumeadjust);
            
            if (!IsChangeMap) FirstStartGame();
            else ChangeElementMap();
            
            await Task.Delay(500);
            
            ShowMoney();
            AmountBullet();

            player.IsDead = false;

            timetochangemov.Restart();
            timetochangemov.Stop();

            GC.Collect();

            while (!player.IsDead)
            {
                if (IsPause)
                {
                    timetochangemov.Stop();
                    await Task.Delay(1);
                    continue;
                }

                if (IsQuit || IsReplay) return;

                if (InShopnInven)
                {
                    if (changetime == 9) return;
                    ReadyToShop();
                    return;
                }

                InShopnInven = false;

                if (player.gun.bulletAmount <= 0) player.gun.HandleReload();
            
                setphase.phase = phase;
                timetochangemov.Start();
            
                await Task.Delay(1);

                if (timetochangemov.Elapsed.Seconds % 5 == 0)
                {
                    await SpawnItem();
                }

                if (IsHaveBoss) continue;

                if (changetime % 3 != 0 || changetime == 0)
                {
                    if (!InShopnInven) await setphase.ChangePhase();
                }

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
                    if (IsHaveBoss) continue;
                    
                    ClearEntity();
                    theme.Stop();

                    await ChangeMap();

                    await Task.Delay(2000);

                    ToBoss();

                    await setphase.ChangePhase();
                }

            }


            if (IsWin) return;
            if (!IsQuit && !IsReplay) GameOver();
        }

        public void ToBoss()
        {
            PlayTheme(themepath!, volumeadjust);

            IsHaveBoss = true;

            player.IsDead = false;
            player.Restart();
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

        // WIN DISPLAY //

        public void WinScene()
        {
            KeyCommandManage("off");
            Main.KeyDown -= ReplayKey;
            Main.KeyDown -= PauseKey;

            
        }


        // GAME OVER DISPLAY //

        public void PlayerDeadWindow()
        {
            DeadWindow deadwindow = new DeadWindow(this, changetime, score);
            
            Playground.Children.Add(deadwindow);
        }

        // REPLAY AND GAME OVER //

        public void GameOver()
        {
            IsClickReplay = false;
            theme.Stop();

            string deadtheme = pathsound + "Melody of Guidance.mp3";
            PlayTheme(deadtheme, 1);

            Dead();

            player.Die(killer);
        }

        public void RestartEntitySpeed()
        {
            setphase.speedfish = 0;
            setphase.speeddemon = 0;
        }
        
        public void Dead()
        {
            player.Die(killer);

            KeyCommandManage("off");

            if (!player.IsDefaultDead) Thread.Sleep(1000);

            player.GetVoiceDead();

            PlayerDeadWindow();

            Main.KeyDown += ReplayKey;
            player.setDefaultDead();
        }

        public void ClearEntity()
        {
            if (entities.Count == 0) return;

            foreach (var entity in entities)
            {
                entity.soundplay.Stop();
                entity.IsDead = true;
                Playground.Children.Remove(entity.entity);
                if (entity.healthbar != null) Playground.Children.Remove(entity.healthbar);
                if (entity.secondhealthbar != null) Playground.Children.Remove(entity.secondhealthbar);
            }

            entities.Clear();
        }

        public void ClearItem()
        {
            foreach (var item in items)
            {
                Playground.Children.Remove(item.item);
            }

            items.Clear();
        }

        public void RestartAllEntity()
        {
            ClearEntity();
            ClearItem();

            player.Restart();
            player.RestartInventory();
        }

        public void RestartEquipmentIndex()
        {
            Score.Text = "Score: 0";
            AmountBullet();
            ShowMoney();
        }

        public void Restart()
        {
            KeySettingOff();
            KeyCommandManage("off");

            timetochangemov.Restart();

            RestartNumberElement();

            RestartBoolElement();

            RestartAllEntity();

            RestartEquipmentIndex();
        }

        public void UnregisterNameWindow(UserControl window)
        {
            switch (window)
            {
                case DeadWindow:
                    UnregisterName("DeadWindow");
                    break;

                case Pause:
                    UnregisterName("Pause");
                    break;

                default:
                    return;
            }
        }

        public async void RestartMap(UserControl window)
        {
            Playground.Children.Remove(window);

            GetPathMapandTheme();
            await BlackScreenChanging();
            ChangeBackground();
            ChangeMapName("Map: Galaxy", Brushes.Purple);
        }

        public void RestartBoolElement()
        {
            IsBreakHiScore = false;
            IsChangeMap = false;
            NotChangeMap = false;
            IsHaveBoss = false;
            IsWin = false;
            setphase.AlreadyHaveBoss = false;
        }

        public void RestartNumberElement()
        {
            phase = 1;
            changetime = 0;
            score = 0;
            mapindex = 1;
            volumeadjust = 0.2;
            money = 0;
        }

        public void Replay(UserControl window)
        {
            IsReplay = true;
            IsPause = false;

            UnregisterNameWindow(window);
            theme.Stop();

            Restart();
            RestartEntitySpeed();
            RestartMap(window);

            GameStart();
        }

        // BOSS //

        public void CheckBoss(Entity entity)
        {
            if (!entity.IsDead) return;

            IsHaveBoss = false;
            setphase.AlreadyHaveBoss = false;
            BossIsDefeated();
        }

        public void BossIsDefeated()
        {
            mapindex++;
            changetime++;

            InShopnInven = true;

            ClearEntity();
        }

        // ENTITY //

        public void SetEntity(ref Entity entity)
        {
            entity.player = player;
            entity.playground = Playground;
            entity.main = (MainWindow)Main;
        }

        public async Task SpawnEntity(Entity newentity)
        {
            Entity entity;

            entity = newentity;

            SetEntity(ref entity);

            Playground.Children.Add(entity.entity);
            entities.Add(entity);

            await entity.Action();

            CheckEntity(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            if (IsHaveBoss) CheckBoss(entity);
            ScoreUpType(entity);
            GetMoneyType(entity);
            Playground.Children.Remove(entity.entity);
        }

        private void CheckEntity(Entity entity)
        {
            // entity get hit by bullet //
            if (entity.getHit)
            {
                if (!entity.CheckGetHit()) { }
                else
                {
                    RemoveEntity(entity);
                }
            }
            else
            {
                // player avoid //
                if (!player.IsDead && !InShopnInven)
                {
                    Playground.Children.Remove(entity.entity);
                    if (!entity.IsHarmless) ScoreUp(1);
                }
                // player get hit //
                else return;
            }

            if (!player.IsDead) entities.Remove(entity);
        }

        // ITEM //

            // BULLET AND MAGAZINE //

        public async Task SpawnItem()
        {
            Random spawn = new Random();
            int spawnindex = spawn.Next(0, 201);

            if (spawnindex % setphase.itemchance != 0) return;
            
            Random itemrand = new Random();
            int itemindex = itemrand.Next(0, 10);

            if (itemindex <= 1 || phase != 3) await SpawnMag();
            else if (itemindex <= 4 || phase != 3) await SpawnArmor();
        }

        public void AmountBullet()
        {
            BulletAmount.Text = "Bullet: " + player.gun.bulletAmount + "/" + player.gun.magazinebullet;
        }

        public async Task SpawnMag()
        {
            Magazine mag = new Magazine()
            {
                player = this.player,
                main = this,
            };

            Playground.Children.Add(mag.item);
            items.Add(mag);

            await mag.Move();

            if (mag.IsTaken)
            {

                mag.MagIsTaken(ref player.gun.magazinebullet, ref player.gun.magazinebulletlimit, ref player.gun.bulletAmount, ref player.gun.bulletlimit);
                AmountBullet();
            }

            Playground.Children.Remove(mag.item);
            items.Remove(mag);
        }

            // ARMOR //

        public async Task SpawnArmor()
        {
            Armor armor = new Armor()
            {
                player = this.player,
                playground = Playground,
                main = this
            };

            Playground.Children.Add(armor.item);
            items.Add(armor);

            await armor.Move();

            if (armor.IsTaken)
            {
                if (!player.IsHaveArmor)
                {
                    armor.ShowArmor();
                }
                player.IsHaveArmor = true;
            }

            Playground.Children.Remove(armor.item);
            items.Add(armor);
        }

        public void BreakArmorUI()
        {
            player.IsDead = false;
            player.IsHaveArmor = false;

            Rectangle armoricon = (Rectangle)Playground.FindName("ArmorIcon");

            UnregisterName(armoricon.Name);

            Playground.Children.Remove(armoricon);
        }

        public void ArmorVisibility(Visibility visibility)
        {
            if (!player.IsHaveArmor) return;

            Rectangle armoricon = (Rectangle)Playground.FindName("ArmorIcon");

            armoricon.Visibility = visibility;
        }

        // KEY COMMAND //

        public void ReplayKey(object sender, KeyEventArgs e)
        {
            if (IsQuit || IsReplay) return;
            Key key = e.Key;
            if (key == Key.R)
            {
                GetSideWindow();
                IsClickReplay = true;
            }
        }

        public void GetSideWindow()
        {
            if (IsClickReplay) return;
            UserControl window;
            if (player.IsDead) window = (DeadWindow)Playground.FindName("DeadWindow");
            else window = (Pause)Playground.FindName("Pause");

            Replay(window);
        }

        public void CheckCrouchPause(Key key)
        {
            if (key == Key.LeftCtrl)
            {
                player.crouch = true;
                player.IsCrouch = true;
            }
        }

        public void PauseKey(object sender, KeyEventArgs e)
        {
            Main.KeyDown -= ReplayKey;
            if (IsQuit) return;
            Key key = e.Key;

            CheckCrouchPause(key);

            switch (key)
            {
                case Key.P:
                    var window = (Pause)Playground.FindName("Pause");
                    Resume(window);
                    break;
            }
        }

        public void KeyCommand(object sender, KeyEventArgs e)
        {
            if (player.IsDead) return;

            e.Handled = true;
            Key key = e.Key;

            switch (key)
            {
                case Key.Space:
                    player.HandleJump();
                    break;

                case Key.LeftCtrl:
                    player.HandleCrouch();
                    break;

                case Key.J:
                    if (!player.IsHaveAwp) player.HandleShoot();
                    else player.HandleAwpShoot();
                    break;

                case Key.R:
                    player.gun.HandleReload();
                    break;

                case Key.P:
                    HandlePause();
                    break;

                default:
                    return;
            }
        }

        public void ReleaseCrouchPause(Key key)
        {
            if (key == Key.LeftCtrl)
            {
                player.crouch = false;
                player.IsCrouch = false;
                player.IsHoldCtrlLeft = false;
            }
        }

        public void ReleaseKey(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if (IsPause)
            {
                ReleaseCrouchPause(key);
                return;
            }

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

        // Release Key //   

        private void ReleaseCtrlLeft()
        {
            player.crouch = false;
            player.IsHoldCtrlLeft = false;

            if (player.IsJump) return;

            player.setDefault();
            player.Default();

            if (player.IsDead) return;

        }

        private void ReleaseJ()
        {
            var top = Canvas.GetTop(player.playershape);
            if (top >= 250) Canvas.SetTop(player.playershape, 260);

            player.Default();

            if (player.IsDead) return;
        }

        // OUT SHOP //

        public void ResetVisibility()
        {
            ChangeGameVisibility(Visibility.Visible);
            ArmorVisibility(Visibility.Visible);

            player.Restart();
            player.playershape.Visibility = Visibility.Visible;
        }

        public async void ContinueToGame(ShopnInven shopnninven)
        {
            Playground.Children.Remove(shopnninven);

            await ChangeMap();
            IsChangeMap = true;

            KeyCommandManage("on");

            ResetVisibility();

            try
            {
                InShopnInven = false;
                GameStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        // SHOP AND INVENTORY //

        public void InShopVisibility()
        {
            player.playershape.Visibility = Visibility.Hidden;
            ChangeGameVisibility(Visibility.Hidden);
            ArmorVisibility(Visibility.Hidden);
        }

        public void InShopElement()
        {
            InShopnInven = true;
            InShop = true;
            InInventory = false;
        }

        public async void ReadyToShop()
        {
            InShopElement();
            ClearEntity();

            if (player.IsDead) player.IsDead = false;
            KeySettingOff();
            KeyCommandManage("off");

            InShopVisibility();

            backgroundpath = pathpic + "shopshiba.jpg";
            await BlackScreenChanging();

            ToShop();
        }

        public void ToShop()
        {
            themepath = pathsound + "ireallywannastayatyourhouse.mp3";
            PlayTheme(themepath, 0.4);

            ShopnInven shopninven = new ShopnInven(this);

            Playground.Children.Add(shopninven);
        }

        // PAUSE GAME //

        public void HandlePause()
        {
            if (InShopnInven && player.IsDead) return;

            IsPause = true;
            Pause pausewindow = new Pause(this);
            Playground.Children.Add(pausewindow);

            KeyCommandManage("off");
            Main.KeyUp += ReleaseKey;
            Main.KeyDown += PauseKey;
        }

        // QUIT //

        public void Quit(UserControl window)
        {
            KeySettingOff();
            KeyCommandManage("off");

            IsQuit = true;
            IsPause = false;

            Playground.Children.Remove(window);
            if (player.IsHaveArmor) BreakArmorUI();

            player.playershape.Visibility = Visibility.Hidden;

            ChangeGameVisibility(Visibility.Hidden);

            UnregisterNameWindow(window);
            Restart();
            CreateGameDisplay();
            MainMenu();
        }

            // RESUME //

        public void Resume(UserControl window)
        {
            Playground.Children.Remove(window);
            UnregisterNameWindow(window);
            IsPause = false;

            PlayerAfterResume();

            KeySettingOff();
            KeyCommandManage("on");
        }

        public void PlayerAfterResume()
        {
            var playertop = Canvas.GetTop(player.playershape);
            if (playertop >= 250 && !player.IsCrouch)
            {
                player.setDefault();
                player.Default();
            }
        }
    }
}