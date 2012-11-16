using System;
using System.Collections.Generic;

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
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.DefaultHeroType = Hero;

            //LevelSeedData.CustomDifficulty custom = DifficultyGroups.FixedPieceMod(Difficulty, data);
            Lambda_1<PieceSeedData> custom = DifficultyGroups.FixedPieceMod(Difficulty, data);
            data.Initialize(NormalGameData.Factory, LevelGeometry.Right, 1, Length, custom);

            return data;
        }
    }
}