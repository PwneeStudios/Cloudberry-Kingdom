using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class SpaceshipLevel
    {
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