using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public class Challenge_Fireballs : Challenge
    {
        static readonly Challenge_Fireballs instance = new Challenge_Fireballs();
        public static Challenge_Fireballs Instance { get { return instance; } }

        Challenge_Fireballs()
        {
            ID = new Guid("b22675a3-a67c-42e3-b65f-2b26cdaf144a");
            Name = "Great Balls";
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

            //for (int i = 0; i < 2; i++)
            //MakeList.Add(() => AllVertical(Difficulty));

            /*
             * Special stage1 fill funcs
             *      Hallway (narrow can't jump, half jump, etc) black everywhere else with cement blocks
             *      Tiled floor, emitters only on ceiling
             *      Up hallway (narrow can't move) (half screen width) black everywhere else
             *      up with 'ceiling' = dense cement long blocks, move back as need. style = one side only, alternate, etc
             *      Down hallway (half screen width), down with 'ceiling'
             *          Down with ceiling, nothing else to land on. allow comp to land on ceiling but finalize block then
             *      Down with nothing (all black background?) MAKE black background class
             * Right: all vertical, all 45, bishot, trishot, arcing, sine, exploding
             * two periods, three, four
             * Double up layer
             * Hallway
             * Up level: lined with all horizontal, all 45, all arcing up then down
             * down level: survival style stream from the bottom
             * couple of nova, either on edges or centered, emit DENSE ring
             * couple of sources, emit machine gun like rotating source
             * intertwined fireballs, two orbiting while moving forward (3?) wider space between them
             * moving emitters: falling, orbit
             * phase in/out fireballs
             * sharp turn fireballs: square wave, triangle wave (big amplitudes)
             * BIG fireballs
             * fireshoes!
             */

            int diff = new int[] { 8, 17, 23, 30 }[Difficulty];
            int jump1 = new int[] { 3, 4, 5, 6 }[Difficulty];
            int jump2 = new int[] { 6, 7, 8, 9 }[Difficulty];

            MakeList.Add(() => BottomOnly(diff++ - 0, 1, -1, 4500, LevelGeometry.Right, jump1));
            MakeList.Add(() => Hallway(diff++, 1, 2, 4500, LevelGeometry.Up));
            MakeList.Add(() => Hallway(diff++, 3, 2, 7000, LevelGeometry.Right));
            MakeList.Add(() => OnePiece(diff++, 3, 3, 7000, LevelGeometry.Down, jump1));
            MakeList.Add(() => OnePiece(diff++, 1, 3, 7500, LevelGeometry.Right, jump2));

            return MakeList;
        }

        void StandardInit(LevelSeedData data)
        {
            data.Seed = Tools.Rnd.Next();

            data.SetBackground(BackgroundType.Outside);
            data.DefaultHeroType = BobPhsxNormal.Instance;
        }

        void SetBorder(Level level, StyleData style, int NumAngles, int NumOffsets)
        {
            FireballEmitter_Parameters Params = (FireballEmitter_Parameters)style.FindParams(FireballEmitter_AutoGen.Instance);
            Params.Special.BorderFill = true;
            Params.NumAngles = NumAngles;
            Params.NumOffsets = NumOffsets;
            //Params.Arc = true;

            Params.DoStage2Fill = false;
        }

        void SetHallway(Level level, StyleData style, LevelGeometry geometry)
        {
            NormalBlock_Parameters NParams = (NormalBlock_Parameters)style.FindParams(NormalBlock_AutoGen.Instance);
            NParams.SetHallway(geometry);
        }

        LevelSeedData BottomOnly(int Difficulty, int NumAngles, int NumOffsets, int Length, LevelGeometry Geometry, int Jump)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.Initialize(NormalGameData.Factory, Geometry, 1, Length, piece =>
            {
                piece.MyUpgrades1[Upgrade.Jump] = Jump;
                piece.MyUpgrades1[Upgrade.Fireball] = Difficulty / 3;
                piece.MyUpgrades1[Upgrade.Ceiling] = 2;
                piece.MyUpgrades1[Upgrade.General] = Difficulty / 3 + (Difficulty % 3 > 0 ? 1 : 0);
                piece.MyUpgrades1[Upgrade.Speed] = Difficulty / 3 + (Difficulty % 3 > 1 ? 1 : 0);

                piece.Paths = 1;

                piece.Style.MyModParams = (level, p) =>
                {
                    SetBorder(level, p.Style, NumAngles, NumOffsets);
                    FireballEmitter_Parameters Params = (FireballEmitter_Parameters)p.Style.FindParams(FireballEmitter_AutoGen.Instance);
                    Params.BorderTop = false;
                };

                piece.StandardClose();
            });

            return data;
        }

        LevelSeedData OnePiece(int Difficulty, int NumAngles, int NumOffsets, int Length, LevelGeometry Geometry, int Jump)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);
            
            data.Initialize(NormalGameData.Factory, Geometry, 1, Length, piece =>
            {                
                piece.MyUpgrades1[Upgrade.Jump] = Jump;
                piece.MyUpgrades1[Upgrade.Fireball] = Difficulty / 3;
                piece.MyUpgrades1[Upgrade.Ceiling] = 2;
                piece.MyUpgrades1[Upgrade.General] = Difficulty / 3 + Difficulty % 3 > 0 ? 1 : 0;
                piece.MyUpgrades1[Upgrade.Speed] = Difficulty / 3 + Difficulty % 3 > 1 ? 1 : 0;

                piece.Paths = 1;

                piece.Style.MyModParams = (level, p) =>
                {
                    SetBorder(level, p.Style, NumAngles, NumOffsets);
                };

                piece.StandardClose();
            });

            return data;
        }

        LevelSeedData Hallway(int Difficulty, int NumAngles, int NumOffsets, int Length, LevelGeometry Geometry)
        {
            LevelSeedData data = new LevelSeedData();

            StandardInit(data);

            data.Initialize(NormalGameData.Factory, Geometry, 1, Length, piece =>
            {
                piece.MyUpgrades1[Upgrade.Jump] = 3;
                piece.MyUpgrades1[Upgrade.Fireball] = Difficulty / 3;
                piece.MyUpgrades1[Upgrade.General] = Difficulty / 3 + Difficulty % 3 > 0 ? 1 : 0;
                piece.MyUpgrades1[Upgrade.Speed] = Difficulty / 3 + Difficulty % 3 > 1 ? 1 : 0;

                piece.Paths = 1;

                piece.Style.MyModParams = (level, p) =>
                {
                    SetBorder(level, p.Style, NumAngles, NumOffsets);
                    SetHallway(level, p.Style, Geometry);
                };

                piece.StandardClose();
            });

            return data;
        }
    }
}