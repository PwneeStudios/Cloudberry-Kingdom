using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Challenge_Spaceship : Challenge
    {
        static readonly Challenge_Spaceship instance = new Challenge_Spaceship();
        public static Challenge_Spaceship Instance { get { return instance; } }

        Challenge_Spaceship()
        {
            ID = new Guid("a2c3bc59-2bd3-4037-93b1-3760915e6825");
            Name = "The Final Frontier";
        }

        public override void Start(int Difficulty)
        {
            base.Start(Difficulty);

            StringWorldEndurance StringWorld = new StringWorldEndurance(GetSeeds(), null, 25);
            SetGameParent(StringWorld);
            StringWorld.Init();
            StringWorld.SetLevel(0);
        }

        protected override List<MakeSeed> MakeMakeList(int Difficulty)
        {
            List<MakeSeed> MakeList = new List<MakeSeed>();

            int diff = new int[] { 2, 4, 6, 9 }[Difficulty];

            for (int i = 0; i < 3; i++)
                MakeList.Add(() => StandardLevel(diff, LevelGeometry.Right));

            return MakeList;
        }

        public static void StandardInit(LevelSeedData data)
        {
            data.Seed = data.Rnd.Rnd.Next();

            data.SetTileSet(TileSets.Castle);
            data.DefaultHeroType = BobPhsxSpaceship.Instance;
        }

        // Random levels
        public static LevelSeedData StandardLevel(int Difficulty, LevelGeometry Geometry)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            LevelSeedData.CustomDifficulty custom = StandardPieceMod(Difficulty, data);

            data.Initialize(NormalGameData.Factory, Geometry, 2, 4500, custom);

            return data;
        }

        public static List<Upgrade> BlockUpgrades, ObjectUpgrades;
        public static void InitLists()
        {
            if (BlockUpgrades == null)
            {
                BlockUpgrades = new List<Upgrade>(new Upgrade[] { Upgrade.MovingBlock, Upgrade.GhostBlock, Upgrade.FlyBlob, Upgrade.Elevator });
                ObjectUpgrades = new List<Upgrade>(new Upgrade[] { Upgrade.Fireball, Upgrade.FireSpinner, Upgrade.SpikeyGuy, Upgrade.Pinky, Upgrade.Laser, Upgrade.Spike });
            }
        }

        static List<Upgrade> Picks1, Picks2;
        static void SetPieceSeed(PieceSeedData piece, TileSet tileset, int BlockLevel, int ObjectLevel, int Speed, int BlockComplexity, int ObjectComplexity)
        {
            InitLists();

            // Pick upgrades            
            if (piece.MyPieceIndex == 0)
            {
                Picks1 = piece.Rnd.Choose(BlockUpgrades, BlockComplexity);
                Picks2 = piece.Rnd.Choose(ObjectUpgrades, ObjectComplexity);
            }

            foreach (Upgrade upgrade in Picks1)
                piece.MyUpgrades1[upgrade] = BlockLevel;

            int DodgeLevel = (int)(ObjectLevel / (1 + .5f * (ObjectComplexity - 1)));
            if (DodgeLevel < 1) DodgeLevel = 1;

            foreach (Upgrade upgrade in Picks2)
                piece.MyUpgrades1[upgrade] = DodgeLevel;

            piece.MyUpgrades1[Upgrade.Jump] = BlockLevel;
            piece.MyUpgrades1[Upgrade.Ceiling] = (BlockLevel + ObjectLevel) / 2;
            piece.MyUpgrades1[Upgrade.General] = ObjectLevel;
            piece.MyUpgrades1[Upgrade.Speed] = Speed;

            piece.Style.MyModParams = (level, p) =>
            {
                NormalBlock_Parameters NParams = (NormalBlock_Parameters)p.Style.FindParams(NormalBlock_AutoGen.Instance);
            };

            piece.StandardClose();
        }

        public static LevelSeedData.CustomDifficulty StandardPieceMod(int Difficulty, LevelSeedData LevelSeed)
        {
            int BlockLevel, ObjectLevel, Speed, BlockComplexity, ObjectComplexity;

            switch (LevelSeed.Rnd.RndInt(0, 1))
            {
                default:
                    BlockLevel = Difficulty;
                    ObjectLevel = Difficulty;
                    Speed = (int)DifficultyHelper.Interp159(2, 4, 9, Difficulty);
                    BlockComplexity = LevelSeed.Rnd.RndInt(1, 3);
                    ObjectComplexity = LevelSeed.Rnd.RndInt(1, 4);
                    break;
            }

            return
                piece => SetPieceSeed(piece, LevelSeed.MyTileSet, BlockLevel, ObjectLevel, Speed, BlockComplexity, ObjectComplexity);
        }
    }
}