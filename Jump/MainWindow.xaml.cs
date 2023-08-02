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

namespace Jump
{
    public partial class MainWindow : Window
    {
        public int phase = 1;
        public int score = 0;
        public int timechange = 40;
        public int changetime = 0;
        public int hiscore;
        public int bulletAmount = 7;
        public int bulletlimit = 7;
        public int magazinebullet = 35;
        public int magazinebulletlimit = 35;
        public int mapindex = 1;
        public int limitchangetime = 5;
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

        private readonly string pathsave = $"{Directory.GetCurrentDirectory()}\\HiScore.txt";
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public string? backgroundpath;
        public string? undergroundpath;
        public string? themepath;

        public Stopwatch timetochangemov = new Stopwatch();
        public PlayerCharacter player = new PlayerCharacter();
        public MediaPlayer soundreload = new MediaPlayer();
        public MediaPlayer theme = new MediaPlayer();
        public MediaPlayer voice = new MediaPlayer();
        public ListEntity changeentity = new ListEntity();
        public List<Entity> entities = new List<Entity>();
        public Phase setphase = new Phase();
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
        }

        // PRE START GAME //

        public Visibility GameVisibility { get; set; } = Visibility.Collapsed;

        public void MainMenu()
        {
            string pathmainmenu = pathsound + "Melody of Guidance.mp3";

            PlayTheme(pathmainmenu, 1);

            Ground.Visibility = Visibility.Hidden;

            Rectangle Title = new Rectangle();
            Title.Name = "Title";
            RegisterName(Title.Name, Title);

            Title.Width = 700;
            Title.Height = 200;
            Title.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "Title.png")),
                Stretch = Stretch.Fill,
            };
            Title.HorizontalAlignment = HorizontalAlignment.Center;
            Title.VerticalAlignment = VerticalAlignment.Top;

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

            VoiceStart();

            ChangeGameVisibility(Visibility.Visible);


            player.playershape.Name = "Player";
            player.main = (MainWindow?)Main;
            RegisterName(player.playershape.Name, player.playershape);
            Playground.Children.Add(player.playershape);

            setphase.main = (MainWindow)Main;

            RestartEntitySpeed();
            ChangeMapName("Galaxy", Brushes.Purple);
            GameStart();
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

            ClearEntity();
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

        // SCORE //

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

        public async void GameStart()
        {
            if (IsReplay)
            {
                DeleteReplayElement();
                ChangeMapName("Galaxy", Brushes.Purple);
                VoiceStart();
            }

            await Task.Delay(2000);

            Main.KeyDown += KeyCommand;
            Main.KeyUp += ReleaseKey;

            themepath = pathsound + "Luminous Memory.mp3";

            PlayTheme(themepath, volumeadjust);

            await Task.Delay(500);

            using var load = new StreamReader(pathsave);
            hiscore = int.Parse(load.ReadToEnd());
            HiScore.Text = "Highest Score: " + hiscore;
            load.Close();

            AmountBullet();

            timetochangemov.Restart();
            timetochangemov.Stop();

            while (!player.IsDead)
            {
                if (bulletAmount <= 0)
                {
                    await Reload();
                    AmountBullet();
                }

                if (IsChangeMap) ChangeElementMap();

                setphase.phase = phase;
                timetochangemov.Start();

                //await Task.Delay(1);

                await setphase.ChangePhase();

                int elapsedtime = timetochangemov.Elapsed.Seconds;
                if (elapsedtime == timechange)
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

                    await ChangeMap();

                    IsChangeMap = true;
                }

            }

            GameOver();
        }

        // GAME OVER DISPLAY //

        private void CreateDeadTitle()
        {
            Rectangle DeadTitle = new Rectangle();
            DeadTitle.Name = "DeadTitle";
            RegisterName(DeadTitle.Name, DeadTitle);

            DeadTitle.Width = 900;
            DeadTitle.Height = 400;
            DeadTitle.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "die.png")),
                Stretch = Stretch.Fill,
            };
            DeadTitle.HorizontalAlignment = HorizontalAlignment.Center;
            DeadTitle.VerticalAlignment = VerticalAlignment.Top;

            Playground.Children.Add(DeadTitle);
        }
        
        private void CreateButtonReplay()
        {
            Button replay = new Button();
            replay.Name = "Replay";
            RegisterName(replay.Name, replay);

            replay.Height = 50;
            replay.Width = 300;
            replay.HorizontalAlignment = HorizontalAlignment.Center;

            Grid replaystack = new Grid();
            replaystack.Height = 50;
            replaystack.Width = 300;
            replaystack.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new(pathpic + "start.jpg")),
                Stretch = Stretch.Fill,
            };

            TextBlock replaytxt = new TextBlock();
            replaytxt.FontSize = 25;
            replaytxt.Text = "REPLAY";

            replaytxt.Foreground = Brushes.LightSkyBlue;
            replaytxt.FontWeight = FontWeights.Bold;
            replaytxt.HorizontalAlignment = HorizontalAlignment.Center;
            replaytxt.VerticalAlignment = VerticalAlignment.Center;
            replaystack.Children.Add(replaytxt);

            replay.Content = replaystack;
            replay.Click += Replay;
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
        
        public void ChangeElementMap()
        {
            if (NotChangeMap) return;
            timetochangemov.Stop();
            IsChangeMap = false;
            phase++;
            timetochangemov.Restart();
            PlayTheme(themepath!, volumeadjust);
        }

        public void Dead()
        {
            player.Die();

            Random voicedead = new Random();
            int voicedeadindex = voicedead.Next(1, 3);
            string deadvoice = pathsound + "voicedead" + voicedeadindex + ".mp3";

            VoicePlay(deadvoice);

            CreateDeadTitle();

            CreateButtonReplay();

            Main.KeyDown -= KeyCommand;
            Main.KeyUp -= ReleaseKey;
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
            for (int entityindex = 0; entityindex < entities.Count; entityindex++)
            {
                entities[entityindex].soundplay.Stop();
                Playground.Children.Remove(entities[entityindex].entity);
            }
        }

        public void RestartElement()
        {
            timetochangemov.Restart();

            entities.Clear();

            phase = 1;
            changetime = 0;
            score = 0;
            bulletAmount = 7;
            magazinebullet = 35;
            mapindex = 1;
            volumeadjust = 0.2;

            IsJump = false;
            IsShoot = false;
            IsCrouch = false;
            IsReload = false;
            IsBreakHiScore = false;
            IsChangeMap = false;
            NotChangeMap = false;
            player.IsDead = false;

            Score.Text = "Score: 0";

            AmountBullet();
            CreateGameDisplay();

            player.Default();
            player.playershape.Margin = new Thickness(0, 0, 700, 80);
            theme.Stop();
        }

        public void Replay(object sender, RoutedEventArgs e)
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
            entity.main = Main;

            Playground.Children.Add(entity.entity);
            entities.Add(entity);

            await entity.MoveType(entity);

            CheckEntity(entity);
        }

        private void CheckEntity(Entity entity)
        {
            if (entity.getHit)
            {
                ScoreUpType(entity);
                Playground.Children.Remove(entity.entity);
            }
            else
            {
                if (!player.IsDead)
                {
                    Playground.Children.Remove(entity.entity);
                    if (!entity.IsHarmless) ScoreUp(1);
                }
                else
                {
                    return;
                }
            }

            entities.Remove(entity);
        }

        // BULLET AND MAGAZINE //

        public void PlayReloadSound()
        {
            soundreload.Open(new(pathsound + "deserteaglereload.mp3"));
            soundreload.Volume = 100;
            soundreload.Play();
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
                magazinebullet += 35;
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

            PlayReloadSound();

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

        private void KeyCommand(object sender, KeyEventArgs e)
        {
            if (player.IsDead)
            {
                return;
            }
            Key key = e.Key;
            switch (key)
            {
                case Key.Space:
                    HandleJump();
                    break;
                case Key.LeftCtrl:
                    HandleCrouch();
                    break;
                case Key.E:
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

            Bullet bullet = new Bullet();
            bullet.entities = entities;
            bullet.posout = player.playershape.Margin.Bottom;

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

        // SHOP AND INVENTORY //

    }
}
