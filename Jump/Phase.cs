using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump
{
    public class Phase
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";
        public MainWindow? main { get; set; }
        public ListEntity changeentity = new ListEntity();


        public int phase { get; set; }
        public int speedfish { get; set; }
        public int speeddemon { get; set; }
        public int entitychance { get; set; }

        public int limitphase = 2;

        public async Task ChangePhase()
        {
            Random spawn = new Random();
            int spawnindex = spawn.Next(200);

            SpawnItem(spawnindex);

            if (phase > limitphase) phase = limitphase;

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

                default:
                    return;
            }
        }


        public void SpawnItem(int spawnindex)
        {
            if (spawnindex % 50 != 0) return;

            Random itemrand = new Random();
            int itemindex = itemrand.Next(0, 2);

            if (itemindex == 0) main!.SpawnMag();
            else main!.SpawnArmor();
        }

        public async void Phase1(int spawnindex)
        {
            if (spawnindex > entitychance) return;

            Random randenemy = new Random();
            int enemyindex = randenemy.Next(2);
            Entity newentity = changeentity.ChangeEntityMap1(enemyindex);
            changeentity.SetSpeedMap1(newentity, main!);
            await main!.SpawnEntity(phase, newentity);

        }

        public async void Phase2(int spawnindex)
        {
            if (spawnindex > entitychance) return;

            Random randenemy = new Random();
            int enemyindex = randenemy.Next(2);
            Entity newentity = changeentity.ChangeEntityMap2(enemyindex);
            changeentity.SetSpeedMap2(newentity, main!);
            await main!.SpawnEntity(phase, newentity);

        }
    }
}
