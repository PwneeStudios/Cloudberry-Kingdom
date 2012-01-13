using System;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class Challenge_TimeCrisis : Challenge
    {
        static readonly Challenge_TimeCrisis instance = new Challenge_TimeCrisis();
        public static Challenge_TimeCrisis Instance { get { return instance; } }

        Challenge_TimeCrisis()
        {
            ID = new Guid("a2c3bc59-2bd3-4037-93b1-3760915e6825");
            Name = "Time Crisis";
        }

        public override void Start(int Difficulty)
        {
            base.Start(Difficulty);

            GUI_Timer Timer = new GUI_Timer();

            Timer.CoinTimeValue = 50;
            Timer.MaxTime = 62 * 240;
            Timer.Time = 62 * 180;

            StringWorldTimed StringWorld = new StringWorldTimed(GetSeeds(), Timer);

            StringWorld.OnLevelBegin += lvl => LevelSeedData.AddGameObjects_Default(lvl, false);

            SetGameParent(StringWorld);
            StringWorld.Init();
            StringWorld.SetLevel(0);
        }

        protected override List<MakeSeed> MakeMakeList(int Difficulty)
        {
            List<MakeSeed> MakeList = new List<MakeSeed>();

            for (int i = 0; i < 60; i++)
            {
                MakeList.Add(() => RegularLevel.FixedLevel(Difficulty));
            }

            return MakeList;

            //int diff = new int[] { 2, 4, 6, 9 }[Difficulty];

            //for (int i = 0; i < 60; i++)
            //{
            //    int AddedDifficulty = i / 6;
            //    MakeList.Add(() => RegularLevel.StandardLevel(diff, LevelGeometry.Right));
            //}

            //return MakeList;
        }
    }
}