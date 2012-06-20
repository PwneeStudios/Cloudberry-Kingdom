using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class StyleData
    {
        public delegate void ModParams(Level level, PieceSeedData data);

        /// <summary>
        /// A callback to modify AutoGen parameters after they have been set
        /// </summary>
        public ModParams MyModParams
        {
            set { ModParamList.Add(value); }
        }

        public List<ModParams> ModParamList = new List<ModParams>();

        public void Release()
        {
            ModParamList = null;
            GenParams = null;
        }

        public Dictionary<AutoGen, AutoGen_Parameters> GenParams;
        public void CalcGenParams(PieceSeedData SeedData, Level level)
        {
            GenParams = new Dictionary<AutoGen, AutoGen_Parameters>();
            foreach (AutoGen gen in Generators.Gens)
                GenParams.Add(gen, gen.SetParameters(SeedData, level));

            foreach (ModParams mod in ModParamList)
                mod(level, SeedData);

            // Change data depending on hero type
            level.DefaultHeroType.ModData(ref level.CurMakeData, this);

            // Set time type
            level.TimeType = TimeType;
        }
        public AutoGen_Parameters FindParams(AutoGen gen)
        {
            return GenParams[gen];
        }

        public StyleData Clone()
        {
            return (StyleData)this.MemberwiseClone();
        }

        public float Zoom = 1;
        public Level.TimeTypes TimeType = Level.TimeTypes.Regular;

        public bool SuppressCoins, SuppressCheckpoints;

        public float TopSpace, BottomSpace;
        public float FillxStep, FillyStep;

        public float ModNormalBlockWeight = 1f;

        /// <summary>
        /// The length of time the computer waits at the beginning of the level.
        /// </summary>
        public Vector2 ComputerWaitLengthRange = new Vector2(15, 36);//50);

        public enum FinalPlatsType { Seed, Door, DarkBottom };
        public FinalPlatsType MyFinalPlatsType = FinalPlatsType.Seed;
        
        public enum FinalDoorStyle { Normal, TerraceToCastle };
        public FinalDoorStyle MyFinalDoorStyle = FinalDoorStyle.Normal;

        public enum InitialPlatsType { Normal, LandingZone, Door, Spaceship, CastleToTerrace, Up_TiledFloor };
        public InitialPlatsType MyInitialPlatsType = InitialPlatsType.Normal;

        public bool MakeInitialPlats;
        public float UpperSafetyNetOffset, LowerSafetyNetOffset;

        public enum GroundType { None, SafetyNet, InvisibleUsed, Used, InvertedUsed, VirginUsed };
        public GroundType MyGroundType, MyTopType;

        public bool RemovedUnusedOverlappingBlocks;
        public bool RemoveBlockOnCol;
        public bool RemoveBlockOnOverlap;
        public float MinBlockDist;

        public enum _BlockFillType { Regular, TopOnly, Spaceship, Invertable, Sideways };
        float[] BlockFillTypeRatio = { 1f, 0f };
        public _BlockFillType BlockFillType;


        public enum _SparsityType { Regular, LilSparse, VerySparse };
        float[] Sparsity = { 1, 1.5f, 2f, 4f };
        float[] SparsityTypeRatio = { .5f, .35f, .1f, .05f };
        public _SparsityType SparsityType;

        /// <summary>
        /// When true the computer's destination is always super curvy.
        /// </summary>
        public bool AlwaysCurvyMove = false;

        public bool AlwaysEdgeJump = false;

        public enum _MoveTypePeriod { Inf, Short, Normal1, Normal2 };
        public _MoveTypePeriod MoveTypePeriod;

        public enum _MoveTypeInnerPeriod { Long, Short, Normal };
        public _MoveTypeInnerPeriod MoveTypeInnerPeriod;

        public enum _PauseType { None, Limited, Normal, Normal2 };
        public _PauseType PauseType;

        public enum _ReverseType { None, Normal, Normal2, Normal3 };
        public _ReverseType ReverseType;

        public enum _JumpType { Always, Alot, Normal, Normal2 };
        public _JumpType JumpType;

        public enum _ElevatorSwitchType { Random, Alternate, AllUp, AllDown };
        public _ElevatorSwitchType ElevatorSwitchType;
        float[] ElevatorSwitchTypeRatio = { .35f, .5f, .1f, .05f };

        public enum _OffsetType { Random, AllSame, SpatiallyPeriodic };
        public _OffsetType MovingBlock2OffsetType, FlyingBlobOffsetType;
        float[] OffsetTypeRatio = { .7f, .25f, .05f };

        public enum _FillType { Rnd, HalfnHalf, Pure };
        float[] FillTypeRatio = { .7f, .25f, .05f };
        public _FillType FillType;

        public enum _SinglePathType { Normal, Low, Mid, High };
        static float[] _SinglePathRatio = { .7f, .1f, .1f, .1f };
        public enum _DoublePathType { Separated, Gap, Independent };
        public enum _TriplePathType { Separated, Independent };
        public _SinglePathType SinglePathType;
        public _DoublePathType DoublePathType;
        public _TriplePathType TriplePathType;
        public enum _StartType { Top, Middle, Bottom };
        public _StartType Bob1Start, Bob2Start, Bob3Start;

        public float ChanceToKeepUnused;

        /// <summary>
        /// When true the computer stops less, reverses less, etc.
        /// </summary>
        public bool FunRun = true;

        public Rand Rnd;
        public StyleData(Rand Rnd)
        {
            this.Rnd = Rnd;

            Randomize();
        }

        public void Calculate(Upgrades u)
        {
            float JumpLevel = u[Upgrade.Jump];

            float[] PauseTypeRatio = { 1f, 1f, 1f, 1f };
            PauseTypeRatio[0] += .3f * JumpLevel;
            PauseType = (_PauseType)Rnd.Rnd.Next(0, Tools.Length<_PauseType>());            
            
            ReverseType = (_ReverseType)Rnd.Rnd.Next(0, Tools.Length<_ReverseType>());

            CalculateKeepUnused(JumpLevel);
        }

        protected virtual void CalculateKeepUnused(float JumpLevel)
        {
            // Extra fill: keep unused
            float ChanceToHaveUnused = .4f - .1f * (.4f - -.2f) * JumpLevel;
            if (Rnd.RndFloat(0, 1) < ChanceToHaveUnused)
            {
                float chance = .1f - .1f * (.1f - 0) * JumpLevel;
                ChanceToKeepUnused = Rnd.RndFloat(0, chance);
            }
        }

        public virtual void Randomize()
        {
            FillxStep = 225;
            FillyStep = 200;

            //FillxStep = 175;
            //FillyStep = 175;

            UpperSafetyNetOffset = 800;

            MyGroundType = GroundType.SafetyNet;
            MyTopType = GroundType.InvisibleUsed;

            // Fast
            //TopSpace = 560;
            //BottomSpace = 250;

            // Compromise, still too slow
            TopSpace = 360;
            BottomSpace = 180;

            // Ideal, way too slow
            //TopSpace = -250;
            //BottomSpace = -250;
            
            MakeInitialPlats = true;

            SparsityType = (_SparsityType)Rnd.Choose(SparsityTypeRatio);

            ElevatorSwitchType = (_ElevatorSwitchType)Rnd.Choose(ElevatorSwitchTypeRatio);

            FlyingBlobOffsetType = (_OffsetType)Rnd.Choose(OffsetTypeRatio);
            MovingBlock2OffsetType = (_OffsetType)Rnd.Choose(OffsetTypeRatio);

            JumpType = (_JumpType)Rnd.Rnd.Next(0, Tools.Length<_JumpType>());
            

            MoveTypePeriod = (_MoveTypePeriod)Rnd.Rnd.Next(0, Tools.Length<_MoveTypePeriod>());
            MoveTypeInnerPeriod = (_MoveTypeInnerPeriod)Rnd.Rnd.Next(0, Tools.Length<_MoveTypeInnerPeriod>());

            FillType = (_FillType)Rnd.Choose(FillTypeRatio);

            PauseType = (_PauseType)Rnd.Rnd.Next(0, Tools.Length<_PauseType>());

            // Path types
            Bob1Start = (_StartType)Rnd.Rnd.Next(0, Tools.Length<_StartType>());
            Bob2Start = (_StartType)Rnd.Rnd.Next(0, Tools.Length<_StartType>());
            Bob3Start = (_StartType)Rnd.Rnd.Next(0, Tools.Length<_StartType>());
            SinglePathType = (_SinglePathType)Rnd.Choose(_SinglePathRatio);
            DoublePathType = (_DoublePathType)Rnd.Rnd.Next(0, Tools.Length<_DoublePathType>());
            TriplePathType = (_TriplePathType)Rnd.Rnd.Next(0, Tools.Length<_TriplePathType>());
        }

        public float GetSparsity()
        {
            return Sparsity[(int)SparsityType];
        }

        public int GetOffset(int Period, Vector2 pos, _OffsetType Type)
        {
            switch (Type)
            {
                case _OffsetType.Random: return Rnd.Rnd.Next(0, Period);
                case _OffsetType.AllSame: return 0;
                case _OffsetType.SpatiallyPeriodic: return ((int)(Period * (pos.X / 2000f))) % Period;
            }

            return 0;
        }





        void SetStartType(ref PhsxData Start, ref Vector2 CheckpointShift, _StartType StartType, PieceSeedData Piece)
        {
            switch (StartType)
            {
                case _StartType.Top:
                    Start.Position = new Vector2(100, 600) + Piece.Start;
                    CheckpointShift = new Vector2(0, 50);
                    break;

                case _StartType.Middle:
                    Start.Position = new Vector2(100, -75) + Piece.Start;
                    CheckpointShift = new Vector2(0, 210);
                    break;

                case _StartType.Bottom:
                    Start.Position = new Vector2(100, -530) + Piece.Start;
                    CheckpointShift = new Vector2(0, 220);
                    break;
            }
        }

        public void SetSinglePathType(Level.MakeData makeData, Level level, PieceSeedData Piece)
        {
            SetStartType(ref makeData.Start[0], ref makeData.CheckpointShift[0], Bob1Start, Piece);

            switch (SinglePathType)
            {
                case _SinglePathType.Normal:
                    makeData.MoveData[0].MaxTargetY = 875;
                    makeData.MoveData[0].MinTargetY = -625;
                    break;

                case _SinglePathType.High:
                    makeData.MoveData[0].MaxTargetY = 950;
                    makeData.MoveData[0].MinTargetY = 100;
                    break;

                case _SinglePathType.Mid:
                    makeData.MoveData[0].MaxTargetY = 450;
                    makeData.MoveData[0].MinTargetY = -100;
                    break;

                case _SinglePathType.Low:
                    makeData.MoveData[0].MaxTargetY = 300;
                    makeData.MoveData[0].MinTargetY = -500;
                    break;
            }
        }

        public void SetDoubePathType(Level.MakeData makeData, Level level, PieceSeedData Piece)
        {
            if (DoublePathType == _DoublePathType.Independent)
                DoublePathType = _DoublePathType.Gap;

            // Make sure double paths are always well separated
            if (Rnd.RndBool())
            {
                Bob1Start = _StartType.Bottom;
                Bob2Start = _StartType.Top;
            }
            else
            {
                Bob1Start = _StartType.Top;
                Bob2Start = _StartType.Bottom;
            }

            SetStartType(ref makeData.Start[0], ref makeData.CheckpointShift[0], Bob1Start, Piece);
            SetStartType(ref makeData.Start[1], ref makeData.CheckpointShift[1], Bob2Start, Piece);
            makeData.Start[1].Position.X += 100;

            switch (DoublePathType)
            {
                case _DoublePathType.Independent:
                    for (int i = 0; i < 2; i++)
                    {
                        makeData.MoveData[i].MaxTargetY = 900;
                        makeData.MoveData[i].MinTargetY = -600;
                    }
                    break;

                case _DoublePathType.Separated:
                    makeData.MoveData[0].MaxTargetY = 900;
                    makeData.MoveData[0].MinTargetY = 120;

                    makeData.MoveData[1].MaxTargetY = 100;
                    makeData.MoveData[1].MinTargetY = -600;
                    break;

                case _DoublePathType.Gap:
                    makeData.MoveData[0].MaxTargetY = 1000;
                    makeData.MoveData[0].MinTargetY = 700;

                    makeData.MoveData[1].MaxTargetY = -300;
                    makeData.MoveData[1].MinTargetY = -700;
                    break;
            }
        }

        public void SetTriplePathType(Level.MakeData makeData, Level level, PieceSeedData Piece)
        {
            if (TriplePathType == _TriplePathType.Independent)
                TriplePathType = _TriplePathType.Separated;

            SetStartType(ref makeData.Start[0], ref makeData.CheckpointShift[0], Bob1Start, Piece);
            SetStartType(ref makeData.Start[1], ref makeData.CheckpointShift[1], Bob2Start, Piece);
            SetStartType(ref makeData.Start[2], ref makeData.CheckpointShift[2], Bob3Start, Piece);
            makeData.Start[1].Position.X += 60;
            makeData.Start[2].Position.X += 120;

            switch (TriplePathType)
            {
                case _TriplePathType.Independent:
                    for (int i = 0; i < 2; i++)
                    {
                        makeData.MoveData[i].MaxTargetY = 1000;
                        makeData.MoveData[i].MinTargetY = -650;
                    }
                    break;

                case _TriplePathType.Separated:
                    makeData.MoveData[0].MaxTargetY = 1000;
                    makeData.MoveData[0].MinTargetY = 450;

                    makeData.MoveData[1].MaxTargetY = 450;
                    makeData.MoveData[1].MinTargetY = -200;

                    makeData.MoveData[2].MaxTargetY = -280;
                    makeData.MoveData[2].MinTargetY = -650;
                    break;
            }
        }

        public void SuppressGroundCeiling(PieceSeedData piece)
        {
            var Ceiling_Params = (Ceiling_Parameters)FindParams(Ceiling_AutoGen.Instance);
            Ceiling_Params.Make = false;
            var NBlock_Params = (NormalBlock_Parameters)FindParams(NormalBlock_AutoGen.Instance);
            NBlock_Params.Make = false;
        }

        public void SetToMake_BouncyHallway(PieceSeedData piece)
        {
            SuppressGroundCeiling(piece);

            var Bounce_Params = (BouncyBlock_Parameters)FindParams(BouncyBlock_AutoGen.Instance);
            Bounce_Params.Special.Hallway = true;
        }
    }
}