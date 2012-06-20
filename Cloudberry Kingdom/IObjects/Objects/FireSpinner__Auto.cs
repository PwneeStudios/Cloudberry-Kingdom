using System;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Blocks;
using CloudberryKingdom.FireSpinners;

namespace CloudberryKingdom.Levels
{
    public class FireSpinner_Parameters : AutoGen_Parameters
    {
        /*
        public enum DirectionStyle { Homogenous, Random, HorizontalSplit, VerticalSplit };
        public DirectionStyle Direction;
        public int Orientation;
        public enum Style { Regular, Clockwork, DoubleOrbit, Rotated, Gap, VariableLength, VariableGap };
        public Style Style1, Style2;
        public int NumStyles;
        */

        public Param FireSpinnerMinDist, MinFireSpinnerDensity, MaxFireSpinnerDensity, FireSpinnerLength, FireSpinnerPeriod;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            // General difficulty
            float FirespinnerLevel = PieceSeed.MyUpgrades1[Upgrade.FireSpinner];
            if (FirespinnerLevel > 6) NumOffsets = 8;
            else NumOffsets = 4;

            BobWidthLevel = new Param(PieceSeed);
            BobWidthLevel.SetVal(u =>
            {
                return u[Upgrade.FireSpinner];
            });

            FireSpinnerMinDist = new Param(PieceSeed);
            FireSpinnerMinDist.SetVal(u =>
                Tools.DifficultyLerp159(650, 270, 110, u[Upgrade.FireSpinner]));

            FireSpinnerLength = new Param(PieceSeed);
            FireSpinnerLength.SetVal(u =>
            {
                return 240 + 36 * u[Upgrade.FireSpinner];
            });

            FireSpinnerPeriod = new Param(PieceSeed);
            FireSpinnerPeriod.SetVal(u =>
            {
                return Math.Max(38, 150 + 13 * u[Upgrade.FireSpinner] - 13 * u[Upgrade.Speed]);
            });

            MinFireSpinnerDensity = new Param(PieceSeed);
            MinFireSpinnerDensity.SetVal(u =>
            {
                //return 29 * u[Upgrade.FireSpinner];
                if (u[Upgrade.FireSpinner] == 0)
                    return 0;

                return Tools.DifficultyLerp(60, 80, u[Upgrade.FireSpinner]);
            });

            MaxFireSpinnerDensity = new Param(PieceSeed);
            MaxFireSpinnerDensity.SetVal(u =>
            {
                //return 29 * u[Upgrade.FireSpinner];
                if (u[Upgrade.FireSpinner] == 0)
                    return 0;

                return Tools.DifficultyLerp(40, 73, u[Upgrade.FireSpinner]);
            });
        }
    }

    public sealed class FireSpinner_AutoGen : AutoGen
    {
        static readonly FireSpinner_AutoGen instance = new FireSpinner_AutoGen();
        public static FireSpinner_AutoGen Instance { get { return instance; } }

        static FireSpinner_AutoGen() { }
        FireSpinner_AutoGen()
        {
            Do_PreFill_2 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            FireSpinner_Parameters Params = new FireSpinner_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoFireSpinners();
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupFireSpinners(BL, TR);
        }
    }

    public partial class Level
    {
        public void CleanupFireSpinners(Vector2 BL, Vector2 TR)
        {
            // Get FireSpinner parameters
            FireSpinner_Parameters Params = (FireSpinner_Parameters)Style.FindParams(FireSpinner_AutoGen.Instance);
            
            Cleanup(ObjectType.FireSpinner, delegate(Vector2 pos)
            {
                float dist = Params.FireSpinnerMinDist.GetVal(pos);
                return new Vector2(dist, dist);
            }, BL, TR);
        }
        public void AutoFireSpinners()
        {
            // Get FireSpinner parameters
            FireSpinner_Parameters Params = (FireSpinner_Parameters)Style.FindParams(FireSpinner_AutoGen.Instance);

            float SpinnerTopOffset = Info.Spinners.TopOffset;
            float SpinnerBottomOffset = Info.Spinners.BottomOffset;

            foreach (BlockBase block in Blocks)
            {
                if (block.Core.Placed) continue;

                if (block.BlockCore.Virgin) continue;
                if (block.BlockCore.Finalized) continue;
                if (block.BlockCore.MyType == ObjectType.LavaBlock) continue;

                // Add fire spinners
                float xdif = block.Box.Current.TR.X - block.Box.Current.BL.X - 30;
                float density = Rnd.RndFloat(Params.MinFireSpinnerDensity.GetVal(block.Core.Data.Position),
                                                  Params.MaxFireSpinnerDensity.GetVal(block.Core.Data.Position));
                float average = (int)(xdif * density / 2000f);
                int n = (int)average;
                if (average < 1) if (Rnd.Rnd.NextDouble() < average) n = 1;

                for (int i = 0; i < n; i++)
                {
                    if (xdif > 0)
                    {
                        float x = (float) Rnd.Rnd.NextDouble() * xdif + block.Box.Target.BL.X + 35;
                        float y;
                        if (block.BlockCore.BlobsOnTop)
                        {
                            y = block.Box.Target.TR.Y + SpinnerTopOffset;
                        }
                        else
                        {
                            y = block.Box.Target.BL.Y + SpinnerBottomOffset;
                        }

                        if (x > CurMakeData.PieceSeed.End.X - 400) continue;

                        FireSpinner spinner_1, spinner_2, spinner_3;
                        
                        spinner_1 = (FireSpinner)Recycle.GetObject(ObjectType.FireSpinner, true);
                        spinner_1.Core.StartData.Position = spinner_1.Core.Data.Position = new Vector2(x, y);
                        
                        spinner_1.Orientation = 1;
                        //spinner_1.Orientation = MyLevel.Rnd.RndBit();
                        
                        spinner_1.Radius = Params.FireSpinnerLength.GetVal(block.Core.Data.Position);
                        
                        int Period = (int)Params.FireSpinnerPeriod.GetVal(block.Core.Data.Position);

                        //if (MyLevel.Rnd.RndBool()) Period -= Period / 3;
                        //else Period += Period / 3;
                        
                        int NumOffsets = Params.NumOffsets;
                        Period = (int)(Period / NumOffsets) * NumOffsets;
                        spinner_1.Period = Period;
                        spinner_1.Offset = Params.ChooseOffset(Period, Rnd);

                        spinner_1.SetParentBlock(block);

                        AddObject(spinner_1);

                        /*
                        spinner_2 = (FireSpinner)Tools.Recycle.GetObject(ObjectType.FireSpinner, true);
                        spinner_2.Core.StartData.Position = spinner_2.Core.Data.Position = new Vector2(x, y);
                        spinner_2.Orientation = -1;
                        spinner_2.Radius = Params.FireSpinnerLength.GetVal(block.Core.Data.Position);
                        spinner_2.Offset = spinner_1.Offset + spinner_1.Period / 2;
                        //spinner_2.Offset = spinner_1.Offset + spinner_1.Period / 3;
                        spinner_2.Period = (int)Params.FireSpinnerPeriod.GetVal(block.Core.Data.Position);
                        spinner_2.Core.SetParentBlock(block);

                        AddObject(spinner_2);
                        */

                        /*
                        spinner_3 = (FireSpinner)Tools.Recycle.GetObject(ObjectType.FireSpinner, true);
                        spinner_3.Core.StartData.Position = spinner_3.Core.Data.Position = new Vector2(x, y);
                        spinner_3.Radius = Params.FireSpinnerLength.GetVal(block.Core.Data.Position);
                        spinner_3.Offset = spinner_1.Offset + 2 * spinner_1.Period / 3;
                        spinner_3.Period = (int)Params.FireSpinnerPeriod.GetVal(block.Core.Data.Position);
                        spinner_3.Core.SetParentBlock(block);

                        AddObject(spinner_3);
                        */

                        //ObjectData.AddAssociation(true, true, spinner_1, spinner_2);
                        //ObjectData.AddAssociation(false, true, spinner_1, spinner_2, spinner_3);
                    }
                }
            }
        }
    }
}
