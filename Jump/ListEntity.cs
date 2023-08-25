using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jump
{
    public class ListEntity
    {
        public MainWindow? main { get; set; }
        public void SetSpeedMap1(Entity entity, MainWindow main)
        {
            switch (main.changetime)
            {
                case 0:
                    entity.movementspeed = 30;
                    return;

                case 1:
                    if (entity is GoldFish)
                    {
                        entity.movementspeed = 50;
                        return;
                    }
                    break;

                case 2:
                    if (entity is Demon)
                    {
                        entity.movementspeed = 45;
                        return;
                    }
                    entity.movementspeed = 60;
                    main.timechange = 20;
                    return;

                case 3:
                    if (entity is Demon)
                    {
                        entity.movementspeed = 45;
                        return;
                    }
                    entity.movementspeed = 60;
                    main.timechange = 60;
                    return;

                default:
                    return;
            }

            entity.movementspeed = 30;
        }

        public void SetSpeedMap2(Entity entity, MainWindow main)
        {
            switch (main.changetime)
            {
                case 4:
                    if (entity is Mine)
                    {
                        entity.movementspeed = 50;
                        return;
                    }
                    main.timechange = 40;
                    entity.movementspeed = 35;
                    entity.bulletspeed = 70;
                    return;

                case 5:
                    if (entity is Mine)
                    {
                        entity.movementspeed = 70;
                        return;
                    }
                    entity.movementspeed = 50;
                    entity.bulletspeed = 90;
                    main.timechange = 30;
                    return;

                case 6:
                    if (entity is Mine)
                    {
                        entity.movementspeed = 70;
                        return;
                    }
                    entity.movementspeed = 50;
                    entity.bulletspeed = 90;
                    main.timechange = 59;
                    return;

                default:
                    main.changetime--;
                    SetSpeedMap2(entity, main);
                    return;
            }
        }

        public void SetSpeedMap3(Entity entity, MainWindow main)
        {
            switch (main.changetime)
            {
                case 7:
                    entity.movementspeed = 35;
                    entity.bulletspeed = 30;
                    main.timechange = 40;
                    return;

                case 8:
                    entity.movementspeed = 35;
                    entity.bulletspeed = 30;
                    main.timechange = 59;
                    return;

                default:
                    main.changetime--;
                    SetSpeedMap3(entity, main);
                    return;
            }
        }

        public Entity ChangeEntityMap1(int enemyindex)
        {
            Entity newentity;
            switch (enemyindex)
            {
                case 0:
                    GoldFish goldFish = new GoldFish();
                    newentity = goldFish;
                    break;

                case 1:
                    Demon demon = new Demon();
                    newentity = demon;
                    break;

                case 2:
                    BigGoldFish biggoldfish = new BigGoldFish();
                    newentity = biggoldfish;
                    break;

                default:
                    return newentity = new Entity();
            }
            return newentity;
        }

        public Entity ChangeEntityMap2(int enemyindex)
        {
            Entity newentity;
            switch (enemyindex)
            {
                case 0:
                    Bush bush = new Bush();
                    newentity = bush;
                    break;

                case 1:
                    Mine mine = new Mine();
                    newentity = mine;
                    break;

                case 2:
                    Samurai samurai = new Samurai();
                    newentity = samurai;
                    break;

                default:
                    return newentity = new Entity();
            }
            return newentity;
        }

        public Entity ChangeEntityMap3(int enemyindex)
        {
            Entity newentity;
            switch (enemyindex)
            {
                case 0:
                    Warrior warrior = new Warrior();
                    newentity = warrior;
                    break;

                case 1:
                    PirateCaptain piratecap = new PirateCaptain();
                    newentity = piratecap;
                    break;

                case 2:
                    Apostate apostate = new Apostate();
                    newentity = apostate;
                    break;

                case 3:
                    Kamikaze kamikaze = new Kamikaze();
                    newentity = kamikaze;
                    break;

                default:
                    return newentity = new Entity();
            }
            return newentity;
        }
    }
}
