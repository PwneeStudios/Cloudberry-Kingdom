using System;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class Challenge_Endurance : Challenge
    {
        static readonly Challenge_Endurance instance = new Challenge_Endurance();
        public static Challenge_Endurance Instance { get { return instance; } }

        Challenge_Endurance()
        {
            ID = new Guid("3a5141f9-585e-4716-8398-0c96b6ebdec7");
            Name = "Endurance";
        }

        public override void Start(int Difficulty)
        {
            base.Start(Difficulty);

            StringWorldEndurance StringWorld = new StringWorldEndurance(GetSeeds(), 10, 25);
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
            //    MakeList.Add(() => RegularLevel.StandardLevel(diff + AddedDifficulty, LevelGeometry.Right));
            //}

            return MakeList;
        }
    }
}