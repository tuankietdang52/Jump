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

namespace Jump
{
    public partial class MainWindow : Window
    {
        public int phase = 3;
        public int mapindex = 3;
        public int changetime = 9;

        public int timechange = 30;
        public int limitchangetime = 9;

        public int money = 20000;
        public int score = 0;
        public int hiscore;
        public int bulletAmount;
        public int bulletlimit;
        public int magazinebullet;
        public int magazinebulletlimit;
        public double volumeadjust = 0.2;

        public bool IsJump = false;
        public bool IsShoot = false;
        public bool IsCrouch = false;
        public bool IsReload = false;
        public bool IsBreakHiScore = false;
        public bool IsReplay = false;
        public bool IsHoldCtrlLeft = false;
        public bool IsChangeMap = false;
        public bool NotChangeMap = false;
        public bool InShopnInven = false;
        public bool InShop = false;
        public bool InInventory = false;
        public bool IsSpawnPirate = false;
        public bool IsHaveAspotate = false;
        public bool IsHaveBoss = false;
        public bool IsPause = false;
        public bool IsQuit = false;

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
        public Inventory item = new Inventory();
        public Rectangle? armoricon;
        public MainWindow()
        {
            InitializeComponent();

            ChangeGameVisibility(Visibility.Hidden);

            CreateGameDisplay();
            MainMenu();
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
            string pathmainmenu = pathsound + "Main Theme.mp3";

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
            };

            Canvas.SetLeft(Title, 150);

            RegisterName(Title.Name, Title);
            Playground.Children.Add(Title);

            CreateStartButton();
        }

        public Button CreateButton(string text, string name)
        {
            TextBlock txt = new()
            {
                FontSize = 25,
                Text = text,

                Foreground = Brushes.LightSkyBlue,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid stack = new()
            {
                Height = 50,
                Width = 300,
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "buttonimg.jpg")),
                    Stretch = Stretch.Fill,
                }
            };

            stack.Children.Add(txt);

            Button button = new()
            {
                Name = name,
                Height = 50,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = stack
            };

            return button;
        }

        private void CreateStartButton()
        {
            Button start = CreateButton("START", "Start");


            start.Click += PreStartGame;

            RegisterName(start.Name, start);

            Canvas.SetLeft(start, 345);
            Canvas.SetTop(start, 225);

            Playground.Children.Add(start);
        }

        private async void PreStartGame(object sender, RoutedEventArgs e)
        {
            Rectangle title = (Rectangle)Playground.FindName("Title");
            Button start = (Button)Playground.FindName("Start");

            Playground.Children.Remove(title);
            Playground.Children.Remove(start);

            UnregisterName("Title");
            UnregisterName("Start");

            theme.Stop();

            if (!IsQuit) SetPlayer();
            else ShowPlayer();

            GetPathMapandTheme();
            await BlackScreenChanging();

            ChangeGameVisibility(Visibility.Visible);

            setphase.main = (MainWindow)Main;

            RestartEntitySpeed();

            GameStart();
        }

        public void SetPlayer()
        {
            player.main = this;
            player.playershape.Name = "Player";
            gun.player = this.player;

            RegisterName(player.playershape.Name, player.playershape);

            Playground.Children.Add(player.playershape);

            getAmountBullet();
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

        public void getHiScore()
        {
            using var load = new StreamReader(pathsave);
            hiscore = int.Parse(load.ReadToEnd());
            HiScore.Text = "Highest Score: " + hiscore;
            load.Close();
        }

        public void ScoreUp(int amountscore)
        {
            if (IsQuit) return;
            GunScore(ref amountscore);

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

        public void GunScore(ref int amountscore)
        {
            switch (player.indexgun)
            {
                case 1:
                    amountscore = amountscore / 2;
                    break;
                default:
                    return;
            }
        }

        // SOUND AND MUSIC //

        public void PlayTheme(string path, double volume)
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

            PlayTheme(themepath!, volumeadjust);
        }

        public async void GameStart()
        {
            IsQuit = false;
            if (IsReplay)
            {
                GetPathMapandTheme();
                ChangeBackground();
                DeleteReplayElement();
                ChangeMapName("Map: Galaxy", Brushes.Purple);
            }

            VoiceStart();

            player.setDefault();
            player.Default();
            await Task.Delay(2000);

            PlayTheme(themepath!, volumeadjust);
            
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
                if (IsPause)
                {
                    timetochangemov.Stop();
                    await Task.Delay(1);
                    continue;
                }

                if (IsQuit) return;

                if (InShopnInven)
                {
                    ToShop();
                    return;
                }

                InShopnInven = false;

                if (bulletAmount <= 0)
                {
                    await Reload();
                    AmountBullet();
                }
            
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
                    await setphase.ChangePhase();
                }

                int elapsedtime = timetochangemov.Elapsed.Seconds;
            
                if (elapsedtime >= 1)
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

                    PlayTheme(themepath!, volumeadjust);

                    IsHaveBoss = true;

                    player.IsDead = false;
                    RestartPlayer();

                    await setphase.ChangePhase();
                }

            }

            if (!IsQuit) GameOver();
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

            Canvas.SetLeft(DeadTitle, 40);
            Canvas.SetTop(DeadTitle, -40);

            RegisterName(DeadTitle.Name, DeadTitle);

            Playground.Children.Add(DeadTitle);
        }
        
        private void CreateButtonReplay()
        {
            Button replay = CreateButton("REPLAY", "Replay");

            RegisterName(replay.Name, replay);

            replay.Click += HandleReplay;

            Canvas.SetLeft(replay, 345);
            Canvas.SetTop(replay, 225);

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

            player.Die(killer);
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

            Main.KeyDown -= KeyCommand;
            Main.KeyUp -= ReleaseKey;

            if (!player.IsDefaultDead) Thread.Sleep(1000);

            Random voicedead = new Random();
            int voicedeadindex = voicedead.Next(1, 3);
            string deadvoice = pathsound + "voicedead" + voicedeadindex + ".mp3";

            VoicePlay(deadvoice);

            CreateDeadTitle();

            CreateButtonReplay();

            Main.KeyDown += ReplayKey;
            player.setDefaultDead();
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

        public void RestartPlayer()
        {
            player.setDefault();

            if (player.IsDead)
            {
                player.IsHaveArmor = false;
                player.IsHaveAwp = false;
            }
            player.IsDead = false;
            player.IsVietCongKilled = false;

            player.Default();
        }

        public void RestartInventory()
        {
            player.indexgun = 0;

            player.inventory.Clear();
            player.inventory.Add("de");
            gun.getPathGun(player.indexgun);
        }

        public void RestartElement()
        {
            if (!IsQuit) Main.KeyDown -= ReplayKey;

            timetochangemov.Restart();

            RestartNumberElement();

            RestartBoolElement();

            Score.Text = "Score: 0";

            ClearEntity();
            RestartPlayer();
            RestartInventory();

            getAmountBullet();
            AmountBullet();
            ShowMoney();

            theme.Stop();
        }

        public void RestartBoolElement()
        {
            IsJump = false;
            IsShoot = false;
            IsCrouch = false;
            IsReload = false;
            IsBreakHiScore = false;
            IsChangeMap = false;
            NotChangeMap = false;
            IsHaveBoss = false;
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

        public async Task SpawnEntity(Entity newentity)
        {
            Entity entity;

            entity = newentity;

            entity.player = player;
            entity.playground = Playground;
            entity.main = (MainWindow)Main;

            Playground.Children.Add(entity.entity);
            entities.Add(entity);

            await entity.Action();

            CheckEntity(entity);
        }

        private void CheckEntity(Entity entity)
        {
            // entity get hit by bullet //
            if (entity.getHit)
            {
                if (!entity.CheckGetHit()) { }
                else
                {
                    if (IsHaveBoss) CheckBoss(entity);
                    ScoreUpType(entity);
                    GetMoneyType(entity);
                    Playground.Children.Remove(entity.entity);
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
                else
                {
                    killer = entity;
                    return;
                }
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

        public async Task SpawnMag()
        {
            Magazine mag = new Magazine();
            mag.player = this.player;
            mag.main = this;

            Playground.Children.Add(mag.item);

            await mag.Move();

            if (mag.IsTaken)
            {

                mag.MagIsTaken(ref magazinebullet, ref magazinebulletlimit, ref bulletAmount, ref bulletlimit);
                AmountBullet();
            }

            Playground.Children.Remove(mag.item);
        }

            // ARMOR //

        public async Task SpawnArmor()
        {
            Armor armor = new Armor();
            armor.player = this.player;
            armor.playground = Playground;
            armor.main = this;

            Playground.Children.Add(armor.item);

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
        }

        public void BreakArmor()
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

        private void ReplayKey(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if (key == Key.R)
            {
                Replay();
            }
        }

        public void ResumeKey(Key key)
        {
            if (key == Key.P)
            {
                IsPause = false;
                Resume();

                var playertop = Canvas.GetTop(player.playershape);
                if (playertop >= 250 && !IsCrouch)
                {
                    player.setDefault();
                    player.Default();
                }
            }
        }

        public void CheckCrouchPause(Key key)
        {
            if (key == Key.LeftCtrl)
            {
                player.crouch = true;
                IsCrouch = true;
            }
        }

        private void KeyCommand(object sender, KeyEventArgs e)
        {
            if (player.IsDead) return;

            Key key = e.Key;

            if (IsPause)
            {
                CheckCrouchPause(key);
                ResumeKey(key);
                return;
            }

            switch (key)
            {
                case Key.Space:
                    HandleJump();
                    break;

                case Key.LeftCtrl:
                    HandleCrouch();
                    break;

                case Key.J:
                    if (!player.IsHaveAwp) HandleShoot();
                    else HandleAwpShoot();
                    break;

                case Key.R:
                    HandleReload();
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
                IsCrouch = false;
                IsHoldCtrlLeft = false;
            }
        }

        private void ReleaseKey(object sender, KeyEventArgs e)
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

        // HANDLE ACTION //

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

            player.setDefault();
            player.jump = true;

            player.Jump();
        }

        private void HandleCrouch()
        {
            IsHoldCtrlLeft = true;

            if (IsJump) return;

            player.setDefault();

            IsCrouch = true;
            player.crouch = true;
            player.Crouch();
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
                main = this,
                posout = Canvas.GetTop(player.playershape),
            };

            bullet.setPosition();

            Playground.Children.Add(bullet.bullet);
            player.Shoot(bullet, Playground);
        }

        private async void HandleAwpShoot()
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
                main = this,
                posout = Canvas.GetTop(player.playershape),
            };

            bullet.setPosition();

            Playground.Children.Add(bullet.bullet);
            player.Shoot(bullet, Playground);

            await Task.Delay(500);
            item.PlaySound("awp");
            await Task.Delay(800);
            IsShoot = false;
        }

        private void ReleaseCtrlLeft()
        {
            player.crouch = false;
            IsHoldCtrlLeft = false;

            if (IsJump) return;

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
            ArmorVisibility(Visibility.Visible);

            player.crouch = false;
            player.setDefault();
            player.Default();
            player.playershape.Visibility = Visibility.Visible;

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
            InShopnInven = true;
            InShop = true;
            InInventory = false;
            ClearEntity();

            if (player.IsDead) player.IsDead = false;
            Main.KeyDown -= KeyCommand;
            Main.KeyUp -= ReleaseKey;

            player.playershape.Visibility = Visibility.Hidden;
            ChangeGameVisibility(Visibility.Hidden);
            ArmorVisibility(Visibility.Hidden);

            backgroundpath = pathpic + "shopshiba.jpg";
            await BlackScreenChanging();

            CreateShopDisplay();
            CreateButtonPlay();
            MyMoney();

            themepath = pathsound + "ireallywannastayatyourhouse.mp3";
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

            AddStalls(weaponstalls);

            return weaponstalls;
        }

        public void AddStalls(StackPanel weaponstalls)
        {
            var equipment = CreateEquipmentStalls();
            var rifle = CreateRifleStalls();

            weaponstalls.Children.Add(equipment);
            weaponstalls.Children.Add(rifle);

            ItemEquipmentStalls(equipment);
            ItemRifleStalls(rifle);
        }

        public StackPanel CreateEquipmentStalls()
        {
            StackPanel equipment = new StackPanel()
            {
                Width = 900,
                Height = 325,
                Orientation = Orientation.Horizontal,
                Background = Brushes.Transparent,
            };
            return equipment;
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

        public void ItemEquipmentStalls(StackPanel equipmentstalls)
        {
            

        }

        public void ItemRifleStalls(StackPanel riflestalls)
        {
            AddItemShop("m4a4", riflestalls);
            AddItemShop("awp", riflestalls);
        }

        public void AddItemShop(string gunname, StackPanel stalls)
        {
            Inventory newitem = new Inventory()
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
                Inventory newitem = new Inventory()
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
                InInventory = true;
                InShop = false;
                AddItemInventory();
            }
            else
            {
                InShop = true;
                InInventory = false;
            }
        }

        // PAUSE GAME //

        public void HandlePause()
        {
            if (!InShopnInven && !player.IsDead)
            {
                IsPause = true;
                CreateBlackScreen();
                ButtonInPause();
            }
        }
        
        public void CreateBlackScreen()
        {
            Rectangle blackscreen = new Rectangle()
            {
                Height = 664,
                Width = 1002,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Focusable = false,
                Name = "BlackScreen",
            };

            RegisterName(blackscreen.Name, blackscreen);

            SolidColorBrush black = new SolidColorBrush();
            black.Color = Colors.Black;

            blackscreen.Fill = black;
            blackscreen.Fill.Opacity = 0.4;

            Playground.Children.Add(blackscreen);
        }

        public void ButtonInPause()
        {
            StackPanel buttoninpause = new StackPanel()
            {
                Height = 650,
                Width = 350,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Name = "ButtonsInPause",
            };

            Canvas.SetLeft(buttoninpause, 345);

            RegisterName(buttoninpause.Name, buttoninpause);

            AddButtonToPause(buttoninpause);

            Playground.Children.Add(buttoninpause);
        }

        public void AddButtonToPause(StackPanel buttoninpause)
        {
            var resume = CreateButtonResume();
            var quit = CreateButtonQuit();

            buttoninpause.Children.Add(resume);
            buttoninpause.Children.Add(quit);
        }

        public Button CreateButtonResume()
        {
            Button resume = CreateButton("RESUME", "Resume");

            resume.Margin = new Thickness(0, 60, 0, 0);
            resume.Click += HandleResume;

            return resume;
        }

        public Button CreateButtonQuit()
        {
            Button quit = CreateButton("QUIT", "Quit");

            quit.Margin = new Thickness(0, 80, 0, 0);
            quit.Click += Quit;

            return quit;
        }

            // QUIT //

        public void Quit(object sender, RoutedEventArgs e)
        {
            IsQuit = true;
            IsPause = false;

            RestartElement();
            DeletePauseElement();

            Main.KeyDown -= KeyCommand;
            Main.KeyUp -= ReleaseKey;
            player.playershape.Visibility = Visibility.Hidden;

            ChangeGameVisibility(Visibility.Hidden);

            CreateGameDisplay();
            MainMenu();
        }

            // RESUME //

        public void HandleResume(object sender, RoutedEventArgs e)
        {
            ResumeKey(Key.P);
        }

        public void Resume()
        {
            DeletePauseElement();
        }

        public void DeletePauseElement()
        {
            Rectangle blackscreen = (Rectangle)Playground.FindName("BlackScreen");
            StackPanel buttoninpause = (StackPanel)Playground.FindName("ButtonsInPause");

            Playground.Children.Remove(blackscreen);
            Playground.Children.Remove(buttoninpause);

            UnregisterName(blackscreen.Name);
            UnregisterName(buttoninpause.Name);
        }
    }
}