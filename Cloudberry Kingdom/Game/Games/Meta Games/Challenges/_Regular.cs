using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class RegularLevel
    {
        RegularLevel()
        {
        }

        static void StandardInit(LevelSeedData data)
        {
            data.Seed = data.Rnd.Rnd.Next();

            data.SetTileSet(null);

            data.DefaultHeroType = BobPhsxNormal.Instance;
        }

        // -------------------------
        // Fixed upgrade lists
        // -------------------------
        public static LevelSeedData HeroLevel(float Difficulty, BobPhsx Hero, int Length)
        {
            if (Hero == ArcadeMenu.Ultimate)
            {
                Length *= 2;
                Difficulty *= 2;
            }

            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.DefaultHeroType = Hero;

            LevelSeedData.CustomDifficulty custom = DifficultyGroups.FixedPieceMod(Difficulty, data);

            data.Initialize(NormalGameData.Factory, LevelGeometry.Right, 1, Length, custom);

            return data;
        }

        public static LevelSeedData FixedLevel(int Difficulty)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            LevelSeedData.CustomDifficulty custom = FixedPieceMod(Difficulty, data);

            data.Initialize(NormalGameData.Factory, LevelGeometry.Right, 2, 4500, custom);

            return data;
        }

        public static LevelSeedData.CustomDifficulty FixedPieceMod(int Difficulty, LevelSeedData LevelSeed)
        {
            return piece => FixedPieceSeed(piece, Difficulty);
        }

        static void FixedPieceSeed(PieceSeedData piece, int Difficulty)
        {
            InitFixedUpgrades();

            switch (Difficulty)
            {
                case 0: Tools.GlobalRnd.Choose(EasyUpgrades)(piece.MyUpgrades1); break;
                case 1: Tools.GlobalRnd.Choose(NormalUpgrades)(piece.MyUpgrades1); break;
                case 2: Tools.GlobalRnd.Choose(AbusiveUpgrades)(piece.MyUpgrades1); break;
                default: Tools.GlobalRnd.Choose(HardcoreUpgrades)(piece.MyUpgrades1); break;
            }

            piece.StandardClose();
        }

        static void InitFixedUpgrades()
        {
            if (EasyUpgrades != null)
                return;

            EasyUpgrades = new List<Action<Upgrades>>();

            MakeEasyUpgrades();
            MakeNormalUpgrades();
            MakeAbusiveUpgrades();
            MakeHardcoreUpgrades();
        }

        static List<Action<Upgrades>> EasyUpgrades;
        static void MakeEasyUpgrades()
        {
            List<Action<Upgrades>> f = EasyUpgrades;

            f.Add(u =>
            {
                u[Upgrade.Jump] = 2;
                u[Upgrade.Speed] = 1;
                u[Upgrade.MovingBlock] = 1;
                u[Upgrade.FallingBlock] = 1;
                u[Upgrade.FlyBlob] = 1;
            });

            f.Add(u =>
            {
                u[Upgrade.SpikeyGuy] = 2;
                u[Upgrade.Jump] = 3.5f;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 3.5f;
                u[Upgrade.Pinky] = 2;
                u[Upgrade.Speed] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 3;
                u[Upgrade.Cloud] = 2;
                u[Upgrade.SpikeyGuy] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Elevator] = 3;
                u[Upgrade.Laser] = 2;
                u[Upgrade.MovingBlock] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.FallingBlock] = 4;
                u[Upgrade.BouncyBlock] = 1.8f;
                u[Upgrade.Spike] = 5;
            });

            f.Add(u =>
            {
                u[Upgrade.BouncyBlock] = 4;
            });
        }

        static List<Action<Upgrades>> NormalUpgrades = new List<Action<Upgrades>>();
        static void MakeNormalUpgrades()
        {
            List<Action<Upgrades>> f = NormalUpgrades;

            f.Add(u =>
            {
                u[Upgrade.Jump] = 5.5f;
                u[Upgrade.Speed] = 2;
                u[Upgrade.MovingBlock] = 1;
                u[Upgrade.FallingBlock] = 1;
                u[Upgrade.FlyBlob] = 1;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 5.5f;
                u[Upgrade.MovingBlock] = 2;
                u[Upgrade.GhostBlock] = 2;
                u[Upgrade.BouncyBlock] = 2;
                u[Upgrade.FlyBlob] = 2;
                u[Upgrade.Spike] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 4;
                u[Upgrade.Speed] = 2;
                u[Upgrade.MovingBlock] = 2;
                u[Upgrade.BouncyBlock] = 2;
                u[Upgrade.FireSpinner] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 6.5f;
                u[Upgrade.Speed] = 2;
                u[Upgrade.Elevator] = 4;
                u[Upgrade.Laser] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 2;
                u[Upgrade.Speed] = 2;
                u[Upgrade.Spike] = 4;
                u[Upgrade.SpikeyGuy] = 4;
            });

            f.Add(u =>
            {
                u[Upgrade.Speed] = 2;
                u[Upgrade.Fireball] = 2;
                u[Upgrade.Laser] = 2;
                u[Upgrade.FireSpinner] = 2;
                u[Upgrade.Spike] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.MovingBlock] = 9;
                u[Upgrade.Jump] = 4;
                u[Upgrade.Speed] = 3;
            });

            f.Add(u =>
            {
                u[Upgrade.FlyBlob] = 2;
                u[Upgrade.FallingBlock] = 2;
                u[Upgrade.Speed] = 6;
                u[Upgrade.GhostBlock] = 6;
            });

            f.Add(u =>
            {
                u[Upgrade.Fireball] = 2;
                u[Upgrade.Pinky] = 2;
                u[Upgrade.FlyBlob] = 2;
                u[Upgrade.Jump] = 4;
            });

            f.Add(u =>
            {
                u[Upgrade.Fireball] = 2;
                u[Upgrade.Pinky] = 2;
                u[Upgrade.BouncyBlock] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Fireball] = 6;
                u[Upgrade.Speed] = 4;
            });

            f.Add(u =>
            {
                u[Upgrade.Spike] = 9;
                u[Upgrade.BouncyBlock] = 9;
                u[Upgrade.MovingBlock] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 2;
                u[Upgrade.FireSpinner] = 2;
                u[Upgrade.Speed] = 2;
                u[Upgrade.MovingBlock] = 4;
                u[Upgrade.SpikeyGuy] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Fireball] = 2;
                u[Upgrade.FireSpinner] = 2;
                u[Upgrade.FlyBlob] = 2;
                u[Upgrade.MovingBlock] = 4;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 6.5f;
                u[Upgrade.GhostBlock] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.BouncyBlock] = 2;
                u[Upgrade.FlyBlob] = 2;
                u[Upgrade.Spike] = 2;
            });
        }

        static List<Action<Upgrades>> AbusiveUpgrades = new List<Action<Upgrades>>();
        static void MakeAbusiveUpgrades()
        {
            List<Action<Upgrades>> f = AbusiveUpgrades;

            f.Add(u =>
            {
                u[Upgrade.Jump] = 8;
                u[Upgrade.Speed] = 6;
                u[Upgrade.MovingBlock] = 2;
                u[Upgrade.GhostBlock] = 2;
                u[Upgrade.BouncyBlock] = 2;
                u[Upgrade.FallingBlock] = 2;
                u[Upgrade.FlyBlob] = 2;
                u[Upgrade.Spike] = 3;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 8;
                u[Upgrade.Speed] = 9;
                u[Upgrade.MovingBlock] = 2;
                u[Upgrade.FallingBlock] = 2;
                u[Upgrade.FlyBlob] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 8;
                u[Upgrade.Speed] = 4;
                u[Upgrade.SpikeyGuy] = 6;
                u[Upgrade.FallingBlock] = 9;
                u[Upgrade.FlyBlob] = 2;
            });


            f.Add(u =>
            {
                u[Upgrade.Jump] = 8;
                u[Upgrade.Speed] = 4;
                u[Upgrade.Laser] = 4;
                u[Upgrade.FallingBlock] = 9;
                u[Upgrade.FlyBlob] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 4;
                u[Upgrade.Speed] = 6;
                u[Upgrade.MovingBlock] = 4;
                u[Upgrade.BouncyBlock] = 4;
                u[Upgrade.FireSpinner] = 4;
            });


            f.Add(u =>
            {
                u[Upgrade.Jump] = 8;
                u[Upgrade.Speed] = 6;
                u[Upgrade.Elevator] = 8;
                u[Upgrade.Laser] = 2;
                u[Upgrade.Spike] = 4;
            });


            f.Add(u =>
            {
                u[Upgrade.Jump] = 2;
                u[Upgrade.Speed] = 6;
                u[Upgrade.SpikeyGuy] = 5f;
                u[Upgrade.Spike] = 7.5f;
            });

            f.Add(u =>
            {
                u[Upgrade.MovingBlock] = 4;
                u[Upgrade.FlyBlob] = 4;
                u[Upgrade.FallingBlock] = 4;
                u[Upgrade.SpikeyGuy] = 4;
                u[Upgrade.FireSpinner] = 4;
                u[Upgrade.Pinky] = 4;
            });


            f.Add(u =>
            {
                u[Upgrade.FireSpinner] = 4;
                u[Upgrade.Pinky] = 4;
                u[Upgrade.FallingBlock] = 4;
                u[Upgrade.Cloud] = 4;
                u[Upgrade.SpikeyGuy] = 4;
                u[Upgrade.BouncyBlock] = 4;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 8;
                u[Upgrade.Speed] = 4;
                u[Upgrade.FallingBlock] = 9;
                u[Upgrade.BouncyBlock] = 9;
                u[Upgrade.SpikeyGuy] = 4;
                u[Upgrade.Pinky] = 4;
            });

            f.Add(u =>
            {
                u[Upgrade.FireSpinner] = 2;
                u[Upgrade.FlyBlob] = 2;
                u[Upgrade.Laser] = 2;
                u[Upgrade.GhostBlock] = 2;
                u[Upgrade.Speed] = 6;
            });

            f.Add(u =>
            {
                u[Upgrade.Cloud] = 2;
                u[Upgrade.FireSpinner] = 4;
                u[Upgrade.FlyBlob] = 4;
                u[Upgrade.Jump] = 4;
                u[Upgrade.Speed] = 2;
                u[Upgrade.MovingBlock] = 4;
            });

            f.Add(u =>
            {
                u[Upgrade.FireSpinner] = 6;
                u[Upgrade.Laser] = 4;
                u[Upgrade.Jump] = 4;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 8;
                u[Upgrade.GhostBlock] = 6;
            });

            f.Add(u =>
            {
                u[Upgrade.SpikeyGuy] = 6;
                u[Upgrade.Pinky] = 7;
                u[Upgrade.Speed] = 4;
            });
        }

        static List<Action<Upgrades>> HardcoreUpgrades = new List<Action<Upgrades>>();
        static void MakeHardcoreUpgrades()
        {
            List<Action<Upgrades>> f = HardcoreUpgrades;

            f.Add(u =>
            {
                u[Upgrade.MovingBlock] = 9;
                u[Upgrade.FireSpinner] = 9;
                u[Upgrade.Speed] = 9;
            });

            f.Add(u =>
            {
                u[Upgrade.Laser] = 8.5f;
            });

            f.Add(u =>
            {
                u[Upgrade.FireSpinner] = 4;
                u[Upgrade.Pinky] = 6;
                u[Upgrade.MovingBlock] = 9;
            });

            f.Add(u =>
            {
                u[Upgrade.FireSpinner] = 9;
                u[Upgrade.Laser] = 6;
            });

            f.Add(u =>
            {
                u[Upgrade.FireSpinner] = 9;
                u[Upgrade.Speed] = 4;
                u[Upgrade.MovingBlock] = 2;
            });

            f.Add(u =>
            {
                u[Upgrade.Pinky] = 9;
                u[Upgrade.SpikeyGuy] = 9;
                u[Upgrade.Spike] = 4;
                u[Upgrade.Speed] = 9;
            });


            f.Add(u =>
            {
                u[Upgrade.Jump] = 9;
                u[Upgrade.Speed] = 9;
                u[Upgrade.MovingBlock] = 6;
                u[Upgrade.GhostBlock] = 6;
                u[Upgrade.BouncyBlock] = 4;
                u[Upgrade.FlyBlob] = 4;
                u[Upgrade.Spike] = 9;
            });

            f.Add(u =>
            {
                u[Upgrade.Jump] = 6;
                u[Upgrade.Speed] = 6;
                u[Upgrade.MovingBlock] = 4;
                u[Upgrade.GhostBlock] = 4;
                u[Upgrade.FlyBlob] = 4;
                u[Upgrade.Pinky] = 4;
                u[Upgrade.Laser] = 4;
            });


            f.Add(u =>
            {
                u[Upgrade.Jump] = 6;
                u[Upgrade.Speed] = 6;
                u[Upgrade.Elevator] = 9;
                u[Upgrade.Laser] = 6;
            });


            f.Add(u =>
            {
                u[Upgrade.Jump] = 4;
                u[Upgrade.Speed] = 9;
                u[Upgrade.Spike] = 9;
                u[Upgrade.SpikeyGuy] = 9;
            });
        }
    }
}