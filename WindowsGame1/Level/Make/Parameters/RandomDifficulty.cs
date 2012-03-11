using System;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class RndDifficulty
    {
        static public int ChoosePaths(PieceSeedData data)
        {
            float JumpDifficulty = data.MyUpgrades1[Upgrade.Jump];

            double p = data.Rnd.Rnd.NextDouble();
            if (p < .75f + .25f * .1f * JumpDifficulty) return 1;
            //else if (p < .9f + .1f * .1f * JumpDifficulty) return 2;
            //else return 3;
            else return 2;
        }

        static public Level.LadderType ChooseLadder(int Difficulty)
        {
            //if (Difficulty < 30)
            //    return Level.LadderType.Simple2;
            //else
                return Level.LadderType.DoubleMoving;
        }

        static public void ZeroUpgrades(Upgrades upgrades)
        {
            for (int i = 0; i < Tools.UpgradeTypes; i++)
                upgrades.UpgradeLevels[i] = 0;
        }

        //static public int[] ChooseUpgrades(int Num)
        //{
        //    bool[] Valid = new bool[Tools.UpgradeTypes];
        //    for (int i = 0; i < Tools.UpgradeTypes; i++)
        //        Valid[i] = true;
        //    Valid[(int)Upgrade.Speed] = false;
        //    Valid[(int)Upgrade.General] = false;
        //    return MyLevel.Rnd.RndIndex(Tools.UpgradeTypes, Num, Valid);
        //}


        static public void EnforceLevelCap(Upgrades upgrades, int Cap, int LowerCap)
        {
            for (int i = 0; i < Tools.UpgradeTypes; i++)
                upgrades.UpgradeLevels[i] = Math.Max(LowerCap, Math.Min(Cap, upgrades.UpgradeLevels[i]));
        }

        static public void UseUpgrades(PieceSeedData Seed, Upgrades u)
        {
            ZeroUpgrades(Seed.MyUpgrades1);
            Seed.MyUpgrades1.CopyFrom(u);
            Seed.MyUpgrades1.CalcGenData(Seed.MyGenData.gen1, Seed.Style);

            ZeroUpgrades(Seed.MyUpgrades2);
            Seed.MyUpgrades2.CopyFrom(u);
            Seed.MyUpgrades2.CalcGenData(Seed.MyGenData.gen2, Seed.Style);
        }

        /// <summary>
        /// Randomizes the obstacles upgrades of a seed, using the seed's integer valued difficulty.
        /// </summary>
        /// <param name="Seed"></param>
        static public void IntToDifficulty(PieceSeedData Seed, TileSet TileType)
        {
            // Get the tile set's associated information
            TileSets.Get(TileType);

            ZeroUpgrades(Seed.MyUpgrades1);
            Seed.MyUpgrades1[Upgrade.FireSpinner] = 10;
            Seed.MyUpgrades1[Upgrade.General] = 10;
            Seed.MyUpgrades1.CalcGenData(Seed.MyGenData.gen1, Seed.Style);

            ZeroUpgrades(Seed.MyUpgrades2);
            Seed.MyUpgrades1.UpgradeLevels.CopyTo(Seed.MyUpgrades2.UpgradeLevels, 0);
            Seed.MyUpgrades2.CalcGenData(Seed.MyGenData.gen2, Seed.Style);

            /*
            int Types;
            if (Difficulty == 0) Types = 0;
            else Types = MyLevel.Rnd.Rnd.Next(Math.Min(Tools.UpgradeTypes - 1, Generic.MinTypes),
                                        Math.Min(Tools.UpgradeTypes - 1, Generic.MaxTypes));

            int[] UpgradeChoices = ChooseUpgrades(Types);

            ZeroUpgrades(Seed.MyUpgrades1);
            UpgradeUpgrades(Seed.MyUpgrades1, UpgradeChoices, Difficulty / 5);
            EnforceLevelCap(Seed.MyUpgrades1, Generic.MaxLevel / 2, Generic.MinLevel / 2);
            Seed.MyUpgrades1.CalcGenData(Seed.MyGenData.gen1, Seed.Style);

            ZeroUpgrades(Seed.MyUpgrades2);
            UpgradeUpgrades(Seed.MyUpgrades2, UpgradeChoices, Difficulty);
            EnforceLevelCap(Seed.MyUpgrades2, Generic.MaxLevel, Generic.MinLevel);
            Seed.MyUpgrades2.CalcGenData(Seed.MyGenData.gen2, Seed.Style);

            Seed.MyUpgrades1.UpgradeLevels.CopyTo(Seed.MyUpgrades2.UpgradeLevels, 0);*/
        }
    }
}