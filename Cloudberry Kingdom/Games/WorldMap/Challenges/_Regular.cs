using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class RegularLevel : Challenge
    {
        static readonly RegularLevel instance = new RegularLevel();
        public static RegularLevel Instance { get { return instance; } }

        RegularLevel()
        {
            ID = new Guid("5ec69b87-3ef7-4a50-bf3a-8e55499e9bd0");
            Name = "Regular";
        }

        public override void Start(int Difficulty)
        {
            base.Start(Difficulty);

            StringWorldEndurance StringWorld =
                new StringWorldEndurance(GetSeeds(), null, 25);
            SetGameParent(StringWorld);
            StringWorld.Init();
            StringWorld.SetLevel(0);
        }

        protected override List<MakeSeed> MakeMakeList(int Difficulty)
        {
            List<MakeSeed> MakeList = new List<MakeSeed>();

            int diff = new int[] { 2, 4, 6, 9 }[Difficulty];

            for (int i = 0; i < 10; i++)
            MakeList.Add(() => StandardLevel(diff, LevelGeometry.Right));

            return MakeList;
        }

        static void StandardInit(LevelSeedData data)
        {
            data.Seed = data.Rnd.Rnd.Next();

            //data.MyBackgroundType = BackgroundType.Outside;
            data.SetTileSet(data.Rnd.Choose(new TileSet[] { TileSets.Terrace, TileSets.Dungeon, TileSets.Castle }));
            
            data.DefaultHeroType = BobPhsxNormal.Instance;
        }

        public static List<Upgrade> ObstacleUpgrades, JumpUpgrades, DodgeUpgrades;
        public static void InitLists()
        {
            if (JumpUpgrades == null)
            {
                JumpUpgrades = new List<Upgrade>(new Upgrade[] { Upgrade.MovingBlock, Upgrade.GhostBlock, Upgrade.FlyBlob, Upgrade.FallingBlock, Upgrade.Elevator, Upgrade.Cloud, Upgrade.BouncyBlock });


                DodgeUpgrades = new List<Upgrade>(new Upgrade[] { Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Laser, Upgrade.Spike });
                //DodgeUpgrades = new List<Upgrade>(new Upgrade[] { Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Laser, Upgrade.Spike, Upgrade.SpikeyLine, Upgrade.Firesnake });

                ObstacleUpgrades = new List<Upgrade>();
                ObstacleUpgrades.AddRange(JumpUpgrades);
                ObstacleUpgrades.AddRange(DodgeUpgrades);
            }
        }

        static List<Upgrade> Picks1, Picks2;
        static void SetPieceSeed(PieceSeedData piece, TileSet tileset, int Jump, int Dodge, int Speed, int JumpComplexity, int DodgeComplexity)
        {
            InitLists();

            // Pick upgrades            
            if (piece.MyPieceIndex == 0)
            {
                Picks1 = piece.Rnd.Choose(tileset.JumpUpgrades, JumpComplexity);
                Picks2 = piece.Rnd.Choose(tileset.DodgeUpgrades, DodgeComplexity);

                //Picks1 = Tools.Choose(JumpUpgrades, JumpComplexity);
                //Picks2 = Tools.Choose(DodgeUpgrades, DodgeComplexity);
            }

            foreach (Upgrade upgrade in Picks1)
                piece.MyUpgrades1[upgrade] = Jump;

            int DodgeLevel = (int)(Dodge);
            if (DodgeComplexity == 2) DodgeLevel = DodgeLevel - 1;
            else if (DodgeComplexity == 3) DodgeLevel = DodgeLevel - 2;
            else if (DodgeComplexity == 4) DodgeLevel = DodgeLevel - 3;
            
            //int DodgeLevel = (int)(Dodge / (1 + .5f * (DodgeComplexity - 1)));
            //if (DodgeLevel < 1) DodgeLevel = 1;
            
            foreach (Upgrade upgrade in Picks2)
                piece.MyUpgrades1[upgrade] = DodgeLevel;

            piece.MyUpgrades1[Upgrade.Jump] = Jump;
            piece.MyUpgrades1[Upgrade.Ceiling] = (Jump + Dodge) / 2;
            piece.MyUpgrades1[Upgrade.General] = Dodge;
            piece.MyUpgrades1[Upgrade.Speed] = Speed;

            piece.Style.MyModParams = (level, p) =>
            {
                NormalBlock_Parameters NParams = (NormalBlock_Parameters)p.Style.FindParams(NormalBlock_AutoGen.Instance);
                //NParams.SetHallway(p.GeometryType);
            };

            piece.StandardClose();
        }

        public static LevelSeedData StandardLevel(int Difficulty, LevelGeometry Geometry)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            LevelSeedData.CustomDifficulty custom = StandardPieceMod(Difficulty, data);

            data.Initialize(NormalGameData.Factory, Geometry, 2, 4500, custom);

            return data;
        }

        public static LevelSeedData.CustomDifficulty StandardPieceMod(int Difficulty, LevelSeedData LevelSeed)
        {
            int Jump, Dodge, Speed, JumpComplexity, DodgeComplexity;

            switch (LevelSeed.Rnd.RndInt(0, 3))
            {
                case 0:
                    Jump = Difficulty;
                    Dodge = Difficulty;
                    Speed = (int)Tools.DifficultyLerp159(2, 4, 7, Difficulty);
                    JumpComplexity = 2;
                    DodgeComplexity = 1;
                    break;

                case 1:
                    Jump = Difficulty / 2;
                    Dodge = (int)(1.5f * Difficulty);
                    Speed = (int)Tools.DifficultyLerp159(2, 3, 6, Difficulty);
                    JumpComplexity = 1;
                    DodgeComplexity = 2;
                    break;

                case 2:
                    Jump = Difficulty / 2;
                    Dodge = (int)(1.5f * Difficulty);
                    Speed = (int)Tools.DifficultyLerp159(2, 4, 9, Difficulty);
                    JumpComplexity = 0;
                    DodgeComplexity = LevelSeed.Rnd.RndInt(1, 3);
                    break;

                default:
                    Jump = Difficulty;
                    Dodge = 0;
                    Speed = (int)Tools.DifficultyLerp159(2, 4, 6, Difficulty);
                    JumpComplexity = LevelSeed.Rnd.RndInt(1, 3);
                    DodgeComplexity = 0;
                    break;
            }

            return
                piece => SetPieceSeed(piece, LevelSeed.MyTileSet, Jump, Dodge, Speed, JumpComplexity, DodgeComplexity);
        }


        // -------------------------
        // Fixed upgrade lists
        // -------------------------
        public static LevelSeedData HeroLevel(float Difficulty, BobPhsx Hero, int Length)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.DefaultHeroType = Hero;

            //LevelSeedData.CustomDifficulty custom = FixedPieceMod(Difficulty, data);
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
                case 0: piece.Rnd.Choose(EasyUpgrades)(piece.MyUpgrades1); break;
                case 1: piece.Rnd.Choose(NormalUpgrades)(piece.MyUpgrades1); break;
                case 2: piece.Rnd.Choose(AbusiveUpgrades)(piece.MyUpgrades1); break;
                default: piece.Rnd.Choose(HardcoreUpgrades)(piece.MyUpgrades1); break;
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
                u[Upgrade.SpikeyGuy] = 6;
                u[Upgrade.Spike] = 9;
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