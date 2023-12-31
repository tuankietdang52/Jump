﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jump.EnemyEntity;

namespace Jump
{
    public class Phase
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public MainWindow? main { get; set; }
        public ListEntity changeentity = new ListEntity();

        public bool AlreadyHaveBoss = false;

        public int itemchance = 50;
        public int phase { get; set; }
        public int speedfish { get; set; }
        public int speeddemon { get; set; }
        public int entitychance { get; set; }

        public int limitphase = 3;

        public async Task ChangePhase()
        {
            changeentity.main = this.main;

            Random spawn = new Random();
            int spawnindex = spawn.Next(200);

            if (phase > limitphase) phase = limitphase;

            if (main!.changetime % 3 == 0 && main!.changetime != 0)
            {
                SpawnBoss();
                await Task.Delay(1000);
                return;
            }

            itemchance = 50;

            switch (phase)
            {
                case 1:
                    entitychance = 100;
                    Phase1(spawnindex);
                    await Task.Delay(1000);
                    break;

                case 2:
                    entitychance = 80;
                    if (main!.changetime == 5) entitychance = 100;
                    Phase2(spawnindex);
                    await Task.Delay(1000);
                    break;

                case 3:
                    entitychance = 150;
                    Phase3(spawnindex);
                    await Task.Delay(1000);
                    break;

                default:
                    return;
            }
        }

        public void SpawnBoss()
        {
            switch (phase)
            {
                case 1:
                    Boss1();
                    return;

                case 2:
                    Boss2();
                    return;

                case 3:
                    Boss3();
                    return;

                default:
                    main!.IsHaveBoss = false;
                    main!.BossIsDefeated();
                    return;
            }
        }

        public async void Boss1()
        {
            if (!AlreadyHaveBoss && main!.IsHaveBoss)
            {
                Entity newentity = changeentity.ChangeEntityMap1(2);

                AlreadyHaveBoss = true;

                await main!.SpawnEntity(newentity);

            }
            else return;
        }

        public async void Boss2()
        {
            if (!AlreadyHaveBoss && main!.IsHaveBoss)
            {
                Entity newentity = changeentity.ChangeEntityMap2(2);

                AlreadyHaveBoss = true;

                await main!.SpawnEntity(newentity);

            }
            else return;
        }

        public async void Boss3()
        {
            if (!AlreadyHaveBoss && main!.IsHaveBoss)
            {
                Entity newentity = changeentity.ChangeEntityMap3(4);

                AlreadyHaveBoss = true;

                await main!.SpawnEntity(newentity);

            }
            else return;
        }

        public async void Phase1(int spawnindex)
        {
            if (spawnindex > entitychance) return;

            Random randenemy = new Random();
            int enemyindex = randenemy.Next(2);

            Entity newentity;

            if (!main!.IsHaveBoss) newentity = changeentity.ChangeEntityMap1(enemyindex);
            else return;

            changeentity.SetSpeedMap1(newentity, main!);
            await main!.SpawnEntity(newentity);

        }

        public async void Phase2(int spawnindex)
        {
            if (spawnindex > entitychance) return;

            Random randenemy = new Random();
            int enemyindex = randenemy.Next(2);
            Entity newentity;

            if (!main!.IsHaveBoss) newentity = changeentity.ChangeEntityMap2(enemyindex);
            else return;

            changeentity.SetSpeedMap2(newentity, main!);
            await main!.SpawnEntity(newentity);
        }

        public async void Phase3(int spawnindex)
        {
            if (spawnindex > entitychance) return;

            Random randenemy = new Random();
            int enemyindex;
            if (main!.changetime == 7) enemyindex = randenemy.Next(0, 2);
            else enemyindex = randenemy.Next(0, 4);

            if (main!.IsSpawnPirate && enemyindex == 1) return;
            if (main!.IsHaveAspotate && enemyindex == 2) return;
            Entity newentity;

            if (!main!.IsHaveBoss) newentity = changeentity.ChangeEntityMap3(enemyindex);
            else return;

            changeentity.SetSpeedMap3(newentity, main!);
            await main!.SpawnEntity(newentity);
        }
    }
}
