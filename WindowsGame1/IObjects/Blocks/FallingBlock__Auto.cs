using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class FallingBlock_Parameters : AutoGen_Parameters
    {
        public Param Delay, Width, AngryRatio, AngrySpeed, AngryAccel, KeepUnused;

        public override void SetParameters(PieceSeedData PieceSeed, Level level)
        {
            base.SetParameters(PieceSeed, level);

            KeepUnused = new Param(PieceSeed);
            if (level.DefaultHeroType is BobPhsxSpaceship)
            {
                KeepUnused.SetVal(u => BobPhsxSpaceship.KeepUnused(u[Upgrade.FallingBlock]));
            }

            FillWeight = new Param(PieceSeed);
            FillWeight.SetVal(u =>
            {
                return u[Upgrade.FallingBlock];
            });

            Delay = new Param(PieceSeed);
            Delay.SetVal(u =>
            {
                return Math.Max(1, 60 - 7 * u[Upgrade.FallingBlock]);
            });

            Width = new Param(PieceSeed);
            Width.SetVal(u =>
            {
                return Math.Max(70, 113 - .1f * (110 - 70) * u[Upgrade.FallingBlock]);
            });

            AngryAccel = new Param(PieceSeed);
            AngryAccel.SetVal(u =>
            {
                return Tools.DifficultyLerp(-70, 320, u[Upgrade.BouncyBlock]);
                //return -70 + 40 * u[Upgrade.FallingBlock];
            });

            AngryRatio = new Param(PieceSeed);
            AngryRatio.SetVal(u =>
            {
                return Tools.DifficultyLerp(-27, 35, u[Upgrade.BouncyBlock]);
                //return -40 + 10 * u[Upgrade.FallingBlock];
            });

            AngrySpeed = new Param(PieceSeed);
            AngrySpeed.SetVal(u =>
            {
                return 4 * u[Upgrade.FallingBlock];
            });
        }
    }

    public sealed class FallingBlock_AutoGen : AutoGen
    {
        static readonly FallingBlock_AutoGen instance = new FallingBlock_AutoGen();
        public static FallingBlock_AutoGen Instance { get { return instance; } }

        static FallingBlock_AutoGen() { }
        FallingBlock_AutoGen()
        {
            Do_WeightedPreFill_1 = true;
            //Generators.AddGenerator(this);
        }

        public override AutoGen_Parameters SetParameters(PieceSeedData data, Level level)
        {
            FallingBlock_Parameters Params = new FallingBlock_Parameters();
            Params.SetParameters(data, level);

            return (AutoGen_Parameters)Params;
        }

        public override void PreFill_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.PreFill_2(level, BL, TR);
            level.AutoFallingBlocks();
        }

        public override void Cleanup_2(Level level, Vector2 BL, Vector2 TR)
        {
            base.Cleanup_2(level, BL, TR);
            level.CleanupFallingBlocks(BL, TR);
        }

        public override ObjectBase CreateAt(Level level, Vector2 pos, Vector2 BL, Vector2 TR)
        {
            base.CreateAt(level, pos, BL, TR);

            StyleData Style = level.Style;
            RichLevelGenData GenData = level.CurMakeData.GenData;
            PieceSeedData piece = level.CurMakeData.PieceSeed;

            // Get FallingBlock parameters
            FallingBlock_Parameters Params = (FallingBlock_Parameters)level.Style.FindParams(FallingBlock_AutoGen.Instance);

            FallingBlock fblock;
            float Width = Params.Width.GetVal(pos);
            Vector2 size = new Vector2(Width, Width);
            Vector2 offset = new Vector2(level.Rnd.Rnd.Next(0, 0), level.Rnd.Rnd.Next(0, 0) - size.Y);

            if (level.Style.BlockFillType == StyleData._BlockFillType.Spaceship)
            {
                offset += new Vector2(level.Rnd.Rnd.Next(0, 100), level.Rnd.Rnd.Next(0, 100));
                if (pos.X > level.CurMakeData.PieceSeed.End.X - 400) offset.X -= pos.X - level.CurMakeData.PieceSeed.End.X + 400;
                if (pos.X < level.CurMakeData.PieceSeed.Start.X + 400) offset.X += level.CurMakeData.PieceSeed.Start.X - pos.X + 400;
            }

            fblock = (FallingBlock)level.Recycle.GetObject(ObjectType.FallingBlock, true);
            int Life = (int)Params.Delay.GetVal(pos);
            fblock.Init(pos + offset, size, Life);
            fblock.BlockCore.BlobsOnTop = true;
            
            fblock.BlockCore.Decide_RemoveIfUnused(Params.KeepUnused.GetVal(pos), level.Rnd);
            fblock.BlockCore.GenData.EdgeSafety = GenData.Get(DifficultyParam.EdgeSafety, pos);

            if (level.Rnd.Rnd.NextDouble() < Params.AngryRatio.GetVal(pos) / 100f)
            {
                // Make angry
                fblock.Thwomp = true;
                fblock.AngryMaxSpeed = Params.AngrySpeed.GetVal(pos);
                fblock.AngryAccel = new Vector2(0, Params.AngryAccel.GetVal(pos) / 100f);
            }

            if (level.Style.RemoveBlockOnOverlap)
                fblock.BlockCore.GenData.RemoveIfOverlap = true;

            level.AddBlock(fblock);

            return fblock;
        }
    }

    public partial class Level
    {
        public void CleanupFallingBlocks(Vector2 BL, Vector2 TR)
        {
            // Get FallingBlock parameters
            FallingBlock_Parameters Params = (FallingBlock_Parameters)Style.FindParams(FallingBlock_AutoGen.Instance);
        }
        public void AutoFallingBlocks()
        {
            // Get FallingBlock parameters
            FallingBlock_Parameters Params = (FallingBlock_Parameters)Style.FindParams(FallingBlock_AutoGen.Instance);
        }
    }
}
